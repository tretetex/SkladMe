using System;
using System.Collections.Generic;
using System.Linq;

namespace SkServ.Utils
{
    public static class Utilities
    {
        /// <summary>
		/// Преобразовать в перечисление из числа.
		/// </summary>
		/// <typeparam name="T">Тип перечисления.</typeparam>
		/// <param name="value">Числовое значение.</param>
		/// <returns>Перечисление указанного типа.</returns>
		/// <exception cref="System.ArgumentException">value</exception>
		public static T EnumFrom<T>(int value)
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                throw new ArgumentException($"Enum value {value} not defined!", nameof(value));
            }

            return (T)(object)value;
        }

        /// <summary>
        /// Преобразовать в перечисление из числа.
        /// </summary>
        /// <typeparam name="T">Тип перечисления.</typeparam>
        /// <param name="value">Числовое значение.</param>
        /// <returns>Перечисление указанного типа.</returns>
        public static T? NullableEnumFrom<T>(int value) where T : struct
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                return null;
            }

            return (T)(object)value;
        }

        /// <summary>
        /// Получение идентификатора.
        ///
        /// Применять когда id может быть задано как строкой так и числом в json'e.
        /// </summary>
        /// <param name="response">Ответ от сервера vk.com</param>
        /// <returns>Число типа long или null</returns>
        public static long? GetNullableLongId(SkServ.Utils.ApiResponse response)
        {
            return !string.IsNullOrWhiteSpace(response?.ToString()) ? System.Convert.ToInt64(response.ToString()) : (long?)null;
        }

        /// <summary>
		/// Объединить не пустую коллекцию.
		/// </summary>
		/// <typeparam name="T">Тип коллекции.</typeparam>
		/// <param name="collection">Коллекция.</param>
		/// <param name="separator">Разделитель.</param>
		/// <returns>Строковое представление коллекции через разделитель.</returns>
		public static string JoinNonEmpty<T>(this IEnumerable<T> collection, string separator = ",")
        {
            if (collection == null)
            {
                return string.Empty;
            }

            return string.Join(
                separator,
                collection.Select(i => i.ToString().Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s)
                    ));
        }
    }
}
