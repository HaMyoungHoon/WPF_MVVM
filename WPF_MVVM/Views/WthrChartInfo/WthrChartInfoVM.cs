using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WPF_MVVM.Bases;
using WPF_MVVM.Interfaces.DataGov;
using WPF_MVVM.Models.DataGovAPI;
using WPF_MVVM.Models.Messages;
using WPF_MVVM.Models.RestAPI.DataGov;
using WPF_MVVM.Models.RestAPI.DataGov.WthrChartInfo;

namespace WPF_MVVM.Views.WthrChartInfo
{
    internal partial class WthrChartInfoPageVM : PaneDocumentViewModel
    {
        [ObservableProperty]
        private List<CommonComboItem> _serviceItems;
        private int _selectedServiceIndex;
        [ObservableProperty]
        private List<CommonComboItem> _code1Items;
        private int _selectedCode1Index;
        [ObservableProperty]
        private List<CommonComboItem> _code2Items;
        [ObservableProperty]
        private int _selectedCode2Index;
        [ObservableProperty]
        private bool _code2Enable;
        [ObservableProperty]
        private List<CommonComboItem> _timeItems;
        [ObservableProperty]
        private int _selectedTimeIndex;
        [ObservableProperty]
        private List<string> _contentImgList;

        private readonly CancellationTokenSource _cts;
        private readonly IWthrChartInfoService _wthrChartInfoService;
        public WthrChartInfoPageVM() : base()
        {
            Title = "WthrChartInfo";
            ContentID = nameof(WthrChartInfoPageVM);

            _serviceItems = new();
            _selectedServiceIndex = -1;
            _code1Items = new();
            _selectedCode1Index = -1;
            _code2Items = new();
            _selectedCode2Index = -1;
            _timeItems = new();
            _selectedTimeIndex = -1;
            _contentImgList = new();

            _cts = new CancellationTokenSource();
            _wthrChartInfoService = App.Current.Services.GetService(typeof(IWthrChartInfoService)) as WthrChartInfoService ?? new();
            SearchCommand = new AsyncRelayCommand(SearchEvent);
            Init();
        }

        public int SelectedServiceIndex
        {
            get => _selectedServiceIndex;
            set
            {
                _selectedServiceIndex = value;
                ServiceChange();
            }
        }
        public int SelectedCode1Index
        {
            get => _selectedCode1Index;
            set
            {
                _selectedCode1Index = value;
                OnPropertyChanged();
                Code1Change();
            }
        }
        public IRelayCommand SearchCommand { get; }

        protected override void CloseEvent(object? data)
        {
            WeakReferenceMessenger.Default.Send(new NavigationMessage(nameof(WthrChartInfoPage)) { Sender = this, Close = true });
            base.CloseEvent(data);
        }
        private void Init()
        {
            ServiceItems.Clear();
            ServiceItems.Add(new CommonComboItem()
            {
                Index = 0,
                Name = "지상일기도조회",
                Description = "getSurfaceChart"
            });
            ServiceItems.Add(new CommonComboItem()
            {
                Index = 1,
                Name = "보조일기도조회",
                Description = "getAuxillaryChart"
            });
            SelectedServiceIndex = 0;

            TimeItems.Clear();
            TimeItems.Add(new CommonComboItem()
            {
                Index = 0,
                Name = "오늘",
                Description = DateTime.Now.ToString("yyyyMMdd")
            });
            TimeItems.Add(new CommonComboItem()
            {
                Index = 0,
                Name = "어제",
                Description = DateTime.Now.AddDays(-1).ToString("yyyyMMdd")
            });
            TimeItems.Add(new CommonComboItem()
            {
                Index = 0,
                Name = "그제",
                Description = DateTime.Now.AddDays(-2).ToString("yyyyMMdd")
            });
            SelectedTimeIndex = 0;
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
            base.OnNavigated(sender, e);
        }

        private async Task SearchEvent()
        {
            switch (SelectedServiceIndex)
            {
                case 0: await Service1Call(); break;
                case 1: await Service2Call(); break;
            }
        }
        private void ServiceChange()
        {
            SelectedCode1Index = -1;
            Code2Items.Clear();
            Code2Enable = false;
            switch (SelectedServiceIndex)
            {
                case 0:
                    {
                        var buff = new List<CommonComboItem>()
                        {
                            new CommonComboItem()
                            {
                                Index = 1,
                                Name = "지상 03",
                                Description = "03"
                            },
                            new CommonComboItem()
                            {
                                Index = 1,
                                Name = "지상 12",
                                Description = "12"
                            },
                            new CommonComboItem()
                            {
                                Index = 1,
                                Name = "지상 24",
                                Description = "24"
                            }
                        };
                        Code1Items = buff;
                        SelectedCode1Index = 0;
                    }
                    break;
                case 1:
                    {
                        var buff = new List<CommonComboItem>()
                        {
                            new CommonComboItem()
                            {
                                Index = 1,
                                Name = "북반구",
                                Description = "N500"
                            },
                            new CommonComboItem()
                            {
                                Index = 1,
                                Name = "보조 구분",
                                Description = "R30I"
                            }
                        };
                        Code1Items = buff;
                        SelectedCode1Index = 0;
                    }
                    break;
                default: Code1Items.Clear(); break;
            }
        }
        private void Code1Change()
        {
            SelectedCode2Index = -1;
            Code2Items.Clear();
            Code2Enable = false;
            if (SelectedServiceIndex != 1)
            {
                return;
            }
            switch (SelectedCode1Index)
            {
                case 0:
                    {
                        Code2Items.Add(new CommonComboItem()
                        {
                            Index = 0,
                            Name = "북반구 500",
                            Description = "ANL"
                        });
                        Code2Items.Add(new CommonComboItem()
                        {
                            Index = 0,
                            Name = "북반구 편차",
                            Description = "DIF"
                        });
                        SelectedCode2Index = 0;
                        Code2Enable = true;
                    }
                    break;
                case 1:
                    {
                        Code2Items.Add(new CommonComboItem()
                        {
                            Index = 0,
                            Name = "AXFE01",
                            Description = "AXFE01"
                        });
                        Code2Items.Add(new CommonComboItem()
                        {
                            Index = 0,
                            Name = "AXFE02",
                            Description = "AXFE02"
                        });
                        SelectedCode2Index = 0;
                        Code2Enable = true;
                    }
                    break;
            }
        }

        private async Task Service1Call()
        {
            var ret = await _wthrChartInfoService.GetSurfaceChartAsync(new()
            {
                serviceKey = "UHJOJbJDV20giqvWT8qvULmXjGxPkvLA8lTyIJBOug8cNUFF9Huu1iL1xS%2FyabZhebJZqbTZRLFx7JgvuLMYPQ%3D%3D",
                pageNo = 1,
                numOfRows = 10,
                dataType = "JSON",
                code = Code1Items[SelectedCode1Index].Description ?? "24",
                time = TimeItems[SelectedTimeIndex].Description ?? DateTime.Now.ToString("yyyyMMdd"),
            }, _cts);

            if (ret.response == null)
            {
                AlertMessageCall(ret.Serialize());
                return;
            }
            else if (ret.response.header == null)
            {
                AlertMessageCall(ret.Serialize());
                return;
            }
            else if (ret.response.header.resultCode != DataGovResultCode.OK)
            {
                AlertMessageCall(ret.response.header.resultMsg ?? ret.Serialize());
                return;
            }

            ContentImgList = ret?.response?.body?.items?.item?.FirstOrDefault()?.GetFileList() ?? new();
        }

        private async Task Service2Call()
        {
            var ret = await _wthrChartInfoService.GetAuxillaryChartAsync(new()
            {
                serviceKey = "UHJOJbJDV20giqvWT8qvULmXjGxPkvLA8lTyIJBOug8cNUFF9Huu1iL1xS%2FyabZhebJZqbTZRLFx7JgvuLMYPQ%3D%3D",
                pageNo = 1,
                numOfRows = 10,
                dataType = "JSON",
                code1 = Code1Items[SelectedCode1Index].Description ?? "N500",
                code2 = Code2Items[SelectedCode1Index].Description ?? "DIF",
                time = TimeItems[SelectedTimeIndex].Description ?? DateTime.Now.ToString("yyyyMMdd"),
            }, _cts);

            if (ret.response == null)
            {
                AlertMessageCall(ret.Serialize());
                return;
            }
            else if (ret.response.header == null)
            {
                AlertMessageCall(ret.Serialize());
                return;
            }
            else if (ret.response.header.resultCode != DataGovResultCode.OK)
            {
                AlertMessageCall(ret.response.header.resultMsg ?? ret.Serialize());
                return;
            }

            ContentImgList = ret?.response?.body?.items?.item?.FirstOrDefault()?.GetFileList() ?? new();
        }

        private void AlertMessageCall(string resultMsg)
        {
            WeakReferenceMessenger.Default.Send(new AlertMessage(true) { Sender = this, Header = "통신 오류", Message = resultMsg });
        }
    }
}
