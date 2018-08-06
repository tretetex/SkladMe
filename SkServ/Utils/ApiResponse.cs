using System;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json.Linq;

namespace SkServ.Utils
{
    public partial class ApiResponse
    {
        private readonly JToken _token;
        public string RawJson { get; set; }

        public ApiResponse(JToken token)
        {
            _token = token;
        }

		public bool ContainsKey(string key)
        {
            if (!(_token is JObject))
            {
                return false;
            }

            var token = _token[key];
            return token != null;
        }

        public ApiResponse this[object key]
        {
            get
            {
                if (_token is JArray && key is string)
                {
                    return null;
                }

                var token = _token[key];
                return token != null ? new ApiResponse(_token[key]) : null;
            }
        }

        public static implicit operator ApiResponseArray(ApiResponse response)
        {
            if (response == null)
            {
                return null;
            }

            var resp = response.ContainsKey("items") ? response["items"] : response;

            var array = resp._token as JArray;
            return array == null ? null : new ApiResponseArray(array);
        }

        public override string ToString() => _token.ToString();

        #region System types

        public static implicit operator bool(ApiResponse response) => response != null && response == 1;

        public static implicit operator bool? (ApiResponse response) => response == null ? (bool?)null : response == 1;

        public static implicit operator long(ApiResponse response) => (long) response._token;

        public static implicit operator long? (ApiResponse response) => response != null ? (long?)response._token : null;

        public static implicit operator ulong(ApiResponse response) => (ulong) response._token;

        public static implicit operator ulong?(ApiResponse response) => response != null ? (ulong?) response._token : null;

        public static implicit operator Collection<long>(ApiResponse response) => response?.ToCollectionOf<long>(i => i);

        public static implicit operator float(ApiResponse response) => (float)response._token;

        public static implicit operator float? (ApiResponse response) => response != null ? (float?)response._token : null;

        public static implicit operator decimal(ApiResponse response) => (decimal)response._token;

        public static implicit operator decimal? (ApiResponse response) => response != null ? (decimal?)response._token : null;

        public static implicit operator uint(ApiResponse response) => (uint)response._token;

        public static implicit operator uint? (ApiResponse response) => response != null ? (uint?)response._token : null;

        public static implicit operator int(ApiResponse response) => (int) response._token;

        public static implicit operator int? (ApiResponse response) => response != null ? (int?)response._token : null;

        public static implicit operator string(ApiResponse response) => response == null ? null : WebUtility.HtmlDecode((string)response._token);

        public static implicit operator Collection<string>(ApiResponse response) => response.ToCollectionOf<string>(s => s);

        public static implicit operator DateTime? (ApiResponse response)
        {
            var dateStringValue = response?.ToString();
            DateTime dateTime;
            if (string.IsNullOrWhiteSpace(dateStringValue) ||
                !DateTime.TryParse(dateStringValue, out dateTime))
            {
                return null;
            }
            return dateTime;
        }

        public static implicit operator DateTime(ApiResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var dateStringValue = response.ToString();
            if (string.IsNullOrWhiteSpace(dateStringValue))
            {
                throw new ArgumentException("ѕустое значение невозможно преобразовать в дату", nameof(response));
            }

            DateTime dateTime;
            if (!DateTime.TryParse(dateStringValue, out dateTime))
            {
                throw new ArgumentException("Ќевозможно преобразовать в дату", nameof(response));
            }
            return dateTime;
        }
        
        public static implicit operator Uri(ApiResponse response)
        {
            Uri uriResult;

            return Uri.TryCreate(response, UriKind.Absolute, out uriResult) ? uriResult : null;
        }

        #endregion
    }
}