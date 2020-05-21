using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using CredentialCat.Shared.Entities;

namespace CredentialCat.Console
{  internal static class App
    {
        private static async Task Main()
        {
            var home = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CREDENTIAL_CAT_HOME"))
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".credential_cat")
                : Environment.GetEnvironmentVariable("CREDENTIAL_CAT_HOME");

            var configurationFile = Path.Combine(home ?? throw new NullReferenceException(nameof(home)), "configuration.json");

            ConfigurationEntity configuration;

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
        }
    }
}
