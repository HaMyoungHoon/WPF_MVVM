using System.Collections.Generic;
using System.Windows.Media;

namespace WPF_MVVM.Bases
{
    internal partial class FBaseConfig
    {
        private const string CFG_PATH = "WPF_MVVMCfg.xml";
        private readonly string _filePath;
        public bool IsDarkTheme { get; set; }
        public Color PrimaryColor { get; private set; }
        public Color SecondaryColor { get; private set; }
        public Color PrimaryForegroundColor { get; private set; }
        public Color SecondaryForegroundColor { get; private set; }
        public bool IsGuardScreenSaver { get; set; }
        public List<string> NotifyOptionList { get; }
        public string NotifyOption { get; private set; }
        public int NotifyDuration { get; private set; }
        public string[] ImageFilter { get; private set; }
        public string[] VideoFilter { get; private set; }
    }
}
