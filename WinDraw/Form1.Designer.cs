namespace WinDraw
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.打开ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpencadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开dwgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.绘图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showpic1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showpic2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showpic3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.axAcCtrl1 = new AxACCTRLLib.AxAcCtrl();
            this.loadarxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axAcCtrl1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(612, 316);
            this.textBox1.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开ToolStripMenuItem,
            this.绘图ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(626, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 打开ToolStripMenuItem
            // 
            this.打开ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpencadToolStripMenuItem,
            this.打开dwgToolStripMenuItem,
            this.loadarxToolStripMenuItem});
            this.打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            this.打开ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.打开ToolStripMenuItem.Text = "打开";
            // 
            // OpencadToolStripMenuItem
            // 
            this.OpencadToolStripMenuItem.Name = "OpencadToolStripMenuItem";
            this.OpencadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.OpencadToolStripMenuItem.Text = "打开cad";
            // 
            // 打开dwgToolStripMenuItem
            // 
            this.打开dwgToolStripMenuItem.Name = "打开dwgToolStripMenuItem";
            this.打开dwgToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.打开dwgToolStripMenuItem.Text = "打开dwg";
            this.打开dwgToolStripMenuItem.Click += new System.EventHandler(this.打开dwgToolStripMenuItem_Click);
            // 
            // 绘图ToolStripMenuItem
            // 
            this.绘图ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showpic1ToolStripMenuItem,
            this.showpic2ToolStripMenuItem,
            this.showpic3ToolStripMenuItem});
            this.绘图ToolStripMenuItem.Name = "绘图ToolStripMenuItem";
            this.绘图ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.绘图ToolStripMenuItem.Text = "绘图";
            // 
            // showpic1ToolStripMenuItem
            // 
            this.showpic1ToolStripMenuItem.Name = "showpic1ToolStripMenuItem";
            this.showpic1ToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.showpic1ToolStripMenuItem.Text = "showpic1";
            // 
            // showpic2ToolStripMenuItem
            // 
            this.showpic2ToolStripMenuItem.Name = "showpic2ToolStripMenuItem";
            this.showpic2ToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.showpic2ToolStripMenuItem.Text = "showpic2";
            // 
            // showpic3ToolStripMenuItem
            // 
            this.showpic3ToolStripMenuItem.Name = "showpic3ToolStripMenuItem";
            this.showpic3ToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.showpic3ToolStripMenuItem.Text = "showpic3";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(626, 348);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(618, 322);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.pictureBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(618, 322);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(612, 316);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.axAcCtrl1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(618, 322);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // axAcCtrl1
            // 
            this.axAcCtrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axAcCtrl1.Enabled = true;
            this.axAcCtrl1.Location = new System.Drawing.Point(3, 3);
            this.axAcCtrl1.Name = "axAcCtrl1";
            this.axAcCtrl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAcCtrl1.OcxState")));
            this.axAcCtrl1.Size = new System.Drawing.Size(612, 316);
            this.axAcCtrl1.TabIndex = 0;
            // 
            // loadarxToolStripMenuItem
            // 
            this.loadarxToolStripMenuItem.Name = "loadarxToolStripMenuItem";
            this.loadarxToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadarxToolStripMenuItem.Text = "loadarx";
            this.loadarxToolStripMenuItem.Click += new System.EventHandler(this.loadarxToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 373);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axAcCtrl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 打开ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpencadToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem 绘图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showpic1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showpic2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showpic3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开dwgToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage3;
        private AxACCTRLLib.AxAcCtrl axAcCtrl1;
        private System.Windows.Forms.ToolStripMenuItem loadarxToolStripMenuItem;

    }
}

