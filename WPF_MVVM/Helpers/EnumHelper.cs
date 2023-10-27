using System;
using System.ComponentModel;
using System.Reflection;

namespace WPF_MVVM.Helpers
{
    internal static class EnumHelper
    {
        public static string GetDescription(this Enum? value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());
            try
            {
                return fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && 0 < attributes.Length ? attributes[0].Description : value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
