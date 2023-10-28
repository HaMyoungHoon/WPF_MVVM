using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_MVVM.Models.RestAPI.DataGov
{
    internal class DataGovBody<T>
    {
        public string? dataType { get; set; }
        public int? pageNo { get; set; }
        public int? numOfRows { get; set; }
        public int? totalCount { get; set; }
        public T? items { get; set; }
    }
}
