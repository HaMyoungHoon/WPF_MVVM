using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WPF_MVVM.Libs
{
    public class FPrintf
    {
        /// <summary>delegate for Callback Function</summary>
        /// <param name="data">Log Data</param>
        public delegate void PrintCallbackFunc(char[] data);
        /// <summary>Log Path</summary>
        public string FilePath { get; private set; }
        /// <summary>Log File Name</summary>
        public string FileName { get; private set; }
        /// <summary>Log delete Date</summary>
        public uint ExpiryDate { get; private set; }
        /// <summary>refer eMODE</summary>
        public E_MODE SaveMode { get; private set; }

        /// <summary>Save Mode</summary>
        public enum E_MODE
        {
            /// <summary>_filePath -> _fileName_yyyy-MM-dd</summary>
            SORT_DATETIME_FILE = 0,
            /// <summary>_filePath -> yyyy-MM-dd -> _fileName</summary>
            SORT_DATETIME_FOLDER,
        }

        private bool _notUse;
        private readonly PrintCallbackFunc? _printCallbackFunc;

        /// <summary>Class generator</summary>
        /// <param name="filePath">log path</param>
        /// <param name="fileName">log file Name</param>
        /// <param name="saveMode">save mode</param>
        /// <param name="printCallbackFunc">set callback function</param>
        /// <param name="expiryDate">delete file</param>
        public FPrintf(string filePath, string fileName, E_MODE saveMode = E_MODE.SORT_DATETIME_FILE, PrintCallbackFunc? printCallbackFunc = null, uint expiryDate = 60)
        {
            this.FilePath = filePath;
            this.FileName = fileName;
            this.SaveMode = saveMode;

            this._notUse = false;
            this._printCallbackFunc = printCallbackFunc;

            this.ExpiryDate = expiryDate;
        }

        /// <summary>can use config setting</summary>
        /// <param name="data"></param>
        public void NotUsePrintf(bool data = true)
        {
            _notUse = data;
        }
        private void DeleteFolder()
        {
            if (new DirectoryInfo(FilePath).Exists)
            {
                DirectoryInfo di;
                DateTime lowerFolder;
                DateTime expirydate = DateTime.Now.AddDays(-ExpiryDate);
                string[] dir = Directory.GetDirectories(FilePath, "*", SearchOption.TopDirectoryOnly);
                switch (SaveMode)
                {
                    case E_MODE.SORT_DATETIME_FILE:
                        {
                            for (int i = 0; i < dir.Length; i++)
                            {
                                if (int.TryParse(dir[i].Remove(0, dir[i].LastIndexOf('\\') + 1), out int parseDir))
                                {
                                    lowerFolder = new DateTime(parseDir / 100, parseDir % 100, 1);
                                    if (expirydate.Ticks - lowerFolder.Ticks >= 0)
                                    {
                                        di = new DirectoryInfo(string.Format(@"{0}\{1}", FilePath, lowerFolder.ToString("yyyyMM")));
                                        if (di.Exists)
                                        {
                                            di.Delete(true);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case E_MODE.SORT_DATETIME_FOLDER:
                        {
                            Regex rx = new(@"\d{1,4}-\d{1,2}-\d{1,2}");
                            for (int i = 0; i < dir.Length; i++)
                            {
                                if (rx.IsMatch(dir[i].Remove(0, dir[i].LastIndexOf('\\') + 1)))
                                {
                                    _ = DateTime.TryParse(dir[i].Remove(0, dir[i].LastIndexOf('\\') + 1), out lowerFolder);
                                    if (expirydate.Ticks - lowerFolder.Ticks >= 0)
                                    {
                                        di = new DirectoryInfo(string.Format(@"{0}\{1}", FilePath, lowerFolder.ToString("yyyy-MM-dd")));
                                        if (di.Exists)
                                        {
                                            di.Delete(true);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }

            }
        }
        private void WriteFile(string data)
        {
            System.Globalization.CultureInfo culture = new("en-US");
            DirectoryInfo di;
            FileInfo fi;
            var yyyymmdd = DateTime.Now;
            switch (SaveMode)
            {
                case E_MODE.SORT_DATETIME_FILE:
                    {
                        di = new DirectoryInfo(string.Format(@"{0}\{1}", FilePath, yyyymmdd.ToString("yyyyMM")));
                        fi = new FileInfo(string.Format(@"{0}\{1}\{2}_{3}.log", FilePath, yyyymmdd.ToString("yyyyMM"), FileName, yyyymmdd.ToString("yyyyMMdd")));
                    }
                    break;
                case E_MODE.SORT_DATETIME_FOLDER:
                    {
                        di = new DirectoryInfo(string.Format(@"{0}\{1}", FilePath, yyyymmdd.ToString("yyyy-MM-dd")));
                        fi = new FileInfo(string.Format(@"{0}\{1}\{2}.log", FilePath, yyyymmdd.ToString("yyyy-MM-dd"), FileName));
                    }
                    break;
                default: return;
            }
            if (di.Exists == false)
            {
                di.Create();
            }
            if (fi.Exists)
            {
                using StreamWriter sw = File.AppendText(fi.FullName);
                sw.WriteLine(string.Format("[{0}]   {1}", yyyymmdd.ToString("yyyy-MM-dd tt hh:mm:ss", culture), data));
                sw.Close();
            }
            else
            {
                using StreamWriter sw = new(fi.FullName);
                sw.WriteLine(string.Format("[{0}]   {1}", yyyymmdd.ToString("yyyy-MM-dd tt hh:mm:ss", culture), data));
                sw.Close();
            }
        }
        private async Task WriteFileAsync(string data)
        {
            System.Globalization.CultureInfo culture = new("en-US");
            DirectoryInfo di;
            FileInfo fi;
            var yyyymmdd = DateTime.Now;
            switch (SaveMode)
            {
                case E_MODE.SORT_DATETIME_FILE:
                    {
                        di = new DirectoryInfo(string.Format(@"{0}\{1}", FilePath, yyyymmdd.ToString("yyyyMM")));
                        fi = new FileInfo(string.Format(@"{0}\{1}\{2}_{3}.log", FilePath, yyyymmdd.ToString("yyyyMM"), FileName, yyyymmdd.ToString("yyyyMMdd")));
                    }
                    break;
                case E_MODE.SORT_DATETIME_FOLDER:
                    {
                        di = new DirectoryInfo(string.Format(@"{0}\{1}", FilePath, yyyymmdd.ToString("yyyy-MM-dd")));
                        fi = new FileInfo(string.Format(@"{0}\{1}\{2}.log", FilePath, yyyymmdd.ToString("yyyy-MM-dd"), FileName));
                    }
                    break;
                default: return;
            }
            if (di.Exists == false)
            {
                di.Create();
            }
            if (fi.Exists)
            {
                using StreamWriter sw = File.AppendText(fi.FullName);
                await sw.WriteLineAsync(string.Format("[{0}]   {1}", yyyymmdd.ToString("yyyy-MM-dd tt hh:mm:ss", culture), data));
                sw.Close();
            }
            else
            {
                using StreamWriter sw = new(fi.FullName);
                await sw.WriteLineAsync(string.Format("[{0}]   {1}", yyyymmdd.ToString("yyyy-MM-dd tt hh:mm:ss", culture), data));
                sw.Close();
            }
        }
        /// <summary>Save Log</summary>
        /// <param name="data">formated string</param>
        /// <param name="ps">params</param>
        public void PRINT_F(string data, params object[] ps)
        {
            string log = string.Empty;
            // 잘못 했으면 터져야지.
            //            try
            //            {
            log = string.Format(data, ps);
            //            }
            //            catch (Exception ex)
            //            {
            //                log = ex.ToString();
            //            }
            PRINT_F(log.ToCharArray());
        }
        /// <summary>Save Log</summary>
        /// <param name="data">string type data</param>
        public void PRINT_F(string data)
        {
            PRINT_F(data.ToCharArray());
        }
        /// <summary>Save Log</summary>
        /// <param name="data">byte array type data</param>
        public void PRINT_F(byte[] data)
        {
            PRINT_F(Encoding.UTF8.GetString(data).ToCharArray());
        }
        /// <summary>Save Log</summary>
        /// <param name="data">char array type data</param>
        public void PRINT_F(char[] data)
        {
            if (_notUse == true)
            {
                return;
            }

            if (ExpiryDate != 0)
            {
                try
                {
                    DeleteFolder();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("printf : " + ex.ToString());
                }
            }
            try
            {
                WriteFile(new string(data));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("printf : " + ex.ToString());
            }
            _printCallbackFunc?.Invoke(data);
        }

        /// <summary>Save Log</summary>
        /// <param name="data">string type data</param>
        public async Task PRINT_F_Async(string data)
        {
            await PRINT_F_Async(data.ToCharArray());
        }
        /// <summary>Save Log</summary>
        /// <param name="data">byte array type data</param>
        public async Task PRINT_F_Async(byte[] data)
        {
            await PRINT_F_Async(Encoding.UTF8.GetString(data).ToCharArray());
        }
        /// <summary>Save Log</summary>
        /// <param name="data">char array type data</param>
        public async Task PRINT_F_Async(char[] data)
        {
            if (_notUse == true)
            {
                return;
            }

            if (ExpiryDate != 0)
            {
                try
                {
                    DeleteFolder();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("printf : " + ex.ToString());
                }
            }
            try
            {
                await WriteFileAsync(new string(data));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("printf : " + ex.ToString());
            }
            _printCallbackFunc?.Invoke(data);
        }
    }
}
