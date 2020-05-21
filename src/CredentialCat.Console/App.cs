using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.CommandLine;
using System.Threading.Tasks;
using System.CommandLine.Invocation;
using Microsoft.EntityFrameworkCore;

using static System.Console;

using CredentialCat.Shared;
using CredentialCat.Shared.Enums;
using CredentialCat.Shared.Entities;

namespace CredentialCat.Console
{  internal static class App
    {
        private static async Task<int> Main(string[] args)
        {
            var home = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CREDENTIAL_CAT_HOME"))
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".credential_cat")
                : Environment.GetEnvironmentVariable("CREDENTIAL_CAT_HOME");

            var configurationFile = Path.Combine(home ?? throw new NullReferenceException(nameof(home)), "configuration.json");

            ConfigurationEntity configuration;

            var context = new DatabaseContext($"Data Source = {Path.Combine(home, "cache.sqlite")}");

            #region Startup validations

            if (!Directory.Exists(home))
            {
                Directory.CreateDirectory(home);
            }

            if (!File.Exists(configurationFile))
            {
                configuration = new ConfigurationEntity();

                var content = JsonSerializer.Serialize(configuration);
                await File.WriteAllTextAsync(configurationFile, content);
            }

            configuration =
                JsonSerializer.Deserialize<ConfigurationEntity>(await File.ReadAllTextAsync(configurationFile));

            if (await context.Database.EnsureCreatedAsync())
            {
                await context.Database.MigrateAsync();
            }

            #endregion

            #region Interruption validation

            CancelKeyPress += async (sender, args) =>
            {
                WriteLine();
                WriteLine("[+] Exiting gratefully...");
                await context.DisposeAsync();

                args.Cancel = false;
            };

            #endregion

            #region Application comands

            Command Search()
            {
                var searchCommand = new Command("search",
                    $"Execute searches against current data source ({Enum.GetName(typeof(SourceEnum), configuration.DefaultSource)})")
                {
                    // Logic options
                    new Option<bool>(new [] {"-i", "--ignore-cache"})
                    {
                        Description = "If given query ignore cached values",
                        Name = "ignore"
                    },

                    new Option<bool>(new [] {"-iU", "--ignore-update"})
                    {
                        Description = "Ignore update on cache database when query results exceed the timestamp",
                        Name = "ignoreUpdate"
                    },

                    new Option<bool>(new [] {"-f", "--force-update"})
                    {
                        Description = "Force update on cache database with every query result",
                        Name = "forceUpdate"
                    },

                    new Option<bool>(new [] {"-c", "--case-sensitive"})
                    {
                        Description = "If the engine will respect case sensitive to match query pattern",
                        Name = "caseSensitive"
                    },

                    // Data input options

                    // Password and hashes
                    new Option<string>(new [] {"-p", "--password", "-h", "--hash"})
                    {
                        Description = "Query to this specific password or hash",
                        Name = "password",
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ZeroOrMore, Name = "query value", 
                            Description = "Password or hash"
                        }
                    },

                    new Option<string>(new [] {"-pL", "--password-list", "-hL", "--hash-list"})
                    {
                        Description = "Query to passwords and/or hashes on given wordlist",
                        Name = "passwordList",
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ZeroOrMore, Name = "wordlist path",
                            Description = "Wordlist with password(s) and/or hash(es)"
                        }
                    },

                    // User(name) and email addresses
                    new Option<string>(new [] {"-u", "--user", "-e", "--email"})
                    {
                        Description = "Query to this specific user(name) or email address",
                        Name = "user",
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ZeroOrMore, Name = "query value",
                            Description = "Email or user(name)"
                        }
                    },

                    new Option<string>(new [] {"-uL", "--user-list", "-eL", "--email-list"})
                    {
                        Description = "Query to user(s) and/or email address(es) on given wordlist",
                        Name = "userList",
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ZeroOrMore, Name = "wordlist path",
                            Description = "Wordlist with user(s) and/or address(es)"
                        }
                    },

                    // Leak origin
                    new Option<string>(new [] {"-o", "--origin"})
                    {
                        Description = "Query to origin",
                        Name = "origin",
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ZeroOrMore, Name = "origin name",
                            Description = "Origin name to search"
                        }
                    }
                };

                return searchCommand;
            }

            Command Cache()
            {
                var cacheCommand = new Command("cache", "Flush or query inside the cache database")
                {
                    new Option<bool>(new[] {"-f", "--flush"})
                    {
                        Description = "Flush the entire cache database. Permanent data lost (Dangerous!)",
                        Name = "flush",
                    },

                    new Option<string>(new [] {"-q", "--query"})
                    {
                        Description = "Perform a SQlite raw query inside cache database (Dangerous, you can have SQLi here!)",
                        Name = "query",
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ExactlyOne, Name = "query",
                            Description = "SQlite query"
                        }
                    }
                };

                cacheCommand.Handler = CommandHandler.Create<bool, string>(async (flush, query) =>
                {
                    if (!string.IsNullOrEmpty(query))
                    {
                        var command = context.Database.GetDbConnection().CreateCommand();
#pragma warning disable CA2100
                        command.CommandText = query;
#pragma warning restore CA2100

                        await context.Database.OpenConnectionAsync();
                        var result = await command.ExecuteReaderAsync();

                        if (!result.HasRows)
                        {
                            WriteLine("[!] No rows returned!");
                            Environment.Exit(1);
                        }

                        while (await result.ReadAsync())
                        {
                            for (var i = 0; i < result.FieldCount; i++)
                            {
                                string dump;

                                if (await result.IsDBNullAsync(i))
                                    dump = "NULL";
                                else
                                    dump = result.GetString(i);

                                var column = result.GetName(0);
                                WriteLine($"{result.GetName(i)} : {dump}");
                            }

                            WriteLine();
                        }

                    }

                    if (flush)
                    {
                        Write("[!] If you continue, the entire database will be erased [y/N] ");

                        if (ReadKey().Key == ConsoleKey.Y)
                        {
                            WriteLine();
                            await context.Database.EnsureDeletedAsync();
                            WriteLine("[+] Database flushed!");
                        }
                    }
                });

                return cacheCommand;
            }

            Command Source()
            {
                var sourceCommand = new Command("source",
                    "List and select default data source for credentials enumeration")
                {
                    new Option<bool>(new[] {"--list", "-l"})
                    {
                        Description = "List available sources and current active data source",
                        Name = "list"
                    },

                    new Option<string>(new[] {"--set-default", "-s"})
                    {
                        Description = "Set the new default data source",
                        Name = "source",
                        Argument = new Argument<string>
                        {
                            Arity = ArgumentArity.ExactlyOne, Name = "data source", Description = "Data source name"
                        }
                    }
                };

                sourceCommand.Handler = CommandHandler.Create<bool, string>(async (list, source) =>
                {
                    if (!string.IsNullOrEmpty(source))
                    {
                        if (!Enum.GetNames(typeof(SourceEnum)).Contains(source))
                        {
                            WriteLine("[!] Invalid data source!");
                            WriteLine("[+] List available data sources with `sources --list`");
                            Environment.Exit(1);
                        }

                        var newSource = (SourceEnum) Enum.Parse(typeof(SourceEnum), source);

                        if (newSource == configuration.DefaultSource)
                        {
                            WriteLine($"[!] {source} already is the default data source!");
                            WriteLine("[+] List available data sources with `sources --list`");
                            Environment.Exit(1);
                        }

                        configuration.DefaultSource = newSource;

                        JsonSerializer.Deserialize<ConfigurationEntity>(await File.ReadAllTextAsync(configurationFile));
                    }

                    if (list)
                    {
                        WriteLine("[+] Application available sources:");

                        Enum.GetNames(typeof(SourceEnum))
                            .Select(e =>
                            {
                                if ((SourceEnum)Enum.Parse(typeof(SourceEnum), e) == configuration.DefaultSource)
                                {
                                    e += " (current source)";
                                }

                                return e;
                            })
                            .ToList()
                            .ForEach(e => WriteLine($" > {e}"));
                    }
                });

                return sourceCommand;
            }

            Command Proxy()
            {
                var proxyCommand = new Command("proxy", "Create, list, import and export proxies");

                return proxyCommand;
            }

            #endregion

            #region Application options

            static Option ApplicationEnvironment()
            {
                var statementOption = new Option(new[] { "-e", "--env" })
                {
                    Description = "Show application environment variables"
                };

                return statementOption;
            }

            #endregion

            var commands = new RootCommand
            {
                // Options
                ApplicationEnvironment(),

                // Commands
                Source(),
                Search(),
                Cache(),
                Proxy()
            };

            commands.Handler = CommandHandler.Create<bool, bool>((env, source) =>
            {
                if (env)
                {
                    WriteLine("[+] Application environment variables:");

                    var envHome = Environment.GetEnvironmentVariable("CREDENTIAL_CAT_HOME");

                    WriteLine(string.IsNullOrEmpty(envHome)
                        ? $" > CREDENTIAL_CAT_HOME is empty, default value in use: {home}"
                        : $" > CREDENTIAL_CAT_HOME: {envHome}");
                }
            });

            #region Application banner

            var banner = Environment.NewLine;
            banner += @"               /\____/\    __";
            banner += Environment.NewLine;
            banner += "             .'  \"\"\"\"  `,-'  `--.__";
            banner += Environment.NewLine;
            banner += "        __,- :   -  -  ;  \" ::     `-. -.__";
            banner += Environment.NewLine;
            banner += "     ,-sssss `._  `' _,'\"     ,'~~~::`.sssss-.";
            banner += Environment.NewLine;
            banner += @"    |ssssss ,' ,_`--'_    __,' ::  `  `.ssssss|";
            banner += Environment.NewLine;
            banner += @"   |sssssss `-._____~ `,,'_______,---_;; ssssss|";
            banner += Environment.NewLine;
            banner += @"    |ssssssssss     `--'~{__   ____   ,'ssssss|";
            banner += Environment.NewLine;
            banner += @"     `-ssssssssssssssssss ~~~~~~~~~~~~ ssss.-'";
            banner += Environment.NewLine;
            banner += @"          `---.sssssssssssssssssssss.---'";
            banner += Environment.NewLine;
            banner += "         credential-cat your purrrfect tool!";
            banner += Environment.NewLine;

            WriteLine(banner);

            #endregion

            commands.Description = "credential-cat help you to enumerate leaked credentials on several sources around the dark web (also in surface).";
            return await commands.InvokeAsync(args);
        }
    }
}
