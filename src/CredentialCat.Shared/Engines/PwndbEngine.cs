using System.Linq;
using System.Threading.Tasks;

using CredentialCat.Shared.Enums;
using CredentialCat.Shared.Entities;
using CredentialCat.Shared.Interfaces;

namespace CredentialCat.Shared.Engines
{
    /// <summary>
    /// Implements search capabilities for <see cref="SourceEnum.Pwndb"/>
    /// </summary>
    public class PwndbEngine : IEngine
    {
        private readonly DatabaseContext _context;

        public PwndbEngine(DatabaseContext databaseContext)
        {
            _context = databaseContext;
        }

        public Task<IQueryable<CacheEntity>> SearchByUserOrEmail(string value, bool ignoreCache, bool ignoreUpdate, bool forceUpdate, bool caseSensitive,
            int timeout, int limit, bool bypassProxy)
        {
            throw new System.NotImplementedException();
        }

        public Task<IQueryable<CacheEntity>> SearchByUserOrEmailWithWordlist(string wordListPath, bool ignoreCache, bool ignoreUpdate, bool forceUpdate,
            bool caseSensitive, int timeout, int limit, bool bypassProxy)
        {
            throw new System.NotImplementedException();
        }

        public Task<IQueryable<CacheEntity>> SearchByPasswordOrHash(string value, bool ignoreCache, bool ignoreUpdate, bool forceUpdate, bool caseSensitive,
            int timeout, int limit, bool bypassProxy)
        {
            throw new System.NotImplementedException();
        }

        public Task<IQueryable<CacheEntity>> SearchByPasswordOrHashWithWordList(string wordListPath, bool ignoreCache, bool ignoreUpdate, bool forceUpdate,
            bool caseSensitive, int timeout, int limit, bool bypassProxy)
        {
            throw new System.NotImplementedException();
        }
    }
}
