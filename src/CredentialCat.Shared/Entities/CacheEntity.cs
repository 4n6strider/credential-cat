﻿using System;
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
        [Required] public SecretTypeEnum SecretType { get; set; }
        public string SourceReferenceId { get; set; }
        public string Secret { get; set; }
        public string Domain { get; set; }
        public string Origin { get; set; }
        public DateTime WhenHappened { get; set; }
        [Required] public DateTime IndexedAt { get; set; }
    }
}
