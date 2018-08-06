using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SkServ.Utils
{
	public static class ApiResponseExt
    {
        public static Collection<T> ToCollection<T>(this IEnumerable<T> source)
        {
            return new Collection<T>(new List<T>(source));
        }

        public static Collection<T> ToCollectionOf<T>(this SkServ.Utils.ApiResponse response, Func<SkServ.Utils.ApiResponse, T> selector) //where T : class
        {
            if (response == null)
            {
                return new Collection<T>(new List<T>());
            }

            var responseArray = (ApiResponseArray)response;
            if (responseArray == null) //TODO: V3022 http://www.viva64.com/en/w/V3022 Expression 'responseArray == null' is always false.
            {
                return new Collection<T>(new List<T>());
            }

            return responseArray.Select(selector).Where(i => i != null).ToCollection(); //TODO: V3111 http://www.viva64.com/en/w/V3111 Checking value of 'i' for null will always return false when generic type is instantiated with a value type.
        }

        public static Collection<T> ToCollectionOf<T>(this IEnumerable<SkServ.Utils.ApiResponse> responses, Func<SkServ.Utils.ApiResponse, T> selector)
        {
            if (responses == null)
            {
                return new Collection<T>(new List<T>());
            }

            return responses.Select(selector).ToCollection();
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            return new ReadOnlyCollection<T>(new List<T>(source));
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollectionOf<T>(
            this SkServ.Utils.ApiResponse response, Func<SkServ.Utils.ApiResponse, T> selector) where T : class
        {
            if (response == null)
            {
                return new ReadOnlyCollection<T>(new List<T>());
            }

            var responseArray = (ApiResponseArray)response;
            if (responseArray == null) //TODO: V3022 http://www.viva64.com/en/w/V3022 Expression 'responseArray == null' is always false.
            {
                return new ReadOnlyCollection<T>(new List<T>());
            }

            return responseArray.Select(selector).Where(i => i != null).ToReadOnlyCollection();
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollectionOf<T>(this IEnumerable<SkServ.Utils.ApiResponse> responses, Func<SkServ.Utils.ApiResponse, T> selector)
        {
            if (responses == null)
            {
                return new ReadOnlyCollection<T>(new List<T>());
            }

            return responses.Select(selector).ToReadOnlyCollection();
        }

        public static List<T> ToListOf<T>(this SkServ.Utils.ApiResponse response, Func<SkServ.Utils.ApiResponse, T> selector)
        {
            if (response == null)
            {
                return new List<T>();
            }

            var responseArray = (ApiResponseArray)response;
            if (responseArray == null) //TODO: V3022 http://www.viva64.com/en/w/V3022 Expression 'responseArray == null' is always false.
            {
                return new List<T>();
            }

            return responseArray.Select(selector).Where(i => i != null).ToList(); //TODO: V3111 http://www.viva64.com/en/w/V3111 Checking value of 'i' for null will always return false when generic type is instantiated with a value type.
        }

        public static List<T> ToListOf<T>(this IEnumerable<SkServ.Utils.ApiResponse> responses, Func<SkServ.Utils.ApiResponse, T> selector)
        {
            return responses?.Select(selector).ToList() ?? new List<T>();
        }

        public static ApiCollection<T> ToApiCollectionOf<T>(this SkServ.Utils.ApiResponse response, Func<SkServ.Utils.ApiResponse, T> selector)
        {
            if (response == null)
            {
                return new ApiCollection<T>(0, Enumerable.Empty<T>());
            }

            var data = response.ContainsKey("items") ? response["items"] : response;
            var resultCollection = data.ToCollectionOf(selector);
            var totalCount = response.ContainsKey("count") ? response["count"] : (ulong)resultCollection.Count;

            return new ApiCollection<T>(totalCount, resultCollection);
        }
    }
}
