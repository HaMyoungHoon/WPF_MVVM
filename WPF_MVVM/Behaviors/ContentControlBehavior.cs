using Microsoft.Xaml.Behaviors;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using WPF_MVVM.Interfaces;

namespace WPF_MVVM.Behaviors
{
    internal class ContentControlBehavior : Behavior<ContentControl>
    {
        public static readonly DependencyProperty ControlNameProperty = DependencyProperty.Register(nameof(ControlName), typeof(string), typeof(ContentControlBehavior), new PropertyMetadata(null, ControlNameChanged));
        public static readonly DependencyProperty ParameterProperty = DependencyProperty.Register(nameof(Parameter), typeof(object), typeof(ContentControlBehavior), new PropertyMetadata(null, ParameterChanged));
        public static readonly DependencyProperty SenderProperty = DependencyProperty.Register(nameof(Sender), typeof(object), typeof(ContentControlBehavior), new PropertyMetadata(null, SenderChanged));
        public static readonly DependencyProperty ShowLayerPopupProperty = DependencyProperty.Register(nameof(ShowLayerPopup), typeof(bool), typeof(ContentControlBehavior), new PropertyMetadata(false, ShowLayerPopupChanged));
        public string ControlName
        {
            get => (string)GetValue(ControlNameProperty);
            set => SetValue(ControlNameProperty, value);
        }
        public object? Parameter
        {
            get => (object?)GetValue(ParameterProperty);
            set => SetValue(ParameterProperty, value);
        }
        public object? Sender
        {
            get => (object?)GetValue(SenderProperty);
            set => SetValue(SenderProperty, value);
        }
        public bool ShowLayerPopup
        {
            get => (bool)GetValue(ShowLayerPopupProperty);
            set => SetValue(ShowLayerPopupProperty, value);
        }

        private static void ControlNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ContentControlBehavior)d).ResolveControl();
        }
        private static void ParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ContentControlBehavior)d).ResolveControl();
        }
        private static void SenderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ContentControlBehavior)d).ResolveControl();
        }
        private static void ShowLayerPopupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ContentControlBehavior)d).CheckShowLayerPopup();
        }

        private void ResolveControl()
        {
            if (string.IsNullOrEmpty(ControlName))
            {
                AssociatedObject.Content = null;
            }
            else
            {
                string name = Assembly.GetExecutingAssembly().GetName().Name ?? string.Empty;
                var type = Type.GetType($"{name}.Controls.{ControlName}, {name}, Version={Assembly.GetExecutingAssembly().GetName().Version?.ToString()}, Culture=neutral, PublicKeyToken=null");
                if (type == null)
                {
                    return;
                }
                var control = App.Current.Services.GetService(type);
                AssociatedObject.Content = control;
                if (control is UserControl uc && uc.DataContext is INavigationAware aware)
                {
                    aware.OnNavigated(null, Parameter);
                }
            }
        }

        private void CheckShowLayerPopup()
        {
            if (ShowLayerPopup == false)
            {
                AssociatedObject.Content = null;
            }
            else
            {
                ResolveControl();
            }
        }
    }
}
