using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading;
using WPF_MVVM.Bases;
using WPF_MVVM.Interfaces.DataGov;
using WPF_MVVM.Models.Messages;
using WPF_MVVM.Models.RestAPI.DataGov.WthrChartInfo;

namespace WPF_MVVM.Views.Home
{
    internal partial class HomePageVM : PaneDocumentViewModel
    {
        [ObservableProperty]
        private string _testString;

        private readonly CancellationTokenSource _cts;
        private readonly IWthrChartInfoService _wthrChartInfoService;
        public HomePageVM() : base()
        {
            Title = "HOME";
            ContentID = nameof(HomePageVM);

            _testString = string.Empty;
            _cts = new CancellationTokenSource();
            _wthrChartInfoService = App.Current.Services.GetService(typeof(IWthrChartInfoService)) as WthrChartInfoService ?? new();

            TestCommand = new RelayCommand(TestEvent);
        }

        public IRelayCommand TestCommand { get; }

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

        private void TestEvent()
        {
//            GetSurfaceChartReqModel data = new()
//            {
//                serviceKey = "UHJOJbJDV20giqvWT8qvULmXjGxPkvLA8lTyIJBOug8cNUFF9Huu1iL1xS%2FyabZhebJZqbTZRLFx7JgvuLMYPQ%3D%3D",
//                pageNo = 1,
//                numOfRows = 10,
//                dataType = "JSON",
//                code = "24",
//                time = "20231027"
//            };
//            var ret = _wthrChartInfoService.GetSurfaceChart(data, _cts);

            GetAuxillaryChartReqModel data2 = new()
            {
                serviceKey = "UHJOJbJDV20giqvWT8qvULmXjGxPkvLA8lTyIJBOug8cNUFF9Huu1iL1xS%2FyabZhebJZqbTZRLFx7JgvuLMYPQ%3D%3D",
                pageNo = 1,
                numOfRows = 10,
                dataType = "JSON",
                code1 = "N500",
                code2 = "DIF",
                time = "20231027"
            };
            var ret2 = _wthrChartInfoService.GetAuxillaryChart(data2, _cts);
            TestString = ret2.SerializeData();
        }
    }
}
