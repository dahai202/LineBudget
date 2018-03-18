using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace X2Lib.Common.Utilities
{
    public class coding
    {
        public static string getCoded(string codeName, string sourcestring)
        {
            return HttpUtility.UrlEncode(sourcestring, Encoding.GetEncoding("gb2312"));
        }
        public static string unCoded(string codeName, string sourcestring)
        {
           
            return HttpUtility.UrlDecode(sourcestring, Encoding.GetEncoding("GBK"));
        }

        public static string enCodeUnicode(string sourcestring)
        {
            UnicodeEncoding unicode = new UnicodeEncoding();
            Byte[] encodedBytes = unicode.GetBytes(sourcestring);

            StreamReader sr = new StreamReader(new MemoryStream(encodedBytes));
            return sr.ReadToEnd();
        
        }

        public static string deCodeUnicode(string sourcestring)
        {
            UnicodeEncoding unicode = new UnicodeEncoding();

            return "";
        
        }


    }
}
