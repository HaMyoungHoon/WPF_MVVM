using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using WPF_MVVM.Models.RestAPI.Common;

namespace WPF_MVVM.Models.RestAPI.DataGov
{
    internal class DataGovResponse<T> where T : new()
    {
        public InnerResponse? response { get; set; }
        internal class InnerResponse
        {
            public DataGovHeader? header { get; set; }
            public DataGovBody<T>? body { get; set; }
        }

        public static DataGovResponse<T> Deserialize(string data)
        {
            DataGovResponse<T> buff;
            try
            {
                buff = JsonSerializer.Deserialize<DataGovResponse<T>>(data) ?? new();
            }
            catch (Exception ex)
            {
                buff = new()
                {
                    response = new()
                    {
                        header = new()
                        {
                            resultCode = DataGovResultCode.UNKNOWN_ERROR,
                            resultMsg = ex.Message
                        },
                    }
                };
            }

            return buff;
        }
        public static DataGovResponse<T> Deserialize(Stream data)
        {
            DataGovResponse<T> buff;
            try
            {
                buff = JsonSerializer.Deserialize<DataGovResponse<T>>(data) ?? new();
            }
            catch (Exception ex)
            {
                buff = new()
                {
                    response = new()
                    {
                        header = new()
                        {
                            resultCode = DataGovResultCode.UNKNOWN_ERROR,
                            resultMsg = ex.Message
                        },
                    }
                };
            }

            return buff;
        }
        public static async Task<DataGovResponse<T>> DeserializeAsync(Stream data)
        {
            DataGovResponse<T> buff;
            try
            {
                buff = await JsonSerializer.DeserializeAsync<DataGovResponse<T>>(data
                    //                    , new JsonSerializerOptions
                    //                    {
                    //                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    //                    }
                    ) ?? new();
            }
            catch (Exception ex)
            {
                buff = new()
                {
                    response = new()
                    {
                        header = new()
                        {
                            resultCode = DataGovResultCode.UNKNOWN_ERROR,
                            resultMsg = ex.Message
                        },
                    }
                };
                return buff;
            }

            return buff;
        }
        public T DeserializeData<T>(out string err) where T : new()
        {
            T? buff;
            try
            {
                buff = JsonSerializer.Deserialize<T>(response?.ToString() ?? string.Empty) ?? new();
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return new();
            }

            err = string.Empty;
            return buff;
        }
        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
        public string SerializeData()
        {
            if (response == null)
            {
                return "null";
            }
            try
            {
                string? ret = JsonSerializer.Serialize(response);
                if (ret == null)
                {
                    return $"Data.ToString(): {response?.ToString()}";
                }

                return JsonNode.Parse(ret)?.ToString() ?? "null";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
