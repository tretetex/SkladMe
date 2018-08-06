using System;
using ApiResponse = SkServ.Utils.ApiResponse;

namespace SkServ.Model
{
    public class UpdateInfo
    {
        public string Version { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public DateTime Date { get; set; }

        public static UpdateInfo FromJson(ApiResponse response)
        {
            return new UpdateInfo
            {
                Version = response["version"],
                Description = response["description"],
                Link = response["link"],
                Date = response["date"]
            };
        }
    }
}
