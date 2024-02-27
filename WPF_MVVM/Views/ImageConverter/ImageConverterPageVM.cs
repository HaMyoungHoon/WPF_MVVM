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
using WPF_MVVM.Bases;
using WPF_MVVM.Helpers;
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
            _imageHeight = 200;

            OpenCommand = new RelayCommand(OpenEvent);
            ConvertCommand = new RelayCommand(ConvertEvent, CanConvert);
            MouseWheelCommand = new RelayCommand<object?>(MouseWheelEvent);
            MediaLoadCommand = new RelayCommand<object?>(MediaLoadEvent);
            MediaEndCommand = new RelayCommand<object?>(MediaEndEvent);
        }

        public IRelayCommand OpenCommand { get; }
        public IRelayCommand ConvertCommand { get; }
        public IRelayCommand MouseWheelCommand { get; }
        public IRelayCommand MediaLoadCommand { get; }
        public IRelayCommand MediaEndCommand { get; }

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
                        BitmapSource image = GetBitmapImage(array.FileNames[i], out ImageFileType imageFileType);
                        temp.Add(new ImageConverterItem()
                        {
                            Index = i,
                            FileName = array.SafeFileNames[i],
                            FilePath = array.FileNames[i],
                            Image = image,
                            ImageFileType = imageFileType
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
        private void MediaLoadEvent(object? data)
        {
            if (data is not (MediaElement mediaElement, RoutedEventArgs e))
            {
                return;
            }

            mediaElement.Play();
        }
        private void MediaEndEvent(object? data)
        {
            if (data is not (MediaElement mediaElement, RoutedEventArgs e))
            {
                return;
            }
            
            var buff = new Uri(mediaElement.Source.AbsoluteUri, UriKind.Absolute);
            mediaElement.Source = buff;
            mediaElement.Play();
        }

        private bool CanConvert()
        {
            return _convertAble;
        }

        ImageHelper imageHelper = new();
        private BitmapSource GetBitmapImage(string filePath, out ImageFileType imageFileType)
        {
            imageFileType = ImageFileType.OTHER;
            var header = new byte[12];
            using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read))
            {
                fs.Read(header, 0, 12);
            }
            if (IsWebp(header))
            {
                imageFileType = ImageFileType.WEBP;
                return GetWebpImage(filePath);
            }
            else if (IsGif(header))
            {
                return imageHelper.GetComparedDrawingBitmapSource(filePath, 10);
                imageFileType = ImageFileType.GIF;
                var frameIndex = 10; // decoder.Frames.Count - 1;
                BitmapSource ret;
                using (System.Drawing.Image gifImage = System.Drawing.Image.FromFile(filePath))
                {
                    System.Drawing.Imaging.FrameDimension dimension = new(gifImage.FrameDimensionsList[0]);
                    gifImage.SelectActiveFrame(dimension, 0);
                    System.Drawing.Bitmap saveFrame = new(gifImage);
//                    Parallel.For(1, frameIndex, i =>
//                    {
//                        gifImage.SelectActiveFrame(dimension, i);
//                        //                        gifImage.SelectActiveFrame(dimension, frameIndex);
//                        System.Drawing.Bitmap currentFrame = new(gifImage);
//                        for (int x = 0; x < currentFrame.Width; x++)
//                        {
//                            for (int y = 0; y < currentFrame.Height; y++)
//                            {
//                                var currentPixel = currentFrame.GetPixel(x, y);
//                                var saveFramePixel = saveFrame.GetPixel(x, y);
//                                if (currentPixel != saveFramePixel)
//                                {
//                                    saveFrame.SetPixel(x, y, currentPixel);
//                                }
//                            }
//                        }
//                    });
//                    for (int i = 1; i <= frameIndex; i++)
//                    {
//                        gifImage.SelectActiveFrame(dimension, i);
                        gifImage.SelectActiveFrame(dimension, frameIndex);
                        System.Drawing.Bitmap currentFrame = new(gifImage);
                        for (int x = 0; x < currentFrame.Width; x++)
                        {
                            for (int y = 0; y < currentFrame.Height; y++)
                            {
                                var currentPixel = currentFrame.GetPixel(x, y);
                                var saveFramePixel = saveFrame.GetPixel(x, y);
                                if (currentPixel != saveFramePixel)
                                {
                                    saveFrame.SetPixel(x, y, currentPixel);
                                }
                            }
                        }
//                    }
                    saveFrame.Save("D:\\temp0.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    var hBitmap = saveFrame.GetHbitmap();
                    var sizeOptions = BitmapSizeOptions.FromEmptyOptions();
                    ret = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
                    ret.Freeze();
                }
                return ret;
            }
            else
            {
                BitmapDecoder decoder = BitmapDecoder.Create(new Uri(filePath, UriKind.RelativeOrAbsolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                return decoder.Frames.First();
            }
        }
        private bool IsWebp(byte[] header)
        {
            return header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                   header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50;
        }
        private bool IsGif(byte[] header)
        {
            var headerText = Encoding.ASCII.GetString(header);
            return headerText.StartsWith("GIF87a") || headerText.StartsWith("GIF89a");
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
