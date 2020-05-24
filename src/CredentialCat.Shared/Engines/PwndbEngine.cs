using System;
using System.Web;
using System.Data;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Mail;
using System.Globalization;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using CredentialCat.Shared.Enums;
using CredentialCat.Shared.Entities;
using CredentialCat.Shared.Interfaces;
using CredentialCat.Shared.Engines.Generics;

namespace CredentialCat.Shared.Engines
{
    /// <summary>
    /// Implements search capabilities for <see cref="SourceEnum.Pwndb"/>
    /// </summary>
    public class PwndbEngine : BaseEngine, IEngine
    {
        private readonly HttpClient _httpClient;

        public PwndbEngine(DatabaseContext databaseContext, ConfigurationEntity configuration) : base(databaseContext, configuration)
        {
            _httpClient = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = false
            });
        }

        public async Task<IQueryable<CacheEntity>> SearchByUserOrEmail(string value, bool ignoreCache, bool ignoreUpdate, bool forceUpdate, bool caseSensitive,
            int timeout, int limit, bool bypassProxy)
        {
            HttpResponseMessage response;
            string content;

            if (!IsEmailAddress(value ?? throw new NoNullAllowedException(nameof(value))))
            {
                response = await _httpClient.SendAsync(GetSkeletonRequestMessage($"luser={HttpUtility.UrlEncode(value)}&domain=&luseropr=0&domainopr=0&submitform=em"));
                content = await response.Content.ReadAsStringAsync();

                return null;
            }

            var emailAddress = new MailAddress(value);

            response = await _httpClient.SendAsync(GetSkeletonRequestMessage($"luser={HttpUtility.UrlEncode(emailAddress.User)}&domain={HttpUtility.UrlEncode(emailAddress.Host)}&luseropr=0&domainopr=0&submitform=em"));
            content = await response.Content.ReadAsStringAsync();

            return null;
        }

        public Task<IQueryable<CacheEntity>> SearchByUserOrEmailWithWordlist(string wordListPath, bool ignoreCache, bool ignoreUpdate, bool forceUpdate,
            bool caseSensitive, int timeout, int limit, bool bypassProxy)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<CacheEntity>> SearchByPasswordOrHash(string value, bool ignoreCache, bool ignoreUpdate, bool forceUpdate, bool caseSensitive,
            int timeout, int limit, bool bypassProxy)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<CacheEntity>> SearchByPasswordOrHashWithWordList(string wordListPath, bool ignoreCache, bool ignoreUpdate, bool forceUpdate,
            bool caseSensitive, int timeout, int limit, bool bypassProxy)
        {
            throw new NotImplementedException();
        }

        #region Helpers

        private async Task<CacheEntity> PwndbResponseParser(string content)
        {
            throw new NotImplementedException();
        }

        private HttpRequestMessage GetSkeletonRequestMessage(string content) => new HttpRequestMessage(HttpMethod.Post, new Uri("http://pwndb2am4tzkvold.onion/"))
        {
            Content = new StringContent(content ?? throw new ArgumentNullException(nameof(content)), Encoding.UTF8, "application/x-www-form-urlencoded")
        };

        public static bool IsEmailAddress(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            static string DomainMapper(Match match)
            {
                var idn = new IdnMapping();

                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                    RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        #endregion
    }
}
