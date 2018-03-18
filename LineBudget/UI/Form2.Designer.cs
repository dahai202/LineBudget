namespace LineBudget.UI
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.frmMainP0011 = new LineBudget.UI.FrmMainP001();
            this.SuspendLayout();
            // 
            // frmMainP0011
            // 
            this.frmMainP0011.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmMainP0011.Location = new System.Drawing.Point(0, 0);
            this.frmMainP0011.Name = "frmMainP0011";
            this.frmMainP0011.Size = new System.Drawing.Size(962, 435);
            this.frmMainP0011.TabIndex = 0;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(962, 435);
            this.Controls.Add(this.frmMainP0011);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);

        }

        #endregion

        private FrmMainP001 frmMainP0011;

    }
}