using SkladchikGet.Infrastructure;
using SkladchikGet.ServerApi.Categories;
using SkladchikGet.ServerApi.Utils;

namespace SkladchikGet.ServerApi
{
    public class ServerApiClient
    {
        private const string Host = "http://sk.loc/api/";
        private readonly Web _web;

        public AuthCategory Auth { get; set; }

        public ServerApiClient()
        {
            _web = new Web();
            Auth = new AuthCategory(this);
        }

        public ApiResponse Call(string methodName, string parameters)
        {
            var response = Invoke(methodName, parameters);
            return new ApiResponse(response) { RawJson = response };
        }

        private string Invoke(string methodName, string parameters)
        {
            var url = Host + methodName + "?" + parameters;
            var source = _web.HttpGetInCycle(url);
            return source;
        }
    }
}
