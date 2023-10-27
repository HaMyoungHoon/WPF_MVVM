using System.Security.Cryptography;
using System.Text;

namespace WPF_MVVM.Libs
{
    public class FAES128
    {
        public static string Encrypt(string data, string key = "6574852065748520")
        {
            if (data == string.Empty)
            {
                return string.Empty;
            }

            while (key.Length < 16)
            {
                key += "0";
            }
            while (key.Length > 16)
            {
                key = key[..16];
            }

            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] dataArray = Encoding.UTF8.GetBytes(data);

            using var aes = Aes.Create();
            if (aes == null)
            {
                return string.Empty;
            }

            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = keyArray;

            ICryptoTransform trans = aes.CreateEncryptor();
            byte[] ret = trans.TransformFinalBlock(dataArray, 0, dataArray.Length);

            return FConverter.HexByteToHexStr(ret);
        }
        public static string Decrypt(string data, string key = "6574852065748520")
        {
            if (data == string.Empty)
            {
                return string.Empty;
            }

            while (key.Length < 16)
            {
                key += "0";
            }
            while (key.Length > 16)
            {
                key = key[..16];
            }

            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] dataArray = FConverter.HexStrToByte(data);

            using var aes = Aes.Create();
            if (aes == null)
            {
                return string.Empty;
            }

            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = keyArray;

            ICryptoTransform trans = aes.CreateDecryptor();
            byte[] ret = trans.TransformFinalBlock(dataArray, 0, dataArray.Length);

            return Encoding.UTF8.GetString(ret).Replace("\0", "");
        }
    }
}
