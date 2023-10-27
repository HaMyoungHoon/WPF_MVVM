using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WPF_MVVM.Behaviors
{
    internal class MouseBehaviour : Behavior<Control>
    {
        private readonly DispatcherTimer _timer = new();
        public static readonly DependencyProperty MouseMoveCommandProperty = DependencyProperty.RegisterAttached(nameof(MouseMoveCommand), typeof(ICommand), typeof(MouseBehaviour));
        public static readonly DependencyProperty MouseLeaveCommandProperty = DependencyProperty.RegisterAttached(nameof(MouseLeaveCommand), typeof(ICommand), typeof(MouseBehaviour));
        public static readonly DependencyProperty MouseUpCommandProperty = DependencyProperty.RegisterAttached(nameof(MouseUpCommand), typeof(ICommand), typeof(MouseBehaviour));
        public static readonly DependencyProperty MouseDownCommandProperty = DependencyProperty.RegisterAttached(nameof(MouseDownCommand), typeof(ICommand), typeof(MouseBehaviour));
        public static readonly DependencyProperty MouseDoubleClickCommandProperty = DependencyProperty.RegisterAttached(nameof(MouseDoubleClickCommand), typeof(ICommand), typeof(MouseBehaviour));
        public static readonly DependencyProperty MouseWheelCommandProperty = DependencyProperty.RegisterAttached(nameof(MouseWheelCommand), typeof(ICommand), typeof(MouseBehaviour));

        public MouseBehaviour()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += _timer_Tick;
        }

        public ICommand MouseMoveCommand
        {
            get => (ICommand)GetValue(MouseMoveCommandProperty);
            set => SetValue(MouseMoveCommandProperty, value);
        }
        public ICommand MouseLeaveCommand
        {
            get => (ICommand)(GetValue(MouseLeaveCommandProperty));
            set => SetValue(MouseLeaveCommandProperty, value);
        }
        public ICommand MouseUpCommand
        {
            get => (ICommand)(GetValue(MouseUpCommandProperty));
            set => SetValue(MouseUpCommandProperty, value);
        }
        public ICommand MouseDownCommand
        {
            get => (ICommand)(GetValue(MouseDownCommandProperty));
            set => SetValue(MouseDownCommandProperty, value);
        }
        public ICommand MouseDoubleClickCommand
        {
            get => (ICommand)(GetValue(MouseDoubleClickCommandProperty));
            set => SetValue(MouseDoubleClickCommandProperty, value);
        }
        public ICommand MouseWheelCommand
        {
            get => (ICommand)(GetValue(MouseWheelCommandProperty));
            set => SetValue(MouseWheelCommandProperty, value);
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
            AssociatedObject.PreviewMouseLeftButtonUp += AssociatedObject_PreviewMouseLeftButtonUp;
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
            AssociatedObject.PreviewMouseLeftButtonUp -= AssociatedObject_PreviewMouseLeftButtonUp;
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
        }
        private void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseLeaveCommand?.Execute((sender, e));
        }
        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMoveCommand?.Execute((sender, e));
        }
        private void AssociatedObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseUpCommand?.Execute((sender, e));
        }
        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                _timer.Stop();
                MouseDoubleClickCommand?.Execute((sender, e));
            }
            else
            {
                MouseDownCommand?.Execute((sender, e));
            }
        }
        private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MouseWheelCommand?.Execute((sender, e));
        }
        private void _timer_Tick(object? sender, EventArgs e)
        {
            _timer.Stop();
        }
    }
}
