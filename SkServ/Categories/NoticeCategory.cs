using System.Collections.Generic;
using SkServ.Model;
using SkServ.Utils;

namespace SkServ.Categories
{
    public class NoticeCategory
    {
        private readonly ServerApiClient _apiClient;

        public NoticeCategory(ServerApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public List<NoticeInfo> Get()
        {
            var response = _apiClient.Call("notice.get");
            return response.ToListOf<NoticeInfo>(o => o);
        }
    }
}
