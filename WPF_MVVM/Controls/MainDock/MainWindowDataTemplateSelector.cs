using System.Windows;
using System.Windows.Controls;
using WPF_MVVM.Views.Home;
using WPF_MVVM.Views.Setting;

namespace WPF_MVVM.Controls.MainDock
{
    internal class MainWindowDataTemplateSelector : DataTemplateSelector
    {
        public MainWindowDataTemplateSelector()
        {    
        }

        public DataTemplate? Home { get; set; }
        public DataTemplate? Setting { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is HomePageVM && Home is not null) return Home;
            if (item is SettingPageVM && Setting is not null) return Setting;

            return base.SelectTemplate(item, container);
        }
    }
}
