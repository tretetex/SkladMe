using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace SkladMe.Resources.Converters
{
    public class IntegerOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (string) value;
            if (!int.TryParse(str, out int result))
            {
                var match = Regex.Match(str, @"[0-9]+").Value;
                return match == string.Empty ? null : match;
            }
            return result;
        }
    }
}