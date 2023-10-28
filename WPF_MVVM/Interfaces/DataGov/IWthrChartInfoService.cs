using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPF_MVVM.Models.RestAPI.DataGov;
using WPF_MVVM.Models.RestAPI.DataGov.WthrChartInfo;

namespace WPF_MVVM.Interfaces.DataGov
{
    internal interface IWthrChartInfoService
    {
        public DataGovResponse<GetSurfaceChartResModel> GetSurfaceChart(GetSurfaceChartReqModel data, CancellationTokenSource? cts = null);
        public Task<DataGovResponse<GetSurfaceChartResModel>> GetSurfaceChartAsync(GetSurfaceChartReqModel data, CancellationTokenSource? cts = null);
        public DataGovResponse<GetAuxillaryChartResModel> GetAuxillaryChart(GetAuxillaryChartReqModel data, CancellationTokenSource? cts = null);
        public Task<DataGovResponse<GetAuxillaryChartResModel>> GetAuxillaryChartAsync(GetAuxillaryChartReqModel data, CancellationTokenSource? cts = null);
    }
}
