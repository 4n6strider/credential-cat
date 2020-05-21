using System;
using System.IO;

namespace CredentialCat.Console
{  internal static class App
    {
        private static void Main()
        {
            var home = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CREDENTIAL_CAT_HOME"))
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".credential_cat")
                : Environment.GetEnvironmentVariable("CREDENTIAL_CAT_HOME");

            if (!Directory.Exists(home))
                Directory.CreateDirectory(home);
        }
    }
}
