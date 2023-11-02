using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using WPF_MVVM.Bases;
using WPF_MVVM.Views.Home;
using WPF_MVVM.Views.Setting;
using WPF_MVVM.Views.WthrChartInfo;

namespace WPF_MVVM
{
    internal partial class MainWindowVM
    {
        [ObservableProperty]
        private Tuple<string, AvalonDock.Themes.Theme> _selectedTheme;
        [ObservableProperty]
        private ObservableCollection<PaneDocumentViewModel> _dockItemList;
        [ObservableProperty]
        private PaneDocumentViewModel? _activePage;

        public IRelayCommand FrameContentRenderedCommand { get; }

        private void FrameContentRenderedEvent(object? data)
        {
            if (data is not (Frame sender, EventArgs e))
            {
                return;
            }
            if (sender.Content is not Page child)
            {
                return;
            }

            switch (child.GetType().Name)
            {
                case nameof(HomePage):
                    {
                        var findVM = DockItemList.FirstOrDefault(x => x.ContentID == nameof(HomePageVM));
                        if (findVM is HomePageVM vm)
                        {
                            child.DataContext = vm;
                            vm.OnNavigated(this, null);
                        }
                    }
                    break;
                case nameof(WthrChartInfoPage):
                    {
                        var findVM = DockItemList.FirstOrDefault(x => x.ContentID == nameof(WthrChartInfoPageVM));
                        if (findVM is WthrChartInfoPageVM vm)
                        {
                            child.DataContext = vm;
                            vm.OnNavigated(this, null);
                        }
                    }
                    break;
                case nameof(SettingPage):
                    {
                        var findVM = DockItemList.FirstOrDefault(x => x.ContentID == nameof(SettingPageVM));
                        if (findVM is SettingPageVM vm)
                        {
                            child.DataContext = vm;
                            vm.OnNavigated(this, null);
                        }
                    }
                    break;
            }
        }
    }
}
