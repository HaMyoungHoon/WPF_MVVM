using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF_MVVM.Converters
{
    internal class MultiBoolToVisibilityConverter : IMultiValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool IsALLTrue = true;
            if (parameter is string conditions)
            {
                switch (conditions.ToUpper())
                {
                    case "ALL": break;
                    default: IsALLTrue = false; break;
                }
            }

            foreach (var items in values)
            {
                if (items is not bool state)
                {
                    continue;
                }

                if (IsALLTrue && state == false)
                {
                    return FalseValue;
                }
                if (IsALLTrue == false && state == true)
                {
                    return TrueValue;
                }
            }

            if (IsALLTrue)
            {
                return TrueValue;
            }
            else
            {
                return FalseValue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { TrueValue, FalseValue };
        }
    }
}
