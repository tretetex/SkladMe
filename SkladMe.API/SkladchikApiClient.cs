using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SkladMe.API.Methods;
using SkladMe.API.Utils;

namespace SkladMe.API
{
    public class SkladchikApiClient
    {
        private readonly Web _web;
        private readonly CancellationToken _cts;

        public SkladchikApiClient(CancellationToken cts)
        {
            _cts = cts;
            Users = new Users(this);
            Products = new Products(this);
            Chapters = new Chapters(this);

            _web = new Web();
        }

        public const string Domain = "https://skladchik.com/";
        public const string ThreadsUrl = Domain + "threads/";
        public const string ForumsUrl = Domain + "forums/";

        public CookieContainer CookieContainer { get; set; }
        public Users Users { get; }
        public Products Products { get; }
        public Chapters Chapters { get; }
     
        public async Task<string> CallAsync(string url)
        {
            var source = await _web.HttpGetInCycleAsync(url, CookieContainer, _cts).ConfigureAwait(false);
            return source;
        }

        public void ThrowIfCancellationRequested()
        {
            _cts.ThrowIfCancellationRequested();
        }
    }
}
