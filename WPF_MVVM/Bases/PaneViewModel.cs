using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media;

namespace WPF_MVVM.Bases
{
    internal partial class PaneViewModel : ViewModelBase
    {
        [ObservableProperty]
        protected object? _mother;
        [ObservableProperty]
        private string? _title;
        [ObservableProperty]
        private ImageSource? _iconSource;
        [ObservableProperty]
        private string? _contentID;
        [ObservableProperty]
        private bool? _isSelected;
        [ObservableProperty]
        private bool? _canClose;
        [ObservableProperty]
        private bool? _canFloat;

        public PaneViewModel()
        {
            _title = string.Empty;
            _canClose = true;
            _canFloat = true;
            CloseCommand = new RelayCommand<object?>(CloseEvent);
            FloatCommand = new RelayCommand<object?>(FloatEvent);
            DockAsDocumentCommand = new RelayCommand<object?>(DockAsDocumentEvent);
            CloseAllButThisCommand = new RelayCommand<object?>(CloseAllButThisEvent);
            CloseAllCommand = new RelayCommand<object?>(CloseAllEvent);
            ActivateCommand = new RelayCommand<object?>(ActivateEvent);
            NewVerticalTabGroupCommand = new RelayCommand<object?>(NewVerticalTabGroupEvent);
            NewHorizontalTabGroupCommand = new RelayCommand<object?>(NewHorizontalTabGroupEvent);
            MoveToNextTabGroupCommand = new RelayCommand<object?>(MoveToNextTabGroupEvent);
            MoveToPreviousTabGroupCommand = new RelayCommand<object?>(MoveToPreviousTabGroupEvent);
        }

        public IRelayCommand CloseCommand { get; set; }
        public IRelayCommand FloatCommand { get; set; }
        public IRelayCommand DockAsDocumentCommand { get; set; }
        public IRelayCommand CloseAllButThisCommand { get; set; }
        public IRelayCommand CloseAllCommand { get; set; }
        public IRelayCommand ActivateCommand { get; set; }
        public IRelayCommand NewVerticalTabGroupCommand { get; set; }
        public IRelayCommand NewHorizontalTabGroupCommand { get; set; }
        public IRelayCommand MoveToNextTabGroupCommand { get; set; }
        public IRelayCommand MoveToPreviousTabGroupCommand { get; set; }
        protected virtual void CloseEvent(object? data)
        {

        }
        protected virtual void FloatEvent(object? data)
        {

        }
        protected virtual void DockAsDocumentEvent(object? data)
        {
        }
        protected virtual void CloseAllButThisEvent(object? data)
        {
        }
        protected virtual void CloseAllEvent(object? data)
        {
        }
        protected virtual void ActivateEvent(object? data)
        {
        }
        protected virtual void NewVerticalTabGroupEvent(object? data)
        {
        }
        protected virtual void NewHorizontalTabGroupEvent(object? data)
        {
        }
        protected virtual void MoveToNextTabGroupEvent(object? data)
        {
        }
        protected virtual void MoveToPreviousTabGroupEvent(object? data)
        {
        }
    }
}
