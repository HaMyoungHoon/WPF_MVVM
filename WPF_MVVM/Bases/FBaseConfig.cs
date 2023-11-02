using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using WPF_MVVM.Libs;

namespace WPF_MVVM.Bases
{
    internal partial class FBaseConfig
    {
        public FBaseConfig()
        {
            _filePath = @$"{Directory.GetCurrentDirectory()}\Setting\{CFG_PATH}";
            NotifyOptionList = new() { "Snackbar", "Dialog", "MessageBox" };
            NotifyOption = NotifyOptionList.First();
            NotifyDuration = 5;
            ImageFilter = Array.Empty<string>();
            VideoFilter = Array.Empty<string>();
        }

        private static bool CheckFolder(string path)
        {
            string temp = path;
            DirectoryInfo filePath = new(path);
            DirectoryInfo di = new(temp.Replace(filePath.Name, ""));

            if (di.Exists == false)
            {
                di.Create();
                return false;
            }

            return true;
        }
        private static bool CheckFile(string path)
        {
            if (File.Exists(path) == false)
            {
                using (File.Create(path))
                {

                }

                return false;
            }

            return true;
        }
        private static bool DeleteFile(string path)
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            try
            {
                if (File.Exists(path) == true)
                {
                    File.Delete(path);
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        private void CreateSettingFile()
        {
            CheckFolder(_filePath);
            if (CheckFile(_filePath))
            {
                return;
            }

            File.WriteAllText(_filePath, "<?xml version='1.0' encoding='UTF-8'?>\n<ADMIN>\n</ADMIN>");
            FFileParser cfg = new(_filePath, FFileParser.FILE_TYPE.XML);
            CreateUIConfig(cfg);
            CreateEtcConfig(cfg);

        }
        public void LoadSettingFile()
        {
            CreateRdFiles();
            CreateSettingFile();
            FFileParser cfg = new(_filePath, FFileParser.FILE_TYPE.XML);
            GetUIConfig(cfg);
            GetEtcConfig(cfg);
        }
        public void ReCreateSettingFile()
        {
            CheckFolder(_filePath);
            DeleteFile(_filePath);
            LoadSettingFile();
        }

        private void CreateUIConfig(FFileParser data)
        {
            data.SetBool("ADMIN,UI,IsDarkTheme", IsDarkTheme);
            data.SetString("ADMIN,UI,PrimaryColor", "#9E9E9E");
            data.SetString("ADMIN,UI,SecondaryColor", "#2196F3");
            data.SetString("ADMIN,UI,PrimaryForegroundColor", "#DD000000");
            data.SetString("ADMIN,UI,SecondaryForegroundColor", "#FFFFFFFF");
            data.SetBool("ADMIN,UI,IsGuardScreenSaver", IsGuardScreenSaver);
            data.SetString("ADMIN,UI,NotifyOption", NotifyOption);
            data.SetInt("ADMIN,UI,NotifyDuration", NotifyDuration);
        }
        private void CreateEtcConfig(FFileParser data)
        {
            data.SetString("ADMIN,ETC,ImageFilter_0", ".jpg");
            data.SetString("ADMIN,ETC,ImageFilter_1", ".png");
            data.SetString("ADMIN,ETC,ImageFilter_2", ".gif");
            data.SetString("ADMIN,ETC,ImageFilter_3", ".bmp");

            data.SetString("ADMIN,ETC,VideoFilter_0", ".mp4");
            data.SetString("ADMIN,ETC,VideoFilter_1", ".avi");
            data.SetString("ADMIN,ETC,VideoFilter_2", ".wmv");
            data.SetString("ADMIN,ETC,VideoFilter_3", ".mkv");
            data.SetString("ADMIN,ETC,VideoFilter_4", ".mov");
        }

        private void GetUIConfig(FFileParser data)
        {
            IsDarkTheme = data.GetBool("ADMIN,UI,IsDarkTheme", IsDarkTheme);
            PrimaryColor = (Color)ColorConverter.ConvertFromString(data.GetString("ADMIN,UI,PrimaryColor", "#9E9E9E"));
            SecondaryColor = (Color)ColorConverter.ConvertFromString(data.GetString("ADMIN,UI,SecondaryColor", "#2196F3"));
            PrimaryForegroundColor = (Color)ColorConverter.ConvertFromString(data.GetString("ADMIN,UI,PrimaryForegroundColor", "#DD000000"));
            SecondaryForegroundColor = (Color)ColorConverter.ConvertFromString(data.GetString("ADMIN,UI,SecondaryForegroundColor", "#FFFFFFFF"));
            IsGuardScreenSaver = data.GetBool("ADMIN,UI,IsGuardScreenSaver", IsGuardScreenSaver);
            NotifyOption = data.GetString("ADMIN,UI,NotifyOption", NotifyOption);
            NotifyDuration = data.GetInt("ADMIN,UI,NotifyDuration", NotifyDuration);
            if (NotifyOptionList.Find(x => x == NotifyOption) == null)
            {
                NotifyOption = NotifyOptionList.First();
                data.SetString("ADMIN,UI,NotifyOption", NotifyOption);
            }
            if (NotifyDuration < 0 || NotifyDuration > 30)
            {
                NotifyDuration = 5;
            }
        }
        private void GetEtcConfig(FFileParser data)
        {
            int i = 0;
            ImageFilter = Array.Empty<string>();
            VideoFilter = Array.Empty<string>();
            while (true)
            {
                var buff = data.GetString($"ADMIN,ETC,ImageFilter_{i}", "NULL_DATA");
                if (buff.Contains("NULL_DATA") || buff.Contains('.') == false)
                {
                    break;
                }

                ImageFilter = ImageFilter.Append(buff).ToArray();
                i++;
            }
            i = 0;
            while (true)
            {
                var buff = data.GetString($"ADMIN,ETC,VideoFilter_{i}", "NULL_DATA");
                if (buff.Contains("NULL_DATA") || buff.Contains('.') == false)
                {
                    break;
                }

                VideoFilter = VideoFilter.Append(buff).ToArray();
                i++;
            }
        }

        public void SetDarkTheme(bool data)
        {
            FFileParser cfg = new(_filePath, FFileParser.FILE_TYPE.XML);
            cfg.SetBool("ADMIN,UI,IsDarkTheme", data);
            IsDarkTheme = data;
        }
        public void SetNotifyType(string data)
        {
            FFileParser cfg = new(_filePath, FFileParser.FILE_TYPE.XML);
            cfg.SetString("ADMIN,UI,NotifyOption", data);
            NotifyOption = data;
        }
        public void SetNotifyDuration(int data)
        {
            if (data < 0 || data > 30)
            {
                data = 5;
            }
            FFileParser cfg = new(_filePath, FFileParser.FILE_TYPE.XML);
            cfg.SetInt("ADMIN,UI,NotifyDuration", data);
            NotifyDuration = data;
        }
    }
}
