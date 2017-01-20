namespace LoadDataCalc
{
    partial class FormCount
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
            this.btnCheckName = new System.Windows.Forms.Button();
            this.chkName = new System.Windows.Forms.CheckBox();
            this.txName = new System.Windows.Forms.TextBox();
            this.chkDateTime = new System.Windows.Forms.CheckBox();
            this.btnChkAllName = new System.Windows.Forms.Button();
            this.btnChkAllData = new System.Windows.Forms.Button();
            this.btnResult = new System.Windows.Forms.Button();
            this.DateTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.DateTimeStart = new System.Windows.Forms.DateTimePicker();
            this.btnSurvey = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnChangeName = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txMsg = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboType
            // 
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
            this.comboType.Location = new System.Drawing.Point(123, 21);
            this.comboType.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(148, 26);
            this.comboType.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 19);
            this.label2.TabIndex = 7;
            this.label2.Text = "仪器类型:";
            // 
            // btnCheckName
            // 
            this.btnCheckName.Location = new System.Drawing.Point(291, 15);
            this.btnCheckName.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnCheckName.Name = "btnCheckName";
            this.btnCheckName.Size = new System.Drawing.Size(102, 36);
            this.btnCheckName.TabIndex = 4;
            this.btnCheckName.Text = "查询测点";
            this.btnCheckName.UseVisualStyleBackColor = true;
            // 
            // chkName
            // 
            this.chkName.AutoSize = true;
            this.chkName.Location = new System.Drawing.Point(9, 64);
            this.chkName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkName.Name = "chkName";
            this.chkName.Size = new System.Drawing.Size(107, 23);
            this.chkName.TabIndex = 25;
            this.chkName.Text = "指定测点";
            this.chkName.UseVisualStyleBackColor = true;
            // 
            // txName
            // 
            this.txName.Location = new System.Drawing.Point(123, 57);
            this.txName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txName.Name = "txName";
            this.txName.Size = new System.Drawing.Size(148, 28);
            this.txName.TabIndex = 26;
            // 
            // chkDateTime
            // 
            this.chkDateTime.AutoSize = true;
            this.chkDateTime.Location = new System.Drawing.Point(6, 107);
            this.chkDateTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkDateTime.Name = "chkDateTime";
            this.chkDateTime.Size = new System.Drawing.Size(107, 23);
            this.chkDateTime.TabIndex = 23;
            this.chkDateTime.Text = "指定时间";
            this.chkDateTime.UseVisualStyleBackColor = true;
            // 
            // btnChkAllName
            // 
            this.btnChkAllName.Location = new System.Drawing.Point(664, 15);
            this.btnChkAllName.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnChkAllName.Name = "btnChkAllName";
            this.btnChkAllName.Size = new System.Drawing.Size(132, 36);
            this.btnChkAllName.TabIndex = 28;
            this.btnChkAllName.Text = "统计所有测点";
            this.btnChkAllName.UseVisualStyleBackColor = true;
            // 
            // btnChkAllData
            // 
            this.btnChkAllData.Location = new System.Drawing.Point(664, 58);
            this.btnChkAllData.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnChkAllData.Name = "btnChkAllData";
            this.btnChkAllData.Size = new System.Drawing.Size(132, 32);
            this.btnChkAllData.TabIndex = 27;
            this.btnChkAllData.Text = "统计所有数据";
            this.btnChkAllData.UseVisualStyleBackColor = true;
            // 
            // btnResult
            // 
            this.btnResult.Location = new System.Drawing.Point(401, 15);
            this.btnResult.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnResult.Name = "btnResult";
            this.btnResult.Size = new System.Drawing.Size(118, 36);
            this.btnResult.TabIndex = 22;
            this.btnResult.Text = "查询成果值";
            this.btnResult.UseVisualStyleBackColor = true;
            // 
            // DateTimeEnd
            // 
            this.DateTimeEnd.Location = new System.Drawing.Point(498, 102);
            this.DateTimeEnd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DateTimeEnd.MinDate = new System.DateTime(1990, 1, 1, 0, 0, 0, 0);
            this.DateTimeEnd.Name = "DateTimeEnd";
            this.DateTimeEnd.Size = new System.Drawing.Size(147, 28);
            this.DateTimeEnd.TabIndex = 29;
            // 
            // DateTimeStart
            // 
            this.DateTimeStart.Location = new System.Drawing.Point(223, 102);
            this.DateTimeStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DateTimeStart.MinDate = new System.DateTime(1990, 1, 1, 0, 0, 0, 0);
            this.DateTimeStart.Name = "DateTimeStart";
            this.DateTimeStart.Size = new System.Drawing.Size(155, 28);
            this.DateTimeStart.TabIndex = 21;
            // 
            // btnSurvey
            // 
            this.btnSurvey.Location = new System.Drawing.Point(527, 15);
            this.btnSurvey.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnSurvey.Name = "btnSurvey";
            this.btnSurvey.Size = new System.Drawing.Size(118, 36);
            this.btnSurvey.TabIndex = 9;
            this.btnSurvey.Text = "查询测值";
            this.btnSurvey.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(122, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 19);
            this.label1.TabIndex = 30;
            this.label1.Text = "起始时间:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(397, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 19);
            this.label3.TabIndex = 31;
            this.label3.Text = "终止时间:";
            // 
            // btnChangeName
            // 
            this.btnChangeName.Location = new System.Drawing.Point(664, 98);
            this.btnChangeName.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnChangeName.Name = "btnChangeName";
            this.btnChangeName.Size = new System.Drawing.Size(132, 36);
            this.btnChangeName.TabIndex = 32;
            this.btnChangeName.Text = "查询改点名";
            this.btnChangeName.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 140);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(1262, 513);
            this.dataGridView1.TabIndex = 12;
            // 
            // txMsg
            // 
            this.txMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txMsg.Location = new System.Drawing.Point(804, 0);
            this.txMsg.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txMsg.Multiline = true;
            this.txMsg.Name = "txMsg";
            this.txMsg.Size = new System.Drawing.Size(458, 140);
            this.txMsg.TabIndex = 29;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txMsg);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1262, 140);
            this.panel1.TabIndex = 30;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnChangeName);
            this.groupBox1.Controls.Add(this.btnChkAllData);
            this.groupBox1.Controls.Add(this.btnChkAllName);
            this.groupBox1.Controls.Add(this.btnCheckName);
            this.groupBox1.Controls.Add(this.comboType);
            this.groupBox1.Controls.Add(this.btnSurvey);
            this.groupBox1.Controls.Add(this.btnResult);
            this.groupBox1.Controls.Add(this.chkDateTime);
            this.groupBox1.Controls.Add(this.txName);
            this.groupBox1.Controls.Add(this.chkName);
            this.groupBox1.Controls.Add(this.DateTimeEnd);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.DateTimeStart);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(804, 140);
            this.groupBox1.TabIndex = 31;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "查询设置";
            // 
            // FormCount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1262, 653);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormCount";
            this.ShowIcon = false;
            this.Text = "统计";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker DateTimeStart;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnCheckName;
        private System.Windows.Forms.Button btnSurvey;
        private System.Windows.Forms.Button btnResult;
        private System.Windows.Forms.CheckBox chkDateTime;
        private System.Windows.Forms.CheckBox chkName;
        private System.Windows.Forms.TextBox txName;
        private System.Windows.Forms.Button btnChkAllData;
        private System.Windows.Forms.Button btnChkAllName;
        private System.Windows.Forms.TextBox txMsg;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DateTimePicker DateTimeEnd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnChangeName;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}