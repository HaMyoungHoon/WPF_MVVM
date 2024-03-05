using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Text;

namespace WPF_MVVM.Helpers
{
    internal static class VideoHelper
    {
        private static string _ffmpegPath;
        static VideoHelper()
        {
            _ffmpegPath = @"Libs\FFmpeg";
        }
        public static bool IsMP4(byte[] header)
        {
            var headerString = Encoding.ASCII.GetString(header);
            return headerString.StartsWith("\0\0\0\u0018ftyp") || headerString.EndsWith("\0\0\0\u00020ftyp");
        }
        public static bool IsAVI(byte[] header)
        {
            var headerString = Encoding.ASCII.GetString(header);
            return headerString.StartsWith("RIFF") && headerString.EndsWith("AVI ");
        }
        /// <summary>
        /// WMV or WMA
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public static bool IsASF(byte[] header)
        {
            return header[0] == 0x30 && header[1] == 0x26 && header[2] == 0xB2;
        }

        public static TimeSpan GetFFMpegVideoDuration(string path)
        {
            FFMpegStreamDecoder streamDecoder = new(_ffmpegPath);
            var ret = streamDecoder.GetVideoDuration(path);
            streamDecoder.Dispose();
            return ret;
        }
        public static double GetFFMpegFrameRate(string path)
        {
            FFMpegStreamDecoder streamDecoder = new(_ffmpegPath);
            var ret = streamDecoder.GetFrameRate(path);
            streamDecoder.Dispose();
            return ret;
        }
        public static TimeSpan GetShellParseVideoDuration(string path)
        {
            string? duration = null;
            using (ShellObject shell = ShellObject.FromParsingName(path))
            {
                var media = shell.Properties.System.Media;
                duration = media.Duration.FormatForDisplay(PropertyDescriptionFormatOptions.None);
            }

            if (duration == null)
            {
                return TimeSpan.FromSeconds(0);
            }

            var durationSplit = duration.Split(":");
            if (durationSplit.Length < 3)
            {
                return TimeSpan.FromSeconds(0);
            }

            var ret = 0.0;
            if (double.TryParse(durationSplit[0], out double hour))
            {
                ret += hour * 3600;
            }

            if (double.TryParse(durationSplit[1], out double minute))
            {
                ret += minute * 60;
            }
            
            if (double.TryParse(durationSplit[2], out double second))
            {
                ret += second;
            }

            return TimeSpan.FromSeconds(ret);
        }
    }
}
