using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPF_MVVM.Bases;

namespace WPF_MVVM.Controls.WebViews
{
    /// <summary>
    /// WebView2UC.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WebView2UC : UserControl, IBound
    {
        public static readonly DependencyProperty IsNavigationStartProperty = DependencyProperty.Register(nameof(IsNavigationStart), typeof(bool), typeof(WebView2UC), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty WebUrlProperty = DependencyProperty.Register(nameof(WebUrl), typeof(string), typeof(WebView2UC), new PropertyMetadata(string.Empty, WebUrlChanged));
        public static readonly DependencyProperty ExecuteScriptProperty = DependencyProperty.Register(nameof(ExecuteScript), typeof(string), typeof(WebView2UC), new PropertyMetadata(string.Empty, ExecuteScriptChanged));
        public static readonly DependencyProperty ExtraParamProperty = DependencyProperty.Register(nameof(ExtraParam), typeof(Dictionary<string, object?>), typeof(WebView2UC), new PropertyMetadata(null, ExtraParamChanged));
        public static readonly DependencyProperty ExtraCommandProperty = DependencyProperty.Register(nameof(ExtraCommand), typeof(object), typeof(WebView2UC));
        public static readonly RoutedEvent ExtraEventProperty = EventManager.RegisterRoutedEvent(nameof(ExtraEvent), RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(WebView2UC));
        public static readonly RoutedEvent ExtraEventAsyncProperty = EventManager.RegisterRoutedEvent(nameof(ExtraEventAsync), RoutingStrategy.Direct, typeof(Task<RoutedEventHandler>), typeof(WebView2UC));

        public bool IsNavigationStart
        {
            get => (bool)GetValue(IsNavigationStartProperty);
            set => SetValue(IsNavigationStartProperty, value);
        }
        public string WebUrl
        {
            get => (string)GetValue(WebUrlProperty);
            set => SetValue(WebUrlProperty, value);
        }
        public string ExecuteScript
        {
            get => (string)GetValue(ExecuteScriptProperty);
            set => SetValue(ExecuteScriptProperty, value);
        }
        public Dictionary<string, object?>? ExtraParam
        {
            get => (Dictionary<string, object?>)GetValue(ExtraParamProperty);
            set => SetValue(ExtraParamProperty, value);
        }
        public ICommand? ExtraCommand
        {
            get => (ICommand)GetValue(ExtraCommandProperty);
            set => SetValue(ExtraCommandProperty, value);
        }
        public event RoutedEventHandler? ExtraEvent
        {
            add => AddHandler(ExtraEventProperty, value);
            remove => RemoveHandler(ExtraEventProperty, value);
        }
        public event RoutedEventHandler? ExtraEventAsync
        {
            add => AddHandler(ExtraEventAsyncProperty, value);
            remove => RemoveHandler(ExtraEventAsyncProperty, value);
        }

        public WebView2UC()
        {
            InitializeComponent();
        }

        protected async override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            try
            {
                var webView2Env = await CoreWebView2Environment.CreateAsync();
                await webView2.EnsureCoreWebView2Async(webView2Env);
                webView2.CoreWebView2.AddHostObjectToScript("bound", this);
            }
            catch (Exception ex)
            {
                await FBaseFunc.Ins.SetLogAsync(ex.Message);
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            webView2.ContentLoading += WebView2_ContentLoading;
            webView2.CoreWebView2InitializationCompleted += WebView2_CoreWebView2InitializationCompleted;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (IsCoreWebView2Init() == false)
            {
                return;
            }

            webView2.CoreWebView2.NavigationStarting -= CoreWebView2_NavigationStarting;
            webView2.CoreWebView2.SourceChanged -= CoreWebView2_SourceChanged;
            webView2.CoreWebView2.ContentLoading -= CoreWebView2_ContentLoading;
            webView2.CoreWebView2.HistoryChanged -= CoreWebView2_HistoryChanged;
            webView2.CoreWebView2.DOMContentLoaded -= CoreWebView2_DOMContentLoaded;
            webView2.CoreWebView2.NavigationCompleted -= CoreWebView2_NavigationCompleted;
            webView2.Dispose();
        }

        private void WebView2_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (IsCoreWebView2Init() == false)
            {
                return;
            }

            webView2.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            webView2.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
            webView2.CoreWebView2.ContentLoading += CoreWebView2_ContentLoading;
            webView2.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
            webView2.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
            webView2.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;

            if (WebUrl.Length > 0)
            {
                webView2.CoreWebView2.Navigate(WebUrl);
            }
        }
        private void WebView2_ContentLoading(object? sender, CoreWebView2ContentLoadingEventArgs e)
        {
        }
        private void CoreWebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            IsNavigationStart = true;
        }
        private void CoreWebView2_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
        {
        }
        private void CoreWebView2_ContentLoading(object? sender, CoreWebView2ContentLoadingEventArgs e)
        {
        }
        private void CoreWebView2_HistoryChanged(object? sender, object e)
        {
        }
        private void CoreWebView2_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
        }
        private async void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            IsNavigationStart = false;
            var script = "document.documentElement.style.overflow ='hidden'";
            await webView2.CoreWebView2.ExecuteScriptAsync(script);
        }

        private static void WebUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not WebView2UC control)
            {
                return;
            }
            if (e.NewValue == null || e.NewValue?.ToString()?.Trim().Length <= 0)
            {
                return;
            }
            if (control.IsCoreWebView2Init() == false)
            {
                return;
            }
            if (control.webView2.IsEnabled == false)
            {
                return;
            }

            control.webView2.CoreWebView2.Navigate(e.NewValue?.ToString());
        }
        private static async void ExecuteScriptChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not WebView2UC control)
            {
                return;
            }
            if (control.IsCoreWebView2Init() == false)
            {
                return;
            }
            if (e.NewValue is not string script)
            {
                return;
            }
            if (script.Trim().Length <= 0)
            {
                return;
            }

            await control.ExecuteScriptAsync(script);
            Thread.Sleep(500);

        }
        private static void ExtraParamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not WebView2UC control)
            {
                return;
            }

            if (e.NewValue == null || e.NewValue is not Dictionary<string, object?> newValue)
            {
                return;
            }

            control.ExtraParam = newValue;
        }

        private bool IsCoreWebView2Init()
        {
            return webView2.CoreWebView2 != null;
        }

        private async Task ExecuteScriptAsync(string script)
        {
            var ret = await webView2.CoreWebView2.ExecuteScriptAsync(script);
            await Task.Delay(500);
        }

        public void callForm(object? data)
        {
            var ret = new Dictionary<string, object?>()
            {
                { nameof(callForm), data }
            };
            ExtraParam = ret;
            if (ExtraCommand != null)
            {
                ExtraCommand.Execute(ret);
                return;
            }
            RaiseEvent(new(ExtraEventProperty, this));
        }
        public void closeForm()
        {
            var ret = new Dictionary<string, object?>()
            {
                { nameof(closeForm), null }
            };
            ExtraParam = ret;
            RaiseEvent(new(ExtraEventProperty, this));
        }
    }
}
