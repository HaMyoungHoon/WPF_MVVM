using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF_MVVM.Converters
{
    internal class BoolToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool ret || ret == false)
            {
                return 0;
            }
            if (parameter is not string str || double.TryParse(str, out double retVal) == false)
            {
                return 0;
            }

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
