using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_MVVM.Interfaces
{
    internal interface INavigationAware
    {
        void OnNavigating(object? sender, object? e);
        void OnNavigated(object? sender, object? e);
    }
}
