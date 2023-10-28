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
    internal class RestResult<T> : IRestResult<T> where T : new()
    {
        public bool? Result { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public RestResult()
        {
        }
        public static RestResult<T> Deserialize(Stream data)
        {
            RestResult<T> buff;
            try
            {
                buff = JsonSerializer.Deserialize<RestResult<T>>(data
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
                    Message = ex.Message,
                    Result = false
                };
            }

            return buff;
        }
        public static async Task<RestResult<T>> DeserializeAsync(Stream data)
        {
            RestResult<T> buff;
            try
            {
                //                var asdf = await JsonSerializer.DeserializeAsync<RestResult>(data);
                buff = await JsonSerializer.DeserializeAsync<RestResult<T>>(data
                    //                    , new JsonSerializerOptions
                    //                    {
                    //                        NumberHandling = JsonNumberHandling.AllowReadingFromString,
                    //                    }
                    ) ?? new();
            }
            catch (Exception ex)
            {
                buff = new()
                {
                    Result = false,
                    Message = ex.Message
                };
                return buff;
            }

            return buff;
        }
        public T2 DeserializeData<T2>(out string err) where T2 : new()
        {
            T2? buff;
            try
            {
                buff = JsonSerializer.Deserialize<T2>(Data?.ToString() ?? string.Empty) ?? new();
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
