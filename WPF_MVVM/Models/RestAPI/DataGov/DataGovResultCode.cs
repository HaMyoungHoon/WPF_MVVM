using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_MVVM.Converters;

namespace WPF_MVVM.Models.RestAPI.DataGov
{
    [TypeConverter(typeof(EnumToDescriptionConverter<DataGovResultCode>))]
    internal enum DataGovResultCode
    {
        [Description("00")]
        OK,
        [Description("01")]
        APPLICATION_ERROR,
        [Description("04")]
        HTTP_ERROR,
        [Description("12")]
        NO_OPENAPI_SERVICE_ERROR,
        [Description("20")]
        SERVICE_ACCESS_DENIED_ERROR,
        [Description("22")]
        LIMITED_NUMBER_OF_SERVICE_REQUESTS_EXCEEDS_ERROR,
        [Description("30")]
        SERVICE_KEY_IS_NOT_REGISTERED_ERROR,
        [Description("31")]
        DEADLINE_HAS_EXPIRED_ERROR,
        [Description("32")]
        UNREGISTERED_IP_ERROR,
        [Description("99")]
        UNKNOWN_ERROR,
    }
}
