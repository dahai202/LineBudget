using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Web;
using X2Lib.X2Sys;
using System.IO;


namespace X2Lib.Common.Encryption
{
    /// <summary>
    /// X2 标准加密算法
    /// </summary>
    public class X2StandardEncryption
    {
        /// <summary>
        /// 随机数生成
        /// </summary>
        static Random rand = new Random();

        #region Base64加密

        public string Base64Encrypt(string pToEncrypt)
        {
            return Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(pToEncrypt));
        }
        #endregion

        #region Base64编码
        public string Base64Decrypt(string pToDeCrypt)
        {
            return System.Text.Encoding.Default.GetString(Convert.FromBase64String(pToDeCrypt));
        }
        #endregion

        #region MD5加密
        public static string Encrypt_MD5_Standard(string strpwd)
        {
            if (string.IsNullOrEmpty(strpwd))
            {
                return "";
            }

            MD5 MD5 = new MD5CryptoServiceProvider();
            byte[] datSource = System.Text.Encoding.Default.GetBytes(strpwd);
            byte[] newSource = MD5.ComputeHash(datSource);
            string byte2String = null;
            for (int i = 0; i < newSource.Length; i++)
            {
                string thisByte = newSource[i].ToString("x");
                if (thisByte.Length == 1) thisByte = "0" + thisByte;
                byte2String += thisByte;
            }
            return byte2String;
        }
        #endregion

        #region Passport 加密函数
        /// <summary>
        ///  Passport 加密函数
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Passport_Encrypt(string txt, string key)
        {
            Random ran = new Random();
            // 使用随机数发生器产生 0~32000 的值并 MD5()
            string encrypt_key = Encrypt_MD5_Standard(ran.Next(0, 32000).ToString());
            // 变量初始化
            int ctr = 0;
            string tmp = "";

            // for 循环，$i 为从 0 开始，到小于 $txt 字串长度的整数
            for (int i = 0; i < txt.Length; i++)
            {
                // 如果 $ctr = $encrypt_key 的长度，则 $ctr 清零
                ctr = ctr == encrypt_key.Length ? 0 : ctr;
                // $tmp 字串在末尾增加一位，其内容为 $txt 的第 $i 位，
                // 与 $encrypt_key 的第 $ctr + 1 位取异或。然后 $ctr = $ctr + 1
                tmp += encrypt_key[ctr].ToString() + ((char)(txt[i] ^ encrypt_key[ctr++])).ToString();
            }
            // 返回结果，结果为 passport_key() 函数返回值的 base65 编码结果
            return Base64Encrypt(Passport_Key(tmp, key));

        }
        #endregion

        #region Passport 解密函数
        /// <summary>
        /// Passport 解密函数
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Passport_Decrypt(string txt, string key)
        {
            // $txt 的结果为加密后的字串经过 base64 解码，然后与私有密匙一起，
            // 经过 passport_key() 函数处理后的返回值
            txt = Passport_Key(Base64Decrypt(txt), key);

            // 变量初始化
            string tmp = "";

            // for 循环，$i 为从 0 开始，到小于 $txt 字串长度的整数
            for (int i = 0; i < txt.Length; i++)
            {
                // $tmp 字串在末尾增加一位，其内容为 $txt 的第 $i 位，
                // 与 $txt 的第 $i + 1 位取异或。然后 $i = $i + 1
                tmp += ((char)(txt[i] ^ txt[++i])).ToString();
            }


            // 返回 $tmp 的值作为结果
            return tmp;

        }
        #endregion

        #region Passport 密匙处理函数
        public string Passport_Key(string txt, string encrypt_key)
        {
            // 将 $encrypt_key 赋为 $encrypt_key 经 md5() 后的值
            encrypt_key = Encrypt_MD5_Standard(encrypt_key);
            // 变量初始化
            int ctr = 0;
            string tmp = "";

            // for 循环，$i 为从 0 开始，到小于 $txt 字串长度的整数
            for (int i = 0; i < txt.Length; i++)
            {
                // 如果 $ctr = $encrypt_key 的长度，则 $ctr 清零
                ctr = ctr == encrypt_key.Length ? 0 : ctr;
                // $tmp 字串在末尾增加一位，其内容为 $txt 的第 $i 位，
                // 与 $encrypt_key 的第 $ctr + 1 位取异或。然后 $ctr = $ctr + 1
                tmp += ((char)(txt[i] ^ encrypt_key[ctr++])).ToString();
            }
            return tmp;
        }
        #endregion

        #region 可逆编码的解码

        /// <summary>
        /// 可逆编码的解码
        /// </summary>
        /// <param name="str"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string DecodeAscii(string str, int num)
        {
            try
            {
                string dtext = "";
                for (int i = 0; i < str.Length; i += 2)
                {
                    dtext += AsciiToChar(int.Parse(str.Substring(i, 2)) + num);
                }
                ///部分特殊字符需要解码
                return HttpUtility.UrlDecode(dtext);
            }
            catch (Exception ex)
            {
                X2Error.Log("DecodeAscii_Error", ex, string.Format("{0} Encryption error", str));
                return "";
            }
        }

        /// <summary>
        /// ASCII码转字符
        /// </summary>
        /// <param name="asciiCode"></param>
        /// <returns></returns>
        private static string AsciiToChar(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }
        }

        /// <summary>
        /// 字符转ASCII码
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private static int CharToAscii(string character)
        {
            if (character.Length == 1)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                int intAsciiCode = (int)asciiEncoding.GetBytes(character)[0];
                return (intAsciiCode);
            }
            else
            {
                throw new Exception("Character is not valid.");
            }
        }

        /// <summary>
        /// 可逆加密算法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncodeAscii(string str)
        {
            string num_out = "";
            str = HttpUtility.UrlEncodeUnicode(str);
            for (int i = 0; i < str.Length; i++)
            {
                num_out += str[i] - 23;
            }
            return num_out;
        }

        #region 备注js的编码和解码
        //   function str_to_num(str_in) {
        //num_out = "";
        //str_in = escape(str_in);
        //for(i = 0; i < str_in.length; i++) {
        //num_out += str_in.charCodeAt(i) - 20;
        //}
        //return num_out;
        //}
        //function num_to_str(num_out) {
        //str_out = "";

        //for(i = 0; i < num_out.length; i += 2) {
        //var num_in = parseInt(num_out.substr(i,[2])) + 20;
        //num_in = unescape('%' + num_in.toString(16));
        //str_out += num_in;
        //}
        //return  unescape(str_out);  
        //}
        #endregion

        #endregion

        #region DES加解密

        /// <summary>
        /// 随机生成64位密钥
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string CreateKey()
        {
            byte[] result = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                result[i] = (byte)rand.Next(1, 256);
            }
            return Convert.ToBase64String(result);
        }

        /// <summary>  
        /// DES加密算法必须使用Base64的Byte对象  
        /// </summary>  
        /// <param name="data">待加密的字符数据</param>  
        /// <param name="key">密匙，长度必须为64位（byte[8]）)</param>  
        /// <returns>加密后的字符</returns>  
        public static string EnDES(string data, string key)
        {
            byte[] iv = new byte[] { 1, 9, 8, 9, 1, 1, 1, 4 };
            byte[] byteKey;
            try
            {
                byteKey = Convert.FromBase64String(key);
            }
            catch
            {
                throw new Exception("secret key format error");
            }
            DES des = DES.Create();
            byte[] encryptoData;
            ICryptoTransform encryptor = des.CreateEncryptor(byteKey, iv);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (var cs = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(cs))
                    {
                        writer.Write(data);
                        writer.Flush();
                    }
                }
                encryptoData = memoryStream.ToArray();
            }
            des.Clear();
            return Convert.ToBase64String(encryptoData);
        }

        /// <summary>  
        /// DES解密,若解密失败则返回string.Empty
        /// </summary>  
        /// <param name="data">待加密的字符数据</param>  
        /// <param name="key">密匙，长度必须为64位（byte[8]）)</param> 
        /// <returns>加密后的字符</returns>  
        public static string DeDes(string data, string key)
        {
            string resultData = string.Empty;
            byte[] iv = new byte[] { 1, 9, 8, 9, 1, 1, 1, 4 };
            byte[] byteKey;
            byte[] tmpData;
            try
            {
                byteKey = Convert.FromBase64String(key);
                tmpData = Convert.FromBase64String(data);
            }
            catch
            {
                return string.Empty;
            }
            DES des = DES.Create();
            ICryptoTransform decryptor = des.CreateDecryptor(byteKey, iv);
            using (var memoryStream = new MemoryStream(tmpData))
            {
                using (var cs = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    StreamReader reader = new StreamReader(cs);
                    resultData = reader.ReadLine();
                }
            }
            return resultData;
        }
        #endregion
    }
}






