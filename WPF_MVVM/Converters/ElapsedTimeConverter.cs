using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF_MVVM.Converters
{
    internal class ElapsedTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string buff)
            {
                return value;
            }

            if (DateTime.TryParse(buff, out DateTime recvDate))
            {
                return Math.Round((DateTime.Now - recvDate).TotalMinutes);

            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
