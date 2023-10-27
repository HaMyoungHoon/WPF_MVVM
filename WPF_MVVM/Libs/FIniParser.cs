using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace WPF_MVVM.Libs
{
    /// <summary>ini parser class</summary>
    public class FIniParser : IFParser
    {
        private const string _stx = "[";
        private const string _etx = "]";
        private const string _equal = "=";
        private const string _sp = " ";
        private const string _ent = "\r\n";
        private string _filePath;

        /// <summary>current file path</summary>
        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }

        /// <summary>construct</summary>
        /// <param name="filePath">Default File Path</param>
        public FIniParser(string filePath)
        {
            _filePath = filePath;
        }
        private bool CheckFilePath(string filePath = "")
        {
            if (_filePath.Length == 0 && filePath.Length == 0)
            {
                return false;
            }

            if (filePath.Length == 0)
            {
                filePath = _filePath;
            }

            FileInfo fi = new(filePath);
            if (fi.DirectoryName == null)
            {
                return false;
            }

            DirectoryInfo di = new(fi.DirectoryName);
            if (di.Exists == false)
            {
                di.Create();
            }

            return fi.Exists;
        }

        bool IFParser.CreateFile(string root, string value, string filePath)
        {
            var pathBuff = filePath;
            if (CheckFilePath(filePath))
            {
                return false;
            }

            if (_filePath.Length <= 0 && filePath.Length <= 0)
            {
                return false;
            }

            if (pathBuff.Length <= 0)
            {
                pathBuff = _filePath;
            }

            if (root.Length <= 0)
            {
                return false;
            }

            try
            {
                string data = GetStxSectionEtx(root);
                using var file = File.Create(pathBuff);
                file.Write(Encoding.UTF8.GetBytes(data));
            }
            catch
            {
                return false;
            }

            return true;
        }
        void IFParser.SetFilePath(string filePath)
        {
            _filePath = filePath;
        }
        string IFParser.GetString(string cmd, string defValue, string filePath)
        {
            if (CheckFilePath(filePath) == false)
            {
                return defValue;
            }

            var parseData = cmd.Split(',');
            if (parseData.Length < 2)
            {
                return defValue;
            }

            if (filePath.Length == 0)
            {
                filePath = _filePath;
            }

            string retVal = string.Empty;
            int ret = 0;
            try
            {
                ret = GetPrivateProfileString(parseData[0], parseData[1], defValue, out retVal, 1024, filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("iniParser : " + ex.ToString());
            }

            return ret == 0 ? retVal : defValue;
        }
        int IFParser.GetInt(string cmd, int defValue, string filePath)
        {
            if (CheckFilePath(filePath) == false)
            {
                return defValue;
            }

            var parseData = cmd.Split(',');
            if (parseData.Length < 2)
            {
                return defValue;
            }

            if (filePath.Length == 0)
            {
                filePath = _filePath;
            }

            string retVal = string.Empty;
            int ret = 0;
            try
            {
                ret = GetPrivateProfileString(parseData[0], parseData[1], defValue.ToString(), out retVal, 1024, filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("iniParser : " + ex.ToString());
            }

            if (ret == 0)
            {
                if (int.TryParse(retVal.ToString(), out int temp))
                {
                    return temp;
                }
            }

            return defValue;
        }
        double IFParser.GetDouble(string cmd, double defValue, string filePath)
        {
            if (CheckFilePath(filePath) == false)
            {
                return defValue;
            }

            var parseData = cmd.Split(',');
            if (parseData.Length < 2)
            {
                return defValue;
            }

            if (filePath.Length == 0)
            {
                filePath = _filePath;
            }

            string retVal = string.Empty;
            int ret = 0;
            try
            {
                ret = GetPrivateProfileString(parseData[0], parseData[1], defValue.ToString(), out retVal, 1024, filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("iniParser : " + ex.ToString());
            }

            if (ret == 0)
            {
                if (double.TryParse(retVal.ToString(), out double temp))
                {
                    return temp;
                }
            }
            ;

            return defValue;
        }
        bool IFParser.GetBool(string cmd, bool defValue, string filePath)
        {
            if (CheckFilePath(filePath) == false)
            {
                return defValue;
            }

            var parseData = cmd.Split(',');
            if (parseData.Length < 2)
            {
                return defValue;
            }

            if (filePath.Length == 0)
            {
                filePath = _filePath;
            }

            string retVal = string.Empty;
            int ret = 0;
            try
            {
                ret = GetPrivateProfileString(parseData[0], parseData[1], defValue.ToString(), out retVal, 1024, filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("iniParser : " + ex.ToString());
            }

            if (ret == 0)
            {
                if (bool.TryParse(retVal.ToString(), out bool temp))
                {
                    return temp;
                }
            }
            ;

            return defValue;
        }
        bool IFParser.SetString(string cmd, string value, string filePath)
        {
            if (CheckFilePath(filePath) == false)
            {
                return false;
            }

            if (value.Length == 0)
            {
                return false;
            }

            var parseData = cmd.Split(',');
            if (parseData.Length < 2)
            {
                return false;
            }

            if (filePath.Length == 0)
            {
                filePath = _filePath;
            }

            long ret = 0;
            try
            {
                ret = WritePrivateProfileString(parseData[0], parseData[1], value, filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("iniParser : " + ex.ToString());
            }

            return ret != 0;
        }
        bool IFParser.SetInt(string cmd, int value, string filePath)
        {
            if (CheckFilePath(filePath) == false)
            {
                return false;
            }

            var parseData = cmd.Split(',');
            if (parseData.Length < 2)
            {
                return false;
            }

            if (filePath.Length == 0)
            {
                filePath = _filePath;
            }

            long ret = 0;
            try
            {
                ret = WritePrivateProfileString(parseData[0], parseData[1], value.ToString(), filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("iniParser : " + ex.ToString());
            }

            return ret != 0;
        }
        bool IFParser.SetDouble(string cmd, double value, string filePath)
        {
            if (CheckFilePath(filePath) == false)
            {
                return false;
            }

            var parseData = cmd.Split(',');
            if (parseData.Length < 2)
            {
                return false;
            }

            if (filePath.Length == 0)
            {
                filePath = _filePath;
            }

            long ret = 0;
            try
            {
                ret = WritePrivateProfileString(parseData[0], parseData[1], value.ToString(), filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("iniParser : " + ex.ToString());
            }

            return ret != 0;
        }
        bool IFParser.SetBool(string cmd, bool value, string filePath)
        {
            if (CheckFilePath(filePath) == false)
            {
                return false;
            }

            var parseData = cmd.Split(',');
            if (parseData.Length < 2)
            {
                return false;
            }

            if (filePath.Length == 0)
            {
                filePath = _filePath;
            }

            long ret = 0;
            try
            {
                ret = WritePrivateProfileString(parseData[0], parseData[1], value.ToString(), filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("iniParser : " + ex.ToString());
            }

            return ret != 0;
        }

        private static int GetPrivateProfileString(string section, string key, string def, out string ret, int size, string filePath)
        {
            ret = def;

            if (section.Length <= 0 || section.Trim().Length <= 0)
            {
                return -1;
            }
            if (key.Length <= 0 || key.Trim().Length <= 0)
            {
                return -1;
            }
            if (size <= 0)
            {
                return -1;
            }

            List<string> buff;
            try
            {
                buff = File.ReadAllLines(filePath).ToList();
            }
            catch
            {
                return -1;
            }

            if (buff.Count <= 0)
            {
                return -1;
            }

            int posSTX = PasSTX(buff, section);
            if (posSTX == -1)
            {
                return -1;
            }

            int posETX = PasETX(buff, posSTX);
            if (posETX == -1)
            {
                posETX = buff.Count;
            }

            for (int i = posSTX; i < posETX; i++)
            {
                if (buff[i].Trim().Contains(string.Format("{0}{1}", key, _equal)))
                {
                    ret = buff[i][(buff[i].IndexOf(_equal) + 1)..];
                    break;
                }
            }

            while (ret.IndexOf(_sp) == 0)
            {
                _ = ret.Remove(0, 1);
            }

            return 0;
        }
        private long WritePrivateProfileString(string section, string key, string value, string filePath)
        {
            if (section.Length <= 0 || section.Trim().Length <= 0)
            {
                return -1;
            }
            if (key.Length <= 0 || key.Trim().Length <= 0)
            {
                return -1;
            }

            var ret = -1;
            List<string> buff;
            try
            {
                buff = File.ReadAllLines(filePath).ToList();
            }
            catch
            {
                return ret;
            }

            if (buff.Count <= 0)
            {
                return ret;
            }

            int posSTX = PasSTX(buff, section);
            if (posSTX == -1)
            {
                ret = -2;
                buff.Add(string.Format("{0}{1}{2}", key, _equal, value));
                return SaveBuff(buff, ret);
            }

            int posETX = PasETX(buff, posSTX);
            if (posETX == -1)
            {
                posETX = buff.Count;
            }

            var posKey = -1;
            for (int i = posSTX; i < posETX; i++)
            {
                if (buff[i].Trim().Contains(string.Format("{0}{1}", key, _equal)))
                {
                    posKey = i;
                    break;
                }
            }

            if (posKey == -1)
            {
                ret = -3;
                buff.Add(string.Format("{0}{1}{2}", key, _equal, value));
                return SaveBuff(buff, ret);
            }

            ret = 0;
            buff[posKey] = string.Format("{0}{1}", buff[posKey][..(buff[posKey].IndexOf(_equal) + 1)], value);

            return SaveBuff(buff, ret);
        }

        private static int PasSTX(List<string> buff, string section)
        {
            int posSTX = -1;
            foreach (var i in buff)
            {
                if (i == GetStxSectionEtx(section))
                {
                    posSTX++;
                    break;
                }
                posSTX++;
            }

            return posSTX;
        }
        private static int PasETX(List<string> buff, int posSTX)
        {
            int posETX = -1;
            for (int i = posSTX + 1; i < buff.Count; i++)
            {
                if (buff[i].Contains(_stx))
                {
                    posETX = i;
                    break;
                }
            }

            return posETX;
        }
        private int SaveBuff(List<string> buff, int ret)
        {
            try
            {
                string data = string.Empty;
                foreach (var i in buff)
                {
                    if (i == _ent)
                    {
                        continue;
                    }
                    data = string.Format("{0}{1}{2}", data, i, _ent);
                }
                if (data.Length <= 0)
                {
                    return -1;
                }
                File.WriteAllText(_filePath, data);
            }
            catch
            {
                return -1;
            }

            return ret;
        }
        private static string GetStxSectionEtx(string section) => $"{_stx}{section}{_etx}";
    }
}
