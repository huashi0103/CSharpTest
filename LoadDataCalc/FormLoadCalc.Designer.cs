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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLoadCalc));
            this.comboType = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressLoad = new System.Windows.Forms.ToolStripProgressBar();
            this.statuslbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.numericLimit = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnfile = new System.Windows.Forms.Button();
            this.btnNextError = new System.Windows.Forms.Button();
            this.chkShowback = new System.Windows.Forms.CheckBox();
            this.btnlast = new System.Windows.Forms.Button();
            this.numericError = new System.Windows.Forms.NumericUpDown();
            this.btnNext = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnShowNonStress = new System.Windows.Forms.Button();
            this.chkCover = new System.Windows.Forms.CheckBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.radioTime = new System.Windows.Forms.RadioButton();
            this.radioAll = new System.Windows.Forms.RadioButton();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonConfig = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRead = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonWrite = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRollback = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonReadFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCount = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTest = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCheck = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStripButtonAbout = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericLimit)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericError)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
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
            this.comboType.Location = new System.Drawing.Point(108, 22);
            this.comboType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(139, 26);
            this.comboType.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 127);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(1262, 564);
            this.dataGridView1.TabIndex = 3;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressLoad,
            this.statuslbl});
            this.statusStrip1.Location = new System.Drawing.Point(0, 727);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1262, 26);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressLoad
            // 
            this.toolStripProgressLoad.Name = "toolStripProgressLoad";
            this.toolStripProgressLoad.Size = new System.Drawing.Size(133, 20);
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
            this.numericLimit.Location = new System.Drawing.Point(108, 55);
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
            this.numericLimit.Size = new System.Drawing.Size(140, 28);
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
            this.label1.Location = new System.Drawing.Point(45, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 19);
            this.label1.TabIndex = 6;
            this.label1.Text = "限差:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 560F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 540F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 162F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 127F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1262, 127);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnfile);
            this.groupBox2.Controls.Add(this.btnNextError);
            this.groupBox2.Controls.Add(this.chkShowback);
            this.groupBox2.Controls.Add(this.btnlast);
            this.groupBox2.Controls.Add(this.numericError);
            this.groupBox2.Controls.Add(this.btnNext);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.btnSearch);
            this.groupBox2.Controls.Add(this.txNumber);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(563, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(534, 121);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "比对数据";
            // 
            // btnfile
            // 
            this.btnfile.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnfile.Location = new System.Drawing.Point(104, 88);
            this.btnfile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnfile.Name = "btnfile";
            this.btnfile.Size = new System.Drawing.Size(131, 30);
            this.btnfile.TabIndex = 17;
            this.btnfile.Text = "打开点的文件";
            this.btnfile.UseVisualStyleBackColor = true;
            // 
            // btnNextError
            // 
            this.btnNextError.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnNextError.Location = new System.Drawing.Point(243, 88);
            this.btnNextError.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNextError.Name = "btnNextError";
            this.btnNextError.Size = new System.Drawing.Size(123, 30);
            this.btnNextError.TabIndex = 15;
            this.btnNextError.Text = "下一异常点";
            this.btnNextError.UseVisualStyleBackColor = true;
            // 
            // chkShowback
            // 
            this.chkShowback.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkShowback.AutoSize = true;
            this.chkShowback.Location = new System.Drawing.Point(243, 60);
            this.chkShowback.Name = "chkShowback";
            this.chkShowback.Size = new System.Drawing.Size(145, 23);
            this.chkShowback.TabIndex = 26;
            this.chkShowback.Text = "不显示背景色";
            this.chkShowback.UseVisualStyleBackColor = true;
            // 
            // btnlast
            // 
            this.btnlast.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnlast.Location = new System.Drawing.Point(415, 17);
            this.btnlast.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnlast.Name = "btnlast";
            this.btnlast.Size = new System.Drawing.Size(80, 30);
            this.btnlast.TabIndex = 16;
            this.btnlast.Text = "上一点";
            this.btnlast.UseVisualStyleBackColor = true;
            // 
            // numericError
            // 
            this.numericError.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numericError.DecimalPlaces = 3;
            this.numericError.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericError.Location = new System.Drawing.Point(104, 55);
            this.numericError.Name = "numericError";
            this.numericError.Size = new System.Drawing.Size(129, 28);
            this.numericError.TabIndex = 18;
            this.numericError.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericError.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // btnNext
            // 
            this.btnNext.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnNext.Location = new System.Drawing.Point(329, 18);
            this.btnNext.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(78, 30);
            this.btnNext.TabIndex = 13;
            this.btnNext.Text = "下一点";
            this.btnNext.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 19);
            this.label4.TabIndex = 27;
            this.label4.Text = "比对限差:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Location = new System.Drawing.Point(243, 18);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(78, 30);
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "查找";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // txNumber
            // 
            this.txNumber.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txNumber.Location = new System.Drawing.Point(104, 20);
            this.txNumber.Name = "txNumber";
            this.txNumber.Size = new System.Drawing.Size(129, 28);
            this.txNumber.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(44, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 19);
            this.label3.TabIndex = 14;
            this.label3.Text = "点名:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboType);
            this.groupBox1.Controls.Add(this.numericLimit);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnShowNonStress);
            this.groupBox1.Controls.Add(this.chkCover);
            this.groupBox1.Controls.Add(this.dateTimePicker1);
            this.groupBox1.Controls.Add(this.radioTime);
            this.groupBox1.Controls.Add(this.radioAll);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(554, 121);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "读取设置";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 19);
            this.label2.TabIndex = 7;
            this.label2.Text = "仪器类型:";
            // 
            // btnShowNonStress
            // 
            this.btnShowNonStress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnShowNonStress.Location = new System.Drawing.Point(114, 88);
            this.btnShowNonStress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnShowNonStress.Name = "btnShowNonStress";
            this.btnShowNonStress.Size = new System.Drawing.Size(101, 30);
            this.btnShowNonStress.TabIndex = 10;
            this.btnShowNonStress.Text = "无应力计";
            this.btnShowNonStress.UseVisualStyleBackColor = true;
            this.btnShowNonStress.Visible = false;
            // 
            // chkCover
            // 
            this.chkCover.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkCover.AutoSize = true;
            this.chkCover.Location = new System.Drawing.Point(272, 20);
            this.chkCover.Name = "chkCover";
            this.chkCover.Size = new System.Drawing.Size(107, 23);
            this.chkCover.TabIndex = 20;
            this.chkCover.Text = "覆盖导入";
            this.chkCover.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dateTimePicker1.Location = new System.Drawing.Point(384, 49);
            this.dateTimePicker1.MinDate = new System.DateTime(1990, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(150, 28);
            this.dateTimePicker1.TabIndex = 21;
            this.dateTimePicker1.Visible = false;
            // 
            // radioTime
            // 
            this.radioTime.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radioTime.AutoSize = true;
            this.radioTime.Checked = true;
            this.radioTime.Location = new System.Drawing.Point(272, 54);
            this.radioTime.Name = "radioTime";
            this.radioTime.Size = new System.Drawing.Size(106, 23);
            this.radioTime.TabIndex = 23;
            this.radioTime.TabStop = true;
            this.radioTime.Text = "指定时间";
            this.radioTime.UseVisualStyleBackColor = true;
            this.radioTime.Visible = false;
            // 
            // radioAll
            // 
            this.radioAll.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radioAll.AutoSize = true;
            this.radioAll.Location = new System.Drawing.Point(272, 92);
            this.radioAll.Name = "radioAll";
            this.radioAll.Size = new System.Drawing.Size(106, 23);
            this.radioAll.TabIndex = 22;
            this.radioAll.Text = "覆盖全部";
            this.radioAll.UseVisualStyleBackColor = true;
            this.radioAll.Visible = false;
            // 
            // skinEngine1
            // 
            this.skinEngine1.SerialNumber = "";
            this.skinEngine1.SkinDialogs = false;
            this.skinEngine1.SkinFile = null;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonConfig,
            this.toolStripSeparator1,
            this.toolStripButtonRead,
            this.toolStripButtonWrite,
            this.toolStripButtonRollback,
            this.toolStripButtonReadFile,
            this.toolStripSeparator2,
            this.toolStripButtonCount,
            this.toolStripButtonTest,
            this.toolStripButtonCheck,
            this.toolStripButtonAbout});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1262, 36);
            this.toolStrip1.TabIndex = 11;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonConfig
            // 
            this.toolStripButtonConfig.AutoSize = false;
            this.toolStripButtonConfig.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripButtonConfig.Image = global::LoadDataCalc.Properties.Resources.config36;
            this.toolStripButtonConfig.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonConfig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConfig.Margin = new System.Windows.Forms.Padding(2);
            this.toolStripButtonConfig.Name = "toolStripButtonConfig";
            this.toolStripButtonConfig.RightToLeftAutoMirrorImage = true;
            this.toolStripButtonConfig.Size = new System.Drawing.Size(80, 36);
            this.toolStripButtonConfig.Text = "配置";
            this.toolStripButtonConfig.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 36);
            // 
            // toolStripButtonRead
            // 
            this.toolStripButtonRead.AutoSize = false;
            this.toolStripButtonRead.Image = global::LoadDataCalc.Properties.Resources.read32;
            this.toolStripButtonRead.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonRead.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonRead.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRead.Margin = new System.Windows.Forms.Padding(2);
            this.toolStripButtonRead.Name = "toolStripButtonRead";
            this.toolStripButtonRead.Size = new System.Drawing.Size(120, 36);
            this.toolStripButtonRead.Text = "加载数据";
            this.toolStripButtonRead.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // toolStripButtonWrite
            // 
            this.toolStripButtonWrite.AutoSize = false;
            this.toolStripButtonWrite.Image = global::LoadDataCalc.Properties.Resources.write32;
            this.toolStripButtonWrite.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonWrite.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonWrite.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonWrite.Margin = new System.Windows.Forms.Padding(2);
            this.toolStripButtonWrite.Name = "toolStripButtonWrite";
            this.toolStripButtonWrite.Size = new System.Drawing.Size(120, 36);
            this.toolStripButtonWrite.Text = "数据入库";
            // 
            // toolStripButtonRollback
            // 
            this.toolStripButtonRollback.AutoSize = false;
            this.toolStripButtonRollback.Image = global::LoadDataCalc.Properties.Resources.rollback32;
            this.toolStripButtonRollback.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonRollback.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonRollback.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRollback.Margin = new System.Windows.Forms.Padding(2);
            this.toolStripButtonRollback.Name = "toolStripButtonRollback";
            this.toolStripButtonRollback.Size = new System.Drawing.Size(120, 36);
            this.toolStripButtonRollback.Text = "回滚数据";
            // 
            // toolStripButtonReadFile
            // 
            this.toolStripButtonReadFile.AutoSize = false;
            this.toolStripButtonReadFile.Image = global::LoadDataCalc.Properties.Resources.loadfile32;
            this.toolStripButtonReadFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonReadFile.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonReadFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReadFile.Margin = new System.Windows.Forms.Padding(2);
            this.toolStripButtonReadFile.Name = "toolStripButtonReadFile";
            this.toolStripButtonReadFile.Size = new System.Drawing.Size(120, 36);
            this.toolStripButtonReadFile.Text = "导入文件";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 36);
            // 
            // toolStripButtonCount
            // 
            this.toolStripButtonCount.AutoSize = false;
            this.toolStripButtonCount.Image = global::LoadDataCalc.Properties.Resources.count32;
            this.toolStripButtonCount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonCount.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonCount.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCount.Margin = new System.Windows.Forms.Padding(2);
            this.toolStripButtonCount.Name = "toolStripButtonCount";
            this.toolStripButtonCount.Size = new System.Drawing.Size(80, 36);
            this.toolStripButtonCount.Text = "统计";
            // 
            // toolStripButtonTest
            // 
            this.toolStripButtonTest.AutoSize = false;
            this.toolStripButtonTest.Image = global::LoadDataCalc.Properties.Resources.test32;
            this.toolStripButtonTest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonTest.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTest.Name = "toolStripButtonTest";
            this.toolStripButtonTest.Size = new System.Drawing.Size(80, 36);
            this.toolStripButtonTest.Text = "测试";
            this.toolStripButtonTest.Visible = false;
            // 
            // toolStripButtonCheck
            // 
            this.toolStripButtonCheck.AutoSize = false;
            this.toolStripButtonCheck.Image = global::LoadDataCalc.Properties.Resources.check36;
            this.toolStripButtonCheck.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonCheck.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonCheck.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCheck.Name = "toolStripButtonCheck";
            this.toolStripButtonCheck.Size = new System.Drawing.Size(120, 36);
            this.toolStripButtonCheck.Text = "校验基准";
            this.toolStripButtonCheck.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1262, 691);
            this.panel1.TabIndex = 12;
            // 
            // toolStripButtonAbout
            // 
            this.toolStripButtonAbout.AutoSize = false;
            this.toolStripButtonAbout.Image = global::LoadDataCalc.Properties.Resources.about32;
            this.toolStripButtonAbout.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonAbout.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAbout.Name = "toolStripButtonAbout";
            this.toolStripButtonAbout.Size = new System.Drawing.Size(120, 36);
            this.toolStripButtonAbout.Text = "关于";
            // 
            // FormLoadCalc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1262, 753);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FormLoadCalc";
            this.ShowIcon = false;
            this.Text = "加载数据";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericLimit)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericError)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressLoad;
        private System.Windows.Forms.ToolStripStatusLabel statuslbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericLimit;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnShowNonStress;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txNumber;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnNextError;
        private System.Windows.Forms.Button btnlast;
        private System.Windows.Forms.Button btnfile;
        private System.Windows.Forms.NumericUpDown numericError;
        private System.Windows.Forms.CheckBox chkCover;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.RadioButton radioAll;
        private System.Windows.Forms.RadioButton radioTime;
        private System.Windows.Forms.CheckBox chkShowback;
        private Sunisoft.IrisSkin.SkinEngine skinEngine1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRead;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton toolStripButtonWrite;
        private System.Windows.Forms.ToolStripButton toolStripButtonRollback;
        private System.Windows.Forms.ToolStripButton toolStripButtonReadFile;
        private System.Windows.Forms.ToolStripButton toolStripButtonConfig;
        private System.Windows.Forms.ToolStripButton toolStripButtonCount;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripButton toolStripButtonCheck;
        private System.Windows.Forms.ToolStripButton toolStripButtonTest;
        private System.Windows.Forms.ToolStripButton toolStripButtonAbout;
    }
}

