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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
      
        /// <summary>
        /// 新建工程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonItem32_Click(object sender, EventArgs e)
        {
            try
            {
                Form cadForm = new Form();
                AutoCreateCad cad = new AutoCreateCad();
                cad.Dock = System.Windows.Forms.DockStyle.Fill;
                cad.Location = new System.Drawing.Point(0, 0);
                cad.Name = "frmMainP0011";
                cad.Size = new System.Drawing.Size(962, 435);
                cad.TabIndex = 0;
                cad.Show();

                cadForm.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
                cadForm.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                cadForm.ClientSize = new System.Drawing.Size(962, 435);
                cadForm.Controls.Add(cad);
                cadForm.Name = "Form2";
                cadForm.Text = "Form2";

                this.ResumeLayout(false);

                cadForm.MdiParent = this;
                cadForm.WindowState = FormWindowState.Maximized;
                cadForm.Show();
                cadForm.Update();
                cadForm.Text = "New Document " + this.MdiChildren.Length.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonItem36_Click(object sender, EventArgs e)
        {

        }

        private void buttonItem2_Click(object sender, EventArgs e)
        {
            try
            {
                LineBudget.UI.Form2 Form2 = new LineBudget.UI.Form2();
                //Form1 doc = new Form1();
                //frmDocument doc = new frmDocument();
                Form2.MdiParent = this;
                //doc.Dock = DockStyle.Fill;
                Form2.WindowState = FormWindowState.Maximized;
                //doc.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
               // doc.TopLevel = false;
                Form2.Show();
                Form2.Update();
                Form2.Text = "New Document " + this.MdiChildren.Length.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
