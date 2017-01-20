namespace LoadDataCalc
{
    partial class FormConfigFilePath
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.comboType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPath = new System.Windows.Forms.Button();
            this.txPath = new System.Windows.Forms.TextBox();
            this.btnChekAll = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoadAll = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statuslbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.listviewFiles = new System.Windows.Forms.ListView();
            this.treeViewDir = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 124F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 533F));
            this.tableLayoutPanel1.Controls.Add(this.comboType, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txPath, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnChekAll, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnLoadAll, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnSave, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnPath, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(982, 79);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // comboType
            // 
            this.comboType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboType.FormattingEnabled = true;
            this.comboType.Location = new System.Drawing.Point(105, 8);
            this.comboType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(116, 23);
            this.comboType.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "仪器类型:";
            // 
            // btnPath
            // 
            this.btnPath.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnPath.Location = new System.Drawing.Point(344, 5);
            this.btnPath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(83, 30);
            this.btnPath.TabIndex = 10;
            this.btnPath.Text = "路径";
            this.btnPath.UseVisualStyleBackColor = true;
            // 
            // txPath
            // 
            this.txPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txPath.Enabled = false;
            this.txPath.Location = new System.Drawing.Point(434, 7);
            this.txPath.Name = "txPath";
            this.txPath.Size = new System.Drawing.Size(545, 25);
            this.txPath.TabIndex = 11;
            // 
            // btnChekAll
            // 
            this.btnChekAll.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnChekAll.Location = new System.Drawing.Point(230, 5);
            this.btnChekAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnChekAll.Name = "btnChekAll";
            this.btnChekAll.Size = new System.Drawing.Size(104, 30);
            this.btnChekAll.TabIndex = 4;
            this.btnChekAll.Text = "自动识别";
            this.btnChekAll.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSave.Location = new System.Drawing.Point(344, 45);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(83, 30);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnLoadAll
            // 
            this.btnLoadAll.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLoadAll.Location = new System.Drawing.Point(111, 45);
            this.btnLoadAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnLoadAll.Name = "btnLoadAll";
            this.btnLoadAll.Size = new System.Drawing.Size(104, 30);
            this.btnLoadAll.TabIndex = 8;
            this.btnLoadAll.Text = "查询所有";
            this.btnLoadAll.UseVisualStyleBackColor = true;
            this.btnLoadAll.Visible = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statuslbl});
            this.statusStrip1.Location = new System.Drawing.Point(0, 528);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(982, 25);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statuslbl
            // 
            this.statuslbl.Name = "statuslbl";
            this.statuslbl.Size = new System.Drawing.Size(39, 20);
            this.statuslbl.Text = "状态";
            // 
            // listviewFiles
            // 
            this.listviewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listviewFiles.GridLines = true;
            this.listviewFiles.Location = new System.Drawing.Point(0, 0);
            this.listviewFiles.Name = "listviewFiles";
            this.listviewFiles.Size = new System.Drawing.Size(651, 449);
            this.listviewFiles.TabIndex = 1;
            this.listviewFiles.UseCompatibleStateImageBehavior = false;
            this.listviewFiles.View = System.Windows.Forms.View.Details;
            // 
            // treeViewDir
            // 
            this.treeViewDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewDir.Location = new System.Drawing.Point(0, 0);
            this.treeViewDir.Name = "treeViewDir";
            this.treeViewDir.Size = new System.Drawing.Size(327, 449);
            this.treeViewDir.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 79);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewDir);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listviewFiles);
            this.splitContainer1.Size = new System.Drawing.Size(982, 449);
            this.splitContainer1.SplitterDistance = 327;
            this.splitContainer1.TabIndex = 13;
            // 
            // FormConfigFilePath
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 553);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Name = "FormConfigFilePath";
            this.ShowIcon = false;
            this.Text = "配置文件路径";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnChekAll;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statuslbl;
        private System.Windows.Forms.TreeView treeViewDir;
        private System.Windows.Forms.ListView listviewFiles;
        private System.Windows.Forms.Button btnLoadAll;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.TextBox txPath;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}