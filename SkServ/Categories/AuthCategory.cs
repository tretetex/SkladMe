using SkServ.Model;

namespace SkServ.Categories
{
    public class AuthCategory
    {
        private readonly ServerApiClient _apiClient;

        public AuthCategory(ServerApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public LicenseInfo CheckLicense()
        {
            return _apiClient.Call("auth.checkLicense");
        }

        public LicenseInfo SetLicense()
        {
            return _apiClient.Call("auth.setLicense");
        }
    }
}
