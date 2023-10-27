using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace WPF_MVVM.Libs
{
    public class FXamlParser : IFParser
    {
        private string _filePath;
        /// <summary>current file path</summary>
        public string FilePath
        {
            get
            {
                return _filePath;
            }
        }
        public FXamlParser(string filePath)
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

        public bool CreateFile(string section, string value, string filePath = "")
        {
            if (_filePath.Length == 0 && filePath.Length == 0)
            {
                return false;
            }

            if (filePath.Length == 0)
            {
                filePath = _filePath;
            }

            var node = section.Split(',');
            int nodeSize = node.Length;
            if (nodeSize < 1)
            {
                return false;
            }
            try
            {
                using XmlWriter writer = XmlWriter.Create(filePath);
                for (int i = 0; i < nodeSize - 1; i++)
                {
                    writer.WriteStartElement(node[i]);
                }
                if (nodeSize == 1)
                {
                    writer.WriteStartElement(node[0]);
                }
                else
                {
                    writer.WriteElementString(node[nodeSize - 1], value);
                }

                writer.WriteEndElement();
                writer.Flush();
            }
            catch
            {
                return false;
            }

            return true;
        }
        public void SetFilePath(string filePath)
        {
            _filePath = filePath;
        }

        public bool GetBool(string cmd, bool defValue, string filePath = "")
        {
            bool.TryParse(GetData(cmd, defValue.ToString(), filePath), out bool ret);
            return ret;
        }
        public double GetDouble(string cmd, double defValue, string filePath = "")
        {
            double.TryParse(GetData(cmd, defValue.ToString(), filePath), out double ret);
            return ret;
        }

        public int GetInt(string cmd, int defValue, string filePath = "")
        {
            int.TryParse(GetData(cmd, defValue.ToString(), filePath), out int ret);
            return ret;
        }

        public string GetString(string cmd, string defValue, string filePath = "")
        {
            return GetData(cmd, defValue, filePath);
        }

        public bool SetBool(string cmd, bool data, string filePath = "")
        {
            return SetData(cmd, data.ToString(), filePath);
        }
        public bool SetDouble(string cmd, double data, string filePath = "")
        {
            return SetData(cmd, data.ToString(), filePath);
        }

        public bool SetInt(string cmd, int data, string filePath = "")
        {
            return SetData(cmd, data.ToString(), filePath);
        }

        public bool SetString(string cmd, string data, string filePath = "")
        {
            return SetData(cmd, data, filePath);
        }

        private string GetData(string section, string defValue, string filePath)
        {
            if (CheckFilePath(filePath) == false)
            {
                return defValue;
            }

            if (_filePath.Length == 0 && filePath.Length == 0)
            {
                return defValue;
            }

            if (filePath.Length == 0)
            {
                filePath = _filePath;
            }

            string[] temp = section.Split(',');
            int tempSize = temp.Length;
            if (tempSize < 1)
            {
                return defValue;
            }

            XmlDocument xmlDoc = new();
            try
            {
                xmlDoc.Load(filePath);
                XmlNodeList? nodeList = xmlDoc.FirstChild?.ChildNodes;
                if (nodeList == null)
                {
                    return defValue;
                }

                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlNode? node = nodeList[i];
                    if (node == null)
                    {
                        break;
                    }
                    if (node.Name != temp[0])
                    {
                        continue;
                    }
                    if (node.Attributes?["x:Key"]?.Value != temp[1])
                    {
                        continue;
                    }

                    return node.Attributes?[temp[2]]?.Value ?? defValue;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("xamlParser : " + ex.ToString());
            }

            return defValue;
        }
        private bool SetData(string section, string data, string filePath)
        {
            if (CheckFilePath(filePath) == false)
            {
                return false;
            }

            if (_filePath.Length == 0 && filePath.Length == 0)
            {
                return false;
            }

            if (filePath.Length == 0)
            {
                filePath = _filePath;
            }

            string[] temp = section.Split(',');
            int tempSize = temp.Length;
            if (tempSize < 1)
            {
                return false;
            }

            XmlDocument xmlDoc = new();
            try
            {
                xmlDoc.Load(filePath);
                var nodeList = xmlDoc.FirstChild?.ChildNodes;
                if (nodeList == null)
                {
                    return false;
                }

                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlNode? buff = nodeList[i];
                    if (buff == null)
                    {
                        break;
                    }
                    if (buff.Name != temp[0])
                    {
                        continue;
                    }
                    if (buff.Attributes == null)
                    {
                        continue;
                    }
                    if (buff.Attributes?["x:Key"]?.Value != temp[1])
                    {
                        continue;
                    }

                    if (buff.Attributes[temp[2]] is not XmlAttribute buffAttr)
                    {
                        return false;
                    }

                    buffAttr.Value = data;
                    xmlDoc.Save(filePath);
                    return true;
                }

                XmlElement node = xmlDoc.CreateElement(temp[0], xmlDoc.FirstChild?.GetNamespaceOfPrefix(""));
                XmlAttribute xKey = xmlDoc.CreateAttribute("x", "Key", xmlDoc.FirstChild?.GetNamespaceOfPrefix("x"));
                xKey.Prefix = "x";
                xKey.Value = temp[1];
                XmlAttribute attr = xmlDoc.CreateAttribute(temp[2]);
                attr.Value = data;
                node.Attributes?.Append(xKey);
                node.Attributes?.Append(attr);
                xmlDoc.FirstChild?.AppendChild(node);
                xmlDoc.Save(filePath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("xamlParser: " + ex.ToString());
            }

            return false;
        }
    }
}
