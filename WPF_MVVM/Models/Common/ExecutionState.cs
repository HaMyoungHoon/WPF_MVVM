using System;

namespace WPF_MVVM.Models.Common
{

    [Flags]
    internal enum ExecutionState : uint
    {
        /// <summary> 
        /// ES_SYSTEM_REQUIRED 
        /// </summary> 
        ES_SYSTEM_REQUIRED = 0x00000001,
        /// <summary> 
        /// ES_DISPLAY_REQUIRED 
        /// </summary> 
        ES_DISPLAY_REQUIRED = 0x00000002,
        /// <summary> 
        /// ES_AWAYMODE_REQUIRED 
        /// </summary> 
        ES_AWAYMODE_REQUIRED = 0x00000040,
        /// <summary> 
        /// ES_CONTINUOUS 
        /// </summary> 
        ES_CONTINUOUS = 0x80000000,

        ES_ALL = ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED | ES_AWAYMODE_REQUIRED | ES_CONTINUOUS
    }
}
