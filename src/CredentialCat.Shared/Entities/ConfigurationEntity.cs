using System;
using System.Collections.Generic;

namespace CredentialCat.Shared.Entities
{
    /// <summary>
    /// Represent application general configuration
    /// </summary>
    public class ConfigurationEntity
    {
        /// <summary>
        /// List of Tor HTTP proxies addresses
        /// </summary>
        /// <para>Default proxy entity is 127.0.0.1:8118</para>
        public ICollection<ProxyEntity> Proxies { get; set; } = new List<ProxyEntity>(new List<ProxyEntity>
        {
            new ProxyEntity
            {
                Address = "127.0.0.1",
                Port = 8118
            }
        });

        /// <summary>
        /// Set if the application ignore the local cache
        /// </summary>
        public bool OnlyFreshResults { get; set; } = false;

        /// <summary>
        /// Set the application max time for a credential cache
        /// </summary>
        public TimeSpan MaxCache { get; set; } = TimeSpan.FromDays(31);
    }
}
