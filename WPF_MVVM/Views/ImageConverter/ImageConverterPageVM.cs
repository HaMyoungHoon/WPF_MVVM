using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.AI.MachineLearning;
using WPF_MVVM.Bases;
using WPF_MVVM.Controls.ImageViews;
using WPF_MVVM.Helpers;
using WPF_MVVM.Models.ImageConverter;
using WPF_MVVM.Models.Messages;

namespace WPF_MVVM.Views.ImageConverter
{
    internal partial class ImageConverterPageVM : PaneDocumentViewModel
    {
        private bool _convertAble;
        private bool _zoomAble;
        [ObservableProperty]
        private ObservableCollection<ImageConverterItem> _imageConverterItemList;
        [ObservableProperty]
        private int _gridColumnsCount;
        [ObservableProperty]
        private double _imageHeight;
        public ImageConverterPageVM() : base()
        {
            Title = "ImageConverter";
            ContentID = nameof(ImageConverterPageVM);
            _convertAble = false;
            _zoomAble = false;
            _imageConverterItemList = new();
            _gridColumnsCount = 3;
            _imageHeight = 200;

            OpenCommand = new RelayCommand(OpenEvent);
            ConvertCommand = new RelayCommand(ConvertEvent, CanConvert);
            MouseWheelCommand = new RelayCommand<object?>(MouseWheelEvent);
            GetMediaItemCaptureCommand = new RelayCommand<object?>(GetMediaItemCaptureEvent);
            KeyDownCommand = new RelayCommand<object?>(KeyDownEvent);
            KeyUpCommand = new RelayCommand<object?>(KeyUpEvent);
        }

        public IRelayCommand OpenCommand { get; }
        public IRelayCommand ConvertCommand { get; }
        public IRelayCommand MouseWheelCommand { get; }
        public IRelayCommand GetMediaItemCaptureCommand { get; }
        public IRelayCommand KeyDownCommand { get; }
        public IRelayCommand KeyUpCommand { get; }

        protected override void CloseEvent(object? data)
        {
            WeakReferenceMessenger.Default.Send(new NavigationMessage(nameof(ImageConverterPage)) { Sender = this, Close = true });
            base.CloseEvent(data);
        }
        public override void OnNavigated(object? sender, object? e)
        {
            ImageConverterItemList.Clear();
        }
        public override void OnNavigating(object? sender, object? e)
        {
        }

        private void OpenEvent()
        {
            OpenFileDialog ofd = new()
            {
//                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
//                Filter = "사진(*.jpg;*.png;*.gif;*.bmp;*.webp)|*.jpg;*.png;*.gif;*.bmp;*.webp",
                Title = "열기",
                Multiselect = true,
            };
            if (ofd.ShowDialog() == true)
            {
                var array = (ofd.FileNames, ofd.SafeFileNames);
                if (array.FileNames.Length < 3)
                {
                    GridColumnsCount = array.FileNames.Length;
                }
                else
                {
                    GridColumnsCount = 3;
                }

                List<ImageConverterItem> temp = new();
                for (int i = 0; i < array.FileNames.Length; i++)
                {
                    try
                    {
                        temp.Add(new ImageConverterItem()
                        {
                            FilePath = array.FileNames[i],
                        });
                    }
                    catch (Exception ex)
                    {
                        WeakReferenceMessenger.Default.Send(new AlertMessage(true) { Header = "image load", Message = ex.Message ?? "" });
                    }
                }
                ImageConverterItemList = new(temp);
            }

            _convertAble = ImageConverterItemList.Count > 0;
            ConvertCommand.NotifyCanExecuteChanged();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
        private void ConvertEvent()
        {
//            ImageConverterItemList.Clear();
//            _convertAble = ImageConverterItemList.Count > 0;
//            ConvertCommand.NotifyCanExecuteChanged();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
        private void MouseWheelEvent(object? data)
        {
            if (data == null || data is not (ListBox sender, MouseWheelEventArgs e))
            {
                return;
            }
            if (ImageConverterItemList.Count <= 0) 
            {
                return;
            }
            if (_zoomAble == false)
            {
                return;
            }

            if (e.Delta > 0)
            {
                if (ImageHeight > 501)
                {
                    return;
                }

                ImageHeight += 10;
            }
            else
            {
                if (ImageHeight < 51)
                {
                    return;
                }

                ImageHeight -= 10;
            }
        }
        private void KeyDownEvent(object? data)
        {
            if (data is not (ListBox sender, KeyEventArgs e))
            {
                return;
            }

            if (e.Key == Key.LeftCtrl)
            {
                _zoomAble = true;
            }
        }
        private void KeyUpEvent(object? data)
        {
            if (data is not (ListBox sender, KeyEventArgs e))
            {
                return;
            }

            if (e.Key == Key.LeftCtrl)
            {
                _zoomAble = false;
            }
        }

        private void GetMediaItemCaptureEvent(object? data)
        {
            if (data == null || data is not RenderTargetBitmap renderBitmap)
            {
                return;
            }


        }

        private bool CanConvert()
        {
            return _convertAble;
        }
    }
}
