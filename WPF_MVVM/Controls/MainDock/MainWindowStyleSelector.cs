using System.Windows;
using System.Windows.Controls;
using WPF_MVVM.Views.Home;
using WPF_MVVM.Views.Setting;
using WPF_MVVM.Views.WthrChartInfo;

namespace WPF_MVVM.Controls.MainDock
{
    internal class MainWindowStyleSelector : StyleSelector
    {
        public MainWindowStyleSelector()
        {
        }

        public Style? Home { get; set; }
        public Style? WthrChartInfo { get; set; }
        public Style? Setting { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is HomePageVM && Home is not null) return Home;
            if (item is WthrChartInfoPageVM && WthrChartInfo is not null) return WthrChartInfo;
            if (item is SettingPageVM && Setting is not null) return Setting;

            return base.SelectStyle(item, container);
        }
    }
}
