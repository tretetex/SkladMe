using SkServ.Model;

namespace SkServ.Categories
{
    public class DataCategory
    {
        private readonly ServerApiClient _apiClient;

        public DataCategory(ServerApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public DataDict Get()
        {
            return _apiClient.Call("data.get");
        }
    }
}
