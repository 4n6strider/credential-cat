using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using CredentialCat.Shared.Enums;

namespace CredentialCat.Shared.Entities
{
    [Table("CachedSearches")]
    public class CacheEntity
    {
        [Key] public string Id { get; set; }
        [Required] public SourceEnum Source { get; set; }
        [Required] public string User { get; set; }
        [Required] public bool IsSecretHashed { get; set; }
        public string Secret { get; set; }
        public string Domain { get; set; }
        [Required] public DateTime IndexedAt { get; set; }
    }
    `
}
