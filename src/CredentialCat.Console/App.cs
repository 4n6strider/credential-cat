using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.CommandLine;
using System.Threading.Tasks;
using System.CommandLine.Invocation;
using System.Globalization;
using static System.Console;

using CredentialCat.Shared.Entities;
using CredentialCat.Shared.Enums;

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

            #endregion

            #region Application comands

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
                    WriteLine(source);
                    
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
                Source()
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

            commands.Description = "credential-cat help you to enumerate leaked credentials on several sources around the dark web (also in surface).";
            return await commands.InvokeAsync(args);
        }
    }
}
