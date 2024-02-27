using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WPF_MVVM.Helpers
{
    // static으로 할까
    internal class ImageHelper
    {
        public bool IsWebp(byte[] header)
        {
            return header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                   header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50;
        }
        public bool IsGif(byte[] header)
        {
            var headerText = Encoding.ASCII.GetString(header);
            return headerText.StartsWith("GIF87a") || headerText.StartsWith("GIF89a");
        }

        public BitmapImage GetBitmapImage(Uri uri)
        {
            BitmapImage ret = new();
            ret.BeginInit();
            ret.UriSource = uri;
            ret.EndInit();
            return ret;
        }
        public BitmapImage GetBitmapImage(string path) 
        {
            return GetBitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
        }
        public BitmapSource GetBitmapSource(Uri uri)
        {
            BitmapDecoder decoder = BitmapDecoder.Create(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            return decoder.Frames.First();
        }
        public BitmapSource GetBitmapSource(string path)
        {
            return GetBitmapSource(new Uri(path, UriKind.RelativeOrAbsolute));
        }

        public GifBitmapDecoder GetGifBitmapDecoder(Uri uri)
        {
            GifBitmapDecoder ret = new(uri, BitmapCreateOptions.None, BitmapCacheOption.None);
            return ret;
        }
        public GifBitmapDecoder GetGifBitmapDecoder(string path)
        {
            return GetGifBitmapDecoder(new Uri(path, UriKind.RelativeOrAbsolute));
        }
        public int GetGifFrameSize(GifBitmapDecoder decoder)
        {
            return decoder.Frames.Count;
        }
        public int GetGifFrameSize(Uri uri)
        {
            return GetGifBitmapDecoder(uri).Frames.Count;
        }
        public int GetGifFrameSize(string path)
        {
            return GetGifBitmapDecoder(path).Frames.Count;
        }
        public int GetGifFrameDelay(GifBitmapDecoder decoder, int frame = 0)
        {
            int searchFrame = frame;
            int ret = 0;
            if (frame < 0)
            {
                searchFrame = 0;
            }
            if (frame >= decoder.Frames.Count)
            {
                searchFrame = decoder.Frames.Count - 1;
            }
            if (decoder.Frames[searchFrame].Metadata is not BitmapMetadata metadata || !metadata.ContainsQuery("/grctlext/Delay"))
            {
                return ret;
            }

            if (metadata.GetQuery("/grctlext/Delay") is int temp)
            {
                ret = temp;
            }

            return ret;
        }
        public int GetGifFrameDelay(Uri uri, int frame = 0)
        {
            var decoder = GetGifBitmapDecoder(uri);
            int ret = 0;
            int searchFrame = frame;
            if (frame < 0)
            {
                searchFrame = 0;
            }
            if (frame >= decoder.Frames.Count)
            {
                searchFrame = decoder.Frames.Count - 1;
            }
            if (decoder.Frames[searchFrame].Metadata is not BitmapMetadata metadata || !metadata.ContainsQuery("/grctlext/Delay"))
            {
                return ret;
            }

            if (metadata.GetQuery("/grctlext/Delay") is int temp)
            {
                ret = temp;
            }

            return ret;
        }
        public int GetGifFrameDelay(string path, int frame = 0)
        {
            return GetGifFrameDelay(new Uri(path, UriKind.RelativeOrAbsolute), frame);
        }
        public BitmapImage GetBitmapImage(GifBitmapDecoder decoder, int frame = 0)
        {
            BitmapImage ret = new();
            int searchFrame = frame;
            if (frame < 0)
            {
                searchFrame = 0;
            }
            if (frame >= decoder.Frames.Count)
            {
                searchFrame = decoder.Frames.Count - 1;
            }
            using (MemoryStream ms = new())
            {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(decoder.Frames[searchFrame]);
                encoder.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                ret.BeginInit();
                ret.CacheOption = BitmapCacheOption.OnLoad;
                ret.StreamSource = ms;
                ret.EndInit();
            }
            return ret;
        }
        public BitmapImage GetGifBitmapImage(Uri uri, int frame = 0)
        {
            BitmapImage ret = new();
            var gifDecoder = GetGifBitmapDecoder(uri);
            int searchFrame = frame;
            if (frame < 0)
            {
                searchFrame = 0;
            }
            if (frame >= gifDecoder.Frames.Count)
            {
                searchFrame = gifDecoder.Frames.Count - 1;
            }
            using (MemoryStream ms = new())
            {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(gifDecoder.Frames[searchFrame]);
                encoder.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                ret.BeginInit();
                ret.CacheOption = BitmapCacheOption.OnLoad;
                ret.StreamSource = ms;
                ret.EndInit();
            }
            return ret;
        }
        public BitmapImage GetGifBitmapImage(string path, int frame = 0)
        {
            return GetGifBitmapImage(new Uri(path, UriKind.RelativeOrAbsolute), frame);
        }
        public BitmapSource GetGifBitmapSource(GifBitmapDecoder decoder, int frame = 0)
        {
            int searchFrame = frame;
            if (frame < 0)
            {
                searchFrame = 0;
            }
            if (frame >= decoder.Frames.Count)
            {
                searchFrame = decoder.Frames.Count - 1;
            }
            return decoder.Frames[searchFrame];
        }
        public BitmapSource GetGifBitmapSource(Uri uri, int frame = 0)
        {
            var gifDecoder = GetGifBitmapDecoder(uri);
            int searchFrame = frame;
            if (frame < 0)
            {
                searchFrame = 0;
            }
            if (frame >= gifDecoder.Frames.Count)
            {
                searchFrame = gifDecoder.Frames.Count - 1;
            }
            return gifDecoder.Frames[searchFrame];
        }
        public BitmapSource GetGifBitmapSource(string path, int frame = 0) 
        {
            return GetGifBitmapSource(new Uri(path, UriKind.RelativeOrAbsolute), frame);
        }
        public BitmapSource GetComparedGifBitmapSource(Uri uri, int frame = 0, bool compareAll = false, bool skipMode = false)
        {
            var gifDecoder = GetGifBitmapDecoder(uri);
            if (compareAll == true)
            {
                return GetGifBitmapSourceCompareAll(gifDecoder, frame, skipMode);
            }
            else
            {
                return GetGifBitmapSourceCompare(gifDecoder, frame);
            }
        }
        public BitmapSource GetComparedGifBitmapSource(string path, int frame = 0, bool compareAll = false, bool skipMode = false)
        {
            return GetComparedGifBitmapSource(new Uri(path, UriKind.RelativeOrAbsolute), frame, compareAll, skipMode);
        }
        private BitmapSource GetGifBitmapSourceCompareAll(GifBitmapDecoder decoder, int frame, bool skipMode)
        {
            int searchFrame = frame;
            if (frame <= 0)
            {
                return decoder.Frames[0];
            }
            if (frame >= decoder.Frames.Count)
            {
                searchFrame = decoder.Frames.Count - 1;
                return decoder.Frames[searchFrame];
            }
            BitmapSource ret = decoder.Frames.First();
            int stride = ret.PixelWidth * ((ret.Format.BitsPerPixel + 7) / 8);
            int size = ret.PixelHeight * stride;
            byte[] retPixel = new byte[size];
            ret.CopyPixels(retPixel, stride, 0);
            for (int i = 1; i <= searchFrame; i++)
            {
                if (skipMode == true)
                {
                    i++;
                    if (i > searchFrame)
                    {
                        i = searchFrame;
                    }
                }
                BitmapSource currentSource = decoder.Frames[i];
                if (ret.PixelWidth != currentSource.PixelWidth || ret.PixelHeight != currentSource.PixelHeight)
                {
                    break;
                }
                byte[] currentPixel = new byte[size];
                currentSource.CopyPixels(currentPixel, stride, 0);
                for (int j = 0; j < size; j++)
                {
                    if (retPixel[j] != currentPixel[j])
                    {
                        retPixel[j] = currentPixel[j];
                    }
                }
            }
            ret = BitmapSource.Create(ret.PixelWidth, ret.PixelHeight, ret.DpiX, ret.DpiY,
                ret.Format, ret.Palette, retPixel, stride);

            return ret;
        }
        private BitmapSource GetGifBitmapSourceCompare(GifBitmapDecoder decoder, int frame)
        {
            int searchFrame = frame;
            if (frame <= 0)
            {
                return decoder.Frames[0];
            }
            if (frame >= decoder.Frames.Count)
            {
                searchFrame = decoder.Frames.Count - 1;
                return decoder.Frames[searchFrame];
            }
            BitmapSource ret = decoder.Frames.First();
            int stride = ret.PixelWidth * ((ret.Format.BitsPerPixel + 7) / 8);
            int size = ret.PixelHeight * stride;
            byte[] retPixel = new byte[size];
            ret.CopyPixels(retPixel, stride, 0);
            BitmapSource currentSource = decoder.Frames[searchFrame];
            if (ret.PixelWidth != currentSource.PixelWidth || ret.PixelHeight != currentSource.PixelHeight)
            {
                return ret;
            }
            byte[] currentPixel = new byte[size];
            currentSource.CopyPixels(currentPixel, stride, 0);
            for (int j = 0; j < size; j++)
            {
                if (retPixel[j] != currentPixel[j])
                {
                    retPixel[j] = currentPixel[j];
                }
            }

            ret = BitmapSource.Create(ret.PixelWidth, ret.PixelHeight, ret.DpiX, ret.DpiY, ret.Format, ret.Palette, retPixel, stride);
            return ret;
        }

        /// <summary>
        /// 명시적으로 System.Drawing.Image dispose 해줘야 함.
        /// </summary>
        /// <param name="path">file full path</param>
        /// <returns></returns>
        public System.Drawing.Image GetDrawingImage(string path)
        {
            return System.Drawing.Image.FromFile(path);
        }
        /// <summary>
        /// 명시적으로 System.Drawing.Image dispose 해줘야 함.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public int GetDrawingImageFrameSize(System.Drawing.Image image)
        {
            System.Drawing.Imaging.FrameDimension dimension = new(image.FrameDimensionsList[0]);
            return image.GetFrameCount(dimension);
        }
        public int GetDrawingImageFrameSize(string path)
        {
            int ret = 0;
            using (var image = System.Drawing.Image.FromFile(path))
            {
                System.Drawing.Imaging.FrameDimension dimension = new(image.FrameDimensionsList[0]);
                ret = image.GetFrameCount(dimension);
            }
            return ret;
        }
        /// <summary>
        /// 명시적으로 System.Drawing.Image dispose 해줘야 함.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public System.Drawing.Bitmap GetDrawingBitmap(System.Drawing.Image image, int frame = 0)
        {
            System.Drawing.Imaging.FrameDimension dimension = new(image.FrameDimensionsList[0]);
            int searchFrame = frame;
            if (frame < 0)
            {
                searchFrame = 0;
            }
            if (frame >= image.GetFrameCount(dimension))
            {
                searchFrame = image.GetFrameCount(dimension) - 1;
            }
            image.SelectActiveFrame(dimension, searchFrame);
            return new(image);
        }
        public System.Drawing.Bitmap GetDrawingBitmap(string path, int frame = 0)
        {
            System.Drawing.Bitmap ret;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                System.Drawing.Imaging.FrameDimension dimension = new(image.FrameDimensionsList[0]);
                int searchFrame = frame;
                if (frame < 0)
                {
                    searchFrame = 0;
                }
                if (frame >= image.GetFrameCount(dimension))
                {
                    searchFrame = image.GetFrameCount(dimension) - 1;
                }
                image.SelectActiveFrame(dimension, searchFrame);
                ret = new(image);
            }
            return ret;
        }
        public BitmapSource GetDrawingBitmapSource(string path, int frame = 0)
        {
            BitmapSource ret;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                System.Drawing.Imaging.FrameDimension dimension = new(image.FrameDimensionsList[0]);
                int searchFrame = frame;
                if (frame < 0)
                {
                    searchFrame = 0;
                }
                if (frame >= image.GetFrameCount(dimension))
                {
                    searchFrame = image.GetFrameCount(dimension) - 1;
                }
                image.SelectActiveFrame(dimension, searchFrame);
                System.Drawing.Bitmap saveFrame = new(image);
                var hBitmap = saveFrame.GetHbitmap();
                var sizeOptions = BitmapSizeOptions.FromEmptyOptions();
                ret = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
                ret.Freeze();
            }
            return ret;
        }
        public BitmapSource GetComparedDrawingBitmapSource(string path, int frame = 0, bool compareAll = false, bool skipMode = false)
        {
            BitmapSource ret;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                System.Drawing.Bitmap saveFrame;
                if (compareAll == true)
                {
                    saveFrame = GetDrawingBitmapCompareAll(image, frame, skipMode);
                }
                else
                {
                    saveFrame = GetDrawingBitmapCompare(image, frame);
                }
                ret = GetDrawingBitmapToBitmapSource(saveFrame);
            }
            return ret;
        }
        private System.Drawing.Bitmap GetDrawingBitmapCompareAll(System.Drawing.Image image, int frame, bool skipMode)
        {
            System.Drawing.Imaging.FrameDimension dimension = new(image.FrameDimensionsList[0]);
            int searchFrame = frame;
            if (frame <= 0)
            {
                return new System.Drawing.Bitmap(image);
            }
            if (frame >= image.GetFrameCount(dimension))
            {
                searchFrame = image.GetFrameCount(dimension) - 1;
                image.SelectActiveFrame(dimension, searchFrame);
                return new System.Drawing.Bitmap(image);
            }
            image.SelectActiveFrame(dimension, 0);
            System.Drawing.Bitmap ret = new(image);
            for (int i = 1; i <= searchFrame; i++)
            {
                if (skipMode == true)
                {
                    i += 1;
                    if (i > searchFrame)
                    {
                        i = searchFrame;
                    }
                }
                image.SelectActiveFrame(dimension, i);
                System.Drawing.Bitmap currentFrame = new(image);
                for (int x = 0; x < currentFrame.Width; x++)
                {
                    for (int y = 0; y < currentFrame.Height; y++)
                    {
                        var currentPixel = currentFrame.GetPixel(x, y);
                        var saveFramePixel = ret.GetPixel(x, y);
                        if (currentPixel != saveFramePixel)
                        {
                            ret.SetPixel(x, y, currentPixel);
                        }
                    }
                }
            }
            return ret;
        }
        private System.Drawing.Bitmap GetDrawingBitmapCompare(System.Drawing.Image image, int frame)
        {
            System.Drawing.Imaging.FrameDimension dimension = new(image.FrameDimensionsList[0]);
            int searchFrame = frame;
            if (frame <= 0)
            {
                return new System.Drawing.Bitmap(image);
            }
            if (frame >= image.GetFrameCount(dimension))
            {
                searchFrame = image.GetFrameCount(dimension) - 1;
                image.SelectActiveFrame(dimension, searchFrame);
                return new System.Drawing.Bitmap(image);
            }
            image.SelectActiveFrame(dimension, 0);
            System.Drawing.Bitmap ret = new(image);
            image.SelectActiveFrame(dimension, searchFrame);
            System.Drawing.Bitmap currentFrame = new(image);
            for (int x = 0; x < currentFrame.Width; x++)
            {
                for (int y = 0; y < currentFrame.Height; y++)
                {
                    var currentPixel = currentFrame.GetPixel(x, y);
                    var saveFramePixel = ret.GetPixel(x, y);
                    if (currentPixel != saveFramePixel)
                    {
                        ret.SetPixel(x, y, currentPixel);
                    }
                }
            }
            return ret;
        }

        public BitmapSource GetDrawingBitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            BitmapSource ret;
            var hBitmap = bitmap.GetHbitmap();
            var sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            ret = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            ret.Freeze();
            return ret;
        }
        public System.Drawing.Bitmap GetBitmapSourceToDrawingBitmap(BitmapSource bitmapSource)
        {
            System.Drawing.Bitmap ret;
            using (MemoryStream ms = new())
            {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(ms);
                ret = new(ms);
            }
            return ret;
        }

        public int GetWebpFrameSize(string path)
        {
            int ret = 0;
            using (SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(path))
            {
                ret = img.Frames.Count;
            }
            return ret;
        }
        public int GetWebpFrameDelay(string path, int frame = 0)
        {
            int ret = 0;
            int searchFrame = frame;
            if (frame < 0)
            {
                searchFrame = 0;
            }
            using (SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(path))
            {
                if (frame >= img.Frames.Count)
                {
                    searchFrame = img.Frames.Count - 1;
                }

                if (img.Frames[searchFrame].Metadata.TryGetGifMetadata(out GifFrameMetadata? gifMetaData))
                {
                    ret = gifMetaData?.FrameDelay ?? 0;
                }
            }

            return ret;
        }
        public BitmapImage GetWebpBitmapImage(string path, int frame = 0)
        {
            BitmapImage ret = new();
            int searchFrame = frame;
            if (frame < 0)
            {
                searchFrame = 0;
            }
            using (SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(path))
            {
                if (frame >= img.Frames.Count)
                {
                    searchFrame = img.Frames.Count - 1;
                }
                using MemoryStream ms = new();
                if (img.Frames.First().Metadata.TryGetGifMetadata(out SixLabors.ImageSharp.Formats.Gif.GifFrameMetadata? gifFrameMetadata))
                {
                    img.SaveAsGif(ms);
                    ms.Position = 0;
                    GifBitmapDecoder decoder = new(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    ret.BeginInit();
                    ret.StreamSource = new MemoryStream();
                    BmpBitmapEncoder encoder = new();
                    encoder.Frames.Add(decoder.Frames[searchFrame]);
                    encoder.Save(ret.StreamSource);
                    ret.StreamSource.Position = 0;
                    ret.EndInit();
                }
                else
                {
                    img.SaveAsBmp(ms);
                    ms.Position = 0;
                    ret.BeginInit();
                    ret.CacheOption = BitmapCacheOption.OnLoad;
                    ret.StreamSource = ms;
                    ret.EndInit();
                }
            }

            return ret;
        }
        public BitmapSource GetWebpBitmapSource(string path, int frame = 0)
        {
            int searchFrame = frame;
            if (frame < 0)
            {
                searchFrame = 0;
            }
            BitmapSource ret;
            using (SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(path))
            {
                if (frame >= img.Frames.Count)
                {
                    searchFrame = img.Frames.Count - 1;
                }
                using (MemoryStream ms = new())
                {
                    if (img.Frames.First().Metadata.TryGetGifMetadata(out SixLabors.ImageSharp.Formats.Gif.GifFrameMetadata? gifFrameMetadata))
                    {
                        img.SaveAsGif(ms);
                        ms.Position = 0;
                        GifBitmapDecoder decoder = new(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                        ret = decoder.Frames[searchFrame];
                    }
                    else
                    {
                        img.SaveAsBmp(ms);
                        ms.Position = 0;
                        ret = BitmapDecoder.Create(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad).Frames[searchFrame];
                    }
                }
            }
            return ret;
        }
    }
}
