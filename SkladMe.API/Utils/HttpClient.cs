using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SkladMe.API.Utils
{
    public class Web
    {
        public static string UserAgent { get; set; } =
            "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0";

        public async Task<string> HttpGetAsync(string url,
            CookieContainer cookieContainer = default(CookieContainer),
            CancellationToken token = default(CancellationToken))
        {
            if (cookieContainer is null)
            {
                cookieContainer = new CookieContainer();
            }

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 10,
                CookieContainer = cookieContainer
            };

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", UserAgent);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
                client.Timeout = TimeSpan.FromSeconds(5);

                using (var response = await client.GetAsync(url, token).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                    using (var streamReader = new StreamReader(decompressedStream))
                    {
                        var result = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                        return result;
                    }
                }
            }
        }

        private static int commonTimeout;
        private static object timeoutSync = new object();

        public async Task<string> HttpGetInCycleAsync(string url, CookieContainer cookieContainer = null,
            CancellationToken token = default(CancellationToken))
        {
            string source = null;
            var ioExceptionsCount = 0;
            var requestCount = 0;
            var maxRequestCount = 50;

            do
            {
                if (requestCount >= maxRequestCount)
                {
                    return null;
                }

                ++requestCount;
                try
                {
                    source = await HttpGetAsync(url, cookieContainer, token).ConfigureAwait(false);

                    lock (timeoutSync)
                    {
                        commonTimeout = 1;
                    }

                }
                catch (IOException)
                {
                    Interlocked.Increment(ref commonTimeout);

                    if (ioExceptionsCount == 20)
                        throw;
                    await Task.Delay(50, token).ConfigureAwait(false);
                    ioExceptionsCount++;
                }
                catch (WebException ex)
                {
                    Interlocked.Increment(ref commonTimeout);

                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var response = ex.Response as HttpWebResponse;
                        if (response == null)
                        {
                            continue;
                        }

                        var code = (int) response.StatusCode;
                        if (code == 503 || code == 524) //524 - origin timeout, 503 - Service Temporarily Unavailable
                        {
                            int randomTimeout = new Random().Next(5, 15) * 1000;
                            await Task.Delay(randomTimeout, token).ConfigureAwait(false);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }

                    if (commonTimeout > 60)
                    {
                        commonTimeout = 60;
                    }
                    await Task.Delay(commonTimeout * 1000, token).ConfigureAwait(false);
                }
                catch (HttpRequestException hre)
                {
                    var isUnavailable = hre.Message.Contains("503") || hre.Message.Contains("504");
                    if (isUnavailable)
                    {
                        int randomTimeout = new Random().Next(5, 15) * 1000;
                        await Task.Delay(randomTimeout, token).ConfigureAwait(false);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (TaskCanceledException)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                }
            } while (source == null);

            return source;
        }


        private async Task<string> Get(string url, int timeout = 5000, CookieContainer cookieContainer = null,
            CancellationToken token = default(CancellationToken))
        {
            string source = null;
            try
            {
                source = await HttpGetAsync(url, cookieContainer, token).ConfigureAwait(false);

                lock (timeoutSync)
                {
                    commonTimeout = 1;
                }
            }

            catch (TimeoutException)
            {
                Interlocked.Increment(ref commonTimeout);
                await Task.Delay(50, token).ConfigureAwait(false);
            }
            catch (WebException ex)
            {
                Interlocked.Increment(ref commonTimeout);

                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;

                    var code = (int) response.StatusCode;
                    if (code == 503 || code == 524) //524 - origin timeout, 503 - Service Temporarily Unavailable
                    {
                        int randomTimeout = new Random().Next(5, 15) * 1000;
                        await Task.Delay(randomTimeout, token).ConfigureAwait(false);
                    }
                    else
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }

                if (commonTimeout > 60)
                {
                    commonTimeout = 60;
                }
                await Task.Delay(commonTimeout * 1000, token).ConfigureAwait(false);
            }

            return source;
        }
    }
}