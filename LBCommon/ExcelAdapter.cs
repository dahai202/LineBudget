using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using X2Lib.IO;

namespace LineBudget.Common
{
    public class ExcelAdapter
    {
        /// <summary>
        /// 读取excel表单数据
        /// </summary>
        /// <param name="fileInfo">excel文件信息</param>
        /// <param name="currentUser">用户信息</param>
        /// <returns>表格的形式返回</returns>
        public  DataTable GetExcelDataTable(string fielName)
        {
            ///文件信息异常处理
            if (string.IsNullOrEmpty(fielName) || !X2File.IsExist(fielName)) return null;

            ///连接字符串
            string connectionString = "Provider=Microsoft.Ace.OleDb.12.0;" + "Data Source=" + fielName + ";" + "Extended Properties='Excel 12.0;HDR=Yes;IMEX=1';";
            ///97-03版excel
            if (fielName.EndsWith( ".xls"))
            {
                connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Extended Properties=Excel 8.0;" + "data source=" + fielName + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
            }
            OleDbConnection conn = new OleDbConnection(connectionString);
            try
            {
                ///新建内存表单存储excel数据量
                DataTable dt = new DataTable();
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                ///新建内存表单存储
                DataTable dtData = new DataTable();
                ///获取所有的数据
                string strExcel = "select * from [Sheet1$]";
                OleDbDataAdapter adapter = new OleDbDataAdapter(strExcel, conn);
                adapter.Fill(dtData);
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                if (dtData == null || dtData.Rows == null || dtData.Rows.Count <= 1)
                {
                    if (conn.State != ConnectionState.Closed)
                        conn.Close();
                    return null;
                }

                return dtData;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                ///关闭连接
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }
    }
}
