namespace LoadDataCalc
{
    partial class FormLoadCalc
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
            this.comboType = new System.Windows.Forms.ComboBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnRead = new System.Windows.Forms.Button();
            this.listFiles = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btncalc = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // comboType
            // 
            this.comboType.FormattingEnabled = true;
            this.comboType.Items.AddRange(new object[] {
            "渗压计",
            "单点位移计",
            "固定测斜仪",
            "土压力计",
            "多点位移计",
            "多点锚杆应力计",
            "应变计",
            "应变计组",
            "无应力计",
            "测缝计",
            "温度计",
            "裂缝计",
            "量水堰",
            "钢板计",
            "钢筋计",
            "锚杆应力计",
            "锚索测力计"});
            this.comboType.Location = new System.Drawing.Point(2, 2);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(121, 20);
            this.comboType.TabIndex = 1;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(129, 2);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(87, 23);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "查询所有文件";
            this.btnLoad.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(210, 28);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(568, 472);
            this.dataGridView1.TabIndex = 3;
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(222, 2);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(87, 23);
            this.btnRead.TabIndex = 4;
            this.btnRead.Text = "读取数据";
            this.btnRead.UseVisualStyleBackColor = true;
            // 
            // listFiles
            // 
            this.listFiles.FormattingEnabled = true;
            this.listFiles.ItemHeight = 12;
            this.listFiles.Location = new System.Drawing.Point(2, 28);
            this.listFiles.Name = "listFiles";
            this.listFiles.Size = new System.Drawing.Size(202, 472);
            this.listFiles.TabIndex = 5;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(2, 503);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(776, 21);
            this.textBox1.TabIndex = 6;
            // 
            // btncalc
            // 
            this.btncalc.Location = new System.Drawing.Point(315, 2);
            this.btncalc.Name = "btncalc";
            this.btncalc.Size = new System.Drawing.Size(87, 23);
            this.btncalc.TabIndex = 7;
            this.btncalc.Text = "计算";
            this.btncalc.UseVisualStyleBackColor = true;
            this.btncalc.Click += new System.EventHandler(this.btncalc_Click);
            // 
            // FormLoadCalc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 530);
            this.Controls.Add(this.btncalc);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listFiles);
            this.Controls.Add(this.btnRead);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.comboType);
            this.Name = "FormLoadCalc";
            this.ShowIcon = false;
            this.Text = "加载数据";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.ListBox listFiles;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btncalc;
    }
}

