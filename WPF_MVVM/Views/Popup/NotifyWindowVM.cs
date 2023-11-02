using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using WPF_MVVM.Bases;
using WPF_MVVM.Interfaces;
using WPF_MVVM.Models.Messages;

namespace WPF_MVVM.Views.Popup
{
    internal partial class NotifyWindowVM : ViewModelBase, IPopupWindow
    {
        public object? Sender { get; set; }
        [ObservableProperty]
        private string _header;
        [ObservableProperty]
        private string _message;
        [ObservableProperty]
        private string _duration;
        private double _during;
        private DispatcherTimer _timer;
        public NotifyWindowVM()
        {
            _header = string.Empty;
            _message = string.Empty;
            _duration = string.Empty;
            _during = 0;
            _timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += _timer_Tick;

            CloseCommand = new RelayCommand(CloseEvent);
        }
        public IRelayCommand CloseCommand { get; }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            if (_during <= 0)
            {
                _timer.Stop();
                WeakReferenceMessenger.Default.Send(new AlertMessage(true) { Sender = Sender, Close = true });
                return;
            }
            _during -= 0.1;
            Duration = $"{Math.Round(_during, 1)} 초 후에 자동으로 닫힙니다.";
        }
        private void CloseEvent()
        {
            _timer.Stop();
            WeakReferenceMessenger.Default.Send(new AlertMessage(true) { Sender = Sender, Close = true });
        }
        public override void OnNavigating(object? sender, object? navigationEventArgs)
        {
            base.OnNavigating(sender, navigationEventArgs);
            _timer.Stop();
        }
        public override void OnNavigated(object? sender, object? navigatedEventArgs)
        {
            Sender = sender;
            base.OnNavigated(sender, navigatedEventArgs);
            if (navigatedEventArgs is Dictionary<string, string> dict)
            {
                Header = dict.ContainsKey("Header") ? dict["Header"] : string.Empty;
                Message = dict.ContainsKey("Message") ? dict["Message"] : string.Empty;
                _ = double.TryParse(dict.ContainsKey("During") ? dict["During"] : "0", out double during);
                _during = during;
                if (_during <= 0)
                {
                    _during = 3600;
                }
                Duration = $"{Math.Round(_during, 1)} 초 후에 자동으로 닫힙니다.";
            }
            if (_timer.IsEnabled == false)
            {
                _timer.Start();
            }
        }
    }
}
