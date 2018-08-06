namespace SkladchikGet.ServerApi.Categories
{
    public class AuthCategory
    {
        private readonly ServerApiClient _serverApi;

        public AuthCategory(ServerApiClient serverApi)
        {
            _serverApi = serverApi;
        }

        public bool CheckLicense(string key, string hw)
        {
            var parameters = $"key={key}&hw={hw}";
            return _serverApi.Call("auth.checkLicense", parameters);
        }
    }
}
