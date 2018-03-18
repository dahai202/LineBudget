using DevComponents.DotNetBar;
using LineBudget.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LineBudget
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.superTabControl1.Visible = false;
        }

        private void buttonItem32_Click(object sender, EventArgs e)
        {

        }

        private void buttonItem36_Click(object sender, EventArgs e)
        {

        }

        private void buttonItem2_Click(object sender, EventArgs e)
        {
            Form1 doc = new Form1();
            //frmDocument doc = new frmDocument();
            doc.MdiParent = this;
             doc.Dock = DockStyle.Fill;
            doc.WindowState = FormWindowState.Maximized;
            doc.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            doc.TopLevel = false;
            doc.Show();
            doc.Update();
            doc.Text = "New Document " + this.MdiChildren.Length.ToString();
        }
    }
}
