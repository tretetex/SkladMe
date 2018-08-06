using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace SkladMe.Resources.Converters
{
    public class DoubleOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result;
            var str = (string) value;
            if (!double.TryParse(str.Replace(".", ","), out result))
            {
                var match = Regex.Match(str, @"\d+\.{0,1}\d*").Value.Replace(".", ",");
                return match == string.Empty ? null : match;
            }
            return result;
        }
    }
}