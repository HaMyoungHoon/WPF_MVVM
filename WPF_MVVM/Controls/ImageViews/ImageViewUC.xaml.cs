using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPF_MVVM.Helpers;

namespace WPF_MVVM.Controls.ImageViews
{
    /// <summary>
    /// ImageViewUC.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageViewUC : UserControl
    {
        public static readonly DependencyProperty ImageFilePathProperty = DependencyProperty.Register(nameof(ImageFilePath), typeof(string), typeof(ImageViewUC), new FrameworkPropertyMetadata(string.Empty, ImageFilePathChanged));
        public static readonly DependencyProperty SetCaptureMediaCommandProperty = DependencyProperty.Register(nameof(SetCaptureMediaCommand), typeof(bool), typeof(ImageViewUC), new PropertyMetadata(false, SetCaptureMediaCommandChanged);
        public static readonly DependencyProperty GetCaptureMediaCommandProperty = DependencyProperty.Register(nameof(GetCaptureMediaCommand), typeof(object), typeof(ImageViewUC));
        public static readonly DependencyProperty ErrorCommandProperty = DependencyProperty.Register(nameof(ErrorCommand), typeof(object), typeof(ImageViewUC));
        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register(nameof(ImageFilePath), typeof(double), typeof(ImageViewUC), new FrameworkPropertyMetadata(100, ImageHeightChanged));
        public static RoutedEvent ErrorEventProperty = EventManager.RegisterRoutedEvent(nameof(ErrorEvent), RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ImageViewUC));
        public string ImageFilePath
        { 
            get => (string)GetValue(ImageFilePathProperty); 
            set => SetValue(ImageFilePathProperty, value);
        }
        public bool SetCaptureMediaCommand
        {
            get => (bool)GetValue(SetCaptureMediaCommandProperty);
            set => SetValue(SetCaptureMediaCommandProperty, value);
        }
        public ICommand? GetCaptureMediaCommand
        {
            get => (ICommand?)GetValue(GetCaptureMediaCommandProperty);
            set => SetValue(GetCaptureMediaCommandProperty, value);
        }
        public ICommand? ErrorCommand
        {
            get => (ICommand?)GetValue(ErrorCommandProperty);
            set => SetValue(ErrorCommandProperty, value);
        }
        public event RoutedEventHandler? ErrorEvent
        {
            add => AddHandler(ErrorEventProperty, value);
            remove => RemoveHandler(ErrorEventProperty, value);
        }

        private MediaState _previousMediaState;
        private DispatcherTimer _timer;

        public ImageViewUC()
        {
            InitializeComponent();
            _timer = new();
            _previousMediaState = MediaState.Close;
        }

        private static void ImageFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ImageViewUC imageViewUC)
            {
                return;
            }
            if (e.NewValue == null || e.NewValue is not string filePath)
            {
                return;
            }

            var header = new byte[12];
            using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read))
            {
                fs.Read(header, 0, 12);
            }

            imageViewUC.imgName.Text = System.IO.Path.GetFileName(filePath);

            if (ImageHelper.IsGif(header))
            {
                imageViewUC.SetGifImage();
                return;
            }
            if (ImageHelper.IsWebp(header))
            {
                imageViewUC.SetWebpImage();
                return;
            }

            imageViewUC.SetImage();

            // 영상은 귀찮으니 나중에
        }
        private static void SetCaptureMediaCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ImageViewUC imageViewUC)
            {
                return;
            }
            if (e.NewValue == null || e.NewValue is not bool capture)
            {
                return;
            }

            if (capture == false)
            {
                return;
            }

            imageViewUC.GetCaptureMediaCommand?.Execute(ImageHelper.GetCaptureMediaFrame(imageViewUC.mediaItem));
            capture = false;
        }
        private static void ImageHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ImageViewUC imageViewUC)
            {
                return;
            }
            if (e.NewValue == null || e.NewValue is not double height)
            {
                return;
            }

            if (height < 50 || height > 500)
            {
                return;
            }

            imageViewUC.mediaItem.Height = height;
            imageViewUC.imgItem.Height = height;
        }

        private void mediaItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!mediaItem.CanPause)
            {
                return;
            }

            switch (ImageHelper.GetMediaState(mediaItem))
            {
                case MediaState.Pause:
                    {
                        mediaItem.Play();
                        _timer.Start();
                        btnStartIcon.Kind = PackIconKind.Pause;
                    }
                    break;
                case MediaState.Play:
                    {
                        mediaItem.Pause();
                        _timer.Stop();
                        btnStartIcon.Kind = PackIconKind.Play;
                    }
                    break;
                case MediaState.Stop:
                    {
                        mediaItem.Play();
                        _timer.Start();
                        btnStartIcon.Kind = PackIconKind.Pause;
                    }
                    break;
            }
        }
        private void mediaItem_MediaOpened(object sender, RoutedEventArgs e)
        {
            mediaItem.Play();
            _timer.Start();
            btnStartIcon.Kind = PackIconKind.Pause;
        }
        private void mediaItem_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ErrorCall(e.ErrorException.Message);
        }
        private void mediaItem_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaItem.Stop();
            _timer.Stop();
            btnStartIcon.Kind = PackIconKind.Play;
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            switch (ImageHelper.GetMediaState(mediaItem))
            {
                case MediaState.Pause:
                    {
                        mediaItem.Play();
                        _timer.Start();
                        btnStartIcon.Kind = PackIconKind.Pause;
                    }
                    break;
                case MediaState.Play:
                    {
                        mediaItem.Pause();
                        _timer.Stop();
                        btnStartIcon.Kind = PackIconKind.Play;
                    }
                    break;
                case MediaState.Stop:
                    {
                        mediaItem.Play();
                        _timer.Start();
                        btnStartIcon.Kind = PackIconKind.Pause;
                    }
                    break;
            }
        }
        private void sliderTimeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ImageHelper.GetMediaState(mediaItem) == MediaState.Play)
            {
                return;
            }
            mediaItem.Position = TimeSpan.FromSeconds(sliderTimeline.Value);
        }
        private void sliderTimeline_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _previousMediaState = ImageHelper.GetMediaState(mediaItem);
            if (_previousMediaState == MediaState.Play)
            {
                mediaItem.Pause();
                _timer.Stop();
            }
        }
        private void sliderTimeline_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_previousMediaState == MediaState.Play)
            {
                mediaItem.Play();
                _timer.Start();
            }
        }
        private void btnMute_Click(object sender, RoutedEventArgs e)
        {
            mediaItem.IsMuted = !mediaItem.IsMuted;
            btnMuteIcon.Kind = mediaItem.IsMuted ? PackIconKind.Mute : PackIconKind.VolumeMedium;
        }

        private void SetGifImage()
        {
            imgItem.Visibility = Visibility.Collapsed;
            btnMute.Visibility = Visibility.Collapsed;
            TimerSetting();
        }

        private void SetWebpImage()
        {
            gridPlayControl.Visibility = Visibility.Collapsed;
            mediaItem.Visibility = Visibility.Collapsed;
        }

        private void SetImage()
        {
            gridPlayControl.Visibility = Visibility.Collapsed;
            mediaItem.Visibility = Visibility.Collapsed;

        }

        private void ErrorCall(string? message)
        {
            var ret = message ?? "not defined error";
            ErrorCommand?.Execute(ret);
            RaiseEvent(new(ErrorEventProperty, this));
        }


        private void TimerSetting()
        {
            sliderTimeline.Maximum = ImageHelper.GetGifFrameTotalDelay(ImageFilePath) / 1000;
            _timer.Interval = TimeSpan.FromMilliseconds(ImageHelper.GetGifFrameDelay(ImageFilePath));
            _timer.Tick += (sender, e) =>
            {
                sliderTimeline.Value = mediaItem.Position.TotalSeconds;
            };
        }
    }
}
