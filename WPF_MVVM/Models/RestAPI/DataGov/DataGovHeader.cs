using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WPF_MVVM.Models.RestAPI.DataGov
{
    internal class DataGovHeader
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DataGovResultCode? resultCode { get; set; }
        public string? resultMsg { get; set; }
    }
}
