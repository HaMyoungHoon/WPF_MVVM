using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WPF_MVVM.Models.ImageConverter
{
    internal partial class ImageConverterItem : ObservableObject, IDisposable
    {
        [ObservableProperty]
        private string? _filePath;
        public string? FileName { get; set; }

        public void Dispose()
        {
            FilePath = null;
        }
    }
}
