using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using WPF_MVVM.Interfaces.RestAPI;

namespace WPF_MVVM.Models.RestAPI.Common
{
    internal class RestResult : IRestResult
    {
        public bool? Result { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public RestResult()
        {
        }

        public static RestResult Deserialize(string data)
        {
            RestResult buff;
            try
            {
                buff = JsonSerializer.Deserialize<RestResult>(data) ?? new();
            }
            catch (Exception ex)
            {
                buff = new()
                {
                    Message = ex.Message,
                    Result = false
                };
            }

            return buff;
        }
        public static RestResult Deserialize(Stream data)
        {
            RestResult buff;
            try
            {
                buff = JsonSerializer.Deserialize<RestResult>(data) ?? new();
            }
            catch (Exception ex)
            {
                buff = new()
                {
                    Message = ex.Message,
                    Result = false
                };
            }

            return buff;
        }
        public static async Task<RestResult> DeserializeAsync(Stream data)
        {
            RestResult buff;
            try
            {
                buff = await JsonSerializer.DeserializeAsync<RestResult>(data
                    //                    , new JsonSerializerOptions
                    //                    {
                    //                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    //                    }
                    ) ?? new();
            }
            catch (Exception ex)
            {
                buff = new();
                buff.Result = false;
                buff.Message = ex.Message;
                return buff;
            }

            return buff;
        }
        public T DeserializeData<T>(out string err) where T : new()
        {
            T? buff;
            try
            {
                buff = JsonSerializer.Deserialize<T>(Data?.ToString() ?? string.Empty) ?? new();
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
            if (Data == null)
            {
                return "null";
            }
            try
            {
                string? ret = JsonSerializer.Serialize(Data);
                if (ret == null)
                {
                    return $"Data.ToString(): {Data?.ToString()}";
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
