using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WPF_MVVM.Converters
{
    internal class EnumToDescriptionConverter<T> : EnumConverter where T : struct
    {
        private static readonly Dictionary<T, string> _descriptions = new();

        public EnumToDescriptionConverter() : base(typeof(T))
        {
            Init();
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType != typeof(string))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            if (value == null)
            {
                return string.Empty;
            }

            if ((value is T) == false)
            {
                return value.ToString();
            }

            T item = (T)value;

            return _descriptions.ContainsKey(item) == false ? value.ToString() : _descriptions[item];
        }

        private static void Init()
        {
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                System.Reflection.FieldInfo? fi = item.GetType().GetField(item.ToString() ?? "NULL");

                if (fi == null && !_descriptions.ContainsKey(item))
                {
                    _descriptions.Add(item, item.ToString() ?? "NULL");
                    continue;
                }

                DescriptionAttribute[]? attributes = fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

                if (attributes?.Length < 1 && !_descriptions.ContainsKey(item))
                {
                    _descriptions.Add(item, item.ToString() ?? "NULL");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(attributes?[0].Description) && !_descriptions.ContainsKey(item))
                {
                    _descriptions.Add(item, item.ToString() ?? "NULL");
                    continue;
                }

                if (!_descriptions.ContainsKey(item))
                {
                    _descriptions.Add(item, attributes?[0].Description ?? "NULL");
                }
            }
        }
    }
}
