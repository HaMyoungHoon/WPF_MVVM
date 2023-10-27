namespace WPF_MVVM.Libs
{
    /// <summary>parser interface</summary>
    public interface IFParser
    {
        /// <summary>
        /// Create Ini File
        /// </summary>
        /// <param name="section">Xml : Node, Ini : Root Key</param>
        /// <param name="value">Xml : Value, Ini : None</param>
        /// <param name="filePath">Default File Path</param>
        /// <returns></returns>
        bool CreateFile(string section, string value, string filePath = "");
        /// <summary>
        /// Re Setting File Path, File Type
        /// </summary>
        /// <param name="filePath">Default File Path</param>
        void SetFilePath(string filePath);
        /// <summary>
        /// GetString("Section,Key");
        /// </summary>
        /// <param name="cmd">ref to summary</param>
        /// <param name="defValue">default value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>string type</returns>
        string GetString(string cmd, string defValue, string filePath = "");
        /// <summary>
        /// Getint("Section,Key");
        /// </summary>
        /// <param name="cmd">ref to summary</param>
        /// <param name="defValue">default value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>int type</returns>
        int GetInt(string cmd, int defValue, string filePath = "");
        /// <summary>
        /// GetDouble("Section,Key");
        /// </summary>
        /// <param name="cmd">ref to summary</param>
        /// <param name="defValue">default value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>double type</returns>
        double GetDouble(string cmd, double defValue, string filePath = "");
        /// <summary>
        /// GetBool("Section,Key");
        /// </summary>
        /// <param name="cmd">ref to summary</param>
        /// <param name="defValue">default value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>bool type</returns>
        bool GetBool(string cmd, bool defValue, string filePath = "");
        /// <summary>
        /// SetString("Section,Key");
        /// </summary>
        /// <param name="cmd">ref summary</param>
        /// <param name="data">setting value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>false : fail</returns>
        bool SetString(string cmd, string data, string filePath = "");
        /// <summary>
        /// SetInt("Section,Key");
        /// </summary>
        /// <param name="cmd">ref summary</param>
        /// <param name="data">setting value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>false : fail</returns>
        bool SetInt(string cmd, int data, string filePath = "");
        /// <summary>
        /// SetDouble("Section,Key");
        /// </summary>
        /// <param name="cmd">ref summary</param>
        /// <param name="data">setting value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>false : fail</returns>
        bool SetDouble(string cmd, double data, string filePath = "");
        /// <summary>
        /// SetBool("Section,Key");
        /// </summary>
        /// <param name="cmd">ref summary</param>
        /// <param name="data">setting value</param>
        /// <param name="filePath">None : Construct Path</param>
        /// <returns>false : fail</returns>
        bool SetBool(string cmd, bool data, string filePath = "");
    }
}
