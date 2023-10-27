using AvalonDock.Themes;
using System;

namespace WPF_MVVM.Helpers
{
    internal class OverrideVS2013DarkTheme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri("pack://application:,,,/Styles/OverrideAvalonDock/OverrideAvalonDockVS2013DarkTheme.xaml");
        }
    }
}
