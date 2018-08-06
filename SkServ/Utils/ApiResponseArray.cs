using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SkServ.Utils
{
    public class ApiResponseArray : IEnumerable<SkServ.Utils.ApiResponse>
    {
        private readonly JArray _array;

        public ApiResponseArray(JArray array)
        {
            _array = array;
        }
        
        public SkServ.Utils.ApiResponse this[object key]
        {
            get
            {
                var token = _array[key];
                return new SkServ.Utils.ApiResponse(token);
            }
        }
        
        public int Count => _array.Count;
        
        public IEnumerator<SkServ.Utils.ApiResponse> GetEnumerator() => _array.Select(i => new SkServ.Utils.ApiResponse(i)).GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
