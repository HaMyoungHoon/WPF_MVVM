using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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
using System.Windows.Controls.Primitives;
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
            ConvertCommand = new AsyncRelayCommand<object?>(ConvertEvent, CanConvert);
            MouseWheelCommand = new RelayCommand<object?>(MouseWheelEvent);
            KeyDownCommand = new RelayCommand<object?>(KeyDownEvent);
            KeyUpCommand = new RelayCommand<object?>(KeyUpEvent);
        }

        public IRelayCommand OpenCommand { get; }
        public IRelayCommand ConvertCommand { get; }
        public IRelayCommand MouseWheelCommand { get; }
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
                foreach (var list in ImageConverterItemList)
                {
                    list.FilePath = null;
                }
                OnPropertyChanged(nameof(ImageConverterItemList));
                ImageConverterItemList.Clear();
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
                            FileName = array.SafeFileNames[i]
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

        private async Task ConvertEvent(object? data)
        {
            if (data is not ListBox listBox)
            {
                return;
            }

            WeakReferenceMessenger.Default.Send(new LoadingMessage(true) { });
            List<ImageViewUC> list = GetImageList(listBox);
            list.ForEach(x =>
            {
                x.Pause();
            });
            List<ImagePixelDataModel> bitmapList = new();
            list.ForEach(x =>
            {
                var temp = x.GetCurrentImage();
                if (temp != null)
                {
                    byte[] pixels = new byte[temp.PixelWidth * temp.PixelHeight * 4];
                    temp.CopyPixels(pixels, temp.PixelWidth * 4, 0);
                    bitmapList.Add(new ImagePixelDataModel(pixels, temp.PixelWidth, temp.PixelHeight, temp.DpiX, temp.DpiY, temp.Format, temp.Palette));
                }
            });
            var safeFileNames = list.Select(x =>
            {
                if (Path.GetExtension(x.ImageSafeFileName).Length == 0)
                {
                    return x.ImageSafeFileName + ".jpg";
                }
                else
                {
                    return string.Concat(x.ImageSafeFileName.AsSpan(0, x.ImageSafeFileName.Length - 4), ".jpg");
                }
            }).ToList();

            await Task.Run(() =>
            {
                string path = $@"{FBaseFunc.Ins.Cfg.ImageConvertSaveDirPath}\{DateTime.Now:yyyyMMdd}";
                DirectoryInfo info = new(path);
                if (info.Exists == false)
                {
                    info.Create();
                }
                for (int i = 0; i < bitmapList.Count; i++)
                {
                    path = $@"{FBaseFunc.Ins.Cfg.ImageConvertSaveDirPath}\{DateTime.Now:yyyyMMdd}\{safeFileNames[i]}";
                    path = GetUniqueFilePath(path);
                    WriteableBitmap bitmap = new(bitmapList[i].Width, bitmapList[i].Height, bitmapList[i].DpiX, bitmapList[i].DpiY, bitmapList[i].Format, bitmapList[i].Palette);
                    bitmap.WritePixels(new Int32Rect(0, 0, bitmapList[i].Width, bitmapList[i].Height), bitmapList[i].Pixels, bitmapList[i].Width * 4, 0);
                    JpegBitmapEncoder encoder = new();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    using FileStream fileStream = new(path, FileMode.Create);
                    encoder.Save(fileStream);
                }

                WeakReferenceMessenger.Default.Send(new AlertMessage(true) { Header = "Converting", Message = "Comp" });
                WeakReferenceMessenger.Default.Send(new LoadingMessage(false) { });
            });
            //            ImageConverterItemList.Clear();
            //            _convertAble = ImageConverterItemList.Count > 0;
            //            ConvertCommand.NotifyCanExecuteChanged();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
        private void MouseWheelEvent(object? data)
        {
            if (data == null || data is not (ListBox, MouseWheelEventArgs e))
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

        private bool CanConvert(object? data)
        {
            return _convertAble;
        }

        private List<ImageViewUC> GetImageList(ListBox listBox)
        {
            List<ImageViewUC> ret = new();
            var listBoxChildCount = VisualTreeHelper.GetChildrenCount(listBox);
            if (listBox.Items.Count <= 0)
            {
                return ret;
            }
            if (listBoxChildCount <= 0)
            {
                return ret;
            }

            for (int i = 0; i < listBoxChildCount; i++)
            {
                var obj = VisualTreeHelper.GetChild(listBox, i);

                while (obj is not null && obj is not UniformGrid)
                {
                    obj = Extensions.GetChild<UniformGrid>(obj);
                }
                var uniformGridCount = VisualTreeHelper.GetChildrenCount(obj);
                for (int j = 0; j < uniformGridCount; j++)
                {
                    var imageViewUC = VisualTreeHelper.GetChild(obj, j);
                    while (imageViewUC is not null && imageViewUC is not ImageViewUC)
                    {
                        imageViewUC = Extensions.GetChild<ImageViewUC>(imageViewUC);
                    }

                    if (imageViewUC is ImageViewUC temp)
                    {
                        ret.Add(temp);
                    }
                }
            }

            return ret;
        }
        private string GetUniqueFilePath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return filePath;
            }

            var directory = Path.GetDirectoryName(filePath) ?? FBaseFunc.Ins.Cfg.ImageConvertSaveDirPath;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string fileExtension = Path.GetExtension(filePath);

            int count = 1;
            string newFilePath;
            do
            {
                string numberedFileName = $"{fileNameWithoutExtension}_{count}{fileExtension}";
                newFilePath = Path.Combine(directory, numberedFileName);
                count++;
            } while (File.Exists(newFilePath));

            return newFilePath;
        }

        public override void Dispose()
        {
            foreach (var list in ImageConverterItemList)
            {
                list.Dispose();
            }
            ImageConverterItemList.Clear();
        }
    }
}
