using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SkladMe.API;
using SkladMe.Exception;

namespace SkladMe.Infrastructure
{
    static class Cookies
    {   
        public static List<Cookie> AllCookies = new List<Cookie>();
        public static bool IsCookieExist(CookieContainer cookieContainer, string cookieName = "__cfduid")
        {
            bool isFindedCookie = false;
            foreach (Cookie cookie in cookieContainer.GetCookies(new Uri(SkladchikApiClient.Domain)))
            {
                if (cookie.Name == cookieName)
                {
                    isFindedCookie = true;
                    break;
                }
            }
            return isFindedCookie;
        }

        public static CookieContainer GetSkladchikCookie()
        {
            var container = new CookieContainer();
            foreach (var cookie in AllCookies)
            {
                container.Add(new Uri(SkladchikApiClient.Domain), new Cookie(cookie.Name, cookie.Value));
            }
            return container;
        }

        private static int _httpRequestExceptionCount;
        public static async Task<bool> IsNeedGetCookie()
        {
            try
            {
                var web = new API.Utils.Web();
                var httpResult = await web.HttpGetAsync(SkladchikApiClient.Domain).ConfigureAwait(false);
                _httpRequestExceptionCount = 0;
            }
            catch (WebException)
            {
                try
                {
                    var cookieContainer = GetSkladchikCookie();
                    bool isFindedCookie = IsCookieExist(cookieContainer);
                    if (!isFindedCookie) throw new CookiesException("Нет нужных куков.");
                }
                catch (System.Exception)
                {
                    return true;
                }
            }
            catch (HttpRequestException e)
            {
                ++_httpRequestExceptionCount;
                if(_httpRequestExceptionCount > 10) return true;
                return await IsNeedGetCookie().ConfigureAwait(false);
            }
            return false;
        }
    }
}
