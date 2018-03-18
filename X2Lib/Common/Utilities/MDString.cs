using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace X2Lib.Common.Utilities
{
    /// <summary>
    /// 正则表达式判断格式
    /// </summary>
    public static class MDString
    {
        /// <summary>
        /// replace a tag with length parmeter
        /// </summary>
        /// <param name="TargetString">the target string</param>
        /// <param name="SourceString">the word will be replaced in the target string</param>
        /// <param name="TagTemplate">the tag template such as SOURCE</param>
        /// <param name="LenthPrex">THE PRE LENGTH SUCH : ,WILL FOLLOWED BY NUMBER</param>
        /// <returns></returns>
        public static string ReplaceTagWithLenth(string TargetString, string SourceString, string TagKey)
        {
            string patten = @"\[" + TagKey + @"(?<LENRESULT>[0-9_]*)\]";
            Regex r = new Regex(patten, RegexOptions.IgnoreCase);
            Match m = r.Match(TargetString);
            while (m.Success)
            {
                int len = -1;
                try
                {
                    len = Convert.ToInt16(m.Result("${LENRESULT}"));
                }
                catch (Exception)
                {
                    len = -1;
                }

                string targetWords = "[" + TagKey + "]";
                if (len != -1)
                {
                    targetWords = "[" + TagKey + ":" + len.ToString() + "]";
                }

                if (len >= SourceString.Length)
                {
                    len = SourceString.Length;
                }

                TargetString = TargetString.Replace(targetWords, SourceString.Substring(0, len));
            }

            return TargetString;
        }

        /// <summary>
        /// get the Tag strings
        /// </summary>
        /// <param name="targetString">e.g. this is a test <X2QL>DDDDD</X2QL>></param>
        /// <param name="tagKey"></param>
        /// <returns></returns>
        public static List<string> GetStringListByTag(string targetString, string TagStr)
        {
            List<string> retList = new List<string>();
            string MatchPattern = @"<" + TagStr + @">(?<X2Result>.*)<\/" + TagStr + ">";
            Regex r = new Regex(MatchPattern, RegexOptions.IgnoreCase);
            Match m = r.Match(targetString);
            while (m.Success)
            {
                string result = m.Result("${X2Result}");
                if (!string.IsNullOrEmpty(result))
                {
                    retList.Add(result);
                }
                m = m.NextMatch();
            }

            return retList;
        }

        /// <summary>
        /// get the Tag strings
        /// </summary>
        /// <param name="targetString">e.g. this is a test 
        /// <param name="tagKey"></param>
        /// <returns></returns>
        public static string ReplaceTagContent(string targetString, string targetTagContent, string newTagContent, string tagKey)
        {
            if (string.IsNullOrEmpty(targetString))
            {
                return targetString;
            }

            return targetString = targetString.Replace("<" + tagKey + ">" + targetTagContent + "</" + tagKey + ">", newTagContent);
        }

        /// <summary>
        ///是否为电话号码
        /// </summary>
        /// <param name="numberString"></param>
        /// <param name="digitalNumbers"></param>
        /// <returns></returns>
        public static bool IsCellNumber(string numberString, int digitalNumbers)
        {
            if (string.IsNullOrEmpty(numberString))
            {
                return false;
            }
            return Regex.IsMatch(numberString, "[0-9]{" + digitalNumbers.ToString() + "}");
        }

        /// <summary>
        ///是否为数字
        /// </summary>
        /// <param name="numberString"></param>
        /// <param name="digitalNumbers"></param>
        /// <returns></returns>
        public static bool IsNumber(string numberString)
        {
            if (string.IsNullOrEmpty(numberString))
            {
                return false;
            }

            return Regex.IsMatch(numberString, "^[0-9.]*$");
        }

        /// <summary>
        /// 是否为整数
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsInteger(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            return Regex.IsMatch(str, @"^\d+$");
        }

        /// <summary>
        /// 是否为邮件地址
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public static bool IsEmail(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return false;
            }

            string patten = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+(?:[a-zA-Z0-9]{3})";
            return Regex.IsMatch(emailAddress, patten);
        }

        /// <summary>
        /// 是否为邮件地址
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public static bool IsEmailFormat(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return false;
            }

            string patten = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            return Regex.IsMatch(emailAddress, patten);
        }

        public static string ReplaceFirst(string origString, string existString, string newString)
        {
            if (string.IsNullOrEmpty(origString) || string.IsNullOrEmpty(existString)) return origString;

            int firstIndex = origString.IndexOf(existString);
            if (firstIndex < 0) return origString;

            origString = origString.Remove(firstIndex, existString.Length);
            origString = origString.Insert(firstIndex, newString);
            return origString;
        }

        /// <summary>
        /// 清除html
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public static string ClearHtml(string contrainHtmlStr)
        {
            if (string.IsNullOrEmpty(contrainHtmlStr))
            {
                return "";
            }

            //发送信件时，清除html
            Regex htmlReg = new Regex(@"<[^>]+>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return htmlReg.Replace(contrainHtmlStr, "");

        }

        /// <summary>
        /// 当字符串是null 或者空格或者多个空格返回true
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
