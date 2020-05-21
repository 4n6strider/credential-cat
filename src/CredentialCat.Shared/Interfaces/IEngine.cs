using System.Linq;
using System.Threading.Tasks;
using CredentialCat.Shared.Entities;

namespace CredentialCat.Shared.Interfaces
{
    /// <summary>
    /// Implement CredentialCat engine contract
    /// </summary>
    public interface IEngine
    {
        Task<IQueryable<CacheEntity>> SearchByUserOrEmail(string value, bool ignoreCache, bool ignoreUpdate, bool forceUpdate, bool caseSensitive, int timeout);
        Task<IQueryable<CacheEntity>> SearchByUserOrEmailWithWordlist(string wordListPath, bool ignoreCache, bool ignoreUpdate, bool forceUpdate, bool caseSensitive, int timeout);
        Task<IQueryable<CacheEntity>> SearchByPasswordOrHash(string value, bool ignoreCache, bool ignoreUpdate, bool forceUpdate, bool caseSensitive, int timeout);
        Task<IQueryable<CacheEntity>> SearchByPasswordOrHashWithWordList(string wordListPath, bool ignoreCache, bool ignoreUpdate, bool forceUpdate, bool caseSensitive, int timeout);
    }
}