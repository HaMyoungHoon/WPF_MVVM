using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using WPF_MVVM.Models.ImageConverter;

namespace WPF_MVVM.Controls.ImageViews
{
    /// <summary>
    /// ImageViewUC.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageViewUC : UserControl
    {
        public static readonly DependencyProperty GifIndexProperty = DependencyProperty.Register(nameof(GifIndex), typeof(int), typeof(ImageViewUC));
        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register(nameof(ImageHeight), typeof(double), typeof(ImageViewUC), new FrameworkPropertyMetadata(100.0, ImageHeightChanged));
        public static readonly DependencyProperty ImageFilePathProperty = DependencyProperty.Register(nameof(ImageFilePath), typeof(string), typeof(ImageViewUC), new FrameworkPropertyMetadata(string.Empty, ImageFilePathChanged));
        public static readonly DependencyProperty ImageSafeFileNameProperty = DependencyProperty.Register(nameof(ImageSafeFileName), typeof(string), typeof(ImageViewUC));
        public static readonly DependencyProperty GetCaptureMediaCommandProperty = DependencyProperty.Register(nameof(GetCaptureMediaCommand), typeof(object), typeof(ImageViewUC));
        public static readonly DependencyProperty GetCaptureMediaProperty = DependencyProperty.Register(nameof(GetCaptureMedia), typeof(object), typeof(ImageViewUC));
        public static readonly DependencyProperty ErrorCommandProperty = DependencyProperty.Register(nameof(ErrorCommand), typeof(object), typeof(ImageViewUC));
        public static RoutedEvent ErrorEventProperty = EventManager.RegisterRoutedEvent(nameof(ErrorEvent), RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ImageViewUC));
        public int GifIndex
        {
            get => (int)GetValue(GifIndexProperty);
            set => SetValue(GifIndexProperty, value);
        }
        public double ImageHeight
        {
            get => (double)GetValue(ImageHeightProperty);   
            set => SetValue(ImageHeightProperty, value);
        }
        public string ImageFilePath
        { 
            get => (string)GetValue(ImageFilePathProperty); 
            set => SetValue(ImageFilePathProperty, value);
        }
        public string ImageSafeFileName
        {
            get => (string)GetValue(ImageSafeFileNameProperty);
            set => SetValue(ImageSafeFileNameProperty, value);
        }
        public ICommand? GetCaptureMediaCommand
        {
            get => (ICommand)GetValue(GetCaptureMediaCommandProperty);
            set => SetValue(GetCaptureMediaCommandProperty, value);
        }
        public BitmapSource? GetCaptureMedia
        {
            get => (BitmapSource?)GetValue(GetCaptureMediaProperty);
            set => SetValue(GetCaptureMediaProperty, value);
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

        private int _gifFrameSize;
        private System.Drawing.Image? _image;
        private ImageFileType _imageFileType;
        private MediaState _previousMediaState;
        private DispatcherTimer _timer;
        private List<BitmapSource> _gifList = new();

        public ImageViewUC()
        {
            InitializeComponent();
            _gifFrameSize = 0;
            _imageFileType = ImageFileType.OTHER;
            _previousMediaState = MediaState.Close;
            _timer = new();
        }
        ~ImageViewUC()
        {
            _timer.Stop();
            _gifList.Clear();
        }

        public void Pause()
        {
            _timer.Stop();
            mediaItem.Pause();
            btnStartIconPlay();
        }
        public BitmapSource? GetCurrentImage() => _imageFileType switch
        {
            ImageFileType.WEBP => ImageHelper.GetWebpBitmapSource(ImageFilePath),
            ImageFileType.OTHER => ImageHelper.GetBitmapSource(ImageFilePath),
            ImageFileType.GIF => ImageHelper.GetDrawingBitmapSource(ImageFilePath, GifIndex),
            ImageFileType.VIDEO => ImageHelper.GetCaptureMediaFrame(mediaItem),
            _ => null
        };

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

            imageViewUC.mediaItem.Height = height;
            imageViewUC.imgItem.Height = height;
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

            imageViewUC.ImageSafeFileName = System.IO.Path.GetFileName(filePath);
            imageViewUC.tbFileName.Text = imageViewUC.ImageSafeFileName;

            if (ImageHelper.IsGif(header))
            {
                imageViewUC._imageFileType = ImageFileType.GIF;
                imageViewUC.SetGifImage();
//                imageViewUC.SetGifImage2();
                return;
            }
            if (ImageHelper.IsWebp(header))
            {
                imageViewUC._imageFileType = ImageFileType.WEBP;
                imageViewUC.SetWebpImage();
                return;
            }

            if (VideoHelper.IsAVI(header))
            {
                imageViewUC._imageFileType = ImageFileType.VIDEO;
                imageViewUC.SetVideo();
                return;
            }
            if (VideoHelper.IsMP4(header))
            {
                imageViewUC._imageFileType = ImageFileType.VIDEO;
                imageViewUC.SetVideo();
                return;
            }
            if (VideoHelper.IsASF(header))
            {
                imageViewUC._imageFileType = ImageFileType.VIDEO;
                imageViewUC.SetVideo();
                return;
            }

            imageViewUC.SetImage();
        }

        private void btnStartIconPause()
        {
            btnStartIcon.Kind = PackIconKind.Pause;
        }
        private void btnStartIconPlay()
        {
            btnStartIcon.Kind = PackIconKind.Play;
        }
        private void mediaItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_imageFileType == ImageFileType.VIDEO)
            {
                MediaControl();
            }
            else
            {

            }
        }
        private void mediaItem_MediaOpened(object sender, RoutedEventArgs e)
        {
            _timer.Start();
            btnStartIconPause();
        }
        private void mediaItem_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ErrorCall(e.ErrorException.Message);
        }
        private void mediaItem_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaItem.Stop();
            _timer.Stop();
            btnStartIconPlay();
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_imageFileType == ImageFileType.VIDEO)
            {
                MediaControl();
            }
            else
            {
                GifControl();
            }
        }
        private void sliderTimeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_imageFileType != ImageFileType.VIDEO)
            {
                if (_timer.IsEnabled)
                {
                    return;
                }
                GifIndex = (int)sliderTimeline.Value;
                if (_image != null)
                {
                    imgItem.Source = ImageHelper.GetDrawingBitmapSource(_image, GifIndex);
                }
            }
            else
            {
                if (ImageHelper.GetMediaState(mediaItem) == MediaState.Play)
                {
                    return;
                }
                mediaItem.Position = TimeSpan.FromSeconds(sliderTimeline.Value);
            }
            SetTimeLineCount();
        }
        private void sliderTimeline_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_imageFileType != ImageFileType.VIDEO)
            {
                _previousMediaState = _timer.IsEnabled ? MediaState.Play : MediaState.Stop;
                if (_previousMediaState == MediaState.Play)
                {
                    _timer.Stop();
                }
            }
            else
            {
                _previousMediaState = ImageHelper.GetMediaState(mediaItem);
                if (_previousMediaState == MediaState.Play)
                {
                    mediaItem.Pause();
                    _timer.Stop();
                }
            }
        }
        private void sliderTimeline_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_imageFileType != ImageFileType.VIDEO)
            {
                if (_previousMediaState == MediaState.Play)
                {
                    _timer.Start();
                }
            }
            else
            {
                if (_previousMediaState == MediaState.Play)
                {
                    mediaItem.Play();
                    _timer.Start();
                }
            }
        }
        private void btnMute_Click(object sender, RoutedEventArgs e)
        {
            mediaItem.IsMuted = !mediaItem.IsMuted;
            btnMuteIcon.Kind = mediaItem.IsMuted ? PackIconKind.Mute : PackIconKind.VolumeMedium;
        }

        private void SetGifImage()
        {
            mediaItem.Visibility = Visibility.Collapsed;
            btnMute.Visibility = Visibility.Collapsed;
            _image = ImageHelper.GetDrawingImage(ImageFilePath);
            imgItem.Source = ImageHelper.GetDrawingBitmapSource(ImageFilePath);
            TimerSetting();
            DrawingGifImages();
        }
        private void DrawingGifImages()
        {
//            for (int i = 0; i < _gifFrameSize; i++)
//            {
//                _gifList.Add(ImageHelper.GetDrawingBitmapSource(_image, i));
//            }
            _timer.Start();
        }
        private void SetGifImage2()
        {
            imgItem.Visibility = Visibility.Collapsed;
            btnMute.Visibility = Visibility.Collapsed;
            mediaItem.Source = new Uri(ImageFilePath, UriKind.RelativeOrAbsolute);
            mediaItem.Play();
            TimerSetting2();
        }
        private void SetWebpImage()
        {
            gridPlayControl.Visibility = Visibility.Collapsed;
            mediaItem.Visibility = Visibility.Collapsed;
            imgItem.Source = ImageHelper.GetWebpBitmapSource(ImageFilePath);
        }
        private void SetImage()
        {
            gridPlayControl.Visibility = Visibility.Collapsed;
            mediaItem.Visibility = Visibility.Collapsed;
            imgItem.Source = ImageHelper.GetDrawingBitmapSource(ImageFilePath);
        }
        private void SetVideo()
        {
            imgItem.Visibility = Visibility.Collapsed;
            mediaItem.Source = new Uri(ImageFilePath, UriKind.RelativeOrAbsolute);
            mediaItem.Play();
            TimerSetting2();
        }
        private void SetTimeLineCount()
        {
            if (_imageFileType == ImageFileType.GIF)
            {
                tbLabTime.Text = $"{GifIndex} / {_gifFrameSize}";
            }
            if (_imageFileType == ImageFileType.WEBP)
            {
                var curTime = mediaItem.Position.ToString();
                curTime = curTime[..curTime.IndexOf('.')];
                var totalTime = "??:??:??";
                if (mediaItem.NaturalDuration.HasTimeSpan)
                {
                    totalTime = mediaItem.NaturalDuration.TimeSpan.ToString();
                }
                tbLabTime.Text = $"{curTime}/{totalTime}";
            }
            if (_imageFileType == ImageFileType.VIDEO)
            {
                var curTime = mediaItem.Position.ToString();
                if (curTime.IndexOf('.') > 0)
                {
                    curTime = curTime[..curTime.IndexOf('.')];
                }
                var totalTime = "??:??:??";
                if (mediaItem.NaturalDuration.HasTimeSpan)
                {
                    totalTime = mediaItem.NaturalDuration.TimeSpan.ToString();
                }
                else
                {
                    totalTime = VideoHelper.GetFFMpegVideoDuration(ImageFilePath).ToString();
                }
                tbLabTime.Text = $"{curTime}/{totalTime}";
            }
        }

        private void ErrorCall(string? message)
        {
            var ret = message ?? "not defined error";
            ErrorCommand?.Execute(ret);
            RaiseEvent(new(ErrorEventProperty, this));
        }

        private void MediaControl()
        {
            switch (ImageHelper.GetMediaState(mediaItem))
            {
                case MediaState.Pause:
                    {
                        mediaItem.Play();
                        _timer.Start();
                        btnStartIconPause();
                    }
                    break;
                case MediaState.Play:
                    {
                        mediaItem.Pause();
                        _timer.Stop();
                        btnStartIconPlay();
                    }
                    break;
                case MediaState.Stop:
                    {
                        mediaItem.Position = TimeSpan.FromMilliseconds(1);
                        mediaItem.Play();
                        _timer.Start();
                        btnStartIconPause();
                    }
                    break;
            }
        }
        private void GifControl()
        {
            if (_imageFileType != ImageFileType.GIF)
            {
                return;
            }

            var isPlay = _timer.IsEnabled;
            var isPause = !isPlay && GifIndex < _gifFrameSize;
            var isStop = !isPlay && GifIndex >= _gifFrameSize;

            if (isPause)
            {
                _timer.Start();
                btnStartIconPause();
            }
            else if (isPlay)
            {
                _timer.Stop();
                btnStartIconPlay();
            }
            else if (isStop)
            {
                GifIndex = 0;
                _timer.Start();
                btnStartIconPause();
            }
        }

        private void TimerSetting()
        {
            Thread.Sleep(100);
            if (mediaItem.NaturalDuration.HasTimeSpan)
            {
                sliderTimeline.Maximum = mediaItem.NaturalDuration.TimeSpan.TotalSeconds;
                _timer.Interval = TimeSpan.FromMilliseconds(ImageHelper.GetGifFrameTotalDelay(ImageFilePath));
                _timer.Tick += (sender, e) =>
                {
                    sliderTimeline.Value = mediaItem.Position.TotalSeconds;
                    SetTimeLineCount();
                };
            }
            else
            {
                _gifFrameSize = ImageHelper.GetGifFrameSize(ImageFilePath);
                sliderTimeline.Maximum = ImageHelper.GetDrawingImageFrameSize(ImageFilePath);
                _timer.Interval = TimeSpan.FromMilliseconds(ImageHelper.GetGifFrameDelay(ImageFilePath));
                _timer.Tick += (sender, e) =>
                {
                    if (GifIndex >= _gifFrameSize)
                    {
                        _timer.Stop();
                        btnStartIconPlay();
                        return;
                    }
                    _timer.Interval = TimeSpan.FromMilliseconds(ImageHelper.GetGifFrameDelay(ImageFilePath));
                    if (_image != null)
                    {
//                        imgItem.Source = _gifList[GifIndex];
                        imgItem.Source = ImageHelper.GetDrawingBitmapSource(_image, GifIndex);
                    }

                    GifIndex++;
                    sliderTimeline.Value = GifIndex;
                    SetTimeLineCount();
                };
            }
        }
        private void TimerSetting2()
        {
            Thread.Sleep(100);
            if (mediaItem.NaturalDuration.HasTimeSpan)
            {
                sliderTimeline.Maximum = mediaItem.NaturalDuration.TimeSpan.TotalSeconds;
                _timer.Interval = TimeSpan.FromMilliseconds(50);
                _timer.Tick += (sender, e) =>
                {
                    sliderTimeline.Value = mediaItem.Position.TotalSeconds;
                    SetTimeLineCount();
                };
            }
            else
            {
                sliderTimeline.Maximum = VideoHelper.GetFFMpegVideoDuration(ImageFilePath).TotalSeconds;
                _timer.Interval = TimeSpan.FromMilliseconds(VideoHelper.GetFFMpegFrameRate(ImageFilePath));
                _timer.Tick += (sender, e) =>
                {
                    sliderTimeline.Value = mediaItem.Position.TotalSeconds;
                    SetTimeLineCount();
                };
            }
        }
    }
}
