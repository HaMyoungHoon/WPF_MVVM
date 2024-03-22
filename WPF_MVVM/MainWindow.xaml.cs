using System;
using System.Windows;
using WPF_MVVM.Helpers.FFMpeg;

namespace WPF_MVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public unsafe MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(MainWindowVM));
//            var temp = new FFMpegHelper();
//            temp.GetSimpleHWVideoSet(@"D:\Temp\Sample\mp4_sample2.mp4");
//            var read = temp.GetSimpleHWVideoFrameData();
//            for (int i = 0; i < 100; i++)
//            {
//                read = temp.GetSimpleHWVideoFrameData();
//            }
//            OpenCvSharp.Cv2.ImShow("test", read);
//            temp.Dispose();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ((MainWindowVM)DataContext).SetMessageHook(this);
        }
    }

}