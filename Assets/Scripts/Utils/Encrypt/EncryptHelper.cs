using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace EncryptHelper
{
    /// <summary>
    /// 加解密帮助类
    /// </summary>
    public class EncryptHelper
    {
        //DES用于加密内容较多的敏感信息
        //AES用于加密内容较少强度较高的信息

        /// <summary>
        /// DES加密方法
        /// </summary>
        /// <param name="value">待加密的字符串</param>
        /// <param name="key">8/16位密钥</param>
        /// <returns></returns>
        public static string DesEncrypt(string value, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(value);
            des.Key = Encoding.ASCII.GetBytes(key);
            des.IV = Encoding.ASCII.GetBytes(key);
            //创建其支持存储区为内存的流
            MemoryStream ms = new MemoryStream();
            //将数据流链接到加密转换的流
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            //用缓冲区的当前状态更新基础数据源或储存库，随后清除缓冲区
            cs.FlushFinalBlock();
            byte[] EncryptData = ms.ToArray();
            return Convert.ToBase64String(EncryptData, 0, EncryptData.Length);
        }
        /// <summary>
        /// DES解密方法
        /// </summary>
        /// <param name="value">需要解密的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns></returns> 
        public static string DesDecrypt(string value, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //Put  the  input  string  into  the  byte  array 
            byte[] inputByteArray = Convert.FromBase64String(value);
            //建立加密对象的密钥和偏移量
            des.Key = Encoding.ASCII.GetBytes(key);
            des.IV = Encoding.ASCII.GetBytes(key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            //Flush  the  data  through  the  crypto  stream  into  the  memory  stream 
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }
        /// <summary>
        /// Aes加密
        /// </summary>
        /// <param name="value">源字符串</param>
        /// <param name="key">aes密钥，长度必须32位</param>
        /// <returns>加密后的字符串</returns>
        public static string AESEncrypt(string value, string key)
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = Encoding.UTF8.GetBytes(key);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor())
                {
                    byte[] inputBuffers = Encoding.UTF8.GetBytes(value);
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    aesProvider.Dispose();
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }
        /// <summary>
        /// Aes解密
        /// </summary>
        /// <param name="value">源字符串</param>
        /// <param name="key">aes密钥，长度必须32位</param>
        /// <returns>解密后的字符串</returns>
        public static string AESDecrypt(string value, string key)
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = Encoding.UTF8.GetBytes(key);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor())
                {
                    byte[] inputBuffers = Convert.FromBase64String(value);
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    return Encoding.UTF8.GetString(results);
                }
            }
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="value">需要加密字符串</param>
        /// <returns>返回32位大写字符</returns>
        public static string MD5Encrypt(string value)
        {
            //将输入字符串转换成字节数组  ANSI代码页编码
            var buffer = Encoding.Default.GetBytes(value);
            //接着，创建Md5对象进行散列计算
            var data = MD5.Create().ComputeHash(buffer);
            //创建一个新的Stringbuilder收集字节
            var sb = new StringBuilder();
            //遍历每个字节的散列数据 
            foreach (var t in data)
            {
                //转换大写十六进制字符串
                sb.Append(t.ToString("X2"));
            }
            //返回十六进制字符串
            return sb.ToString();
        }
        /// <summary>  
        /// SHA1加密
        /// </summary>  
        /// <param name="value">需要加密字符串</param>
        /// <returns>返回40位大写字符串</returns>  
        public static string SHA1(string value)
        {
            //UTF8编码
            var buffer = Encoding.UTF8.GetBytes(value);
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            var data = sha1.ComputeHash(buffer);
            var sb = new StringBuilder();
            foreach (var t in data)
            {
                //转换大写十六进制
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="code_type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string EncodeBase64(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="code_type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string DecodeBase64(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
        /// <summary>
        /// SQL注入字符清理
        /// </summary>
        /// <param name="value">需要清理的字符串</param>
        /// <returns></returns>
        public static string SqlTextClear(string value)
        {
            string[] replaceStr = new string[] { ",", "<", ">", "--", "'", "\"", "=", "%", " " };
            foreach (var item in replaceStr)
            {
                value = value.Replace(item, "");
            }
            return value;
        }
        /// <summary>
        /// 替换特殊字符，防SQL注入
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceSQLChar(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            str = str.Replace("'", "");
            str = str.Replace(";", "");
            str = str.Replace(",", "");
            str = str.Replace("?", "");
            str = str.Replace("<", "");
            str = str.Replace(">", "");
            str = str.Replace("(", "");
            str = str.Replace(")", "");
            str = str.Replace("@", "");
            str = str.Replace("=", "");
            str = str.Replace("+", "");
            str = str.Replace("*", "");
            str = str.Replace("&", "");
            str = str.Replace("#", "");
            str = str.Replace("%", "");
            str = str.Replace("$", "");

            //删除与数据库相关的词
            str = Regex.Replace(str, "select", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "insert", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "delete from", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "count", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "drop table", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "truncate", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "asc", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "mid", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "char", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "xp_cmdshell", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "exec master", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "net localgroup administrators", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "and", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "net user", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "or", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "net", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "-", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "delete", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "drop", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "script", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "update", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "and", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "chr", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "master", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "truncate", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "declare", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, "mid", "", RegexOptions.IgnoreCase);

            return str;
        }
    }
}