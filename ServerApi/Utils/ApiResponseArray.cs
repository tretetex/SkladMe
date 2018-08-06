using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SkladchikGet.ServerApi.Utils
{
    /// <summary>
    /// Массив
    /// </summary>
    public class ApiResponseArray : IEnumerable<ApiResponse>
    {
        /// <summary>
        /// Массив
        /// </summary>
        private readonly JArray _array;

        /// <summary>
        /// Инициализация нового массива.
        /// </summary>
        /// <param name="array">Массив.</param>
        public ApiResponseArray(JArray array)
        {
            _array = array;
        }

        /// <summary>
        /// Взять ApiResponse
        /// </summary>
        /// <value>
        /// The ApiResponse
        /// </value>
        /// <param name="key">Ключ.</param>
        /// <returns>Текущий объект</returns>
        public ApiResponse this[object key]
        {
            get
            {
                var token = _array[key];
                return new ApiResponse(token);
            }
        }

        /// <summary>
        /// Количество.
        /// </summary>
        /// <value>
        /// Количество.
        /// </value>
        public int Count => _array.Count;

        /// <summary>
        /// Возвращает перечислитель, выполняющий итерацию в коллекции.
        /// </summary>
        /// <returns>
        /// Интерфейс T:System
        /// </returns>
        public IEnumerator<ApiResponse> GetEnumerator() => _array.Select(i => new ApiResponse(i)).GetEnumerator();

        /// <summary>
        /// Возвращает перечислитель, который осуществляет перебор элементов коллекции.
        /// </summary>
        /// <returns>
        /// Объект T:System
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
