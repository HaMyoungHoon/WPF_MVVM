using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using WPF_MVVM.Bases;
using WPF_MVVM.Interfaces;
using WPF_MVVM.Interfaces.DataGov;
using WPF_MVVM.Models.RestAPI.DataGov.WthrChartInfo;
using WPF_MVVM.Services;
using WPF_MVVM.Views.Home;
using WPF_MVVM.Views.ImageConverter;
using WPF_MVVM.Views.Popup;
using WPF_MVVM.Views.Setting;
using WPF_MVVM.Views.WthrChartInfo;

namespace WPF_MVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        public new static App Current => (App)Application.Current;
        private static MainWindow? _mainInst;
        public static MainWindow MainInst
        {
            get => _mainInst ??= new MainWindow();
        }

        public App()
        {
            Services = ConfigureServices();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            FBaseFunc.Ins.Init();
            MainInst.Show();
        }

        private static IServiceProvider ConfigureServices()
        {
            string debugUrl = "http://localhost:12800";
            return new ServiceCollection()
                .AddTransient(typeof(MainWindowVM))
                .AddTransient(typeof(NotifyWindowVM))
                .AddTransient(typeof(HomePageVM))
                .AddTransient(typeof(WthrChartInfoPageVM))
                .AddTransient(typeof(ImageConverterPageVM))
                .AddTransient(typeof(SettingPageVM))

                .AddSingleton<IWindowService, WindowService>(obj => new WindowService())

//#if DEBUG
//                .AddSingleton<IWthrChartInfoService, WthrChartInfoService>(obj => new(debugUrl))
//#else
                .AddSingleton<IWthrChartInfoService, WthrChartInfoService>(obj => new())
//#endif

                .BuildServiceProvider();
        }
    }
}
