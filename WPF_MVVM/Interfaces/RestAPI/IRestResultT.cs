using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_MVVM.Interfaces.RestAPI
{
    internal interface IRestResult<T> where T : new()
    {
        public T? Data { get; set; }
    }
}
