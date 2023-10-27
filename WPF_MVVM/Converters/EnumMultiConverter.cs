using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WPF_MVVM.Converters
{
    internal class EnumMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string[] array)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    object? buff = array[i] switch
                    {
//                        "OrderLinkMethod" => (Enum.TryParse(value[i].ToString(), out OrderLinkMethod ret) ? ret : null),
//                        "OrderRoute" => (Enum.TryParse(value[i].ToString(), out OrderRoute ret) ? ret : null),
                        _ => null,
                    };
                    if (buff == null)
                    {
                        continue;
                    }
                    value[i] = (buff?.GetType()?.GetField(buff.ToString() ?? "")?.GetCustomAttributes(typeof(DescriptionAttribute), false).First() as DescriptionAttribute)?.Description ?? value[i];
                }
                return value;
            }

            return value;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
