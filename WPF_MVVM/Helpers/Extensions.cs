using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows;

namespace WPF_MVVM.Helpers
{
    internal static class Extensions
    {
        public static IList<T>? Clone<T>(this IList<T>? list) where T : ICloneable
        {
            if (list == null)
            {
                return null;
            }
            return list.Select(x => (T)x.Clone()).ToList();
        }

        public static DependencyObject? GetChild<T>(DependencyObject? child)
        {
            if (child is T || child == null)
            {
                return child;
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(child);
            if (childrenCount <= 0)
            {
                return null;
            }

            for (int i = 0; i < childrenCount; i++)
            {
                var childBuff = VisualTreeHelper.GetChild(child, i);
                if (childBuff is T || childBuff == null)
                {
                    return childBuff;
                }
                var temp = GetChild<T>(childBuff);
                if (temp is T)
                {
                    return temp;
                }
            }

            return child;
        }

        public static EnumValue? GetEnumValue<EnumValue, EnumType>(EnumType enumData) where EnumType : Enum
        {
            var enumName = Enum.GetName(typeof(EnumType), enumData);

            if (enumName == null)
            {
                return default;
            }

            if (Enum.TryParse(typeof(EnumType), enumName, true, out object? enumValue))
            {
                return (EnumValue)enumValue;
            }


            return default;
        }
    }
}
