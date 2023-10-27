﻿using Microsoft.Extensions.DependencyInjection;
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
using WPF_MVVM.Services;
using WPF_MVVM.Views.Home;
using WPF_MVVM.Views.Popup;
using WPF_MVVM.Views.Setting;

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
//            string weatherChartAPI = "http://apis.data.go.kr/1360000/WthrChartInfoService";

            return new ServiceCollection()
                .AddTransient(typeof(MainWindowVM))
                .AddTransient(typeof(NotifyWindowVM))
                .AddTransient(typeof(HomePageVM))
                .AddTransient(typeof(SettingPageVM))

                .AddSingleton<IWindowService, WindowService>(obj => new WindowService())

                .BuildServiceProvider();
        }
    }
}
