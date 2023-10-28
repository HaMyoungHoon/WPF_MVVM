using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPF_MVVM.Interfaces.RestAPI;
using WPF_MVVM.Models.RestAPI.DataGov;

namespace WPF_MVVM.Models.RestAPI.Common
{
    internal class RestApiService : RestParameter, IRestAPIService
    {
        private string _url;
        private MethodType _methodType;
        private int _timeout;

        public string Url => _url;
        public MethodType MethodType => _methodType;
        public int Timeout => _timeout;

        public RestApiService()
        {
            _url = string.Empty;
            _methodType = MethodType.NULL;
            _timeout = 10 * 1000;
        }
        public RestApiService(string url, MethodType method, int timeout)
        {
            _url = url;
            _methodType = method;
            _timeout = timeout;
        }
        public void SetUrl(string url)
        {
            _url = url;
        }
        public void SetMethod(MethodType method)
        {
            _methodType = method;
        }
        public void SetTimeout(int timeout)
        {
            _timeout = timeout;
        }
        public async Task<IRestResult<T>> SendAsync<T>(CancellationTokenSource? cts = null) where T : new()
        {
            RestResult<T> ret = new();
            if (_url.Length <= 0)
            {
                ret.Message = "Url is Empty";
                ret.Result = false;
                return ret;
            }
            else if (!Enum.IsDefined(_methodType) || _methodType == MethodType.NULL)
            {
                ret.Message = "Request Method is NULL";
                ret.Result = false;
                return ret;
            }

            cts ??= new();

            try
            {
                HttpResponseMessage? res = await RequestDataAsync(cts);
                if (res == null)
                {
                    return ret;
                }

                ret = await RestResult<T>.DeserializeAsync(res.Content.ReadAsStream(cts.Token));
                if (ret.Data == null)
                {
                    ret.Result = false;
                    ret.Message = $"{(int)(res.StatusCode)} {res.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ret.Result = false;
                ret.Message = ex.Message;
                return ret;
            }

            return ret;
        }
        public async Task<IRestResult> SendAsync(CancellationTokenSource? cts = null)
        {
            RestResult ret = new();
            if (_url.Length <= 0)
            {
                ret.Message = "Url is Empty";
                ret.Result = false;
                return ret;
            }
            else if (!Enum.IsDefined(_methodType) || _methodType == MethodType.NULL)
            {
                ret.Message = "Request Method is NULL";
                ret.Result = false;
                return ret;
            }

            cts ??= new();

            try
            {
                HttpResponseMessage? res = await RequestDataAsync(cts);
                if (res == null)
                {
                    return ret;
                }

                ret = await RestResult.DeserializeAsync(res.Content.ReadAsStream(cts.Token));
                if (ret.Data == null)
                {
                    ret.Result = false;
                    ret.Message = $"{(int)(res.StatusCode)} {res.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ret.Result = false;
                ret.Message = ex.Message;
                return ret;
            }

            return ret;
        }
        public IRestResult<T> Send<T>(CancellationTokenSource? cts = null) where T : new()
        {
            RestResult<T> ret = new();
            if (_url.Length <= 0)
            {
                ret.Message = "Url is Empty";
                ret.Result = false;
                return ret;
            }
            else if (!Enum.IsDefined(_methodType) || _methodType == MethodType.NULL)
            {
                ret.Message = "Request Method is NULL";
                ret.Result = false;
                return ret;
            }

            cts ??= new();

            try
            {
                HttpResponseMessage? res = RequestData(cts);
                if (res == null)
                {
                    ret.Result = false;
                    return ret;
                }

                ret = RestResult<T>.Deserialize(Task.Run(async () => await res.Content.ReadAsStreamAsync(cts.Token)).Result);
                if (ret.Data == null)
                {
                    ret.Result = false;
                    ret.Message = $"{(int)(res.StatusCode)} {res.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ret.Result = false;
                ret.Message = ex.Message;
                return ret;
            }

            return ret;
        }
        public IRestResult Send(CancellationTokenSource? cts = null)
        {
            RestResult ret = new();
            if (_url.Length <= 0)
            {
                ret.Message = "Url is Empty";
                ret.Result = false;
                return ret;
            }
            else if (!Enum.IsDefined(_methodType) || _methodType == MethodType.NULL)
            {
                ret.Message = "Request Method is NULL";
                ret.Result = false;
                return ret;
            }

            cts ??= new();

            try
            {
                HttpResponseMessage? res = RequestData(cts);
                if (res == null)
                {
                    ret.Result = false;
                    return ret;
                }

                ret = RestResult.Deserialize(Task.Run(async () => await res.Content.ReadAsStreamAsync(cts.Token)).Result);
                if (ret.Data == null)
                {
                    ret.Result = false;
                    ret.Message = $"{(int)(res.StatusCode)} {res.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ret.Result = false;
                ret.Message = ex.Message;
                return ret;
            }

            return ret;
        }

        public async Task<DataGovResponse<T>> SendDataGovAsync<T>(CancellationTokenSource? cts = null) where T : new()
        {
            DataGovResponse<T> ret = new();
            if (_url.Length <= 0)
            {
                ret.response = new()
                {
                    header = new()
                    {
                        resultCode = DataGovResultCode.UNKNOWN_ERROR,
                        resultMsg = "Url is Empty"
                    },
                };
                return ret;
            }
            else if (!Enum.IsDefined(_methodType) || _methodType == MethodType.NULL)
            {
                ret.response = new()
                {
                    header = new()
                    {
                        resultCode = DataGovResultCode.UNKNOWN_ERROR,
                        resultMsg = "Request Method is NULL"
                    }
                };
                return ret;
            }

            cts ??= new();

            try
            {
                HttpResponseMessage? res = await RequestDataAsync(cts);
                if (res == null)
                {
                    ret.response = new()
                    {
                        header = new()
                        {
                            resultCode = DataGovResultCode.UNKNOWN_ERROR,
                            resultMsg = "response null"
                        }
                    };
                    return ret;
                }

                ret = await DataGovResponse<T>.DeserializeAsync(res.Content.ReadAsStream(cts.Token));
                ret.response ??= new()
                    {
                        header = new()
                        {
                            resultCode = DataGovResultCode.UNKNOWN_ERROR,
                            resultMsg = $"{(int)(res.StatusCode)} {res.StatusCode}"
                        }
                    };
            }
            catch (Exception ex)
            {
                ret.response = new()
                {
                    header = new()
                    {
                        resultCode = DataGovResultCode.UNKNOWN_ERROR,
                        resultMsg = ex.Message
                    }
                };
                return ret;
            }

            return ret;
        }
        public DataGovResponse<T> SendDataGov<T>(CancellationTokenSource? cts = null) where T : new()
        {
            DataGovResponse<T> ret = new();
            if (_url.Length <= 0)
            {
                ret.response = new()
                {
                    header = new()
                    {
                        resultCode = DataGovResultCode.UNKNOWN_ERROR,
                        resultMsg = "Url is Empty"
                    },
                };
                return ret;
            }
            else if (!Enum.IsDefined(_methodType) || _methodType == MethodType.NULL)
            {
                ret.response = new()
                {
                    header = new()
                    {
                        resultCode = DataGovResultCode.UNKNOWN_ERROR,
                        resultMsg = "Request Method is NULL"
                    }
                };
                return ret;
            }

            cts ??= new();

            try
            {
                HttpResponseMessage? res = RequestData(cts);
                if (res == null)
                {
                    ret.response = new()
                    {
                        header = new()
                        {
                            resultCode = DataGovResultCode.UNKNOWN_ERROR,
                            resultMsg = "response null"
                        }
                    };
                    return ret;
                }

                ret = DataGovResponse<T>.Deserialize(Task.Run(async () => await res.Content.ReadAsStreamAsync(cts.Token)).Result);
                ret.response ??= new()
                {
                    header = new()
                    {
                        resultCode = DataGovResultCode.UNKNOWN_ERROR,
                        resultMsg = $"{(int)(res.StatusCode)} {res.StatusCode}"
                    }
                };
            }
            catch (Exception ex)
            {
                ret.response = new()
                {
                    header = new()
                    {
                        resultCode = DataGovResultCode.UNKNOWN_ERROR,
                        resultMsg = ex.Message
                    }
                };
                return ret;
            }

            return ret;
        }

        private HttpResponseMessage? RequestData(CancellationTokenSource cts)
        {
            StringBuilder stb = new();
            if (GetPathsCount() > 0)
            {
                stb.Append($"{_url}{GetPaths()}");
            }
            else
            {
                stb.Append($"{_url}");
            }
            if (GetParamsCount() > 0)
            {
                stb.Append($"?{GetParams()}");
            }

            using var req = new HttpClient(new HttpClientHandler());
            foreach (var header in GetHeaders2())
            {
                req.DefaultRequestHeaders.Add(header.Key, header.Value?.ToString());
            }
            req.DefaultRequestHeaders.Accept.Add(new("application/json"));
            req.Timeout = TimeSpan.FromMilliseconds(_timeout);

            StringContent? strContent = null;
            if (GetFormsCount() > 0)
            {
                strContent = new(GetForms(), Encoding.UTF8, "application/json");
            }
            MultipartFormDataContent? formContent = null;
            if (GetMultipartFormCount() > 0)
            {
                formContent = GetMultipartForms();
                req.Timeout = TimeSpan.FromMilliseconds(_timeout + formContent?.Headers.ContentLength ?? 0);
            }

            return _methodType switch
            {
                MethodType.NULL => null,
                MethodType.GET => req.GetAsync(stb.ToString(), cts.Token).Result,
                MethodType.POST => formContent == null ? req.PostAsync(stb.ToString(), strContent, cts.Token).Result : req.PostAsync(stb.ToString(), formContent, cts.Token).Result,
                MethodType.PUT => formContent == null ? req.PutAsync(stb.ToString(), strContent, cts.Token).Result : req.PutAsync(stb.ToString(), formContent, cts.Token).Result,
                MethodType.DELETE => req.DeleteAsync(stb.ToString(), cts.Token).Result,
                _ => null,
            };
        }
        private async Task<HttpResponseMessage?> RequestDataAsync(CancellationTokenSource cts)
        {
            StringBuilder stb = new();
            if (GetPathsCount() > 0)
            {
                stb.Append($"{_url}{GetPaths()}");
            }
            else
            {
                stb.Append($"{_url}");
            }
            if (GetParamsCount() > 0)
            {
                stb.Append($"?{GetParams()}");
            }

            using var req = new HttpClient(new HttpClientHandler());
            foreach (var header in GetHeaders2())
            {
                req.DefaultRequestHeaders.Add(header.Key, header.Value?.ToString());
            }
            req.DefaultRequestHeaders.Accept.Add(new("application/json"));
            req.Timeout = TimeSpan.FromMilliseconds(_timeout);

            StringContent? strContent = null;
            if (GetFormsCount() > 0)
            {
                strContent = new(GetForms(), Encoding.UTF8, "application/json");
            }
            MultipartFormDataContent? formContent = null;
            if (GetMultipartFormCount() > 0)
            {
                formContent = GetMultipartForms();
                req.Timeout = TimeSpan.FromMilliseconds(_timeout + formContent?.Headers.ContentLength ?? 0);
            }

            return _methodType switch
            {
                MethodType.NULL => null,
                MethodType.GET => await req.GetAsync(stb.ToString(), cts.Token),
                MethodType.POST => await (formContent == null ? req.PostAsync(stb.ToString(), strContent, cts.Token) : req.PostAsync(stb.ToString(), formContent, cts.Token)),
                MethodType.PUT => await (formContent == null ? req.PutAsync(stb.ToString(), strContent, cts.Token) : req.PutAsync(stb.ToString(), formContent, cts.Token)),
                MethodType.DELETE => await req.DeleteAsync(stb.ToString(), cts.Token),
                _ => null,
            };
        }


        public async Task<RestResult<T>> SendWithBodyAsync<T>(CancellationTokenSource? cts = null) where T : new()
        {
            RestResult<T> ret = new();
            if (_url.Length <= 0)
            {
                ret.Message = "Url is Empty";
                ret.Result = false;
                return ret;
            }
            else if (_methodType != MethodType.GET && _methodType != MethodType.DELETE)
            {
                ret.Message = "Request Method is not Get or DELETE";
                ret.Result = false;
                return ret;
            }

            cts ??= new();

            try
            {
                HttpResponseMessage? res = await RequestDataWithBodyAsync(cts);

                if (res == null)
                {
                    return ret;
                }

                ret = await RestResult<T>.DeserializeAsync(res.Content.ReadAsStream(cts.Token));
            }
            catch (Exception ex)
            {
                ret.Result = false;
                ret.Message = ex.Message;
                return ret;
            }

            return ret;
        }
        public async Task<RestResult> SendWithBodyAsync(CancellationTokenSource? cts = null)
        {
            RestResult ret = new();
            if (_url.Length <= 0)
            {
                ret.Message = "Url is Empty";
                ret.Result = false;
                return ret;
            }
            else if (_methodType != MethodType.GET && _methodType != MethodType.DELETE)
            {
                ret.Message = "Request Method is not Get or DELETE";
                ret.Result = false;
                return ret;
            }

            cts ??= new();

            try
            {
                HttpResponseMessage? res = await RequestDataWithBodyAsync(cts);

                if (res == null)
                {
                    return ret;
                }

                ret = await RestResult.DeserializeAsync(res.Content.ReadAsStream(cts.Token));
            }
            catch (Exception ex)
            {
                ret.Result = false;
                ret.Message = ex.Message;
                return ret;
            }

            return ret;
        }
        public RestResult<T> SendWithBody<T>(CancellationTokenSource? cts = null) where T : new()
        {
            RestResult<T> ret = new();
            if (_url.Length <= 0)
            {
                ret.Message = "Url is Empty";
                ret.Result = false;
                return ret;
            }
            else if (_methodType != MethodType.GET && _methodType != MethodType.DELETE)
            {
                ret.Message = "Request Method is not Get or DELETE";
                ret.Result = false;
                return ret;
            }

            cts ??= new();

            try
            {
                HttpResponseMessage? res = RequestDataWithBody(cts);
                if (res == null)
                {
                    ret.Result = false;
                    return ret;
                }

                ret = RestResult<T>.Deserialize(Task.Run(async () => await res.Content.ReadAsStreamAsync(cts.Token)).Result);
            }
            catch (Exception ex)
            {
                ret.Result = false;
                ret.Message = ex.Message;
                return ret;
            }

            return ret;
        }
        public RestResult SendWithBody(CancellationTokenSource? cts = null)
        {
            RestResult ret = new();
            if (_url.Length <= 0)
            {
                ret.Message = "Url is Empty";
                ret.Result = false;
                return ret;
            }
            else if (_methodType != MethodType.GET && _methodType != MethodType.DELETE)
            {
                ret.Message = "Request Method is not Get or DELETE";
                ret.Result = false;
                return ret;
            }

            cts ??= new();

            try
            {
                HttpResponseMessage? res = RequestDataWithBody(cts);
                if (res == null)
                {
                    ret.Result = false;
                    return ret;
                }

                ret = RestResult.Deserialize(Task.Run(async () => await res.Content.ReadAsStreamAsync(cts.Token)).Result);
            }
            catch (Exception ex)
            {
                ret.Result = false;
                ret.Message = ex.Message;
                return ret;
            }

            return ret;
        }

        private HttpResponseMessage? RequestDataWithBody(CancellationTokenSource cts)
        {
            StringBuilder stb = new();
            if (GetPathsCount() > 0)
            {
                stb.Append($"{_url}{GetPaths()}");
            }
            else
            {
                stb.Append($"{_url}");
            }
            if (GetParamsCount() > 0)
            {
                stb.Append($"?{GetParams()}");
            }

            using var req = new HttpClient(new HttpClientHandler());
            foreach (var header in GetHeaders2())
            {
                req.DefaultRequestHeaders.Add(header.Key, header.Value?.ToString());
            }
            req.DefaultRequestHeaders.Accept.Add(new("application/json"));
            req.Timeout = TimeSpan.FromMilliseconds(_timeout);

            StringContent? strContent = null;
            if (GetFormsCount() > 0)
            {
                strContent = new(GetForms(), Encoding.UTF8, "application/json");
            }
            MultipartFormDataContent? formContent = null;
            if (GetMultipartFormCount() > 0)
            {
                formContent = GetMultipartForms();
                req.Timeout = TimeSpan.FromMilliseconds(_timeout + formContent?.Headers.ContentLength ?? 0);
            }

            return _methodType switch
            {
                MethodType.GET => (formContent == null ? req.SendAsync(new HttpRequestMessage(HttpMethod.Get, stb.ToString()) { Content = strContent }, cts.Token).Result : req.SendAsync(new HttpRequestMessage(HttpMethod.Get, stb.ToString()) { Content = formContent }, cts.Token).Result),
                MethodType.DELETE => (formContent == null ? req.SendAsync(new HttpRequestMessage(HttpMethod.Delete, stb.ToString()) { Content = strContent }, cts.Token).Result : req.SendAsync(new HttpRequestMessage(HttpMethod.Delete, stb.ToString()) { Content = formContent }, cts.Token).Result),
                MethodType.POST => null,
                MethodType.PUT => null,
                MethodType.NULL => null,
                _ => null,
            };
        }
        private async Task<HttpResponseMessage?> RequestDataWithBodyAsync(CancellationTokenSource cts)
        {
            StringBuilder stb = new();
            if (GetPathsCount() > 0)
            {
                stb.Append($"{_url}{GetPaths()}");
            }
            else
            {
                stb.Append($"{_url}");
            }
            if (GetParamsCount() > 0)
            {
                stb.Append($"?{GetParams()}");
            }

            using var req = new HttpClient(new HttpClientHandler());
            foreach (var header in GetHeaders2())
            {
                req.DefaultRequestHeaders.Add(header.Key, header.Value?.ToString());
            }
            req.DefaultRequestHeaders.Accept.Add(new("application/json"));
            req.Timeout = TimeSpan.FromMilliseconds(_timeout);

            StringContent? strContent = null;
            if (GetFormsCount() > 0)
            {
                strContent = new(GetForms(), Encoding.UTF8, "application/json");
            }
            MultipartFormDataContent? formContent = null;
            if (GetMultipartFormCount() > 0)
            {
                formContent = GetMultipartForms();
                req.Timeout = TimeSpan.FromMilliseconds(_timeout + formContent?.Headers.ContentLength ?? 0);
            }

            return _methodType switch
            {
                MethodType.GET => await (formContent == null ? req.SendAsync(new HttpRequestMessage(HttpMethod.Get, stb.ToString()) { Content = strContent }, cts.Token) : req.SendAsync(new HttpRequestMessage(HttpMethod.Get, stb.ToString()) { Content = strContent }, cts.Token)),
                MethodType.DELETE => await (formContent == null ? req.SendAsync(new HttpRequestMessage(HttpMethod.Delete, stb.ToString()) { Content = strContent }, cts.Token) : req.SendAsync(new HttpRequestMessage(HttpMethod.Delete, stb.ToString()) { Content = strContent }, cts.Token)),
                MethodType.POST => await (formContent == null ? req.PostAsync(stb.ToString(), strContent, cts.Token) : req.PostAsync(stb.ToString(), formContent, cts.Token)),
                MethodType.PUT => await (formContent == null ? req.PutAsync(stb.ToString(), strContent, cts.Token) : req.PutAsync(stb.ToString(), formContent, cts.Token)),
                MethodType.NULL => null,
                _ => null,
            };
        }

    }
}
