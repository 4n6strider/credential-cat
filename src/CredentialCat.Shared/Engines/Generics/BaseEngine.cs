using CredentialCat.Shared.Entities;

namespace CredentialCat.Shared.Engines.Generics
{
    /// <summary>
    /// Provides all the generic need for create new source engines for credential-cat
    /// </summary>
    public class BaseEngine
    {
        public DatabaseContext Context;
        public ConfigurationEntity Configuration;

        /// <summary>
        /// Default constructor, create a new instance of <see cref="BaseEngine"/>
        /// </summary>
        /// <param name="databaseContext">Shared library created with current general database context</param>
        /// <param name="configuration">Current configuration for credential-cat</param>
        public BaseEngine(DatabaseContext databaseContext, ConfigurationEntity configuration)
        {
            Context = databaseContext;
            Configuration = configuration;
        }
    }
}
