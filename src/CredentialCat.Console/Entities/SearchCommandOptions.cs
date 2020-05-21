namespace CredentialCat.Console.Entities
{
    /// <summary>
    /// Represent all the options for search command"/>
    /// </summary>
    public class SearchCommandOptions
    {
        /// <summary>
        /// If the query will be ignore the cache database
        /// </summary>
        public bool IgnoreCache { get; set; }
        
        /// <summary>
        /// If the cache database update will be ignored
        /// </summary>
        public bool IgnoreUpdate { get; set; }
        
        /// <summary>
        /// If the cache database will be forced to update 
        /// </summary>
        public bool ForceUpdate { get; set; }

        /// <summary>
        /// If case sensitive will be used
        /// </summary>
        public bool CaseSensitive { get; set; }

        /// <summary>
        /// If the default proxy will be ignored
        /// </summary>
        public bool BypassProxy { get; set; }

        /// <summary>
        /// Max time for get an answer of the engine
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Max entities to return from a query
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Path with the extension of the file to be exported
        /// </summary>
        public string Export { get; set; }

        /// <summary>
        /// Password or hash to be searched
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Password and/or hash wordlist to be searched
        /// </summary>
        public string PasswordList { get; set; }

        /// <summary>
        /// User, username or email address
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// User, username or email address wordlist
        /// </summary>
        public string UserList { get; set; }

        /// <summary>
        /// If present, search on cache database with specific origin
        /// </summary>
        public string Origin { get; set; }
    }
}
