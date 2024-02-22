using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPF_MVVM.Bases;
using WPF_MVVM.Models.ImageConverter;
using WPF_MVVM.Models.Messages;

namespace WPF_MVVM.Views.ImageConverter
{
    internal partial class ImageConverterPageVM : PaneDocumentViewModel
    {
        private bool _convertAble;
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
            _imageConverterItemList = new();
            _gridColumnsCount = 4;
            _imageHeight = 100;

            OpenCommand = new RelayCommand(OpenEvent);
            ConvertCommand = new RelayCommand(ConvertEvent, CanConvert);
            MouseWheelCommand = new RelayCommand<object?>(MouseWheelEvent);
        }

        public IRelayCommand OpenCommand { get; }
        public IRelayCommand ConvertCommand { get; }
        public IRelayCommand MouseWheelCommand { get; }

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
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "사진(*.jpg;*.png;*.gif;*.bmp;*.webp)|*.jpg;*.png;*.gif;*.bmp;*.webp",
                Title = "열기",
                Multiselect = true,
            };
            if (ofd.ShowDialog() == true)
            {
                var array = (ofd.FileNames, ofd.SafeFileNames);
                List<ImageConverterItem> temp = new();
                for (int i = 0; i < array.FileNames.Length; i++)
                {
                    try
                    {
                        BitmapImage image = GetBitmapImage(array.FileNames[i]);
                        temp.Add(new ImageConverterItem()
                        {
                            Index = i,
                            FileName = array.SafeFileNames[i],
                            Image = image,
                        });
                    }
                    catch
                    {

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
            ImageConverterItemList.Clear();
            _convertAble = ImageConverterItemList.Count > 0;
            ConvertCommand.NotifyCanExecuteChanged();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
        private void MouseWheelEvent(object? data)
        {
            if (data == null || data is not (ListView sender, MouseWheelEventArgs e))
            {
                return;
            }
            if (ImageConverterItemList.Count <= 0) 
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
        private bool CanConvert()
        {
            return _convertAble;
        }

        private BitmapImage GetBitmapImage(string filePath)
        {
            var header = new byte[12];
            using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read))
            {
                fs.Read(header, 0, 12);
            }
            if (IsWebp(header))
            {
                return GetWebpImage(filePath);
            }
            else
            {
                BitmapImage ret = new();
                ret.BeginInit();
                ret.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
                ret.EndInit();
                return ret;
            }
        }
        private bool IsWebp(byte[] header)
        {
            return header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                   header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50;
        }
        private BitmapImage GetWebpImage(string filePath)
        {
            BitmapImage ret = new();
            using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(filePath))
            {
                using MemoryStream ms = new();
                image.SaveAsBmp(ms);
                ms.Position = 0;
                ret.BeginInit();
                ret.CacheOption = BitmapCacheOption.OnLoad;
                ret.StreamSource = ms;
                ret.EndInit();
            }
            return ret;
        }
    }
}
