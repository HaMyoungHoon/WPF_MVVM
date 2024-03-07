using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF_MVVM.Models.ImageConverter
{
    internal class ImagePixelDataModel
    {
        public ImagePixelDataModel(byte[] pixels, int width, int height, double dpiX, double dpiY, PixelFormat format, BitmapPalette palette) 
        {
            Pixels = pixels;
            Width = width;
            Height = height;
            DpiX = dpiX;
            DpiY = dpiY;
            Format = format;
            Palette = palette;
        }
        public byte[] Pixels { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double DpiX { get; set; }
        public double DpiY { get; set; }
        public PixelFormat Format { get; set; }
        public BitmapPalette Palette { get; set; }
    }
}
