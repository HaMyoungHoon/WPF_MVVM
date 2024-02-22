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
        public int Index { get; set; }
        public string? FileName { get; set; }
        public BitmapImage? Image { get; set; }
    }
}
