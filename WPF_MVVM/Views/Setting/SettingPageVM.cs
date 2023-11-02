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
using WPF_MVVM.Models.DataGovAPI;
using WPF_MVVM.Models.Messages;

namespace WPF_MVVM.Views.Setting
{
    internal partial class SettingPageVM : PaneDocumentViewModel
    {
        [ObservableProperty]
        private bool _isDarkMode;
        [ObservableProperty]
        private List<CommonComboItem> _notifyOptions;
        private int _selectedNotifyOptionIndex;
        [ObservableProperty]
        private int _notifyDuration;
        public SettingPageVM() : base()
        {
            Title = "SETTING";
            ContentID = nameof(SettingPageVM);

            _isDarkMode = FBaseFunc.Ins.Cfg.IsDarkTheme;
            _notifyOptions = new();
            _selectedNotifyOptionIndex = -1;

            IsDarkModeCommand = new RelayCommand<bool>(IsDarkModeEvent);
            NotifyTestCommand = new RelayCommand(NotifyTestEvent);
            NotifyDurationSaveCommand = new RelayCommand(NotifyDurationSaveDurationEvent);

            Init();
        }

        public IRelayCommand IsDarkModeCommand { get; }
        public IRelayCommand NotifyTestCommand { get; }
        public IRelayCommand NotifyDurationSaveCommand { get; }

        public int SelectedNotifyOptionIndex
        {
            get => _selectedNotifyOptionIndex;
            set
            {
                _selectedNotifyOptionIndex = value;
                SaveNotifyType();
                OnPropertyChanged();
            }
        }
        protected override void CloseEvent(object? data)
        {
            WeakReferenceMessenger.Default.Send(new NavigationMessage(nameof(SettingPage)) { Sender = this, Close = true });
            base.CloseEvent(data);
        }

        private void Init()
        {
            NotifyOptions = new()
            {
                new CommonComboItem()
                {
                    Index = 1,
                    Name = "스낵바",
                    Description = "Snackbar"
                },
                new CommonComboItem()
                {
                    Index = 1,
                    Name = "다이얼로그",
                    Description = "Dialog"
                },
                new CommonComboItem()
                {
                    Index = 1,
                    Name = "메세지 박스",
                    Description = "MessageBox"
                }
            };

            switch (FBaseFunc.Ins.Cfg.NotifyOption)
            {
                case "Snackbar": _selectedNotifyOptionIndex = 0; break;
                case "Dialog": _selectedNotifyOptionIndex = 1; break;
                case "MessageBox": _selectedNotifyOptionIndex = 2; break;
            }

            NotifyDuration = FBaseFunc.Ins.Cfg.NotifyDuration;
        }

        private void IsDarkModeEvent(bool data)
        {
            if (data == FBaseFunc.Ins.Cfg.IsDarkTheme)
            {
                return;
            }

            FBaseFunc.Ins.SetDarkTheme(data);
        }
        private void NotifyTestEvent()
        {
            WeakReferenceMessenger.Default.Send(new AlertMessage(true) { Sender = this, Header = "notify test", Message = "노티 테스트" });
        }
        private void SaveNotifyType()
        {
            FBaseFunc.Ins.Cfg.SetNotifyType(NotifyOptions[SelectedNotifyOptionIndex].Description ?? "Snackbar");
        }
        private void NotifyDurationSaveDurationEvent()
        {
            FBaseFunc.Ins.Cfg.SetNotifyDuration(NotifyDuration);
            NotifyDuration = FBaseFunc.Ins.Cfg.NotifyDuration;
        }
    }
}
