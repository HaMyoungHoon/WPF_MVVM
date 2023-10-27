using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
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
using WPF_MVVM.Models.Messages;

namespace WPF_MVVM.Controls.NotifyDialog
{
    /// <summary>
    /// NotifyDialogUC.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NotifyDialogUC : UserControl
    {
        private DispatcherTimer _timer;
        private double _during;
        public NotifyDialogUC()
        {
            InitializeComponent();
            _timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += _timer_Tick;
        }
        private void _timer_Tick(object? sender, EventArgs e)
        {
            if (_during <= 0)
            {
                _timer.Stop();
                WeakReferenceMessenger.Default.Send(new AlertMessage(true) { Sender = this, Close = true });
                return;
            }
            _during -= 0.1;
            tbDuring.Text = $"{Math.Round(_during, 1)} 초 후에 자동으로 닫힙니다.";
        }
        public void SetThis(string? header = "", string? message = "", int during = 5)
        {
            tbHeader.Text = header ?? string.Empty;
            tbMessage.Text = message ?? string.Empty;
            _during = during;
            if (_during <= 0)
            {
                _during = 3600;
            }
            tbDuring.Text = $"{Math.Round(_during, 1)} 초 후에 자동으로 닫힙니다.";
            if (_timer.IsEnabled == false)
            {
                _timer.Start();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new AlertMessage(true) { Sender = this, Close = true });
        }
    }
}
