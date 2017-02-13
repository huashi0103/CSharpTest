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
            this.comboType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txPath = new System.Windows.Forms.TextBox();
            this.btnChekAll = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPath = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statuslbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.listviewFiles = new System.Windows.Forms.ListView();
            this.treeViewDir = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txServer = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txDataBase = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txUser = new System.Windows.Forms.TextBox();
            this.txPwd = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboType
            // 
            this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboType.FormattingEnabled = true;
            this.comboType.Location = new System.Drawing.Point(93, 18);
            this.comboType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(130, 23);
            this.comboType.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "仪器类型:";
            // 
            // txPath
            // 
            this.txPath.Enabled = false;
            this.txPath.Location = new System.Drawing.Point(93, 54);
            this.txPath.Name = "txPath";
            this.txPath.Size = new System.Drawing.Size(437, 25);
            this.txPath.TabIndex = 11;
            // 
            // btnChekAll
            // 
            this.btnChekAll.Location = new System.Drawing.Point(231, 13);
            this.btnChekAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnChekAll.Name = "btnChekAll";
            this.btnChekAll.Size = new System.Drawing.Size(90, 30);
            this.btnChekAll.TabIndex = 4;
            this.btnChekAll.Text = "自动识别";
            this.btnChekAll.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(339, 13);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(83, 30);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnPath
            // 
            this.btnPath.Location = new System.Drawing.Point(9, 49);
            this.btnPath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(77, 30);
            this.btnPath.TabIndex = 10;
            this.btnPath.Text = "路径";
            this.btnPath.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statuslbl});
            this.statusStrip1.Location = new System.Drawing.Point(0, 528);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(991, 25);
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
            this.listviewFiles.Size = new System.Drawing.Size(647, 436);
            this.listviewFiles.TabIndex = 1;
            this.listviewFiles.UseCompatibleStateImageBehavior = false;
            this.listviewFiles.View = System.Windows.Forms.View.Details;
            // 
            // treeViewDir
            // 
            this.treeViewDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewDir.Location = new System.Drawing.Point(0, 0);
            this.treeViewDir.Name = "treeViewDir";
            this.treeViewDir.Size = new System.Drawing.Size(340, 436);
            this.treeViewDir.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 92);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewDir);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listviewFiles);
            this.splitContainer1.Size = new System.Drawing.Size(991, 436);
            this.splitContainer1.SplitterDistance = 340;
            this.splitContainer1.TabIndex = 13;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(991, 92);
            this.panel1.TabIndex = 14;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txPwd);
            this.groupBox1.Controls.Add(this.txUser);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txDataBase);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txServer);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Location = new System.Drawing.Point(536, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(455, 92);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置数据库";
            this.groupBox1.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txPath);
            this.groupBox2.Controls.Add(this.btnChekAll);
            this.groupBox2.Controls.Add(this.comboType);
            this.groupBox2.Controls.Add(this.btnPath);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(536, 92);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "配置文件路径";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器:";
            // 
            // txServer
            // 
            this.txServer.Location = new System.Drawing.Point(78, 23);
            this.txServer.Name = "txServer";
            this.txServer.Size = new System.Drawing.Size(131, 25);
            this.txServer.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "数据库:";
            // 
            // txDataBase
            // 
            this.txDataBase.Location = new System.Drawing.Point(78, 54);
            this.txDataBase.Name = "txDataBase";
            this.txDataBase.Size = new System.Drawing.Size(131, 25);
            this.txDataBase.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(225, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "用户:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(225, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 15);
            this.label5.TabIndex = 5;
            this.label5.Text = "密码:";
            // 
            // txUser
            // 
            this.txUser.Location = new System.Drawing.Point(267, 23);
            this.txUser.Name = "txUser";
            this.txUser.Size = new System.Drawing.Size(120, 25);
            this.txUser.TabIndex = 6;
            // 
            // txPwd
            // 
            this.txPwd.Location = new System.Drawing.Point(267, 54);
            this.txPwd.Name = "txPwd";
            this.txPwd.Size = new System.Drawing.Size(120, 25);
            this.txPwd.TabIndex = 7;
            // 
            // FormConfigFilePath
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(991, 553);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "FormConfigFilePath";
            this.ShowIcon = false;
            this.Text = "配置文件路径";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnChekAll;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statuslbl;
        private System.Windows.Forms.TreeView treeViewDir;
        private System.Windows.Forms.ListView listviewFiles;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.TextBox txPath;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txServer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txDataBase;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txUser;
        private System.Windows.Forms.TextBox txPwd;
    }
}