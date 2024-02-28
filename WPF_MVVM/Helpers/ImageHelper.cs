using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Spreadsheet;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF_MVVM.Helpers
{
    internal static class ImageHelper
    {
        public static bool IsWebp(byte[] header)
        {
            return header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                   header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50;
        }
        public static bool IsGif(byte[] header)
        {
            var headerText = Encoding.ASCII.GetString(header);
            return headerText.StartsWith("GIF87a") || headerText.StartsWith("GIF89a");
        }

        public static BitmapImage GetBitmapImage(Uri uri)
        {
            BitmapImage ret = new();
            ret.BeginInit();
            ret.UriSource = uri;
            ret.EndInit();
            return ret;
        }
        public static BitmapImage GetBitmapImage(string path) 
        {
            return GetBitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
        }
        public static BitmapSource GetBitmapSource(Uri uri)
        {
            BitmapDecoder decoder = BitmapDecoder.Create(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            return decoder.Frames.First();
        }
        public static BitmapSource GetBitmapSource(string path)
        {
            return GetBitmapSource(new Uri(path, UriKind.RelativeOrAbsolute));
        }

        public static GifBitmapDecoder GetGifBitmapDecoder(Uri uri)
        {
            GifBitmapDecoder ret = new(uri, BitmapCreateOptions.None, BitmapCacheOption.None);
            return ret;
        }
        public static GifBitmapDecoder GetGifBitmapDecoder(string path)
        {
            return GetGifBitmapDecoder(new Uri(path, UriKind.RelativeOrAbsolute));
        }
        public static int GetGifFrameSize(GifBitmapDecoder decoder)
        {
            return decoder.Frames.Count;
        }
        public static int GetGifFrameSize(Uri uri)
        {
            return GetGifBitmapDecoder(uri).Frames.Count;
        }
        public static int GetGifFrameSize(string path)
        {
            return GetGifBitmapDecoder(path).Frames.Count;
        }
        public static int GetGifFrameDelay(Uri uri, int frameIndex = 0)
        {
            var decoder = GetGifBitmapDecoder(uri);
            int ret = 0;
            int searchFrame = frameIndex;
            if (frameIndex < 0)
            {
                searchFrame = 0;
            }
            if (frameIndex >= decoder.Frames.Count)
            {
                searchFrame = decoder.Frames.Count - 1;
            }
            if (decoder.Frames[searchFrame].Metadata is not BitmapMetadata metaData || !metaData.ContainsQuery("/grctlext/Delay"))
            {
                return ret;
            }

            ret = metaData.GetQuery("/grctlext/Delay") as ushort? ?? 0;
            ret *= 10;

            return ret;
        }
        public static int GetGifFrameDelay(string path, int frameIndex = 0)
        {
            return GetGifFrameDelay(new Uri(path, UriKind.RelativeOrAbsolute), frameIndex);
        }
        public static int GetGifFrameDelay(GifBitmapDecoder decoder, int frameIndex = 0)
        {
            int searchFrame = frameIndex;
            int ret = 0;
            if (frameIndex < 0)
            {
                searchFrame = 0;
            }
            if (frameIndex >= decoder.Frames.Count)
            {
                searchFrame = decoder.Frames.Count - 1;
            }
            if (decoder.Frames[searchFrame].Metadata is not BitmapMetadata metaData || !metaData.ContainsQuery("/grctlext/Delay"))
            {
                return ret;
            }

            ret = metaData.GetQuery("/grctlext/Delay") as ushort? ?? 0;
            ret *= 10;

            return ret;
        }
        /// <summary>
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>Milliseconds</returns>
        public static int GetGifFrameTotalDelay(Uri uri)
        {
            var decoder = GetGifBitmapDecoder(uri);
            int ret = 0;
            decoder.Frames.Select(x => x.Metadata).ToList().ForEach(y =>
            {
                if (y is BitmapMetadata metaData && metaData.ContainsQuery("/grctlext/Delay"))
                {
                    ret += (metaData.GetQuery("/grctlext/Delay") as ushort? ?? 0) * 10;
                }
            });
            return ret;
        }
        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Milliseconds</returns>
        public static int GetGifFrameTotalDelay(string path)
        {
            return GetGifFrameTotalDelay(new Uri(path, UriKind.RelativeOrAbsolute));
        }
        /// <summary>
        /// </summary>
        /// <param name="decoder"></param>
        /// <returns>Milliseconds</returns>
        public static int GetGifFrameTotalDelay(GifBitmapDecoder decoder)
        {
            int ret = 0;
            decoder.Frames.Select(x => x.Metadata).ToList().ForEach(y =>
            {
                if (y is BitmapMetadata metaData && metaData.ContainsQuery("/grctlext/Delay"))
                {
                    ret += (metaData.GetQuery("/grctlext/Delay") as ushort? ?? 0) * 10;
                }
            });
            return ret;
        }
        public static BitmapImage GetBitmapImage(GifBitmapDecoder decoder, int frameIndex = 0)
        {
            BitmapImage ret = new();
            int searchFrame = frameIndex;
            if (frameIndex < 0)
            {
                searchFrame = 0;
            }
            if (frameIndex >= decoder.Frames.Count)
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
        public static BitmapImage GetGifBitmapImage(Uri uri, int frameIndex = 0)
        {
            BitmapImage ret = new();
            var gifDecoder = GetGifBitmapDecoder(uri);
            int searchFrame = frameIndex;
            if (frameIndex < 0)
            {
                searchFrame = 0;
            }
            if (frameIndex >= gifDecoder.Frames.Count)
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
        public static BitmapImage GetGifBitmapImage(string path, int frameIndex = 0)
        {
            return GetGifBitmapImage(new Uri(path, UriKind.RelativeOrAbsolute), frameIndex);
        }
        public static BitmapSource GetGifBitmapSource(GifBitmapDecoder decoder, int frameIndex = 0)
        {
            int searchFrame = frameIndex;
            if (frameIndex < 0)
            {
                searchFrame = 0;
            }
            if (frameIndex >= decoder.Frames.Count)
            {
                searchFrame = decoder.Frames.Count - 1;
            }
            return decoder.Frames[searchFrame];
        }
        public static BitmapSource GetGifBitmapSource(Uri uri, int frameIndex = 0)
        {
            var gifDecoder = GetGifBitmapDecoder(uri);
            int searchFrame = frameIndex;
            if (frameIndex < 0)
            {
                searchFrame = 0;
            }
            if (frameIndex >= gifDecoder.Frames.Count)
            {
                searchFrame = gifDecoder.Frames.Count - 1;
            }
            return gifDecoder.Frames[searchFrame];
        }
        public static BitmapSource GetGifBitmapSource(string path, int frameIndex = 0) 
        {
            return GetGifBitmapSource(new Uri(path, UriKind.RelativeOrAbsolute), frameIndex);
        }
        public static BitmapSource GetComparedGifBitmapSource(Uri uri, int frameIndex = 0, bool compareAll = false, bool skipMode = false)
        {
            var gifDecoder = GetGifBitmapDecoder(uri);
            if (compareAll == true)
            {
                return GetGifBitmapSourceCompareAll(gifDecoder, frameIndex, skipMode);
            }
            else
            {
                return GetGifBitmapSourceCompare(gifDecoder, frameIndex);
            }
        }
        public static BitmapSource GetComparedGifBitmapSource(string path, int frameIndex = 0, bool compareAll = false, bool skipMode = false)
        {
            return GetComparedGifBitmapSource(new Uri(path, UriKind.RelativeOrAbsolute), frameIndex, compareAll, skipMode);
        }
        private static BitmapSource GetGifBitmapSourceCompareAll(GifBitmapDecoder decoder, int frameIndex, bool skipMode)
        {
            int searchFrame = frameIndex;
            if (frameIndex <= 0)
            {
                return decoder.Frames[0];
            }
            if (frameIndex >= decoder.Frames.Count)
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
        private static BitmapSource GetGifBitmapSourceCompare(GifBitmapDecoder decoder, int frameIndex)
        {
            int searchFrame = frameIndex;
            if (frameIndex <= 0)
            {
                return decoder.Frames[0];
            }
            if (frameIndex >= decoder.Frames.Count)
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
        public static System.Drawing.Image GetDrawingImage(string path)
        {
            return System.Drawing.Image.FromFile(path);
        }
        /// <summary>
        /// 명시적으로 System.Drawing.Image dispose 해줘야 함.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static int GetDrawingImageFrameSize(System.Drawing.Image image)
        {
            System.Drawing.Imaging.FrameDimension dimension = new(image.FrameDimensionsList[0]);
            return image.GetFrameCount(dimension);
        }
        public static int GetDrawingImageFrameSize(string path)
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
        /// <param name="frameIndex"></param>
        /// <returns></returns>
        public static System.Drawing.Bitmap GetDrawingBitmap(System.Drawing.Image image, int frameIndex = 0)
        {
            System.Drawing.Imaging.FrameDimension dimension = new(image.FrameDimensionsList[0]);
            int searchFrame = frameIndex;
            if (frameIndex < 0)
            {
                searchFrame = 0;
            }
            if (frameIndex >= image.GetFrameCount(dimension))
            {
                searchFrame = image.GetFrameCount(dimension) - 1;
            }
            image.SelectActiveFrame(dimension, searchFrame);
            return new(image);
        }
        public static System.Drawing.Bitmap GetDrawingBitmap(string path, int frameIndex = 0)
        {
            System.Drawing.Bitmap ret;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                System.Drawing.Imaging.FrameDimension dimension = new(image.FrameDimensionsList[0]);
                int searchFrame = frameIndex;
                if (frameIndex < 0)
                {
                    searchFrame = 0;
                }
                if (frameIndex >= image.GetFrameCount(dimension))
                {
                    searchFrame = image.GetFrameCount(dimension) - 1;
                }
                image.SelectActiveFrame(dimension, searchFrame);
                ret = new(image);
            }
            return ret;
        }
        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="frameIndex"></param>
        /// <returns>Milliseconds</returns>
        public static int GetDrawingImageFrameDelay(string path, int frameIndex = 0)
        {
            int ret = 0;
            using (System.Drawing.Image img = System.Drawing.Image.FromFile(path))
            {
                FrameDimension dimension = new(img.FrameDimensionsList[0]);
                int searchFrame = frameIndex;
                var frameCount = img.GetFrameCount(dimension);
                if (frameCount < 0)
                {
                    searchFrame = 0;
                }
                if (frameIndex >= img.GetFrameCount(dimension))
                {
                    searchFrame = img.GetFrameCount(dimension) - 1;
                }
                img.SelectActiveFrame(dimension, searchFrame);
                var propertyItem = img.GetPropertyItem(0x5100);
                if (propertyItem != null && propertyItem.Type == 4)
                {
                    if (propertyItem.Value != null)
                    {
                        ret = BitConverter.ToUInt16(propertyItem.Value, 0) * 10;
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Milliseconds</returns>
        public static int GetDrawingImageFrameTotalDelay(string path)
        {
            int ret = 0;
            using (System.Drawing.Image img = System.Drawing.Image.FromFile(path))
            {
                FrameDimension dimension = new(img.FrameDimensionsList[0]);
                var frameCount = img.GetFrameCount(dimension);
                img.SelectActiveFrame(dimension, 0);
                var propertyItem = img.GetPropertyItem(0x5100);
                if (propertyItem != null && propertyItem.Type == 4)
                {
                    if (propertyItem.Value != null)
                    {
                        ret = BitConverter.ToUInt16(propertyItem.Value, 0) * 10 * frameCount;
                    }
                }
            }
            return ret;
        }
        public static BitmapSource GetDrawingBitmapSource(string path, int frameIndex = 0)
        {
            BitmapSource ret;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                System.Drawing.Imaging.FrameDimension dimension = new(image.FrameDimensionsList[0]);
                int searchFrame = frameIndex;
                if (frameIndex < 0)
                {
                    searchFrame = 0;
                }
                if (frameIndex >= image.GetFrameCount(dimension))
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
        public static BitmapSource GetComparedDrawingBitmapSource(string path, int frameIndex = 0, bool compareAll = false, bool skipMode = false)
        {
            BitmapSource ret;
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                System.Drawing.Bitmap saveFrame;
                if (compareAll == true)
                {
                    saveFrame = GetDrawingBitmapCompareAll(image, frameIndex, skipMode);
                }
                else
                {
                    saveFrame = GetDrawingBitmapCompare(image, frameIndex);
                }
                ret = GetDrawingBitmapToBitmapSource(saveFrame);
            }
            return ret;
        }
        private static System.Drawing.Bitmap GetDrawingBitmapCompareAll(System.Drawing.Image image, int frameIndex, bool skipMode)
        {
            System.Drawing.Imaging.FrameDimension dimension = new(image.FrameDimensionsList[0]);
            int searchFrame = frameIndex;
            if (frameIndex <= 0)
            {
                return new System.Drawing.Bitmap(image);
            }
            if (frameIndex >= image.GetFrameCount(dimension))
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
        private static System.Drawing.Bitmap GetDrawingBitmapCompare(System.Drawing.Image image, int frameIndex)
        {
            System.Drawing.Imaging.FrameDimension dimension = new(image.FrameDimensionsList[0]);
            int searchFrame = frameIndex;
            if (frameIndex <= 0)
            {
                return new System.Drawing.Bitmap(image);
            }
            if (frameIndex >= image.GetFrameCount(dimension))
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

        public static BitmapSource GetDrawingBitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            BitmapSource ret;
            var hBitmap = bitmap.GetHbitmap();
            var sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            ret = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            ret.Freeze();
            return ret;
        }
        public static System.Drawing.Bitmap GetBitmapSourceToDrawingBitmap(BitmapSource bitmapSource)
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

        public static int GetWebpFrameSize(string path)
        {
            int ret = 0;
            using (SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(path))
            {
                ret = img.Frames.Count;
            }
            return ret;
        }
        public static int GetWebpFrameDelay(string path, int frameIndex = 0)
        {
            int ret = 0;
            int searchFrame = frameIndex;
            if (frameIndex < 0)
            {
                searchFrame = 0;
            }
            using (SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(path))
            {
                if (frameIndex >= img.Frames.Count)
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
        public static BitmapImage GetWebpBitmapImage(string path, int frameIndex = 0)
        {
            BitmapImage ret = new();
            int searchFrame = frameIndex;
            if (frameIndex < 0)
            {
                searchFrame = 0;
            }
            using (SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(path))
            {
                if (frameIndex >= img.Frames.Count)
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
        public static BitmapSource GetWebpBitmapSource(string path, int frameIndex = 0)
        {
            int searchFrame = frameIndex;
            if (frameIndex < 0)
            {
                searchFrame = 0;
            }
            BitmapSource ret;
            using (SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.Load(path))
            {
                if (frameIndex >= img.Frames.Count)
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

        public static MediaState GetMediaState(MediaElement mediaElement)
        {
            if (typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance) is not FieldInfo hlp)
            {
                return MediaState.Stop;
            }
            if (hlp.GetValue(mediaElement) is not object helperObject)
            {
                return MediaState.Stop;
            }
            if (helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance) is not FieldInfo stateField)
            {
                return MediaState.Stop;
            }
            if (stateField.GetValue(helperObject) is not MediaState state)
            {
                return MediaState.Stop;
            }
            return state;
        }
        public static RenderTargetBitmap? GetCaptureMediaFrame(MediaElement mediaElement)
        {
            var previousState = GetMediaState(mediaElement);
            if (previousState == MediaState.Play)
            {
                mediaElement.Pause();
            }

            var naturalWidth = mediaElement.NaturalVideoWidth;
            var naturalHeight = mediaElement.NaturalVideoHeight;
            if (naturalWidth <= 0 || naturalHeight <= 0)
            {
                return null;
            }
            DrawingVisual visual = new();
            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush(mediaElement), null, new Rect(0, 0, naturalWidth, naturalHeight));
            }
            RenderTargetBitmap renderTargetBitmap = new(naturalWidth, naturalHeight, 96, 96, PixelFormats.Default);
            renderTargetBitmap.Render(visual);


            if (previousState == MediaState.Play)
            {
                mediaElement.Play();
            }

            return renderTargetBitmap;
        }
    }
}
