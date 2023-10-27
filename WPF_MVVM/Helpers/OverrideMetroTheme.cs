using AvalonDock.Themes;
using DocumentFormat.OpenXml.Wordprocessing;
using System;

namespace WPF_MVVM.Helpers
{
    internal class OverrideMetroTheme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri("pack://application:,,,/Styles/OverrideAvalonDock/OverrideAvalonDockMetroTheme.xaml");
        }
    }
}
