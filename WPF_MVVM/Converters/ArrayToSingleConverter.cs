using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF_MVVM.Converters
{
    internal class ArrayToSingleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(parameter.ToString(), out int index) == false)
            {
                return value;
            }

            if (value is int[] intData)
            {
                return intData[index];
            }
            if (value is double[] doubleData)
            {
                return doubleData[index];
            }
            if (value is string[] stringData)
            {
                return stringData[index];
            }
            if (value is object[] objectData)
            {
                return objectData[index];
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
