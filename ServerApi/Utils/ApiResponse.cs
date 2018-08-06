using System;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json.Linq;

namespace SkladchikGet.ServerApi.Utils
{
    public class ApiResponse
    {
        private readonly JToken _token;
        public string RawJson { get; set; }

        public ApiResponse(JToken token)
        {
            _token = token;
        }

        /// <summary>
		/// ����������, �������� �� JSON ��������� ����.
		/// </summary>
		/// <param name="key">����.</param>
		/// <returns>������� ������� ����� � JSON</returns>
		public bool ContainsKey(string key)
        {
            if (!(_token is JObject))
            {
                return false;
            }

            var token = _token[key];
            return token != null;
        }

        /// <summary>
        /// �������� ������ �� ���������� �����.
        /// </summary>
        /// <param name="key">����.</param>
        /// <returns>������</returns>
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

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
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

        /// <summary>
        /// ������������� ������ � ������.
        /// </summary>
        /// <returns>
        /// ��������� ������������� �������.
        /// </returns>
        public override string ToString() => _token.ToString();

        #region System types

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator bool(ApiResponse response) => response != null && response == 1;

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator bool? (ApiResponse response)
        {
            return response == null ? (bool?)null : response == 1;
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator long(ApiResponse response)
        {
            return (long)response._token;
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator long? (ApiResponse response)
        {
            return response != null ? (long?)response._token : null;
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator ulong(ApiResponse response)
        {
            return (ulong)response._token;
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator ulong? (ApiResponse response)
        {
            return response != null ? (ulong?)response._token : null;
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator Collection<long>(ApiResponse response)
        {
            return response?.ToCollectionOf<long>(i => i);
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator float(ApiResponse response)
        {
            return (float)response._token;
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator float? (ApiResponse response)
        {
            return response != null ? (float?)response._token : null;
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator decimal(ApiResponse response)
        {
            return (decimal)response._token;
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator decimal? (ApiResponse response)
        {
            return response != null ? (decimal?)response._token : null;
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator uint(ApiResponse response)
        {
            return (uint)response._token;
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator uint? (ApiResponse response)
        {
            return response != null ? (uint?)response._token : null;
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator int(ApiResponse response)
        {
            return (int)response._token;
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator int? (ApiResponse response)
        {
            return response != null ? (int?)response._token : null;
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator string(ApiResponse response)
        {
            return response == null ? null : WebUtility.HtmlDecode((string)response._token);
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator Collection<string>(ApiResponse response)
        {
            return response.ToCollectionOf<string>(s => s);
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator DateTime? (ApiResponse response)
        {
            var dateStringValue = response?.ToString();
            long unixTimeStamp;
            if (string.IsNullOrWhiteSpace(dateStringValue) ||
                (!long.TryParse(dateStringValue, out unixTimeStamp) || unixTimeStamp <= 0))
            {
                return null;
            }

            // Unix Timestamps is seconds past epoch
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dt.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator DateTime(ApiResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var dateStringValue = response.ToString();
            if (string.IsNullOrWhiteSpace(dateStringValue))
            {
                throw new ArgumentException("������ �������� ���������� ������������� � ����", nameof(response));
            }

            long unixTimeStamp;
            if (!long.TryParse(dateStringValue, out unixTimeStamp) || unixTimeStamp <= 0)
            {
                throw new ArgumentException("���������� ������������� � ����", nameof(response));
            }

            // Unix Timestamps is seconds past epoch
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dt.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        /// <summary>
        /// ��������� ������� �������������� �� ApiResponse
        /// </summary>
        /// <param name="response">����� vk.com</param>
        /// <returns>
        /// ��������� ��������������.
        /// </returns>
        public static implicit operator Uri(ApiResponse response)
        {
            Uri uriResult;

            return Uri.TryCreate(response, UriKind.Absolute, out uriResult) ? uriResult : null;
        }

        #endregion
    }
}