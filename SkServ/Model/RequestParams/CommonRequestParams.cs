using SkServ.Utils;

namespace SkServ.Model.RequestParams
{
    public struct CommonRequestParams
    {
        public string Key { get; set; }
        public string Hw { get; set; }

        public static RequestParameters ToRequestParameters(CommonRequestParams p)
        {
            var parameters = new RequestParameters
            {
                { "key", p.Key },
                { "hw", p.Hw },
            };

            return parameters;
        }
    }
}
