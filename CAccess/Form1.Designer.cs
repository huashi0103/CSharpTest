namespace Mine
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frm1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frm2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frm3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(906, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.frm1ToolStripMenuItem,
            this.frm2ToolStripMenuItem,
            this.frm3ToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // frm1ToolStripMenuItem
            // 
            this.frm1ToolStripMenuItem.Name = "frm1ToolStripMenuItem";
            this.frm1ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.frm1ToolStripMenuItem.Text = "frm1";
            // 
            // frm2ToolStripMenuItem
            // 
            this.frm2ToolStripMenuItem.Name = "frm2ToolStripMenuItem";
            this.frm2ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.frm2ToolStripMenuItem.Text = "frm2";
            // 
            // frm3ToolStripMenuItem
            // 
            this.frm3ToolStripMenuItem.Name = "frm3ToolStripMenuItem";
            this.frm3ToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.frm3ToolStripMenuItem.Text = "frm3";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 596);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem frm1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem frm2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem frm3ToolStripMenuItem;

    }
}

