using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WPF_MVVM.Behaviors
{
    internal class MediaElementBehavior : Behavior<MediaElement>
    {
        public static readonly DependencyProperty MediaLoadedCommandProperty = DependencyProperty.RegisterAttached(nameof(MediaLoadedCommand), typeof(ICommand), typeof(MediaElementBehavior));
        public static readonly DependencyProperty MouseLeftButtonUpCommandProperty = DependencyProperty.RegisterAttached(nameof(MouseLeftButtonUpCommand), typeof(ICommand), typeof(MediaElementBehavior));
        public static readonly DependencyProperty MouseLeftButtonDownCommandProperty = DependencyProperty.RegisterAttached(nameof(MouseLeftButtonDownCommand), typeof(ICommand), typeof(MediaElementBehavior));
        public static readonly DependencyProperty MouseRightButtonUpCommandProperty = DependencyProperty.RegisterAttached(nameof(MouseRightButtonUpCommand), typeof(ICommand), typeof(MediaElementBehavior));
        public static readonly DependencyProperty MouseRightButtonDownCommandProperty = DependencyProperty.RegisterAttached(nameof(MouseRightButtonDownCommand), typeof(ICommand), typeof(MediaElementBehavior));
        public static readonly DependencyProperty MediaOpenedCommandProperty = DependencyProperty.RegisterAttached(nameof(MediaOpenedCommand), typeof(ICommand), typeof(MediaElementBehavior));
        public static readonly DependencyProperty MediaFailedCommandProperty = DependencyProperty.RegisterAttached(nameof(MediaFailedCommand), typeof(ICommand), typeof(MediaElementBehavior));
        public static readonly DependencyProperty MediaEndedCommandProperty = DependencyProperty.RegisterAttached(nameof(MediaEndedCommand), typeof(ICommand), typeof(MediaElementBehavior));

        public MediaElementBehavior()
        {
        }

        public ICommand MediaLoadedCommand
        {
            get => (ICommand)GetValue(MediaLoadedCommandProperty);
            set => SetValue(MediaLoadedCommandProperty, value);
        }
        public ICommand MouseLeftButtonUpCommand
        {
            get => (ICommand)GetValue(MouseLeftButtonUpCommandProperty);
            set => SetValue(MouseLeftButtonUpCommandProperty, value);
        }
        public ICommand MouseLeftButtonDownCommand
        {
            get => (ICommand)GetValue(MouseLeftButtonDownCommandProperty);
            set => SetValue(MouseLeftButtonDownCommandProperty, value);
        }
        public ICommand MouseRightButtonUpCommand
        {
            get => (ICommand)GetValue(MouseRightButtonUpCommandProperty);
            set => SetValue(MouseRightButtonUpCommandProperty, value);
        }
        public ICommand MouseRightButtonDownCommand
        {
            get => (ICommand)GetValue(MouseRightButtonDownCommandProperty);
            set => SetValue(MouseRightButtonDownCommandProperty, value);
        }
        public ICommand MediaOpenedCommand
        {
            get => (ICommand)GetValue(MediaOpenedCommandProperty);
            set => SetValue(MediaOpenedCommandProperty, value);
        }
        public ICommand MediaFailedCommand
        {
            get => (ICommand)GetValue(MediaFailedCommandProperty);
            set => SetValue(MediaFailedCommandProperty, value);
        }
        public ICommand MediaEndedCommand
        {
            get => (ICommand)GetValue(MediaEndedCommandProperty);
            set => SetValue(MediaEndedCommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
        }
        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            MediaLoadedCommand?.Execute((sender, e));
            AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseRightButtonUp += AssociatedObject_MouseRightButtonUp;
            AssociatedObject.MouseRightButtonDown += AssociatedObject_MouseRightButtonDown;
            AssociatedObject.MediaOpened += AssociatedObject_MediaOpened;
            AssociatedObject.MediaFailed += AssociatedObject_MediaFailed;
            AssociatedObject.MediaEnded += AssociatedObject_MediaEnded;
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonUp;
            AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseRightButtonUp -= AssociatedObject_MouseRightButtonUp;
            AssociatedObject.MouseRightButtonDown -= AssociatedObject_MouseRightButtonDown;
            AssociatedObject.MediaOpened -= AssociatedObject_MediaOpened;
            AssociatedObject.MediaFailed -= AssociatedObject_MediaFailed;
            AssociatedObject.MediaEnded -= AssociatedObject_MediaEnded;
        }

        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtonUpCommand?.Execute((sender, e));
        }
        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseLeftButtonDownCommand?.Execute((sender, e));
        }
        private void AssociatedObject_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseRightButtonUpCommand?.Execute((sender, e));
        }
        private void AssociatedObject_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseRightButtonDownCommand?.Execute((sender, e));
        }
        private void AssociatedObject_MediaOpened(object sender, RoutedEventArgs e)
        {
            MediaOpenedCommand?.Execute((sender, e));
        }
        private void AssociatedObject_MediaFailed(object? sender, ExceptionRoutedEventArgs e)
        {
            MediaFailedCommand?.Execute((sender, e));
        }
        private void AssociatedObject_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaEndedCommand?.Execute((sender, e));
        }
    }
}
