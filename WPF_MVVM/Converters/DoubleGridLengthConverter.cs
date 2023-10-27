using System;
using System.Windows;
using System.Windows.Data;

namespace WPF_MVVM.Converters
{
    internal class DoubleGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double[] data && parameter != null)
            {
                if (int.TryParse(parameter.ToString(), out int index))
                {
                    return new GridLength(data[index]);
                }
                return new GridLength(data[0]);
            }
            return new GridLength((double)value);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is GridLength gl)
            {
                return gl.Value;
            }
            GridLength gridLength = new GridLength((double)value);
            return gridLength.Value;
        }
    }
}
