using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPF_MVVM.Interfaces.DataGov;
using WPF_MVVM.Models.RestAPI.Common;

namespace WPF_MVVM.Models.RestAPI.DataGov.WthrChartInfo
{
    internal class WthrChartInfoService : IWthrChartInfoService
    {
        private readonly string _url;
        private int _timeout = 10 * 1000;
        public WthrChartInfoService(string url = "http://apis.data.go.kr/1360000/WthrChartInfoService")
        {
            _url = url;
        }

        public DataGovResponse<GetSurfaceChartResModel> GetSurfaceChart(GetSurfaceChartReqModel data, CancellationTokenSource? cts = null)
        {
            string url = $"{_url}/getSurfaceChart";
            RestApiService service = new(url, Interfaces.RestAPI.MethodType.GET, _timeout);
            service.AddParameter(data);
            return service.SendDataGov<GetSurfaceChartResModel>(cts);
        }
        public async Task<DataGovResponse<GetSurfaceChartResModel>> GetSurfaceChartAsync(GetSurfaceChartReqModel data, CancellationTokenSource? cts = null)
        {
            string url = $"{_url}/getSurfaceChart";
            RestApiService service = new(url, Interfaces.RestAPI.MethodType.GET, _timeout);
            service.AddParameter(data);
            return await service.SendDataGovAsync<GetSurfaceChartResModel>(cts);
        }

        public DataGovResponse<GetAuxillaryChartResModel> GetAuxillaryChart(GetAuxillaryChartReqModel data, CancellationTokenSource? cts = null)
        {
            string url = $"{_url}/getAuxillaryChart";
            RestApiService service = new(url, Interfaces.RestAPI.MethodType.GET, _timeout);
            service.AddParameter(data);
            return service.SendDataGov<GetAuxillaryChartResModel>(cts);
        }
        public async Task<DataGovResponse<GetAuxillaryChartResModel>> GetAuxillaryChartAsync(GetAuxillaryChartReqModel data, CancellationTokenSource? cts = null)
        {
            string url = $"{_url}/getAuxillaryChart";
            RestApiService service = new(url, Interfaces.RestAPI.MethodType.GET, _timeout);
            service.AddParameter(data);
            return await service.SendDataGovAsync<GetAuxillaryChartResModel>(cts);
        }
    }
}
