using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF_MVVM.Converters
{
    internal class SubStringFormatConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return value;
            }
            if (parameter is string[] array)
            {
                if (int.TryParse(array[1], out int intArray) == false)
                {
                    return value;
                }

                return array[0] switch
                {
                    "S" => value?.ToString()?.Length >= intArray ? value?.ToString()?[intArray..] : value,
                    "E" => value?.ToString()?.Length >= intArray ? value?.ToString()?[..intArray] : value,
                    _ => value
                };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
