using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;

namespace LoadDataCalc
{
    public partial class FormLoadCalc : Form
    {
        private LoadDataClass loadData;
        //文件列表
        private List<string> loadFiles = new List<string>();
        /// <summary>各仪器对应文件列表
        /// </summary>
        private Dictionary<string, List<string>> Files = new Dictionary<string, List<string>>();
        private Thread threadLoad = null;
        //数据缓存，删除数据的时候用
        private Dictionary<int, object> CurrentCach = new Dictionary<int, object>();
        //datagridview的列名,不能随便修改，查找和删除数据的时候会用到
        private string[] headers =new string[]{"ID", "测点", "观测日期", "观测时间", "温度", "频率", "备注", "温度结果" ,"计算结果","最终结果","Excel结果","差值" };

        public FormLoadCalc()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            #region//初始化
            skinEngine1.SkinFile = Application.StartupPath + "\\DLLS\\MSN.ssk";
            Status("初始化...");
            toolStripButtonRead.Enabled = false;
            toolStripProgressLoad.Visible = true;
            Thread loadThread = new Thread(() =>
            {
                loadData = LoadDataClass.GetInstance();
                string res = loadData.Init();
                if (res != "OK")
                {
                    Status(String.Format("{0}失败!", res));
                }
                else
                {
                    Thread.Sleep(1000);
                    loadData.ClearDirTmp();
                    
                    Status("初始化成功");
                }
                loadData.StatusAction = new Action<string>((msg) => { Status("正在读取文件:" + msg); });
                loadData.FileIndexAction = new Action<int>((index) => {
                    this.Invoke(new EventHandler(delegate {
                        if (toolStripProgressLoad.Visible) toolStripProgressLoad.Value = index;
                    }));

                });
                Files = Config.ReadFileList();
                if (this.IsDisposed) return;
                this.Invoke(new EventHandler(delegate {
                    loadTypes();
                    if(comboType.Items.Count>0)comboType.SelectedIndex = 0;
                    toolStripProgressLoad.Visible = false;
                    this.Text = Config.ProjectName;
                    toolStripButtonRead.Enabled = true;
                    Type type = dataGridView1.GetType();
                    PropertyInfo pi = type.GetProperty("DoubleBuffered",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    pi.SetValue(dataGridView1, true, null);

                    ContextMenu menu = new ContextMenu();
                    MenuItem item = new MenuItem("删除数据");
                    item.Click += (send, arg) =>
                    {
                        if (MessageBox.Show("确认删除数据？", "删除数据", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            this.dataGridView1.BeginInvoke(new Action(()=>
                            {
                                foreach (var obj in dataGridView1.SelectedRows)
                                {
                                    var row = obj as DataGridViewRow;
                                    int id = Convert.ToInt32(row.Cells["ID"].Value);
                                    string point = row.Cells["测点"].Value.ToString();
                                    var pt = loadData.SurveyDataCach.FirstOrDefault(p => p.SurveyPoint == point);
                                    if (pt != null && CurrentCach.Keys.Contains(id))
                                    {
                                        pt.Datas.Remove(CurrentCach[id] as SurveyData);
                                    }
                                    this.dataGridView1.Rows.Remove(row);
                                }
                            }));
                        }
                    };
                    menu.MenuItems.Add(item);
                    this.dataGridView1.ContextMenu = menu;

                }));
            });
            loadThread.Start();

            #endregion
            #region //窗体事件
            
            toolStripButtonRead.Click += new EventHandler(btnRead_Click);
            toolStripButtonWrite.Click += new EventHandler(btnWrite_Click);
            toolStripButtonRollback.Click += (o, eg) =>
            {
                if (loadData.ID < 0) Status("没有数据可以回滚");
          
                var instype = Config.InsCollection.InstrumentDic[comboType.Text];
                DateTime dt=new DateTime();
                if (loadData.RollbackCheck(instype, out dt))
                {
                    if (MessageBox.Show(string.Format("将回滚{0}以后的数据，确定回滚数据?",dt.ToString()), "提示", MessageBoxButtons.OKCancel) ==
                            System.Windows.Forms.DialogResult.Cancel) return;
                }
                ThreadPool.QueueUserWorkItem((obj) => {
                    loadData.Rollback(instype);
                    Status("回滚成功");
                });
            };
            toolStripButtonConfig.Click += (o, eg) =>
            {
                FormConfigFilePath fcf = new FormConfigFilePath();
                fcf.ShowDialog();
                //fcf.Show();
                Files = Config.ReadFileList();
            };
            numericLimit.ValueChanged += (o, eg) => { Config.LimitZ = (double)numericLimit.Value;
            Config.LimitT = (double)numericLimit.Value;
            };

            comboType.TextChanged+=new EventHandler((send,args)=>{
                btnShowNonStress.Visible = (comboType.Text == "应变计" || comboType.Text == "应变计组");
                
            });
            btnShowNonStress.Click += new EventHandler(btnShowNonStress_Click);
            #region 查找数据相关
            btnSearch.Click+=new EventHandler((send,arg)=>{
                //var data=loadData.SurveyDataCach.Where(p => p.SurveyPoint.ToUpper().Trim()==txNumber.Text.ToUpper().Trim()).FirstOrDefault();
                PointSurveyData data;
                string current = txNumber.Text.ToUpper().Trim();
                if (btnShowNonStress.Text == "无应力计")
                {
                    data = loadData.SurveyDataCach.Where(p => DataUtils.CheckStrIgnoreCN(p.SurveyPoint.ToUpper(), current)).FirstOrDefault();
                    if (data == null || data.Datas.Count == 0)
                    {
                        MessageBox.Show("没有数据");
                        return;
                    }
                    int rowcount = 0;
                    foreach (var pd in loadData.SurveyDataCach)
                    {
                        if (pd.SurveyPoint == data.SurveyPoint) break;
                        rowcount += pd.Datas.Count;
                    }
                    this.dataGridView1.CurrentCell = this.dataGridView1.Rows[rowcount].Cells[0];
                }
                else
                {
                    data = loadData.SurveyDataCachExpand.Where(p => DataUtils.CheckStrIgnoreCN(p.SurveyPoint.ToUpper(),current)).FirstOrDefault();
                    if (data == null || data.Datas.Count == 0)
                    {
                        MessageBox.Show("没有数据");
                        return;
                    }
                    int rowcount = 0;
                    foreach (var pd in loadData.SurveyDataCachExpand)
                    {
                        if (pd.SurveyPoint == data.SurveyPoint) break;
                        rowcount += pd.Datas.Count;
                    }
                    this.dataGridView1.CurrentCell = this.dataGridView1.Rows[rowcount].Cells[0];
                }


            });
            btnNext.Click+=new EventHandler((send,arg)=>{
                if (dataGridView1.CurrentRow == null) return;
                string current = dataGridView1.CurrentRow.Cells["测点"].Value.ToString();
                if (current == null) return;
                int index = dataGridView1.CurrentRow.Index;
                for (int i = index; i < dataGridView1.Rows.Count-1; i++)
                {
                    string tempnumber = dataGridView1.Rows[i].Cells["测点"].Value.ToString();
                    if (tempnumber != current)
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.CurrentCell.OwningRow.Index;
                        break;
                    }
                }
            });
            btnlast.Click += new EventHandler((send, arg) =>
            {
                if (dataGridView1.CurrentRow == null) return;
                string current = dataGridView1.CurrentRow.Cells["测点"].Value.ToString();
                if (current == null) return;
                int index = dataGridView1.CurrentRow.Index;
                for (int i = index; i >0; i--)
                {
                    string tempnumber = dataGridView1.Rows[i].Cells["测点"].Value.ToString();
                    if (tempnumber != current)
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.CurrentCell.OwningRow.Index;
                        break;
                    }
                }
            });
            btnNextError.Click += new EventHandler((send, arg) =>
            {
                if (dataGridView1.CurrentRow == null) return;
                string current = dataGridView1.CurrentRow.Cells["测点"].Value.ToString();
                if (current == null) return;
                int index = dataGridView1.CurrentRow.Index;
                for (int i = index+1; i < dataGridView1.Rows.Count - 1; i++)
                {
                    string tempnumber = dataGridView1.Rows[i].Cells["测点"].Value.ToString();
                    //if (tempnumber != current)
                    {
                        double value1 = Convert.ToDouble(dataGridView1.Rows[i].Cells["计算结果"].Value);
                        double value2 = Convert.ToDouble(dataGridView1.Rows[i].Cells["Excel结果"].Value);
                        if (Math.Abs(value1 - value2) > (double)numericError.Value)
                        {
                            dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.CurrentCell.OwningRow.Index;
                            break;
                        }
                        else
                        {
                            current = tempnumber;
                            continue;
                        }
                    }
                }
            });

            btnfile.Click += new EventHandler((send, arg) => {
                if (dataGridView1.CurrentRow == null) return;
                string current = dataGridView1.CurrentRow.Cells["测点"].Value.ToString();
                if (current == null) return;
                PointSurveyData point;
                if (btnShowNonStress.Text == "无应力计")
                {
                    point = loadData.SurveyDataCach.Where(p => p.SurveyPoint == current).FirstOrDefault();
                    
                }
                else
                {
                    point = loadData.SurveyDataCachExpand.Where(p => p.SurveyPoint == current).FirstOrDefault();
                }
                if (point != null)
                {
                    if (File.Exists(point.ExcelPath)) Process.Start(point.ExcelPath);
                }
            });
            #endregion
            chkCover.CheckedChanged += new EventHandler(chkCover_CheckedChanged);
            radioTime.CheckedChanged += new EventHandler(radioTime_CheckedChanged);
            radioAll.CheckedChanged += ((send, arg) => {
                Config.IsCoveryAll = radioAll.Checked;
            });
            dateTimePicker1.ValueChanged += ((send, arg) => {
                Config.StartTime = dateTimePicker1.Value;
            });
            toolStripButtonReadFile.Click += new EventHandler(btnLoadFile_Click);
            toolStripButtonCount.Click += ((send, arg) => { FormCount fc = new FormCount(); fc.ShowDialog(); });
            dataGridView1.MouseClick += (send, arg) => {
                if (arg.Button == System.Windows.Forms.MouseButtons.Right && dataGridView1.SelectedRows.Count > 0)
                {
                    dataGridView1.ContextMenu.Show(dataGridView1, arg.Location);
                }
            };
            dataGridView1.DataSourceChanged += (send, arg) => { changeBackColor(); };
            chkShowback.CheckedChanged += (send, arg) => { changeBackColor(); };
            numericError.ValueChanged += (send, ar) => { changeBackColor(); };
            toolStripButtonCheck.Click += new EventHandler(toolStripButtonCheck_Click);
            toolStripButtonTest.Click += new EventHandler(toolStripButtonTest_Click);
            #endregion
       
        }

        void toolStripButtonTest_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定写入数据库?", "提示", MessageBoxButtons.OKCancel) ==
                   System.Windows.Forms.DialogResult.Cancel) return;
            var instype = Config.InsCollection.InstrumentDic[comboType.Text];
            //写入之前检查数据，防止多次写入
            if (!loadData.CheckSurveyDate(instype, Config.InsCollection[comboType.Text].Measure_Table))
            {
                MessageBox.Show("数据库或仪器类型已变动,请重新读取数据");
                return;
            }

            setEnable(false);
            toolStripProgressLoad.Visible = true;
            Status("开始写入数据...");
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                bool flag = false;
                flag = loadData.WriteDBExpand();
                string surveyStatus = String.Format("写入{0}", (flag ? "成功" : "失败"));
                Status(surveyStatus + "," + surveyStatus);
                this.Invoke(new EventHandler(delegate
                {
                    setEnable(true);
                    toolStripProgressLoad.Visible = false;
                }));

            });
        }

        void toolStripButtonCheck_Click(object sender, EventArgs e)
        {
            string insname = comboType.Text;
            Status("检查文件");
            Func<bool> checkFunc = new Func<bool>(() =>
            {
                string errfile;
                if (!loadData.CheckFiles(Files[insname], out errfile))
                {
                    this.Invoke(new EventHandler(delegate { MessageBox.Show(errfile + "被占用!"); }));
                    return false;
                }
                return true;
            });
            if (!checkFunc()) return;
            if (loadData.SurveyDataCach.Count != 0)
            {
                if (MessageBox.Show("数据缓存中还有数据,读取数据将清空该数据缓存,点击‘是’确定读取", "提示", MessageBoxButtons.YesNo) ==
                    DialogResult.No)
                {
                    return;
                }
            }
            setEnable(false);
            Status("校验基准");
            loadData.ClearCach();
            this.dataGridView1.DataSource = null;
            toolStripProgressLoad.Visible = true;
            toolStripProgressLoad.Maximum = Files[insname].Count;
            toolStripProgressLoad.Value = 0;
            Action callback = () =>
            {
                if (this.IsDisposed) return;
                this.Invoke(new EventHandler(delegate
                {
                    toolStripProgressLoad.Visible = false;
                    setEnable(true);
                    //ErrorMsg.OpenLog(1);
                    //MessageBox.Show(Config.Tick1.ToString() + "__" + Config.Tick2.ToString());
                }));
            };

            threadLoad = new Thread((cb) =>
            {
                var type = Config.InsCollection.InstrumentDic[insname];

                var res = loadData.CheckStandard(type, Files[insname]);
                this.Invoke(new EventHandler(delegate{
                    DataTable dt = new DataTable();
                    dt.TableName = "showTable";
                    dt.Columns.Add("测点");
                    dt.Columns.Add("行数");
                    dt.Columns.Add("文件");
                    for (int i = 0; i < res.Count;i++ )
                    {
                        DataRow dr = dt.NewRow();
                        dr["测点"] = res[i].name;
                        dr["行数"] = res[i].index;
                        dr["文件"] = res[i].spath;
                        dt.Rows.Add(dr);
                    }
                    dataGridView1.DataSource = dt;
                    dataGridView1.Update();
                    for (int i = 0; i < res.Count; i++)
                    {
                        if (res[i].index < 1) dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    }
                }));
                Action call = cb as Action;
                call();
            });
            threadLoad.Start(callback);
        }

        void btnWrite_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定写入数据库?", "提示", MessageBoxButtons.OKCancel) ==
                   System.Windows.Forms.DialogResult.Cancel) return;
            var instype = Config.InsCollection.InstrumentDic[comboType.Text];
            //写入之前检查数据，防止多次写入
            if (!loadData.CheckSurveyDate(instype, Config.InsCollection[comboType.Text].Measure_Table))
            {
                MessageBox.Show("数据库或仪器类型已变动,请重新读取数据");
                return;
            }
            
            setEnable(false);
            toolStripProgressLoad.Visible = true;
            Status("开始写入数据...");
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                bool flag = false;
                flag = loadData.WirteToSurvey();
                string surveyStatus = String.Format("写入测值{0}", (flag ? "成功" : "失败"));
                flag = loadData.WirteToResult();
                string resultSatus = String.Format("写入成果{0}", (flag ? "成功" : "失败"));
                Status(surveyStatus + "," + resultSatus);
                this.Invoke(new EventHandler(delegate{setEnable(true);
                toolStripProgressLoad.Visible = false;
                }));
            
            });
        }

        void btnLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "D:\\";
            openFileDialog.Filter = "Excel2013|*.xlsx|Excel2007|*.xls|所有文件|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<string>files=new List<string>();
                files.AddRange(openFileDialog.FileNames);
                string insname = comboType.Text;
                Status("检查文件");
                Func<bool> checkFunc = new Func<bool>(() =>
                {
                    string errfile;
                    if (!loadData.CheckFiles(files, out errfile))
                    {
                        this.Invoke(new EventHandler(delegate { MessageBox.Show(errfile + "被占用!"); }));
                        return false;
                    }
                    return true;
                });
                if (!checkFunc()) return;
                if (loadData.SurveyDataCach.Count != 0)
                {
                    if (MessageBox.Show("数据缓存中还有数据,读取数据将清空该数据缓存,点击‘是’确定读取", "是否读取", MessageBoxButtons.YesNo) ==
                        DialogResult.No)
                    {
                        return;
                    }
                }
                setEnable(false);
                Status("读取数据");
                loadData.ClearCach();
                this.dataGridView1.DataSource = null;
                toolStripProgressLoad.Visible = true;
                Action callback = () =>
                {
                    if (this.IsDisposed) return;
                    this.Invoke(new EventHandler(delegate
                    {
                        toolStripProgressLoad.Visible = false;
                        setEnable(true);
                        loaddatagridview(loadData.SurveyDataCach);
                        ErrorMsg.OpenLog(1);
                    }));
                };

                threadLoad = new Thread((cb) =>
                {
                    var type = Config.InsCollection.InstrumentDic[insname];
                    loadData.ReadData(type, files);
                    Status("计算数据");
                    loadData.Calc(type);
                    Action call = cb as Action;
                    call();
                });
                threadLoad.Start(callback);
            }
        }
        void radioTime_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Visible = radioTime.Checked;
        }
        void chkCover_CheckedChanged(object sender, EventArgs e)
        {
            radioAll.Visible = chkCover.Checked;
            radioTime.Visible = chkCover.Checked;
            dateTimePicker1.Visible = radioTime.Visible && radioTime.Checked;
            Config.IsCovery = chkCover.Checked;
        }
        void btnShowNonStress_Click(object sender, EventArgs e)
        {
            if (btnShowNonStress.Text == "无应力计")
            {
                CurrentCach.Clear();
                DataTable dt = new DataTable();
                dt.TableName = "Survey_Leakage_Pressure";
                dt.Columns.Add("ID");
                dt.Columns.Add("测点");
                dt.Columns.Add("观测日期");
                dt.Columns.Add("观测时间");
                dt.Columns.Add("温度");
                dt.Columns.Add("频率");
                dt.Columns.Add("备注");
                dt.Columns.Add("温度结果");
                dt.Columns.Add("计算结果");
                int id = 0;
                foreach (PointSurveyData pd in loadData.SurveyDataCachExpand)
                {
                    foreach (var surveydata in pd.Datas)
                    {
                        id++;
                        DataRow dr = dt.NewRow();
                        dr["ID"] = id;
                        dr["测点"] = pd.SurveyPoint;
                        dr["观测日期"] = surveydata.SurveyDate;
                        dr["观测时间"] = surveydata.SurveyDate.TimeOfDay.ToString();
                        dr["温度"] = surveydata.Survey_RorT;
                        dr["频率"] = surveydata.Survey_ZorR;
                        dr["备注"] = surveydata.Remark;
                        dr["温度结果"] = surveydata.Tempreture;
                        dr["计算结果"] = surveydata.LoadReading;
                        CurrentCach.Add(id, surveydata);
                        dt.Rows.Add(dr);
                    }
                }
                this.dataGridView1.DataSource = dt;
                dataGridView1.Columns["测点"].Frozen = true;
                btnShowNonStress.Text = "应变计";
            }
            else
            {
                loaddatagridview(loadData.SurveyDataCach);
                btnShowNonStress.Text = "无应力计";
            }
        }
        void btnRead_Click(object sender, EventArgs e)
        {
            if (chkCover.Checked)
            {
                //MessageBox.Show("覆盖导入功能请选择指定文件进行导入!");
                //return;
                if (MessageBox.Show("选择了全部覆盖导入,会进行全部覆盖导入该类仪器的数据", "是否继续", MessageBoxButtons.YesNo) ==
                   DialogResult.No)
                {
                    return;
                }
            }
            string insname = comboType.Text;
            Status("检查文件");
            Func<bool> checkFunc = new Func<bool>(() => {
                string errfile;
                if (!loadData.CheckFiles(Files[insname], out errfile))
                {
                    this.Invoke(new EventHandler(delegate { MessageBox.Show(errfile + "被占用!");}));
                    return false;
                }
                return true;
            });
           if (!checkFunc()) return;
           if (loadData.SurveyDataCach.Count != 0)
           {
               if (MessageBox.Show("数据缓存中还有数据,读取数据将清空该数据缓存,点击‘是’确定读取", "提示", MessageBoxButtons.YesNo) ==
                   DialogResult.No)
               {
                   return;
               }
           }
           setEnable(false);
           Status("读取数据");
           loadData.ClearCach();
           this.dataGridView1.DataSource = null;
           toolStripProgressLoad.Visible = true;
           toolStripProgressLoad.Maximum = Files[insname].Count;
           toolStripProgressLoad.Value = 0;
            Action callback = () =>
            {
                if (this.IsDisposed) return;
                this.Invoke(new EventHandler(delegate
                {
                    toolStripProgressLoad.Visible = false;
                    setEnable(true);
                    loaddatagridview(loadData.SurveyDataCach);
                    ErrorMsg.OpenLog(1);
                    //MessageBox.Show(Config.Tick1.ToString() + "__" + Config.Tick2.ToString());
                }));
            };

            threadLoad = new Thread((cb) =>
            {
                var type = Config.InsCollection.InstrumentDic[insname];

                loadData.ReadData(type,Files[insname]);
                //string path = Environment.CurrentDirectory + "\\templist.xml";
                //MultiDisplacementCalc mcalc = new MultiDisplacementCalc();
                //var list = mcalc.GetCalclist(loadData.SurveyDataCach);
                //mcalc.Wirtexml(list, path);
                Status("计算数据");
                loadData.Calc(type);
                Action call = cb as Action;
                call();
            });
            threadLoad.Start(callback);
          
        }

       void Status(string msg)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(delegate
                {
                    statuslbl.Text = msg;
                }));
            }
            else
            {
                statuslbl.Text = msg;
            }
        }
       void loadTypes()
        {
            comboType.Items.Clear();
            foreach (var ins in Config.Instruments) {
                comboType.Items.Add(ins.InsName);         
            }
        }
       void loaddatagridview(List<PointSurveyData> datas)
        {
            if (comboType.Text == "锚索测力计")
            {
                loaddatagridviewExpand(datas);
                return;
            }
            bool flag = comboType.Text == "多点位移计" || comboType.Text == "应变计组" || comboType.Text == "多点锚杆应力计";
            CurrentCach.Clear();
            DataTable dt = new DataTable();
            dt.TableName = "showTable";
            foreach (var h in headers)
            {
                dt.Columns.Add(h);
            }
            if (flag)
            {
                for (int i = 1; i < 7; i++)
                {
                    dt.Columns.Add("F"+i.ToString());
                    dt.Columns.Add("R" + i.ToString());
                    if (comboType.Text == "应变计组" || comboType.Text == "多点锚杆应力计") dt.Columns.Add("T" + i.ToString());
                }                
            }
            int id = 0;//总数据
            int Pcount = 0;//点数
            int PcountEx = 0;//计算下仪器支数
            foreach (PointSurveyData pd in datas)
            {
                
                Pcount++;
                if (flag && pd.Datas.Count > 0) PcountEx += pd.Datas[0].MultiDatas.Count;
                foreach (var surveydata in pd.Datas)
                {
                    id++;
                    DataRow dr = dt.NewRow();
                    dr["ID"] = id;
                    dr["测点"] = pd.SurveyPoint;
                    dr["观测日期"] = surveydata.SurveyDate;
                    dr["观测时间"] = surveydata.SurveyDate.TimeOfDay.ToString();
                    dr["温度"] = surveydata.Survey_RorT;
                    dr["频率"] = surveydata.Survey_ZorR;
                    dr["备注"] = surveydata.Remark;
                    dr["温度结果"] = surveydata.Tempreture;
                    dr["计算结果"] = surveydata.LoadReading;
                    dr["最终结果"] = surveydata.ResultReading;
                    dr["Excel结果"] = surveydata.ExcelResult;
                    dr["差值"] =  Math.Abs(surveydata.ExcelResult - surveydata.LoadReading).ToString("#0.0000"); 
                    if (flag)
                    {
                        dr["最终结果"] = surveydata.AfterLock;
                        int i = 1;
                        foreach (var v in surveydata.MultiDatas)
                        {
                            dr["F" + i.ToString()] = v.Value.Survey_ZorR;
                            dr["R" + i.ToString()] = v.Value.LoadReading;
                            if (comboType.Text == "应变计组" || comboType.Text == "多点锚杆应力计") dr["T" + i.ToString()] = v.Value.Tempreture;
                            
                            i++;
                        }
                    }
                    CurrentCach.Add(id, surveydata);
                    dt.Rows.Add(dr);
                }
            }
           
            this.dataGridView1.DataSource = dt;
            dataGridView1.Columns["测点"].Frozen = true;
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                this.dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            if (flag)
            {
                Status(String.Format("读取{0}个点,{1}支仪器,总共{2}条数据", Pcount,PcountEx, id));
            }
            else
            {
                Status(String.Format("读取{0}个点，总共{1}条数据", Pcount, id));
            }
           
        }
       void loaddatagridviewExpand(List<PointSurveyData> datas)
       {
           CurrentCach.Clear();
           DataTable dt = new DataTable();
           dt.TableName = "showTable";
           foreach (var h in headers)
           {
               dt.Columns.Add(h);
           }
           dt.Columns.Add("clacAverage");
           dt.Columns.Add("Average");
           for (int i = 1; i < 7; i++)
           {
               dt.Columns.Add("F" + i.ToString());
           }   
           int id = 0;//总数据
           int Pcount = 0;//点数
           foreach (PointSurveyData pd in datas)
           {
               Pcount++;
               foreach (var surveydata in pd.Datas)
               {
                   id++;
                   DataRow dr = dt.NewRow();
                   dr["ID"] = id;
                   dr["测点"] = pd.SurveyPoint;
                   dr["观测日期"] = surveydata.SurveyDate;
                   dr["观测时间"] = surveydata.SurveyDate.TimeOfDay.ToString();
                   dr["温度"] = surveydata.Survey_RorT;
                   dr["频率"] = surveydata.Survey_ZorR;
                   dr["备注"] = surveydata.Remark;
                   dr["温度结果"] = surveydata.Tempreture;
                   dr["计算结果"] = surveydata.LoadReading;
                   dr["最终结果"] = surveydata.ResultReading;
                   dr["Excel结果"] = surveydata.ExcelResult;
                   dr["clacAverage"] = surveydata.clacAverage;
                   dr["Average"] = surveydata.Average;
                    dr["差值"] = Math.Abs(surveydata.ExcelResult - surveydata.LoadReading).ToString("#0.0000");
                    int i = 1;
                    foreach (var v in surveydata.MultiDatas)
                    {
                        dr["F" + i.ToString()] = v.Value.Survey_ZorR;
                        i++;
                    }
                    CurrentCach.Add(id, surveydata);
                   dt.Rows.Add(dr);
               }
           }
           this.dataGridView1.DataSource = dt;
           dataGridView1.Columns["测点"].Frozen = true;
           for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
           {
               this.dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
           }
           Status(String.Format("读取{0}个点，总共{1}条数据", Pcount, id));

       }
       void setEnable(bool status)
       {

           toolStripButtonWrite.Enabled = status;
           toolStripButtonConfig.Enabled = status;
           toolStripButtonRead.Enabled = status;
           comboType.Enabled = status;
           numericLimit.Enabled = status;
           btnShowNonStress.Enabled = status;
           toolStripButtonRollback.Enabled = status;
           toolStripButtonReadFile.Enabled = status;
       }
       void changeBackColor()
       {
           if (!dataGridView1.Columns.Contains("差值")) return;
           foreach (var r in dataGridView1.Rows)
           {
               var row = r as DataGridViewRow;
               var cell = row.Cells["差值"];
               double dif = Convert.ToDouble(cell.Value);
               if (dif > (double)numericError.Value)
               {
                   row.DefaultCellStyle.BackColor = chkShowback.Checked ? Color.White : Color.Gray;
               }
               else
               {
                   row.DefaultCellStyle.BackColor = Color.White;
               }
           }
       }
    }
}
