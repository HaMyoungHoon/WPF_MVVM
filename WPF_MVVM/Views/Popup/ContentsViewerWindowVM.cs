using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;
using WPF_MVVM.Bases;
using WPF_MVVM.Interfaces;
using WPF_MVVM.Models.Messages;

namespace WPF_MVVM.Views.Popup
{
    internal partial class ContentsViewerWindowVM : ViewModelBase, IPopupWindow
    {
        public object? Sender { get; set; }
        [ObservableProperty]
        private string _contentSoruce;
        [ObservableProperty]
        private bool _isImage;
        [ObservableProperty]
        private BitmapImage? _imageItem;
        private Window? _window;
        public ContentsViewerWindowVM()
        {
            _contentSoruce = string.Empty;

            MouseDownCommand = new RelayCommand<object?>(MouseDownEvent);
            MouseWheelCommand = new RelayCommand<object?>(MouseWheelEvent);
            KeyDownCommand = new RelayCommand<object?>(KeyDownEvent);
        }
        public IRelayCommand MouseDownCommand { get; }
        public IRelayCommand MouseWheelCommand { get; }
        public IRelayCommand KeyDownCommand { get; }

        public override void OnNavigating(object? sender, object? navigationEventArgs)
        {
        }
        public override void OnNavigated(object? sender, object? navigatedEventArgs)
        {
            Sender = sender;
            if (navigatedEventArgs is string contentSoruce)
            {
                ContentSoruce = contentSoruce;
                //                SetContentSource();
            }
        }
        public void SetWindow(object? data)
        {
            if (data is Window window)
            {
                _window = window;
            }
        }
        private void MouseDownEvent(object? data)
        {
            if (data == null)
            {
                return;
            }

            if (data is not (Window sender, MouseButtonEventArgs e))
            {
                return;
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                _window = sender;
                _window.DragMove();
            }
        }
        private void MouseWheelEvent(object? data)
        {
            if (data == null)
            {
                return;
            }
            if (data is not (Window sender, MouseWheelEventArgs e))
            {
                return;
            }
            _window = sender;
            if (e.Delta > 0)
            {
                if (_window.Width > 1000 || _window.Height > 1000)
                {
                    return;
                }

                _window.Width += 30;
                _window.Height += 30;
            }
            else
            {
                if (_window.Width <= 200 || _window.Height <= 200)
                {
                    return;
                }

                _window.Width -= 30;
                _window.Height -= 30;
            }
        }
        private void KeyDownEvent(object? data)
        {
            if (data is not (object sender, KeyEventArgs args))
            {
                return;
            }

            if (args.Key == Key.Escape)
            {
                WeakReferenceMessenger.Default.Send(new NewWindowMessage(nameof(ContentsViewerWindow)) { Sender = Sender, NavigatedEventArgs = ContentSoruce, Close = true });
            }
        }

        private void SetContentSource()
        {
            if (IsContentsImage())
            {
                SetImage();
                return;
            }
            if (IsContentsVideo())
            {
                SetVideo();
                return;
            }
        }
        private void SetImage()
        {
            ImageItem = FBaseFunc.LoadImage(ContentSoruce);
        }
        private void SetVideo()
        {

        }
        private bool IsContentsImage()
        {
            foreach (var filter in FBaseFunc.Ins.Cfg.ImageFilter)
            {
                if (ContentSoruce.Contains(filter) == true)
                {
                    IsImage = true;
                    return true;
                }
            }

            return false;
        }
        private bool IsContentsVideo()
        {
            foreach (var filter in FBaseFunc.Ins.Cfg.VideoFilter)
            {
                if (ContentSoruce.Contains(filter) == true)
                {
                    IsImage = false;
                    return true;
                }
            }

            return false;
        }
    }
}
