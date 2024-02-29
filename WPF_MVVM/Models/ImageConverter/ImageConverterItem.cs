using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WPF_MVVM.Models.ImageConverter
{
    internal class ImageConverterItem
    {
        public bool IsCapture { get; set; }
        public string? FilePath { get; set; }
        public BitmapSource? Image { get; set; }
        public ImageFileType? ImageFileType { get; set; }
    }
}
