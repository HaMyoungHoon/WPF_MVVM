using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using WPF_MVVM.Libs;

namespace WPF_MVVM.Bases
{
    internal partial class FBaseConfig
    {
        private void CreateRdFiles()
        {
            CreateColorsRd_Light();
            CreateColorsRd_Dark();
        }
        public string GetColorsRd_LightPath()
        {
            return @$"{Directory.GetCurrentDirectory()}\Resources\Styles\ColorsRd_Light.xaml";
        }
        public string GetColorsRd_DarkPath()
        {
            return @$"{Directory.GetCurrentDirectory()}\Resources\Styles\ColorsRd_Dark.xaml";
        }
        private void CreateColorsRd_Light()
        {
            CheckFolder(GetColorsRd_LightPath());
            if (CheckFile(GetColorsRd_LightPath()))
            {
                try
                {
                    var lightTheme = new ResourceDictionary() { Source = new Uri(GetColorsRd_LightPath()) };
                    return;
                }
                catch
                {
                    DeleteFile(GetColorsRd_LightPath());
                    var prefix = String.Concat(typeof(App).Namespace, ";component/");
                    var temp = new Uri("pack://application:,,,/Resources/Styles/ColorsRd_Light.xaml");
                    var darkTemp = new ResourceDictionary() { Source = temp };
                    using StreamWriter writer = new(GetColorsRd_LightPath());
                    XamlWriter.Save(darkTemp, writer);
                    return;
                }
            }

            File.WriteAllText(GetColorsRd_LightPath(), "<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n\n</ResourceDictionary>");

            FFileParser xaml = new(GetColorsRd_LightPath(), FFileParser.FILE_TYPE.XAML);
            CreateDataGridLightColor(xaml);
        }
        private void CreateColorsRd_Dark()
        {
            CheckFolder(GetColorsRd_DarkPath());
            if (CheckFile(GetColorsRd_DarkPath()))
            {
                try
                {
                    var darkTheme = new ResourceDictionary() { Source = new Uri(GetColorsRd_DarkPath()) };
                    return;
                }
                catch
                {
                    DeleteFile(GetColorsRd_DarkPath());
                    var prefix = String.Concat(typeof(App).Namespace, ";component/");
                    var temp = new Uri("pack://application:,,,/Resources/Styles/ColorsRd_Dark.xaml");
                    var darkTemp = new ResourceDictionary() { Source = temp };
                    using StreamWriter writer = new(GetColorsRd_DarkPath());
                    XamlWriter.Save(darkTemp, writer);
                    return;
                }
            }

            File.WriteAllText(GetColorsRd_DarkPath(), "<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n\n</ResourceDictionary>");

            FFileParser xaml = new(GetColorsRd_DarkPath(), FFileParser.FILE_TYPE.XAML);
            CreateDataGridDarkColor(xaml);
        }
        private void CreateDataGridLightColor(FFileParser data)
        {
            data.SetString("SolidColorBrush,absoluteBack,Color", "#FFFFFF");
            data.SetString("SolidColorBrush,absoluteFore,Color", "#000000");
            data.SetString("SolidColorBrush,defBack,Color", "#F0F0F0");
            data.SetString("SolidColorBrush,defFore,Color", "#0F0F0F");
            data.SetString("SolidColorBrush,defShadow,Color", "#66000000");
            data.SetString("SolidColorBrush,defBorderBrush,Color", "#000000");
            data.SetString("SolidColorBrush,defSelectBack,Color", "#666666");
            data.SetString("SolidColorBrush,defSelectFore,Color", "#FFFFFF");
            data.SetString("SolidColorBrush,defRecvMsg,Color", "{Binding Source={StaticResource Yellow2}, Path=Color}");
            data.SetString("SolidColorBrush,defSendMsg,Color", "{Binding Source={StaticResource Green2}, Path=Color}");
            data.SetString("SolidColorBrush,defChatTime,Color", "#979797");

            data.SetString("SolidColorBrush,dgBackground,Color", "{Binding Source={StaticResource Grey1}, Path=Color}");
            data.SetString("SolidColorBrush,dgForeground,Color", "{Binding Source={StaticResource Grey1F}, Path=Color}");

            data.SetString("SolidColorBrush,btnSingleToggleOn,Color", "{Binding Source={StaticResource Yellow1}, Path=Color}");
            data.SetString("SolidColorBrush,btnMultiToggleOn,Color", "#6175E8");
            data.SetString("SolidColorBrush,btnToggleOff,Color", "#DFDFDF");
        }
        private void CreateDataGridDarkColor(FFileParser data)
        {
            data.SetString("SolidColorBrush,absoluteBack,Color", "#000000");
            data.SetString("SolidColorBrush,absoluteFore,Color", "#FFFFFF");
            data.SetString("SolidColorBrush,defBack,Color", "#0F0F0F");
            data.SetString("SolidColorBrush,defFore,Color", "#F0F0F0");
            data.SetString("SolidColorBrush,defShadow,Color", "#66000000");
            data.SetString("SolidColorBrush,defBorderBrush,Color", "#FFFFFF");
            data.SetString("SolidColorBrush,defSelectBack,Color", "#666666");
            data.SetString("SolidColorBrush,defSelectFore,Color", "#FFFFFF");
            data.SetString("SolidColorBrush,defRecvMsg,Color", "{Binding Source={StaticResource Yellow2}, Path=Color}");
            data.SetString("SolidColorBrush,defSendMsg,Color", "{Binding Source={StaticResource Green2}, Path=Color}");
            data.SetString("SolidColorBrush,defChatTime,Color", "#FFFFFF");

            data.SetString("SolidColorBrush,dgBackground,Color", "{Binding Source={StaticResource Grey3}, Path=Color}");
            data.SetString("SolidColorBrush,dgForeground,Color", "{Binding Source={StaticResource Grey3F}, Path=Color}");

            data.SetString("SolidColorBrush,btnSingleToggleOn,Color", "{Binding Source={StaticResource Yellow3}, Path=Color}");
            data.SetString("SolidColorBrush,btnMultiToggleOn,Color", "#6175E8");
            data.SetString("SolidColorBrush,btnToggleOff,Color", "#DFDFDF");
        }
    }
}
