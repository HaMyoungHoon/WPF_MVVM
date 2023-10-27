using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPF_MVVM.Bases;
using WPF_MVVM.Models.Messages;

namespace WPF_MVVM.Views.Setting
{
    internal partial class SettingPageVM : PaneDocumentViewModel
    {
        [ObservableProperty]
        private bool _isDarkMode;
        public SettingPageVM() : base()
        {
            Title = "SETTING";
            _isDarkMode = FBaseFunc.Ins.Cfg.IsDarkTheme;
            ContentID = nameof(SettingPageVM);

            IsDarkModeCommand = new RelayCommand<bool>(IsDarkModeEvent);
        }

        public IRelayCommand IsDarkModeCommand { get; }

        protected override void CloseEvent(object? data)
        {
            WeakReferenceMessenger.Default.Send(new NavigationMessage(nameof(SettingPage)) { Sender = this, Close = true });
            base.CloseEvent(data);
        }


        private void IsDarkModeEvent(bool data)
        {
            if (data == FBaseFunc.Ins.Cfg.IsDarkTheme)
            {
                return;
            }

            FBaseFunc.Ins.SetDarkTheme(data);
        }
    }
}
