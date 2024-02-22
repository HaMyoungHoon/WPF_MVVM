using System.Windows;
using System.Windows.Controls;
using WPF_MVVM.Views.Home;
using WPF_MVVM.Views.ImageConverter;
using WPF_MVVM.Views.Setting;
using WPF_MVVM.Views.WthrChartInfo;

namespace WPF_MVVM.Controls.MainDock
{
    internal class MainWindowDataTemplateSelector : DataTemplateSelector
    {
        public MainWindowDataTemplateSelector()
        {    
        }

        public DataTemplate? Home { get; set; }
        public DataTemplate? WthrChartInfo { get; set; }
        public DataTemplate? ImageConverter { get; set; }
        public DataTemplate? Setting { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is HomePageVM && Home is not null) return Home;
            if (item is WthrChartInfoPageVM && WthrChartInfo is not null) return WthrChartInfo;
            if (item is ImageConverterPageVM && ImageConverter is not null) return ImageConverter;
            if (item is SettingPageVM && Setting is not null) return Setting;

            return base.SelectTemplate(item, container);
        }
    }
}
