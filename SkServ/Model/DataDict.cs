using System.Collections.Generic;
using System.Linq;
using SkServ.Utils;
using ApiResponse = SkServ.Utils.ApiResponse;

namespace SkServ.Model
{
    public class DataDict
    {
        public Dictionary<string, string> Products { get; set; }
        public Dictionary<string, string> Chapters { get; set; }

        public static DataDict FromJson(ApiResponse response)
        {
            var products = response["Products"].ToCollectionOf(x => x);
            var chapters = response["Chapters"].ToCollectionOf(x => x);

            return new DataDict
            {
                Products = products.ToDictionary(x => x["key"].ToString(), x => x["value"].ToString()),
                Chapters = chapters.ToDictionary(x => x["key"].ToString(), x => x["value"].ToString())
            };
        }
    }
}
