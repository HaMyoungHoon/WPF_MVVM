using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace WPF_MVVM.Converters
{
    internal class MenuItemCommandConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<object> list = new();
            foreach (object value in values)
            {
                list.Add(value);
            }
            return list;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
