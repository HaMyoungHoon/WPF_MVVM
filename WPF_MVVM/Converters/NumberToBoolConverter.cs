using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF_MVVM.Converters
{
    internal class NumberToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            if (value?.ToString()?.Trim()?.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
