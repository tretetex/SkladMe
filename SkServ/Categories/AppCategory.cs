using System;
using System.Collections.Generic;
using SkServ.Model;
using SkServ.Utils;

namespace SkServ.Categories
{
    public class AppCategory
    {
        private readonly ServerApiClient _apiClient;

        public AppCategory(ServerApiClient apiClient)
        {
            _apiClient = apiClient; 
        }

        public UpdateInfo CheckUpdate(Version currentVersion)
        {
            var requestParams = new RequestParameters {{"v", currentVersion.ToString()}};
            return _apiClient.Call("app.checkUpdate", requestParams);
        }

        public List<UpdateInfo> GetAllUpdates()
        {
            var response = _apiClient.Call("app.getAllUpdates");
            var updates = new List<UpdateInfo>(response.ToListOf<UpdateInfo>(x => x));
            return updates;
        }
    }
}
