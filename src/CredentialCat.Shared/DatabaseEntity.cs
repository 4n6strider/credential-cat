using System;
using Microsoft.EntityFrameworkCore;

namespace CredentialCat.Shared
{
    /// <summary>
    /// Represent current application cache SQlite database instance
    /// </summary>
    public class DatabaseEntity : DbContext
    {
        private readonly string _connectionString;

        public DatabaseEntity(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}