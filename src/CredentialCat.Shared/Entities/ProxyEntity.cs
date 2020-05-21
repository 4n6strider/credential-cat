using System;
using System.Net;

namespace CredentialCat.Shared.Entities
{
    /// <summary>
    /// Represent the default value for <see cref="WebProxy"/> constructor
    /// </summary>
    public class ProxyEntity
    {
        /// <summary>
        /// Proxy unique identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Proxy address, can be an <see cref="Uri"/> or <see cref="IPAddress"/>
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Port for given <see cref="Address"/>
        /// </summary>
        public int Port { get; set; }
    }
}