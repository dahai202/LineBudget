using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MxDrawXLib;

namespace LineBudget.UI
{
    public partial class FrmMainP001 : UserControl
    {
        public FrmMainP001()
        {
            try
            {
                InitializeComponent();
                string dwgFileName = string.Format("{0}\\1.dwg", System.AppDomain.CurrentDomain.BaseDirectory);
                if (System.IO.File.Exists(dwgFileName))
                    axMxDrawX1.OpenDwgFile(dwgFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                X2Lib.X2Sys.X2Error.Log(ex.Message,ex,string.Format("msg:{0},detail:{1}", ex.Message, ex.StackTrace));
            }
        }

        // 提示鼠标双击下被点击的实体
        private void axMxDrawX1_MouseEvent(object sender, AxMxDrawXLib._DMxDrawXEvents_MouseEventEvent e)
        {
            //"lType 是事件类型，1鼠标移动，2是鼠标左键按下，3是鼠标右键按下，4是鼠标左键双击.lRet 返回非0，消息将不在往下传递"
            //选择后
            if (e.lType == 5)
            {
                // 构建选择集，找到鼠标左建双击下的实体。
                MxDrawSelectionSet ssGet = new MxDrawSelectionSet();
                //构造选择集 
                ssGet.Select(MCAD_McSelect.mcSelectionSetUserSelect, null, null);//MCAD_McSelect构造选择集方式 
                if (ssGet.Count > 0)
                {
                    //contextMenuStrip1.Show(Control.MousePosition.X, Control.MousePosition.Y);
                    DataGridBindDataTable(this.dataGridViewX1, RetTable());
                    DataGridBindDataTable(this.dataGridViewX2, RetDetailTable());
                    MessageBox.Show(ssGet.Count + "个图块被选择");
                    axMxDrawX1.Focus();
                }
            }
            else if (e.lType == 6)
            {
                //控件中的选择集构造管理器,用图面上的实体搜索，与用户交互选择等操作
                MxDrawSelectionSet ssGet = new MxDrawSelectionSet();
                //构造选择集 
                ssGet.Select(MCAD_McSelect.mcSelectionSetUserSelect, null, null);//MCAD_McSelect构造选择集方式 
                if (ssGet.Count > 0)
                {
                    //contextMenuStrip1.Show(Control.MousePosition.X, Control.MousePosition.Y);
                    axMxDrawX1.Focus();
                }
            }
        }

        /// <summary>
        /// 构造假数据
        /// </summary>
        /// <returns></returns>
        private DataTable RetTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PropertyName");
            dt.Columns.Add("Value");

            for (int i = 0; i < 5; i++)
            {
                DataRow row = dt.NewRow();
                row[0] = "类型";
                row[1] = "新建10KV线路";
                dt.Rows.Add(row);
            }
            return dt;
        }

        /// <summary>
        /// 构造假数据
        /// </summary>
        /// <returns></returns>
        private DataTable RetDetailTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PropertyName");
            dt.Columns.Add("Value");
            dt.Columns.Add("3");
            dt.Columns.Add("4");
            dt.Columns.Add("5");
            dt.Columns.Add("6");
            dt.Columns.Add("7");
            dt.Columns.Add("8");
            dt.Columns.Add("9");
            dt.Columns.Add("10");
            dt.Columns.Add("1");
            dt.Columns.Add("12");

            for (int i = 0; i < 5; i++)
            {
                DataRow row = dt.NewRow();
                row[0] = "类型";
                row[1] = "钢芯铝绞线";
                row[2] = "1";
                row[3] = "m";
                row[4] = "0";
                row[5] = "乙供";
                row[6] = "";
                row[7] = "NW240004";
                row[8] = "5003";
                row[9] = "";
                row[10] = "";
                row[11] = "";
                dt.Rows.Add(row);
            }
            return dt;
        }

        /// <summary>
        /// datagridview绑定datatable
        /// </summary>
        /// <param name="dw"></param>
        /// <param name="dt"></param>
        private void DataGridBindDataTable(DataGridView dw, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;
            if (dw == null) return;
            if (dw.Rows.Count > 0) dw.Rows.Clear();

            foreach (DataRow item in dt.Rows)
            {
                int index = dw.Rows.Add();
                DataGridViewRow row = dw.Rows[index];

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    row.Cells[i].Value = item[i];
                }
                //foreach (DataColumn  colItem in dt.Columns)
                //{
                //}
            }
            //dw.DataSource = dt;
            DataRowView drv = (DataRowView)dw.CurrentRow.DataBoundItem;
        }

        private void dView_employee_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.dataGridViewX2.RowHeadersWidth = 12;
            for (int i = 0; i < dataGridViewX2.Rows.Count; i++)
            {
                int j = i + 1;
                dataGridViewX2.Rows[i].HeaderCell.Value = j.ToString();
            }
        }
    }
}
