using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}
