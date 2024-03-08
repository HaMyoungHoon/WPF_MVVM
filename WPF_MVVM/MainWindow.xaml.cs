using System;
using System.Windows;

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
//            var temp = new FFMpegHelper();
//            temp.GetSimpleVideoSet(@"C:\\Users\\basic\\Downloads\\mp4_sample1.mp4");
//            var read = temp.GetSimpleVideoFrameData();
//            temp.Dispose();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ((MainWindowVM)DataContext).SetMessageHook(this);
        }
    }
}
