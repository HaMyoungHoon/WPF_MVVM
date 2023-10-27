using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_MVVM.Interfaces
{
    internal interface IWindowService
    {
        public void ShowWindow<T>(object? dataContext, bool close = false, bool hide = false, object? sender = null, object? navigatedEventArgs = null) where T : Window, new();
        public void CloseAll();
        public object? GetDataContext(object? dataContext);
        public object? GetWindow(object? dataContext);
        public object? GetWindow(object? dataContext, object? contentSource);
    }
}
