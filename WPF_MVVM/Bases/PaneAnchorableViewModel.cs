using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WPF_MVVM.Bases
{
    internal partial class PaneAnchorableViewModel : PaneViewModel
    {
        [ObservableProperty]
        private bool? _canHide;
        [ObservableProperty]
        private bool? _canMove;
        private bool _isVisible;
        public PaneAnchorableViewModel()
        {
            _isVisible = true;

            HideCommand = new RelayCommand<object?>(HideEvent);
            AutoHideCommand = new RelayCommand<object?>(AutoHideEvent);
            DockCommand = new RelayCommand<object?>(DockEvent);
        }

        public IRelayCommand HideCommand { get; }
        public IRelayCommand AutoHideCommand { get; }
        public IRelayCommand DockCommand { get; }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                {
                    return;
                }

                _isVisible = value;
                OnPropertyChanged();
                VisibleChangeEvent();
            }
        }

        protected virtual void VisibleChangeEvent()
        {

        }
        protected virtual void HideEvent(object? data)
        {
        }
        protected virtual void AutoHideEvent(object? data)
        {
        }
        protected virtual void DockEvent(object? data)
        {
        }
    }
}
