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
        public Task<IQueryable<CacheEntity>> SearchByUserOrEmail(string value, bool ignoreCache, bool ignoreUpdate, bool forceUpdate, bool caseSensitive)
        {
            throw new System.NotImplementedException();
        }

        public Task<IQueryable<CacheEntity>> SearchByUserOrEmailWithWordlist(string wordListPath, bool ignoreCache, bool ignoreUpdate, bool forceUpdate,
            bool caseSensitive)
        {
            throw new System.NotImplementedException();
        }

        public Task<IQueryable<CacheEntity>> SearchByPasswordOrHash(string value, bool ignoreCache, bool ignoreUpdate, bool forceUpdate, bool caseSensitive)
        {
            throw new System.NotImplementedException();
        }

        public Task<IQueryable<CacheEntity>> SearchByPasswordOrHashWithWordList(string wordListPath, bool ignoreCache, bool ignoreUpdate, bool forceUpdate,
            bool caseSensitive)
        {
            throw new System.NotImplementedException();
        }
    }
}
