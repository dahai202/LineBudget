using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LineBudget.Common;

using System.IO;
using X2Lib.IO;
//using Autodesk.AutoCAD.Interop;

namespace LineBudget.UI
{
    public partial class AutoCreateCad : UserControl
    {
        public AutoCreateCad()
        {
            InitializeComponent();
        }

        #region 属性
        /// <summary>
        /// excel数据
        /// </summary>
        private DataTable dt;
        #endregion

        #region 桌面事件

        /// <summary>
        /// 选择excel文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            this.openFileDialog2.ShowDialog();
            string fileName = this.openFileDialog2.FileName;
            if (string.IsNullOrEmpty(fileName) || (!fileName.EndsWith(".xls") && !fileName.EndsWith(".xlsl")))
            {
                MessageBox.Show("请选择表示的EXCEL文件");
                return;
            }

            this.textBox2.Text = fileName;
            this.textBox2.Enabled = false;
        }

        /// <summary>
        /// 界面加载数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            ExcelAdapter excel = new ExcelAdapter();
            string fileName = this.textBox2.Text.Trim();
            dt = excel.GetExcelDataTable(fileName);

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("所选文件无数据！");
                return;
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dataGridView2.Columns.Add(i.ToString(), dt.Columns[i].Caption);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dataGridView2.Rows.Add();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    dataGridView2.Rows[i].Cells[j].Value = dt.Rows[i][j];
                }
            }
        }

        /// <summary>
        /// 自动绘制cad图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            #region 暂时删除
            //DxfDocument dxf = new DxfDocument();
            //List<PolyfaceMeshVertex> vertexes = new List<PolyfaceMeshVertex>
            //                                        {
            //                                            new PolyfaceMeshVertex(0, 0, 0),
            //                                            new PolyfaceMeshVertex(10, 0, 0),
            //                                            new PolyfaceMeshVertex(10, 10, 0),
            //                                            new PolyfaceMeshVertex(5, 15, 0),
            //                                            new PolyfaceMeshVertex(0, 10, 0)
            //                                        };
            //List<PolyfaceMeshFace> faces = new List<PolyfaceMeshFace>
            //                                   {
            //                                       new PolyfaceMeshFace(new[] {1, 2, -3}),
            //                                       new PolyfaceMeshFace(new[] {-1, 3, -4}),
            //                                       new PolyfaceMeshFace(new[] {-1, 4, 5})
            //                                   };

            //PolyfaceMesh mesh = new PolyfaceMesh(vertexes, faces);
            //dxf.AddEntity(mesh);
            //dxf.Save("mesh.dxf", DxfVersion.AutoCad2000); 
            #endregion

            string cadFileDir = string.Concat(System.AppDomain.CurrentDomain.BaseDirectory, "TmpFile\\");
            X2Directory.CreatDir(cadFileDir);
            string cadFilePath = string.Concat(cadFileDir, "TmpCadFile.dxf");
            if (X2File.IsExist(cadFilePath)) X2File.DeleteFile(cadFilePath);

            //Polyline(cadFilePath);
            DrawLine();
            MessageBox.Show("绘制完成");
            //string newFilePath = cadFilePath;
            //if (!X2File.IsExist(newFilePath))
            //{
            //    MessageBox.Show("未找到CAD图：" + newFilePath);
            //}
        }

        /// <summary>
        /// 图形预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            //Autodesk.AutoCAD.Interop.AcadApplication AcadApp;
            //Autodesk.AutoCAD.Interop.AcadDocument AcadDoc;
            //AcadApp = new AcadApplication();
            //string cadFileDir = string.Concat(System.AppDomain.CurrentDomain.BaseDirectory, "TmpFile\\");
            //X2Directory.CreatDir(cadFileDir);

            //string filePath = string.Concat(cadFileDir, "TmpCadFile.dxf");
            //if (!X2File.IsExist(filePath))
            //{
            //    MessageBox.Show("未找到CAD图：" + filePath);
            //}
            //AcadApp.Application.Visible = true;
            //AcadDoc = AcadApp.Documents.Open(filePath, null, null);
            //Microsoft.VisualBasic.Interaction.AppActivate(AcadApp.Caption);
        }


        /// <summary>
        /// 图形保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string cadFileDir = string.Concat(System.AppDomain.CurrentDomain.BaseDirectory, "TmpFile\\");
                    X2Directory.CreatDir(cadFileDir);
                    string cadFilePath = string.Concat(cadFileDir, "TmpCadFile.dxf");
                    X2File.CopyFile(this.saveFileDialog1.FileName, cadFilePath);
                    MessageBox.Show("保存完毕！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 开始绘制图形
        /// </summary>
        private void DrawLine()
        {
            //dt = ChangeDt(dt);
            //把颜色改回黑白色
            axMxDrawX1.DrawCADColorIndex = 0;

            axMxDrawX1.OpenDwgFile("1.dwg");

            //把线型改成实线
            axMxDrawX1.LineType = "";

            //设置线宽 4
            axMxDrawX1.LineWidth = 0;

            //创建一个图层,名为"LineLayer"
            axMxDrawX1.AddLayer("LineLayer");

            //设置当前图层为"LineLayer"
            axMxDrawX1.LayerName = "LineLayer";

            double x1 = 0;
            double y1 = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                double x = Convert.ToDouble(row[0]);
                double y = Convert.ToDouble(row[1]);
               
                if (i == 0)
                {
                    x1 = x;
                    y1 = y;
                    //axMxDrawX1.DrawLine(x1, y1, x, y);
                    DrawDwg(x1,y1);
                }
                else
                {
                    //axMxDrawX1.DrawLine(x1,y1,x, y);

                    DrawDwg(x1, y1);

                    x1 = x;
                    y1 = y;
                }
            }

            //axMxDrawX1.OpenDwgFile();

            //把所有的实体都放到当前显示视区
            axMxDrawX1.ZoomAll();

            //更新视区显示
            axMxDrawX1.UpdateDisplay();
        }
        #endregion

        /// <summary>
        /// 根据xy轴的最短距离计算图形
        /// </summary>
        /// <param name="orgDt"></param>
        /// <returns></returns>
        private DataTable ChangeDt(DataTable orgDt)
        {
            if (orgDt == null || orgDt.Rows.Count == 0) return orgDt;

            Dictionary<int, DataRow> allRows = new Dictionary<int, DataRow>();
            Dictionary<int, double> allRowsValue = new Dictionary<int, double>();

            ///构造最小距离值
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                double x = Convert.ToDouble(row[0]);
                double y = Convert.ToDouble(row[1]);

                double valueItem = Math.Sqrt(x * x + y * y);
                allRowsValue.Add(i, valueItem);
            }
            var retValue = from c in allRowsValue
                           orderby c.Value ascending
                           select new { c.Key, c.Value };

            ///根据最小值升序排序数据行
            DataTable retDt = orgDt.Clone();
            foreach (var item in retValue)
            {
                retDt.ImportRow(orgDt.Rows[item.Key]);
            }
            return retDt;
        }

        private void DrawDwg(double x,double y)
        {
            axMxDrawX1.InsertBlock("2.dwg", "TestTree");
            axMxDrawX1.DrawBlockReference(x, y, "TestTree", 1, 0);
        }
    }
}
