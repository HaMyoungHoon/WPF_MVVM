using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF_MVVM.Behaviors
{
    internal class KeyBehaviour : Behavior<Control>
    {
        public static readonly DependencyProperty KeyUpCommandProperty = DependencyProperty.RegisterAttached(nameof(KeyUpCommand), typeof(ICommand), typeof(KeyBehaviour));
        public static readonly DependencyProperty KeyDownCommandProperty = DependencyProperty.RegisterAttached(nameof(KeyDownCommand), typeof(ICommand), typeof(KeyBehaviour));

        public ICommand KeyUpCommand
        {
            get => (ICommand)(GetValue(KeyUpCommandProperty));
            set => SetValue(KeyUpCommandProperty, value);
        }
        public ICommand KeyDownCommand
        {
            get => (ICommand)(GetValue(KeyDownCommandProperty));
            set => SetValue(KeyDownCommandProperty, value);
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
            AssociatedObject.KeyUp += AssociatedObject_KeyUp;
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }
        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
            AssociatedObject.KeyUp -= AssociatedObject_KeyUp;
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
        }
        private void AssociatedObject_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUpCommand?.Execute((sender, e));
        }
        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDownCommand?.Execute((sender, e));
        }
    }
}
