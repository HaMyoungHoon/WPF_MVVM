using CommunityToolkit.Mvvm.Messaging;
using System.Threading;
using WPF_MVVM.Bases;
using WPF_MVVM.Models.Messages;

namespace WPF_MVVM.Views.Home
{
    internal partial class HomePageVM : PaneDocumentViewModel
    {
        private readonly CancellationTokenSource _cts;

        public HomePageVM() : base()
        {
            Title = "HOME";
            ContentID = nameof(HomePageVM);

            _cts = new CancellationTokenSource();
        }

        protected override void CloseEvent(object? data)
        {
            WeakReferenceMessenger.Default.Send(new NavigationMessage(nameof(HomePage)) { Sender = this, Close = true });
            base.CloseEvent(data);
        }
        private void Cancel()
        {
            try
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            catch
            {

            }
        }
        public override void OnNavigating(object? sender, object? e)
        {
            Cancel();
        }
        public override void OnNavigated(object? sender, object? e)
        {

        }
    }
}
