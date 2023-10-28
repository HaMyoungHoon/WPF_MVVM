using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPF_MVVM.Models.RestAPI.Common;
using WPF_MVVM.Models.RestAPI.DataGov;

namespace WPF_MVVM.Interfaces.RestAPI
{
    internal interface IRestAPIService
    {
        string Url { get; }
        MethodType MethodType { get; }
        int Timeout { get; }
        public Task<IRestResult<T>> SendAsync<T>(CancellationTokenSource? cts = null) where T : new();
        public Task<IRestResult> SendAsync(CancellationTokenSource? cts = null);
        public IRestResult<T> Send<T>(CancellationTokenSource? cts = null) where T : new();
        public IRestResult Send(CancellationTokenSource? cts = null);

        public Task<DataGovResponse<T>> SendDataGovAsync<T>(CancellationTokenSource? cts = null) where T : new();
        public DataGovResponse<T> SendDataGov<T>(CancellationTokenSource? cts = null) where T : new();
    }
}
