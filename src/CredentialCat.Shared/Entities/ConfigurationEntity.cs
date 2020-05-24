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
        public ICollection<ProxyEntity> Proxies { get; set; } = new List<ProxyEntity>();

        /// <summary>
        /// Default proxy ID for generic communication
        /// </summary>
        public string DefaultProxyId { get; set; }

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
