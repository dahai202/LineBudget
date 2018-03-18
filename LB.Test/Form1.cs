using DevComponents.DotNetBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LB.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            eTabStripStyle style = eTabStripStyle.Office2003;
            SetTabStyle(style);

        }

        private void SetTabStyle(eTabStripStyle style)
        {
            tabControl1.Style = style;
            foreach (TabItem tab in tabControl1.Tabs)
                tab.PredefinedColor = eTabItemColor.Default;
        }
    }
}
