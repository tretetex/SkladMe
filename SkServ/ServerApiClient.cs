using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;
using SkServ.Categories;
using SkServ.Model.RequestParams;
using SkServ.Utils;
using ApiResponse = SkServ.Utils.ApiResponse;

namespace SkServ
{
    public class ServerApiClient
    {
        public const string Host = "http://ck32221.tmweb.ru/api/";
        private readonly Http.Http _http;
        private readonly RequestParameters _requestParameters;

        public AuthCategory Auth { get; set; }
        public AppCategory App { get; set; }
        public DataCategory Data { get; set; }
        public NoticeCategory Notice { get; set; }

        public ServerApiClient(CommonRequestParams @params, CancellationToken cancellationToken = default(CancellationToken))
        {
            _http = new Http.Http(cancellationToken);
            _requestParameters = CommonRequestParams.ToRequestParameters(@params);

            Auth = new AuthCategory(this);
            App = new AppCategory(this);
            Data = new DataCategory(this);
            Notice = new NoticeCategory(this);
        }

        public ApiResponse Call(string methodName, RequestParameters parameters = null)
        {
            if (parameters == null)
            {
                parameters = new RequestParameters();
            }

            foreach (var parameter in _requestParameters)
            {
                parameters.Add(parameter.Key, parameter.Value);
            }
            
            var response = Invoke(methodName, parameters);

            var json = JObject.Parse(response);
            var rawResponse = json["response"];

            return new ApiResponse(rawResponse) { RawJson = response };
        }

        private string Invoke(string methodName, IDictionary<string, string> parameters)
        {
            var url = Host + methodName;
            var source = _http.GetAsync(url, parameters).Result;
            ServerErrors.IfErrorThrowException(source);
            return source;
        }
    }
}
