using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF_MVVM.Converters
{
    internal class StringToDateTimeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return value;
            }
            if (DateTime.TryParse(value.ToString(), out DateTime ret))
            {
                return ret;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
