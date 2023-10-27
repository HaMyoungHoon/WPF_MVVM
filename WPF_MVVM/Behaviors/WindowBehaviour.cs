using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace WPF_MVVM.Behaviors
{
    internal class WindowBehaviour : Behavior<Window>
    {
        public static readonly DependencyProperty WindowLoadedProperty = DependencyProperty.RegisterAttached(nameof(WindowLoaded), typeof(ICommand), typeof(WindowBehaviour));
        public static readonly DependencyProperty WindowUnLoadedProperty = DependencyProperty.RegisterAttached(nameof(WindowUnloaded), typeof(ICommand), typeof(WindowBehaviour));
        public WindowBehaviour()
        {

        }

        public ICommand WindowLoaded
        {
            get => (ICommand)GetValue(WindowLoadedProperty);
            set => SetValue(WindowLoadedProperty, value);
        }
        public ICommand WindowUnloaded
        {
            get => (ICommand)GetValue(WindowUnLoadedProperty);
            set => SetValue(WindowUnLoadedProperty, value);
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
            WindowLoaded?.Execute((sender, e));
        }
        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
            WindowUnloaded?.Execute((sender, e));
        }
    }
}
