using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Data;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;

namespace X2Lib.Common.Utilities
{

    /// <summary>
    /// json处理类
    /// </summary>
    public static class X2Jason
    {
        /// <summary>
        /// 对象格式转化为json字符串（内存流操作,对象需要加DataContact属性）
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string ToJsonItem(this object item)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(item.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, item);
                StringBuilder sb = new StringBuilder();
                sb.Append(Encoding.UTF8.GetString(ms.ToArray())); return sb.ToString();
            }
        }

        /// <summary>
        /// json字符串转化为对象（内存流操作,对象需要DataContact属性）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T FromJsonTo<T>(this string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));

            T jsonObject = (T)ser.ReadObject(ms);

            return jsonObject;
        }

        /// <summary>
        ///json字符串转化为键值对
        /// </summary>
        /// <param name="jasonString"></param>
        /// <returns></returns>
        public static Dictionary<string,object>GetJasonItems(string jasonString)
        {
            if (string.IsNullOrEmpty(jasonString))
            {
                return new Dictionary<string, object>();
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                jasonString = jasonString.EndsWith(",}")?jasonString.Replace(",}","}"):jasonString;

                Dictionary<string, object> retDic = (Dictionary<string, object>)serializer.DeserializeObject(jasonString);
                return retDic;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// datatable转化为json字符串
        /// </summary>
        /// <param name="dt">data table</param>
        /// <param name="records"> the number of total records </param>
        /// <param name="total">the total page</param>
        /// <param name="page">the current page</param>
        /// <returns></returns>
        public static string GetJsonStringFromDataTable(DataTable dt,int records,int total,int page,int limit,string idFieldName)
        {
            if (dt == null)
            {
                dt = new DataTable();
                records = 0;
                total = 0;
            }

            JsonDataTable jdt = new JsonDataTable(dt, records, total, page, limit, idFieldName);
            return jdt.GetJson();
        }

        #region 新增json字符串转化对象

        /// <summary>
        /// json转对象（为启用 AFAX 的应用程序提供序列化和反序列化功能）
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static object JsonToObj(string json)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.DeserializeObject(json);
            }
            catch (Exception ex)
            {
                throw new Exception("X2Jason.JsonToObj(): " + ex.Message);
            }
        }

        /// <summary>
        /// json转对象（为启用 AFAX 的应用程序提供序列化和反序列化功能）
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static T JsonToObj<T>(string json)
        {
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                throw new Exception("X2Jason.JsonToObj(): " + ex.Message);
            }
        }

        /// <summary> 
        /// 对象转JSON （为启用 AFAX 的应用程序提供序列化和反序列化功能）
        /// </summary> 
        /// <param name="obj">对象</param> 
        /// <returns>JSON格式的字符串</returns> 
        public static string ObjectToJSON(object obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try
            {
                return jss.Serialize(obj);
            }
            catch (Exception ex)
            {
                throw new Exception("X2Jason.ObjectToJSON(): " + ex.Message);
            }
        }
        #endregion

    }

    [DataContract]
    public class JsonRow
    {
        [DataMember]
        public string[] cell;
        [DataMember]
        public string id;
    }

    [DataContract]
    public class JsonDataTable
    {
        [DataMember]
        public int page;     ///the current page
        [DataMember]
        public int total;    ///the total page
        [DataMember]
        public int records;    ///the all records number
        [DataMember]
        public List<JsonRow> rows;

        public JsonDataTable()
        {
        }

        public JsonDataTable(DataTable dt, int _records, int _total, int _page,int _limit,string idFieldName)
        {
            records = _records;
            total = _total;
            page = _page;

            rows = new List<JsonRow>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //get every colum.
                    JsonRow newrow = new JsonRow();
                    newrow.id = (i + (page - 1) * _limit).ToString();
                    List<string> row = new List<string>();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (idFieldName == dt.Columns[j].ColumnName)
                        {
                            newrow.id = dt.Rows[i][j].ToString();
                        }
                        row.Add(dt.Rows[i][j].ToString());
                    }

                    newrow.cell = row.ToArray();

                    rows.Add(newrow);
                    

                }
            }

        }

        public string GetJson()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JsonDataTable));
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                serializer.WriteObject(ms, this);
                ms.Position = 0;
                System.IO.StreamReader sr = new System.IO.StreamReader(ms);
                return sr.ReadToEnd();
            }
        }
    }

}
