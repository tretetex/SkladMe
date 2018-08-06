using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SkServ.Http
{
    public class Http
    {
        private readonly CancellationToken _cancellationToken;
        private readonly HttpClient _client;

        public Http(CancellationToken cancellationToken = default(CancellationToken))
        {
            _cancellationToken = cancellationToken;
            _client = new HttpClient();
        }

        public async Task<string> GetAsync(string url, IDictionary<string, string> parameters)
        {
            var stringParams = string.Empty;
            foreach (var parameter in parameters)
            {
                stringParams += parameter.Key + "=" + parameter.Value + "&";
            }

            url = url + "?" + stringParams;
            _cancellationToken.ThrowIfCancellationRequested();
            var response = await _client.GetAsync(url, _cancellationToken).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return content;
        }
    }
}
