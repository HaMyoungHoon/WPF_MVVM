using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WPF_MVVM.Libs;
using WPF_MVVM.Models.Common;

namespace WPF_MVVM.Bases
{
    internal partial class FBaseFunc : ObservableObject
    {
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)] private static extern ExecutionState SetThreadExecutionState(ExecutionState state);

        private static FBaseFunc? _ins;
        public static FBaseFunc Ins
        {
            get => _ins ??= new();
        }
        public bool IsDispose { get; private set; }
        private readonly FPrintf _log;
        public FBaseConfig Cfg { get; set; }

        [ObservableProperty]
        private string _programVersion;
        public List<Tuple<string, AvalonDock.Themes.Theme>> Themes { get; private set; }

        public const int LIMIT_FILE_SIZE = 1024 * 1024 * 20; // 20 MB
        public const string AES_KEY = "1991031065748520";
        private readonly string[] EXCEL_ARRAY = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
        "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ" };
    }
}
