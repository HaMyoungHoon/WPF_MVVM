namespace WPF_MVVM.Libs
{
    /// <summary>file parser class</summary>
    public class FFileParser
    {
        /// <summary>
        /// File Type
        /// </summary>
        public enum FILE_TYPE
        {
            /// <summary>.ini file</summary>
            INI = 0,
            /// <summary>.xml file</summary>
            XML = 1,
            //            /// <summary>.json file</summary>
            //            JSON = 2,
            XAML = 3,
        }
        private FILE_TYPE _fileType;

        private readonly IFParser _iniParser;
        private readonly IFParser _xmlParser;
        private readonly IFParser _xamlParser;

        /// <summary>Construct</summary>
        /// <param name="filePath">Default File Path</param>
        /// <param name="fileType">File Type</param>
        public FFileParser(string filePath, FILE_TYPE fileType)
        {
            // new는 전체 다 때릴까.
            _iniParser = new FIniParser(filePath);
            _xmlParser = new FXmlParser(filePath);
            _xamlParser = new FXamlParser(filePath);
            _fileType = fileType;
        }

        /// <summary>
        /// Create Ini File
        /// </summary>
        /// <param name="section">Xml : Node, Ini : Root Key</param>
        /// <param name="value">Xml : Value, Ini : None</param>
        /// <param name="filePath">Default File Path</param>
        public void CreateFile(string section, string value, string filePath)
        {
            switch (_fileType)
            {
                case FILE_TYPE.INI: _iniParser.CreateFile(section, value, filePath); break;
                case FILE_TYPE.XML: _xmlParser.CreateFile(section, value, filePath); break;
                case FILE_TYPE.XAML: _xamlParser.CreateFile(section, value, filePath); break;
            }
        }
        /// <summary>
        /// Re Setting File Path, File Type
        /// </summary>
        /// <param name="filePath">Default File Path</param>
        /// <param name="fileType">File Type</param>
        public void SetFilePath(string filePath, FILE_TYPE fileType)
        {
            _fileType = fileType;

            switch (_fileType)
            {
                case FILE_TYPE.INI: _iniParser.SetFilePath(filePath); break;
                case FILE_TYPE.XML: _xmlParser.SetFilePath(filePath); break;
                case FILE_TYPE.XAML: _xamlParser.SetFilePath(filePath); break;
            }
        }

        /// <summary>
        /// GetString("Section,Key");
        /// GetString("Node,ChildNode,ChildNode...");
        /// </summary>
        /// <param name="cmd">ref to summary</param>
        /// <param name="defValue">default value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>string type</returns>
        public string GetString(string cmd, string defValue, string filePath = "") =>
            _fileType switch
            {
                FILE_TYPE.INI => _iniParser.GetString(cmd, defValue, filePath),
                FILE_TYPE.XML => _xmlParser.GetString(cmd, defValue, filePath),
                FILE_TYPE.XAML => _xamlParser.GetString(cmd, defValue, filePath),
                _ => defValue
            };

        /// <summary>
        /// GetInt("Section,Key");
        /// GetInt("Node,ChildNode,ChildNode...");
        /// </summary>
        /// <param name="cmd">ref to summary</param>
        /// <param name="defValue">default value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>int type</returns>
        public int GetInt(string cmd, int defValue, string filePath = "") =>
            _fileType switch
            {
                FILE_TYPE.INI => _iniParser.GetInt(cmd, defValue, filePath),
                FILE_TYPE.XML => _xmlParser.GetInt(cmd, defValue, filePath),
                FILE_TYPE.XAML => _xamlParser.GetInt(cmd, defValue, filePath),
                _ => defValue
            };

        /// <summary>
        /// GetDouble("Section,Key");
        /// GetDouble("Node,ChildNode,ChildNode...");
        /// </summary>
        /// <param name="cmd">ref to summary</param>
        /// <param name="defValue">default value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>double type</returns>
        public double GetDouble(string cmd, double defValue, string filePath = "") =>
            _fileType switch
            {
                FILE_TYPE.INI => _iniParser.GetDouble(cmd, defValue, filePath),
                FILE_TYPE.XML => _xmlParser.GetDouble(cmd, defValue, filePath),
                FILE_TYPE.XAML => _xamlParser.GetDouble(cmd, defValue, filePath),
                _ => defValue
            };
        /// <summary>
        /// GetBool("Section,Key");
        /// GetBool("Node,ChildNode,ChildNode...");
        /// </summary>
        /// <param name="cmd">ref to summary</param>
        /// <param name="defValue">default value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>bool type</returns>
        public bool GetBool(string cmd, bool defValue, string filePath = "") =>
            _fileType switch
            {
                FILE_TYPE.INI => _iniParser.GetBool(cmd, defValue, filePath),
                FILE_TYPE.XML => _xmlParser.GetBool(cmd, defValue, filePath),
                FILE_TYPE.XAML => _xamlParser.GetBool(cmd, defValue, filePath),
                _ => defValue
            };

        /// <summary>
        /// SetString("Section,Key");
        /// SetString("Node,ChildNode,ChildeNode...);
        /// </summary>
        /// <param name="cmd">ref summary</param>
        /// <param name="value">setting value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>false : fail</returns>
        public bool SetString(string cmd, string value, string filePath = "") =>
            _fileType switch
            {
                FILE_TYPE.INI => _iniParser.SetString(cmd, value, filePath),
                FILE_TYPE.XML => _xmlParser.SetString(cmd, value, filePath),
                FILE_TYPE.XAML => _xamlParser.SetString(cmd, value, filePath),
                _ => false
            };

        /// <summary>
        /// SetInt("Section,Key");
        /// SetInt("Node,ChildNode,ChildeNode...);
        /// </summary>
        /// <param name="cmd">ref summary</param>
        /// <param name="value">setting value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>false : fail</returns>
        public bool SetInt(string cmd, int value, string filePath = "") =>
            _fileType switch
            {
                FILE_TYPE.INI => _iniParser.SetInt(cmd, value, filePath),
                FILE_TYPE.XML => _xmlParser.SetInt(cmd, value, filePath),
                FILE_TYPE.XAML => _xamlParser.SetInt(cmd, value, filePath),
                _ => false
            };

        /// <summary>
        /// SetDouble("Section,Key");
        /// SetDouble("Node,ChildNode,ChildeNode...);
        /// </summary>
        /// <param name="cmd">ref summary</param>
        /// <param name="value">setting value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>false : fail</returns>
        public bool SetDouble(string cmd, double value, string filePath = "") =>
            _fileType switch
            {
                FILE_TYPE.INI => _iniParser.SetDouble(cmd, value, filePath),
                FILE_TYPE.XML => _xmlParser.SetDouble(cmd, value, filePath),
                FILE_TYPE.XAML => _xamlParser.SetDouble(cmd, value, filePath),
                _ => false
            };
        /// <summary>
        /// SetBool("Section,Key");
        /// SetBool("Node,ChildNode,ChildeNode...);
        /// </summary>
        /// <param name="cmd">ref summary</param>
        /// <param name="value">setting value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>false : fail</returns>
        public bool SetBool(string cmd, bool value, string filePath = "") =>
            _fileType switch
            {
                FILE_TYPE.INI => _iniParser.SetBool(cmd, value, filePath),
                FILE_TYPE.XML => _xmlParser.SetBool(cmd, value, filePath),
                FILE_TYPE.XAML => _xamlParser.SetBool(cmd, value, filePath),
                _ => false
            };
    }
}
