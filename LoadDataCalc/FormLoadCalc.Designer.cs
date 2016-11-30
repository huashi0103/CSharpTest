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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnRead = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressLoad = new System.Windows.Forms.ToolStripProgressBar();
            this.statuslbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.numericLimit = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConfig = new System.Windows.Forms.Button();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txNumber = new System.Windows.Forms.TextBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnlast = new System.Windows.Forms.Button();
            this.btnShowNonStress = new System.Windows.Forms.Button();
            this.btnNextError = new System.Windows.Forms.Button();
            this.btnfile = new System.Windows.Forms.Button();
            this.numericError = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericLimit)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericError)).BeginInit();
            this.SuspendLayout();
            // 
            // comboType
            // 
            this.comboType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
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
            this.comboType.Location = new System.Drawing.Point(105, 8);
            this.comboType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(127, 26);
            this.comboType.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 119);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1135, 446);
            this.dataGridView1.TabIndex = 3;
            // 
            // btnRead
            // 
            this.btnRead.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRead.Location = new System.Drawing.Point(240, 5);
            this.btnRead.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(104, 30);
            this.btnRead.TabIndex = 4;
            this.btnRead.Text = "加载数据";
            this.btnRead.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressLoad,
            this.statuslbl});
            this.statusStrip1.Location = new System.Drawing.Point(0, 565);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1135, 26);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressLoad
            // 
            this.toolStripProgressLoad.Name = "toolStripProgressLoad";
            this.toolStripProgressLoad.Size = new System.Drawing.Size(133, 20);
            this.toolStripProgressLoad.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // statuslbl
            // 
            this.statuslbl.Name = "statuslbl";
            this.statuslbl.Size = new System.Drawing.Size(39, 21);
            this.statuslbl.Text = "状态";
            // 
            // numericLimit
            // 
            this.numericLimit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numericLimit.Location = new System.Drawing.Point(104, 46);
            this.numericLimit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericLimit.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.numericLimit.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericLimit.Name = "numericLimit";
            this.numericLimit.Size = new System.Drawing.Size(129, 28);
            this.numericLimit.TabIndex = 5;
            this.numericLimit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericLimit.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "限差:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 113F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 124F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 116F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 541F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.numericLimit, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.comboType, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnRead, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnConfig, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnWrite, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.txNumber, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnNext, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnlast, 5, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnNextError, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.numericError, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnfile, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnShowNonStress, 6, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1135, 119);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 19);
            this.label3.TabIndex = 14;
            this.label3.Text = "点名:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 19);
            this.label2.TabIndex = 7;
            this.label2.Text = "仪器类型:";
            // 
            // btnConfig
            // 
            this.btnConfig.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnConfig.Location = new System.Drawing.Point(240, 45);
            this.btnConfig.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(104, 30);
            this.btnConfig.TabIndex = 8;
            this.btnConfig.Text = "配置";
            this.btnConfig.UseVisualStyleBackColor = true;
            // 
            // btnWrite
            // 
            this.btnWrite.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnWrite.Location = new System.Drawing.Point(357, 5);
            this.btnWrite.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(107, 30);
            this.btnWrite.TabIndex = 9;
            this.btnWrite.Text = "写入数据库";
            this.btnWrite.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Location = new System.Drawing.Point(240, 85);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(104, 30);
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "查找";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // txNumber
            // 
            this.txNumber.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txNumber.Location = new System.Drawing.Point(104, 86);
            this.txNumber.Name = "txNumber";
            this.txNumber.Size = new System.Drawing.Size(129, 28);
            this.txNumber.TabIndex = 12;
            // 
            // btnNext
            // 
            this.btnNext.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnNext.Location = new System.Drawing.Point(359, 85);
            this.btnNext.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(104, 30);
            this.btnNext.TabIndex = 13;
            this.btnNext.Text = "下一点";
            this.btnNext.UseVisualStyleBackColor = true;
            // 
            // btnlast
            // 
            this.btnlast.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnlast.Location = new System.Drawing.Point(477, 85);
            this.btnlast.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnlast.Name = "btnlast";
            this.btnlast.Size = new System.Drawing.Size(108, 30);
            this.btnlast.TabIndex = 16;
            this.btnlast.Text = "上一点";
            this.btnlast.UseVisualStyleBackColor = true;
            // 
            // btnShowNonStress
            // 
            this.btnShowNonStress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnShowNonStress.Location = new System.Drawing.Point(594, 5);
            this.btnShowNonStress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnShowNonStress.Name = "btnShowNonStress";
            this.btnShowNonStress.Size = new System.Drawing.Size(111, 30);
            this.btnShowNonStress.TabIndex = 10;
            this.btnShowNonStress.Text = "无应力计";
            this.btnShowNonStress.UseVisualStyleBackColor = true;
            this.btnShowNonStress.Visible = false;
            // 
            // btnNextError
            // 
            this.btnNextError.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnNextError.Location = new System.Drawing.Point(353, 45);
            this.btnNextError.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNextError.Name = "btnNextError";
            this.btnNextError.Size = new System.Drawing.Size(115, 30);
            this.btnNextError.TabIndex = 15;
            this.btnNextError.Text = "下一异常点";
            this.btnNextError.UseVisualStyleBackColor = true;
            // 
            // btnfile
            // 
            this.btnfile.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnfile.Location = new System.Drawing.Point(477, 5);
            this.btnfile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnfile.Name = "btnfile";
            this.btnfile.Size = new System.Drawing.Size(108, 30);
            this.btnfile.TabIndex = 17;
            this.btnfile.Text = "打开文件";
            this.btnfile.UseVisualStyleBackColor = true;
            // 
            // numericError
            // 
            this.numericError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.numericError.DecimalPlaces = 3;
            this.numericError.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericError.Location = new System.Drawing.Point(476, 46);
            this.numericError.Name = "numericError";
            this.numericError.Size = new System.Drawing.Size(110, 28);
            this.numericError.TabIndex = 18;
            this.numericError.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // FormLoadCalc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1135, 591);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FormLoadCalc";
            this.ShowIcon = false;
            this.Text = "加载数据";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericLimit)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressLoad;
        private System.Windows.Forms.ToolStripStatusLabel statuslbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericLimit;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Button btnShowNonStress;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txNumber;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnNextError;
        private System.Windows.Forms.Button btnlast;
        private System.Windows.Forms.Button btnfile;
        private System.Windows.Forms.NumericUpDown numericError;
    }
}

