using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using WPF_MVVM.Bases;
using WPF_MVVM.Interfaces;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using WPF_MVVM.Models.Messages;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WPF_MVVM.Views.Home;
using WPF_MVVM.Views.Popup;
using WPF_MVVM.Controls.NotifyDialog;
using WPF_MVVM.Views.Setting;
using WPF_MVVM.Views.WthrChartInfo;

namespace WPF_MVVM
{
    internal partial class MainWindowVM : ViewModelBase
    {
        private DispatcherTimer _timer;

        [ObservableProperty]
        private string _title;
        [ObservableProperty]
        private bool _isDlgOpen;
        [ObservableProperty]
        private NotifyDialogUC _dlgNotify;

        private readonly List<ContentsViewerWindowVM> _contentsViewerWindowVM;

        [ObservableProperty]
        private bool _isLoading;
        private readonly IList<LoadingMessage> _loading;

        [ObservableProperty]
        SnackbarMessageQueue _notifyQueue;

        private readonly IWindowService _windowService;

        public MainWindowVM(IWindowService windowService)
        {
            _timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += _timer_Tick;
            _timer.Start();
            _title = string.Empty;

            _selectedTheme = FBaseFunc.Ins.GetAvalonDockTheme();
            _dockItemList = new();

            _dlgNotify = new();

            _contentsViewerWindowVM = new();

            _loading = new List<LoadingMessage>();

            _notifyQueue = new();

            _windowService = windowService;

            FrameContentRenderedCommand = new RelayCommand<object?>(FrameContentRenderedEvent, (data) => true);
            MenuButtonCommand = new RelayCommand<object?>(MenuButtonEvent, (obj) => true);
            PageChangeCommand = new RelayCommand<string?>(PageChangeEvent);

            WeakReferenceMessenger.Default.Register<NavigationMessage>(this, OnNavigationMessage);
            WeakReferenceMessenger.Default.Register<LoadingMessage>(this, OnLoadingMessage);
            WeakReferenceMessenger.Default.Register<AlertMessage>(this, OnAlertMessage);
            WeakReferenceMessenger.Default.Register<NewWindowMessage>(this, OnNewWindowMessage);
            WeakReferenceMessenger.Default.Register<EtcMessage>(this, OnEtcMessage);
        }
        private void _timer_Tick(object? sender, EventArgs e)
        {
//            System.GC.Collect();
//            System.GC.WaitForPendingFinalizers();
        }
        public IRelayCommand MenuButtonCommand { get; }
        public IRelayCommand PageChangeCommand { get; }

        private void MenuButtonEvent(object? obj)
        {
            if (obj == null)
            {
                return;
            }
            switch (obj.ToString())
            {

            }
        }
        private void PageChangeEvent(string? page)
        {
            switch (page)
            {
                case nameof(HomePage): WeakReferenceMessenger.Default.Send(new NavigationMessage(nameof(HomePage)) { Sender = this }); break;
                case nameof(WthrChartInfoPage): WeakReferenceMessenger.Default.Send(new NavigationMessage(nameof(WthrChartInfoPage)) { Sender = this }); break;
                case nameof(SettingPage): WeakReferenceMessenger.Default.Send(new NavigationMessage(nameof(SettingPage)) { Sender = this }); break;
            }
        }
        private void OnNavigationMessage(object recipient, NavigationMessage msg)
        {
            var contentID = string.Empty;
            switch (msg.Value)
            {
                case nameof(HomePage): contentID = nameof(HomePageVM); break;
                case nameof(WthrChartInfoPage): contentID = nameof(WthrChartInfoPageVM); break;
                case nameof(SettingPage): contentID = nameof(SettingPageVM); break;
            }

            if (contentID == string.Empty)
            {
                return;
            }

            if (msg.Close && msg.Sender is PaneDocumentViewModel doc)
            {
                DockItemList.Remove(doc);
                if (DockItemList.Count > 0)
                {
                    ActivePage = DockItemList.Last();
                }
                else
                {
                    ActivePage = null;
                }
                return;
            }

            var findVM = DockItemList.FirstOrDefault(x => x.ContentID == contentID);
            if (findVM is not null)
            {
                ActivePage = findVM;
                findVM.IsSelected = true;
                return;
            }

            switch (msg.Value)
            {
                case nameof(HomePage): DockItemList.Add(App.Current.Services.GetService(typeof(HomePageVM)) as HomePageVM ?? new HomePageVM() { Mother = this }); break;
                case nameof(WthrChartInfoPage): DockItemList.Add(App.Current.Services.GetService(typeof(WthrChartInfoPageVM)) as WthrChartInfoPageVM ?? new WthrChartInfoPageVM() { Mother = this }); break;
                case nameof(SettingPage): DockItemList.Add(App.Current.Services.GetService(typeof(SettingPageVM)) as SettingPageVM ?? new SettingPageVM() { Mother = this }); break;
            }
            ActivePage = DockItemList.Last();
        }
        private void OnLoadingMessage(object recipient, LoadingMessage msg)
        {
            if (msg.Value)
            {
                var existLoading = _loading.FirstOrDefault(x => x.LoadingId == msg.LoadingId);
                if (existLoading != null)
                {
                    return;
                }
                _loading.Add(msg);
            }
            else
            {
                var existLoading = _loading.FirstOrDefault(x => x.LoadingId == msg.LoadingId);
                if (existLoading == null)
                {
                    return;
                }
                _loading.Remove(existLoading);
            }
            IsLoading = _loading.Any();
        }
        private void OnAlertMessage(object recipient, AlertMessage msg)
        {
            if (msg.Close)
            {
                if (msg.Sender is NotifyDialogUC)
                {
                    IsDlgOpen = false;
                    return;
                }
                if (msg.Sender is NotifyWindow)
                {
                    WeakReferenceMessenger.Default.Send(new NewWindowMessage(nameof(NotifyWindow)) { Sender = recipient, Close = msg.Close });
                    return;
                }
            }

            switch (FBaseFunc.Ins.Cfg.NotifyOption)
            {
                case "Snackbar":
                    {
                        if (msg.Close)
                        {
                            NotifyQueue.Clear();
                            return;
                        }
                        var duration = (double)(msg.During ?? FBaseFunc.Ins.Cfg.NotifyDuration);
                        NotifyQueue.Enqueue($"{msg.Header}\r\n{msg.Message}\r\nafter {duration} sec close this", null, null, null, false, true, TimeSpan.FromSeconds(duration));
                    }
                    break;
                case "Dialog":
                    {
                        if (msg.Close)
                        {
                            IsDlgOpen = false;
                            return;
                        }
                        var duration = (msg.During ?? FBaseFunc.Ins.Cfg.NotifyDuration);
                        DlgNotify.SetThis(msg.Header, msg.Message, duration);
                        IsDlgOpen = true;
                    }
                    break;
                case "MessageBox":
                    {
                        string duration = (msg.During ?? FBaseFunc.Ins.Cfg.NotifyDuration).ToString();
                        Dictionary<string, string> args = new()
                        {
                            { "Header", msg.Header ?? "" },
                            { "Message", msg.Message ?? "" },
                            { "During", duration },
                        };
                        WeakReferenceMessenger.Default.Send(new NewWindowMessage(nameof(NotifyWindow)) { Sender = this, Close = msg.Close, NavigatedEventArgs = args });
                    }
                    break;
            }
        }
        private void OnNewWindowMessage(object recipient, NewWindowMessage msg)
        {
            switch (msg.Value)
            {
                case nameof(MainWindow): _windowService.CloseAll(); break;
                case nameof(ContentsViewerWindow):
                    {
                        if (msg.NavigatedEventArgs is not string contentSource)
                        {
                            return;
                        }
                        var vm = _contentsViewerWindowVM.Find(x => x.ContentSoruce == contentSource);
                        if (vm == null)
                        {
                            vm = new ContentsViewerWindowVM();
                            _contentsViewerWindowVM.Add(vm);
                        }
                        else
                        {
                            if (_windowService.GetWindow(vm, contentSource) == null)
                            {
                                _contentsViewerWindowVM.Remove(vm);
                                vm = new ContentsViewerWindowVM();
                                _contentsViewerWindowVM.Add(vm);
                            }
                        }
                        _windowService.ShowWindow<ContentsViewerWindow>(vm, msg.Close, msg.Hide, msg.Sender, msg.NavigatedEventArgs);
                        if (msg.Close)
                        {
                            _contentsViewerWindowVM.Remove(vm);
                        }
                    }
                    break;
                case nameof(NotifyWindow):
                    {
                        var dataContext = App.Current.Services.GetService(typeof(NotifyWindowVM));
                        var vm = _windowService.GetDataContext(dataContext) as NotifyWindowVM ?? dataContext as NotifyWindowVM ?? new();
                        _windowService.ShowWindow<NotifyWindow>(vm, msg.Close, msg.Hide, msg.Sender, msg.NavigatedEventArgs);
                    }
                    break;
            }

        }
        private void OnEtcMessage(object recipient, EtcMessage msg)
        {
            switch (msg.Value.Key)
            {
                case "ThemeChange":
                    {
                        SelectedTheme = FBaseFunc.Ins.GetAvalonDockTheme();
                    }
                    break;
            }
        }
        private void ButtonChanged()
        {
            MenuButtonCommand.NotifyCanExecuteChanged();
            PageChangeCommand.NotifyCanExecuteChanged();
        }

        public void SetMessageHook(Window mother)
        {
            WindowInteropHelper helper = new(mother);
            if (helper.Handle == IntPtr.Zero)
            {
                return;
            }
            HwndSource source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(HookingFunc);
        }
        private IntPtr HookingFunc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x10)
            {
                if (MessageBox.Show("종료?", "exit", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _timer.Stop();
                    _windowService.CloseAll();
                    // chattingservice.Close();
                    FBaseFunc.Ins.Dispose();
                    App.Current.Shutdown();
                    return IntPtr.Zero;
                }
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}
