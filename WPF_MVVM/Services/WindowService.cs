using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPF_MVVM.Interfaces;
using WPF_MVVM.Views.Popup;

namespace WPF_MVVM.Services
{
    internal class WindowService : IWindowService
    {
        private SafeHandle _cs;
        private List<Window> _windows;
        public WindowService()
        {
            _windows = new();
            _cs = new SafeFileHandle(IntPtr.Zero, true);
        }
        public void ShowWindow<T>(object? dataContext, bool close = false, bool hide = false, object? sender = null, object? navigatedEventArgs = null) where T : Window, new()
        {
            if (dataContext == null)
            {
                return;
            }

            if (close)
            {
                var closeBuff = _windows.Find(x => x.DataContext.ToString() == dataContext?.ToString());
                if (closeBuff?.DataContext is IPopupWindow popUpVM && popUpVM.Sender != null && popUpVM.Sender != sender)
                {
                    return;
                }
                closeBuff?.Close();
                return;
            }

            if (hide)
            {
                var hideBuff = _windows.Find(x => x.DataContext.ToString() == dataContext?.ToString());
                if (hideBuff?.DataContext is IPopupWindow popUpVM && popUpVM.Sender != null && popUpVM.Sender != sender)
                {
                    return;
                }
                hideBuff?.Hide();
                return;
            }

            var buff = _windows.Find(x => x.DataContext == dataContext);
            if (buff != null)
            {
                buff.DataContext = dataContext;
                if (buff.DataContext is INavigationAware buffedVM)
                {
                    buffedVM.OnNavigated(sender, navigatedEventArgs);
                }
                buff.Focus();
                return;
            }
            Window window = new T
            {
                DataContext = dataContext
            };
            window.Closed += Window_Closed;
            _windows.Add(window);
            window.Show();

            if (window.DataContext is INavigationAware vm)
            {
                vm.OnNavigated(sender, navigatedEventArgs);
            }
        }

        public object? GetDataContext(object? dataContext)
        {
            var buff = GetWindow(dataContext) as Window;
            if (buff != null)
            {
                buff.Focus();
                return buff.DataContext;
            }

            return null;
        }
        public object? GetWindow(object? dataContext)
        {
            var buff = _windows.Find(x => x.DataContext.ToString() == dataContext?.ToString());
            if (buff != null)
            {
                return buff;
            }

            return null;
        }
        public object? GetWindow(object? dataContext, object? contentSource)
        {
            var buff = _windows.FindAll(x => x.DataContext.ToString() == dataContext?.ToString());
            if (buff.Count <= 0)
            {
                return null;
            }
            if (contentSource is not string contentSourceString)
            {
                return null;
            }

            foreach (var items in buff)
            {
                if (items.DataContext is ContentsViewerWindowVM vm && vm.ContentSoruce == contentSourceString)
                {
                    return items;
                }
            }

            return null;
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            if (sender is not Window window)
            {
                return;
            }

            if (window.DataContext is INavigationAware vm)
            {
                vm.OnNavigating(sender, null);
            }

            var buff = _windows.Find(x => x.DataContext == window.DataContext);
            if (buff != null)
            {
                _windows.Remove(buff);
                buff.Closed -= Window_Closed;
            }
        }

        public void CloseAll()
        {
            lock (_cs)
            {
                _windows.ForEach(x =>
                {
                    try
                    {
                        x.Closed -= Window_Closed;
                        x.Close();
                    }
                    catch
                    {

                    }
                });
            }
        }
    }
}
