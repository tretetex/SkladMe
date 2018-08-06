using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SkladchikGet.ServerApi.Utils
{
    /// <summary>
    /// ������
    /// </summary>
    public class ApiResponseArray : IEnumerable<ApiResponse>
    {
        /// <summary>
        /// ������
        /// </summary>
        private readonly JArray _array;

        /// <summary>
        /// ������������� ������ �������.
        /// </summary>
        /// <param name="array">������.</param>
        public ApiResponseArray(JArray array)
        {
            _array = array;
        }

        /// <summary>
        /// ����� ApiResponse
        /// </summary>
        /// <value>
        /// The ApiResponse
        /// </value>
        /// <param name="key">����.</param>
        /// <returns>������� ������</returns>
        public ApiResponse this[object key]
        {
            get
            {
                var token = _array[key];
                return new ApiResponse(token);
            }
        }

        /// <summary>
        /// ����������.
        /// </summary>
        /// <value>
        /// ����������.
        /// </value>
        public int Count => _array.Count;

        /// <summary>
        /// ���������� �������������, ����������� �������� � ���������.
        /// </summary>
        /// <returns>
        /// ��������� T:System
        /// </returns>
        public IEnumerator<ApiResponse> GetEnumerator() => _array.Select(i => new ApiResponse(i)).GetEnumerator();

        /// <summary>
        /// ���������� �������������, ������� ������������ ������� ��������� ���������.
        /// </summary>
        /// <returns>
        /// ������ T:System
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
