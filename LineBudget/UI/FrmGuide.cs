using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LineBudget.UI
{
    public partial class FrmGuide : Form
    {
        public FrmGuide()
        {
            InitializeComponent();

            DataTable dt = new DataTable();
            dt.Columns.Add("代码");
            dt.Columns.Add("名称");
            dt.Columns.Add("内容");

            for (int i = 0; i < 10; i++)
            {
                DataRow row = dt.NewRow();
                row[0] = "xxxx";
                row[1] = "xxxx项目电力系统";
                row[2] = "xxxx项目电力系统";

                dt.Rows.Add(row);
            }
            //this.dataGridView1.DataSource = dt;
           // this.dataGridViewX1.DataSource = dt;
        }

    }
}
