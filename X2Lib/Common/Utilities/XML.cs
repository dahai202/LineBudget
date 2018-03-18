using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using X2Lib.IO;
using System.Data;

namespace X2Lib.Common.Utilities
{
    public class XML
    {
        System.Xml.XmlDocument _xmlDoc;
        public XML()
        { 
        
        }
        public XML(string xmlSource)
        {
            _xmlDoc = new System.Xml.XmlDocument();
            try
            {
                _xmlDoc.LoadXml(xmlSource);
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }

        public string getFirstNodeValue(string xpath)
        { 
           System.Xml.XmlNode node = _xmlDoc.SelectSingleNode(xpath);
           return node.InnerText;
        }

        #region xml对象 序列化处理

        /// <summary>
        /// xml 读取文件（直接转化为对象）
        /// </summary>
        public static T XmlLoadFile<T>(string xmlFilePath)
        {
            //string xmlContent = X2File.ReadFile(xmlFilePath);
            //Object obj2 = XmlDeserialize<T>(xmlContent);

            FileStream fs = null;
            XmlSerializer xs = new XmlSerializer(typeof(T));
            fs = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read);
            Object obj = (T)xs.Deserialize(fs);
            fs.Close();
            return (T)obj; ;
        }

        /// <summary>
        /// xml文件的保存（对象直接保存为文件）
        /// </summary>
        public static void XmlSave<T>(string xmlPath, T obj)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            Stream stream = new FileStream(xmlPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            xs.Serialize(stream, obj);
            stream.Close();
        }

        /// <summary>
        /// XML String 反序列化成对象
        /// </summary>
        public static T XmlDeserialize<T>(string xmlString)
        {
            T t = default(T);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {
                using (XmlReader xmlReader = XmlReader.Create(xmlStream))
                {
                    Object obj = xmlSerializer.Deserialize(xmlReader);
                    t = (T)obj;
                }
            }
            return t;
        }

        /// <summary>
        /// 对象序列化成 XML String
        /// </summary>
        public static string XmlSerialize<T>(T obj)
        {
            string xmlString = string.Empty;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                xmlSerializer.Serialize(ms, obj);
                xmlString = Encoding.UTF8.GetString(ms.ToArray());
            }
            return xmlString;
        }

        /// <summary> 
        /// 将DataTable对象转换成XML字符串 
        /// </summary> 
        /// <param name="dt">DataTable对象</param> 
        /// <returns>XML字符串</returns> 
        public static string DataToXml(DataTable dt)
        {
            if (dt != null)
            {
                MemoryStream ms = null;
                XmlTextWriter XmlWt = null;
                try
                {
                    ms = new MemoryStream();
                    //根据ms实例化XmlWt 
                    XmlWt = new XmlTextWriter(ms, Encoding.Unicode);
                    //获取ds中的数据 
                    dt.WriteXml(XmlWt);
                    int count = (int)ms.Length;
                    byte[] temp = new byte[count];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Read(temp, 0, count);
                    //返回Unicode编码的文本 
                    UnicodeEncoding ucode = new UnicodeEncoding();
                    return ucode.GetString(temp).Trim();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //释放资源 
                    if (XmlWt != null)
                    {
                        ms.Close();
                        ms.Dispose();
                    }
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary> 
        /// 将Xml内容字符串转换成DataTable对象 
        /// </summary> 
        /// <param name="xmlStr">Xml内容字符串</param> 
        /// <returns>DataTable对象</returns> 
        public static DataTable XmlToDataTable(string xmlStr)
        {
            if (!string.IsNullOrEmpty(xmlStr))
            {
                StringReader StrStream = null;
                XmlTextReader Xmlrdr = null;
                try
                {
                    DataSet ds = new DataSet();
                    //读取字符串中的信息 
                    StrStream = new StringReader(xmlStr);
                    //获取StrStream中的数据 
                    Xmlrdr = new XmlTextReader(StrStream);
                    //ds获取Xmlrdr中的数据 
                    ds.ReadXml(Xmlrdr);
                    return ds.Tables[0];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //释放资源 
                    if (Xmlrdr != null)
                    {
                        Xmlrdr.Close();
                        StrStream.Close();
                        StrStream.Dispose();
                    }
                }
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
