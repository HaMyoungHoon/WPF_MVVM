using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WPF_MVVM.Helpers;
using WPF_MVVM.Models.Common;

namespace WPF_MVVM.Converters
{
    [ValueConversion(typeof(Enum), typeof(IEnumerable<ValueDescription>))]
    internal class EnumToCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!value.GetType().IsEnum)
            {
                throw new ArgumentException($"{nameof(value)} must be an enum type");
            }

            return Enum.GetValues(value.GetType()).Cast<Enum>().Select((e) => new ValueDescription() { Value = e, Description = e.GetDescription() }).ToList();
        }
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
