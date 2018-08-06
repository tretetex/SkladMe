using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using SkladMe.ViewModel;

namespace SkladMe.Resources.Converters
{
    class SelectedItemsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var arr = new object[2];

            var list = value as List<ProductVM>;

            arr[0] = list;
            arr[1] = list;
            return arr;
        }
    }
}
