using System;
using System.Collections.Generic;
using CredentialCat.Shared.Enums;

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
        public ICollection<ProxyEntity> Proxies { get; set; }

        /// <summary>
        /// Set if the application ignore the local cache
        /// </summary>
        public bool OnlyFreshResults { get; set; } = false;

        /// <summary>
        /// Set the application max time for a credential cache
        /// </summary>
        public TimeSpan MaxCache { get; set; } = TimeSpan.FromDays(31);

        /// <summary>
        /// Set default application data source for leak enumeration
        /// </summary>
        public SourceEnum DefaultSource { get; set; } = SourceEnum.Pwndb;
    }
}
