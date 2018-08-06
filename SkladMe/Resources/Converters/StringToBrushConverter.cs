using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SkladMe.Resources.Converters
{
    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
            {
                return new SolidColorBrush(Colors.Transparent);
            }
            var convertFromString = ColorConverter.ConvertFromString(value.ToString());
            if (convertFromString != null)
                return new SolidColorBrush((Color) convertFromString);

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }
    }
}