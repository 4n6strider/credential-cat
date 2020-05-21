using System;
using System.IO;
using System.Text.Json;
using System.CommandLine;
using System.Threading.Tasks;
using System.CommandLine.Invocation;

using static System.Console;

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

            #endregion

            #region Application comands



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
                ApplicationEnvironment()
            };

            commands.Handler = CommandHandler.Create<bool>(env =>
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
            return await commands.InvokeAsync(args).ConfigureAwait(true);
        }
    }
}
