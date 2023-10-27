using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPF_MVVM.Interfaces;

namespace WPF_MVVM.Behaviors
{
    internal class FrameBehavior : Behavior<Frame>
    {
        private bool _isWork;
        public static readonly DependencyProperty NavigationSourceProperty = DependencyProperty.Register(nameof(NavigationSource), typeof(string), typeof(FrameBehavior), new PropertyMetadata(null, NavigationSourceChanged));
        public static readonly DependencyProperty NavigationParamProperty = DependencyProperty.Register(nameof(NavigationParam), typeof(object), typeof(FrameBehavior), new PropertyMetadata(null, NavigationParamChanged));
        public static readonly DependencyProperty NavigationSenderProperty = DependencyProperty.Register(nameof(NavigationSender), typeof(object), typeof(FrameBehavior), new PropertyMetadata(null, NavigationSenderChanged));
        public static readonly DependencyProperty ContentRenderedProperty = DependencyProperty.Register(nameof(ContentRendered), typeof(ICommand), typeof(FrameBehavior));
        public string NavigationSource
        {
            get => (string)GetValue(NavigationSourceProperty);
            set => SetValue(NavigationSourceProperty, value);
        }
        public object? NavigationParam
        {
            get => (object?)GetValue(NavigationParamProperty);
            set => SetValue(NavigationParamProperty, value);
        }
        public object? NavigationSender
        {
            get => (object?)GetValue(NavigationSenderProperty);
            set => SetValue(NavigationSenderProperty, value);
        }
        public ICommand ContentRendered
        {
            get => (ICommand)GetValue(ContentRenderedProperty);
            set => SetValue(ContentRenderedProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.Navigating += AssociatedObject_Navigating;
            AssociatedObject.Navigated += AssociatedObject_Navigated;
            AssociatedObject.ContentRendered += AssociatedObject_ContentRendered;
        }
        protected override void OnDetaching()
        {
            AssociatedObject.Navigating -= AssociatedObject_Navigating;
            AssociatedObject.Navigated -= AssociatedObject_Navigated;
            AssociatedObject.ContentRendered -= AssociatedObject_ContentRendered;
        }

        private void AssociatedObject_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (AssociatedObject.Content is Page pageContent &&
                pageContent.DataContext is INavigationAware navigationAware)
            {
                navigationAware?.OnNavigating(sender, e);
            }
        }

        private void AssociatedObject_Navigated(object sender, NavigationEventArgs e)
        {
            _isWork = true;
            NavigationSource = e.Uri.ToString();
            //            NavigationParam = e.ExtraData;
            _isWork = false;
            if (AssociatedObject.Content is Page pageContent &&
                pageContent.DataContext is INavigationAware navigationAware)
            {
                navigationAware.OnNavigated(NavigationSender ?? sender, e);
            }
        }
        private void AssociatedObject_ContentRendered(object? sender, EventArgs e)
        {
            ContentRendered?.Execute((sender, e));
        }
        private static void NavigationSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (FrameBehavior)d;
            if (behavior._isWork)
            {
                return;
            }
            behavior.Navigate();
        }
        private static void NavigationParamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (FrameBehavior)d;
            if (behavior._isWork)
            {
                return;
            }
            behavior.NavigationParam = e.NewValue;
        }
        private static void NavigationSenderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (FrameBehavior)d;
            if (behavior._isWork)
            {
                return;
            }
            behavior.NavigationSender = e.NewValue;
        }
        private void Navigate()
        {
            switch (NavigationSource)
            {
                case "GoBack":
                    {
                        if (AssociatedObject.CanGoBack)
                        {
                            AssociatedObject.GoBack();
                        }
                    }
                    break;
                case null: break;
                case "": break;
                default:
                    {
                        AssociatedObject.Navigate(new Uri(NavigationSource, UriKind.RelativeOrAbsolute), NavigationParam);
                    }
                    break;
            }
        }
    }
}
