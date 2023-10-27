using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF_MVVM.Converters
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (boolValue)
                {
                    return TrueValue;
                }
                else
                {
                    return FalseValue;
                }
            }
            if (value is int intValue)
            {
                if (intValue == 0)
                {
                    return TrueValue;
                }
                else
                {
                    return FalseValue;
                }
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
    }
}
