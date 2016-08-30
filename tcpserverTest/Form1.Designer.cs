namespace tcpserverTest
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
            this.label2 = new System.Windows.Forms.Label();
            this.NUMPort = new System.Windows.Forms.NumericUpDown();
            this.btnlisten = new System.Windows.Forms.Button();
            this.btnCls = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comport = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comb = new System.Windows.Forms.ComboBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.txComm = new System.Windows.Forms.TextBox();
            this.btnComSend = new System.Windows.Forms.Button();
            this.txData = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.NUMPort)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "端口";
            // 
            // NUMPort
            // 
            this.NUMPort.Location = new System.Drawing.Point(47, 8);
            this.NUMPort.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NUMPort.Name = "NUMPort";
            this.NUMPort.Size = new System.Drawing.Size(120, 21);
            this.NUMPort.TabIndex = 4;
            this.NUMPort.Value = new decimal(new int[] {
            8899,
            0,
            0,
            0});
            // 
            // btnlisten
            // 
            this.btnlisten.Location = new System.Drawing.Point(173, 6);
            this.btnlisten.Name = "btnlisten";
            this.btnlisten.Size = new System.Drawing.Size(75, 23);
            this.btnlisten.TabIndex = 5;
            this.btnlisten.Text = "开启监听";
            this.btnlisten.UseVisualStyleBackColor = true;
            this.btnlisten.Click += new System.EventHandler(this.btnlisten_Click);
            // 
            // btnCls
            // 
            this.btnCls.Location = new System.Drawing.Point(254, 6);
            this.btnCls.Name = "btnCls";
            this.btnCls.Size = new System.Drawing.Size(75, 23);
            this.btnCls.TabIndex = 6;
            this.btnCls.Text = "关闭";
            this.btnCls.UseVisualStyleBackColor = true;
            this.btnCls.Click += new System.EventHandler(this.btnCls_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(2, 58);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(595, 149);
            this.textBox1.TabIndex = 7;
            // 
            // comport
            // 
            this.comport.FormattingEnabled = true;
            this.comport.Location = new System.Drawing.Point(47, 213);
            this.comport.Name = "comport";
            this.comport.Size = new System.Drawing.Size(120, 20);
            this.comport.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 216);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "串口";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(184, 216);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "波特率";
            // 
            // comb
            // 
            this.comb.FormattingEnabled = true;
            this.comb.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "115200"});
            this.comb.Location = new System.Drawing.Point(231, 213);
            this.comb.Name = "comb";
            this.comb.Size = new System.Drawing.Size(120, 20);
            this.comb.TabIndex = 11;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(357, 211);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 12;
            this.btnOpen.Text = "打开";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(438, 211);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 14;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txComm
            // 
            this.txComm.Location = new System.Drawing.Point(2, 239);
            this.txComm.Multiline = true;
            this.txComm.Name = "txComm";
            this.txComm.Size = new System.Drawing.Size(595, 209);
            this.txComm.TabIndex = 15;
            // 
            // btnComSend
            // 
            this.btnComSend.Location = new System.Drawing.Point(519, 211);
            this.btnComSend.Name = "btnComSend";
            this.btnComSend.Size = new System.Drawing.Size(75, 23);
            this.btnComSend.TabIndex = 16;
            this.btnComSend.Text = "发送";
            this.btnComSend.UseVisualStyleBackColor = true;
            this.btnComSend.Click += new System.EventHandler(this.btnComSend_Click);
            // 
            // txData
            // 
            this.txData.Location = new System.Drawing.Point(14, 35);
            this.txData.Name = "txData";
            this.txData.Size = new System.Drawing.Size(572, 21);
            this.txData.TabIndex = 17;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 452);
            this.Controls.Add(this.txData);
            this.Controls.Add(this.btnComSend);
            this.Controls.Add(this.txComm);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.comb);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comport);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnCls);
            this.Controls.Add(this.btnlisten);
            this.Controls.Add(this.NUMPort);
            this.Controls.Add(this.label2);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.NUMPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown NUMPort;
        private System.Windows.Forms.Button btnlisten;
        private System.Windows.Forms.Button btnCls;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox comport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comb;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txComm;
        private System.Windows.Forms.Button btnComSend;
        private System.Windows.Forms.TextBox txData;
    }
}

