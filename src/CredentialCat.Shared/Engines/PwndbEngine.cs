using System;
using System.Web;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Mail;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using static System.Console;

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
        private readonly HttpClientHandler _clientHandler;

        public PwndbEngine(DatabaseContext databaseContext, ConfigurationEntity configuration) : base(databaseContext,
            configuration)
        {
            _clientHandler = new HttpClientHandler();
            _httpClient = new HttpClient(_clientHandler);
        }

        public async Task<IQueryable<CacheEntity>> SearchByUserOrEmail(string value, bool ignoreCache,
            bool ignoreUpdate, bool forceUpdate, bool caseSensitive,
            int timeout, int limit, bool bypassProxy)
        {
            HttpResponseMessage response;
            string content;

            SetupProxy(bypassProxy);

            if (!IsEmailAddress(value ?? throw new ArgumentNullException(nameof(value))))
            {
                response = await _httpClient.SendAsync(GetSkeletonRequestMessage(
                    $"luser={HttpUtility.UrlEncode(value)}&domain=&luseropr=0&domainopr=0&submitform=em"));

                if (!response.IsSuccessStatusCode)
                {
                    WriteLine(
                        "[!] A problem occurred doing the request to Pwndb website, check if your proxy is up and running, and is a HTTP Tor one!");
                    WriteLine("[+] Also can be a issue with Pwndb, open a issue in github.com/BizarreNULL/credential-cat");
                    WriteLine($"[+] Error: {response.StatusCode}");

                    Environment.Exit(1);
                }

                content = await response.Content.ReadAsStringAsync();

                var entities = PwndbResponseParser(content)
                    .Where(e => e.Secret != "12cC7BdkBbru6JGsWvTx4PPM5LjLX8g49X");



                return null;
            }

            var emailAddress = new MailAddress(value);

            try
            {
                response = await _httpClient.SendAsync(GetSkeletonRequestMessage($"luser={HttpUtility.UrlEncode(emailAddress.User)}&domain={HttpUtility.UrlEncode(emailAddress.Host)}&luseropr=0&domainopr=0&submitform=em"));

                if (!response.IsSuccessStatusCode)
                {
                    WriteLine(
                        "[!] A problem occurred doing the request to Pwndb website, check if your proxy is up and running, and is a HTTP Tor one!");
                    WriteLine("[+] Also can be a issue with Pwndb, open a issue in github.com/BizarreNULL/credential-cat");
                    WriteLine($"[+] Error: {response.StatusCode}");

                    Environment.Exit(1);
                }
                 
                content = await response.Content.ReadAsStringAsync();

                var entities = PwndbResponseParser(content);

                return null;
            }

            catch (Exception e)
            {
                WriteLine(
                    "[!] A problem occurred doing the request to Pwndb website, check if your proxy is up and running, and is a HTTP Tor one!");
                WriteLine("[+] Also can be a issue with Pwndb, open a issue in github.com/BizarreNULL/credential-cat");
                WriteLine($"[+] Error: {e}");

                return null;
            }
        }

        public Task<IQueryable<CacheEntity>> SearchByDomain(string value, bool ignoreCache, bool ignoreUpdate, bool forceUpdate, bool caseSensitive,
            int timeout, int limit, bool bypassProxy)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<CacheEntity>> SearchByDomainWordlist(string wordListPath, bool ignoreCache, bool ignoreUpdate, bool forceUpdate,
            bool caseSensitive, int timeout, int limit, bool bypassProxy)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<CacheEntity>> SearchByUserOrEmailWithWordlist(string wordListPath, bool ignoreCache,
            bool ignoreUpdate, bool forceUpdate,
            bool caseSensitive, int timeout, int limit, bool bypassProxy)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<CacheEntity>> SearchByPasswordOrHash(string value, bool ignoreCache, bool ignoreUpdate,
            bool forceUpdate, bool caseSensitive,
            int timeout, int limit, bool bypassProxy)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<CacheEntity>> SearchByPasswordOrHashWithWordList(string wordListPath, bool ignoreCache,
            bool ignoreUpdate, bool forceUpdate,
            bool caseSensitive, int timeout, int limit, bool bypassProxy)
        {
            throw new NotImplementedException();
        }

        #region Helpers

        private static IEnumerable<CacheEntity> PwndbResponseParser(string content) =>
            content.Split("Array")
                .Where(d => d.Contains("=>"))
                .SelectMany(d => d.Split("\n"))
                .Where(d => d.Contains("=>"))
                .Select((x, i) => new {Index = i, Value = x})
                .GroupBy(x => x.Index / 4)
                .Select(x => x.Select(v => v.Value).ToList())
                .Select(e =>
                {
                    e = e.Select(s => s.Trim()).ToList();

                    return new CacheEntity
                    {
                        Domain = e.First(s => s.StartsWith("[domain]")).Replace("[domain] => ", string.Empty),
                        SourceReferenceId = e.First(s => s.StartsWith("[id]")).Replace("[id] => ", string.Empty),
                        User = e.First(s => s.StartsWith("[luser]")).Replace("[luser] => ", string.Empty),
                        Secret = e.First(s => s.StartsWith("[password]")).Replace("[password] => ", string.Empty),
                        SecretType = SecretTypeEnum.Mixed
                    };
                });

        private static HttpRequestMessage GetSkeletonRequestMessage(string content) =>
            new HttpRequestMessage(HttpMethod.Post, new Uri("http://pwndb2am4tzkvold.onion/"))
            {
                Content = new StringContent(content ?? throw new ArgumentNullException(nameof(content)), Encoding.UTF8,
                    "application/x-www-form-urlencoded")
            };

        private static bool IsEmailAddress(string email)
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
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
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

        private void SetupProxy(bool bypass)
        {
            if (bypass) return;
            var proxyEntity = Configuration.Proxies.First(p => p.Id == Configuration.DefaultProxyId);
            var proxy = new WebProxy(proxyEntity.Address, proxyEntity.Port);

            _clientHandler.UseProxy = true;
            _clientHandler.Proxy = proxy;
        }

        #endregion
    }
}
 