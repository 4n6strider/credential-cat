using System;
using CredentialCat.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace CredentialCat.Shared
{
    /// <summary>
    /// Represent current application cache SQlite database instance
    /// </summary>
    public class DatabaseContext : DbContext
    {
        private readonly string _connectionString;

        /// <summary>
        /// Cached searches to any data source
        /// </summary>
        public DbSet<CacheEntity> CacheEntities { get; set; }

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}