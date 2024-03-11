using FFmpeg.AutoGen;
using System;
using System.IO;
using System.Text;
using System.Windows;
using WPF_MVVM.Helpers.FFMpeg;

namespace WPF_MVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(MainWindowVM));
            var temp = new FFMpegHelper();
//            temp.GetSimpleHWVideoSet(@"C:\\Users\\basic\\Downloads\\mp4_sample1.mp4");
//            var read = temp.GetSimpleVideoFrameData();
            temp.Dispose();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ((MainWindowVM)DataContext).SetMessageHook(this);
        }
    }
}
