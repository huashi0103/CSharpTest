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
        private List<string> loadFiles = new List<string>();
        /// <summary>各仪器对应文件列表
        /// </summary>
        private Dictionary<string, List<string>> Files = new Dictionary<string, List<string>>();
        private Thread threadLoad = null;

        public FormLoadCalc()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            #region//初始化
            Status("初始化...");
            btnRead.Enabled = false;
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
                Files = Config.ReadFileList();
                if (this.IsDisposed) return;
                this.Invoke(new EventHandler(delegate {
                    loadTypes();
                    if(comboType.Items.Count>0)comboType.SelectedIndex = 0;
                    toolStripProgressLoad.Visible = false;
                    this.Text = Config.ProjectName;
                    btnRead.Enabled = true;
                }));
            });
            loadThread.Start();
            #endregion
            #region //窗体事件
            btnRead.Click += new EventHandler(btnRead_Click);
            btnWrite.Click += (o, eg) => {
                if (MessageBox.Show("确定写入数据库?", "提示", MessageBoxButtons.OKCancel) == 
                    System.Windows.Forms.DialogResult.Cancel) return;
                var instype = Config.InsCollection.InstrumentDic[ comboType.Text];
                bool flag = false;
                flag = loadData.WirteToSurvey();
                string surveyStatus = String.Format("写入测值{0}", (flag ? "成功" : "失败"));
                flag = loadData.WirteToResult();
                string resultSatus = String.Format("写入成果{0}", (flag ? "成功" : "失败"));
                Status(surveyStatus + "," + resultSatus);
            };
            btnConfig.Click += (o, eg) => {
                FormConfigFilePath fcf = new FormConfigFilePath();
                //fcf.ShowDialog();
                fcf.Show();
                Files = Config.ReadFileList();
            };
            numericLimit.ValueChanged += (o, eg) => { Config.LimitZ = (double)numericLimit.Value;
            Config.LimitT = (double)numericLimit.Value;
            };

            Type type = dataGridView1.GetType();
            PropertyInfo pi = type.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dataGridView1, true, null);
            comboType.TextChanged+=new EventHandler((send,args)=>{
                btnShowNonStress.Visible = (comboType.Text == "应变计" || comboType.Text == "应变计组");
            });
            btnShowNonStress.Click += new EventHandler(btnShowNonStress_Click);
            btnSearch.Click+=new EventHandler((send,arg)=>{
                var data=loadData.SurveyDataCach.Where(p => p.SurveyPoint.ToUpper().Trim()==txNumber.Text.ToUpper().Trim()).FirstOrDefault();
                if (data == null)
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
                
                
            });
            btnNext.Click+=new EventHandler((send,arg)=>{
                if (dataGridView1.SelectedRows.Count < 1) return;
                string current = dataGridView1.CurrentRow.Cells["Survey_point_Number"].Value.ToString();
                int index = dataGridView1.CurrentRow.Index;
                for (int i = index; i < dataGridView1.Rows.Count-1; i++)
                {
                    string tempnumber = dataGridView1.Rows[i].Cells["Survey_point_Number"].Value.ToString();
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
                if (dataGridView1.SelectedRows.Count < 1) return;
                string current = dataGridView1.CurrentRow.Cells["Survey_point_Number"].Value.ToString();
                int index = dataGridView1.CurrentRow.Index;
                for (int i = index; i >0; i--)
                {
                    string tempnumber = dataGridView1.Rows[i].Cells["Survey_point_Number"].Value.ToString();
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
                if (dataGridView1.SelectedRows.Count < 1) return;
                string current = dataGridView1.CurrentRow.Cells["Survey_point_Number"].Value.ToString();
                int index = dataGridView1.CurrentRow.Index;
                for (int i = index+1; i < dataGridView1.Rows.Count - 1; i++)
                {
                    string tempnumber = dataGridView1.Rows[i].Cells["Survey_point_Number"].Value.ToString();
                    //if (tempnumber != current)
                    {
                        double value1 = Convert.ToDouble(dataGridView1.Rows[i].Cells["loadreading"].Value);
                        double value2 = Convert.ToDouble(dataGridView1.Rows[i].Cells["ExcelResult"].Value);
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
                if (dataGridView1.SelectedRows.Count < 1) return;
                string current = dataGridView1.CurrentRow.Cells["Survey_point_Number"].Value.ToString();
                var point = loadData.SurveyDataCach.Where(p => p.SurveyPoint == current).FirstOrDefault();
                if (point != null)
                {
                    if (File.Exists(point.ExcelPath)) Process.Start(point.ExcelPath);
                }
            });
            #endregion
        }
        void btnShowNonStress_Click(object sender, EventArgs e)
        {
            if (btnShowNonStress.Text == "无应力计")
            {
                DataTable dt = new DataTable();
                dt.TableName = "Survey_Leakage_Pressure";
                dt.Columns.Add("ID");
                dt.Columns.Add("Survey_point_Number");
                dt.Columns.Add("Observation_Date");
                dt.Columns.Add("Observation_Time");
                dt.Columns.Add("Temperature");
                dt.Columns.Add("Frequency");
                dt.Columns.Add("Remark");
                dt.Columns.Add("UpdateTime");
                dt.Columns.Add("Tresult");
                dt.Columns.Add("loadreading");
                dt.Columns.Add("resultreading");
                int id = 0;
                foreach (PointSurveyData pd in loadData.SurveyDataCachExpand)
                {
                    foreach (var surveydata in pd.Datas)
                    {
                        id++;
                        DataRow dr = dt.NewRow();
                        dr["ID"] = id;
                        dr["Survey_point_Number"] = pd.SurveyPoint;
                        dr["Observation_Date"] = surveydata.SurveyDate;
                        dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString();
                        dr["Temperature"] = surveydata.Survey_RorT;
                        dr["Frequency"] = surveydata.Survey_ZorR;
                        dr["Remark"] = surveydata.Remark;
                        dr["UpdateTime"] = DateTime.Now;
                        dr["Tresult"] = surveydata.Tempreture;
                        dr["loadreading"] = surveydata.LoadReading;
                        dr["resultreading"] = surveydata.ResultReading;
                        dt.Rows.Add(dr);
                    }
                }
                this.dataGridView1.DataSource = dt;
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

           setEnable(false);
           Status("读取数据");
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
                    //MessageBox.Show(Config.Tick1.ToString() + "__" + Config.Tick2.ToString());
                }));
            };

            threadLoad = new Thread((cb) =>
            {
                var type = Config.InsCollection.InstrumentDic[insname];
                loadData.ClearCach();
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
            bool flag = comboType.Text == "多点位移计"||comboType.Text=="应变计组";
            DataTable dt = new DataTable();
            dt.TableName = "showTable";
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("Frequency");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            dt.Columns.Add("Tresult");
            dt.Columns.Add("loadreading");
            dt.Columns.Add("resultreading");
            dt.Columns.Add("ExcelResult");
            dt.Columns.Add("dif");
            
            if (flag)
            {
                dt.Columns.Add("F1");
                dt.Columns.Add("F2");
                dt.Columns.Add("F3");
                dt.Columns.Add("F4");
                dt.Columns.Add("F5");
                dt.Columns.Add("F6");
                dt.Columns.Add("R1");
                dt.Columns.Add("R2");
                dt.Columns.Add("R3");
                dt.Columns.Add("R4");
                dt.Columns.Add("R5");
                dt.Columns.Add("R6");
                
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
                    dr["Survey_point_Number"] = pd.SurveyPoint;
                    dr["Observation_Date"] = surveydata.SurveyDate;
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString();
                    dr["Temperature"] = surveydata.Survey_RorT;
                    dr["Frequency"] = surveydata.Survey_ZorR;
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    dr["Tresult"] = surveydata.Tempreture;
                    dr["loadreading"] = surveydata.LoadReading;
                    dr["resultreading"] = surveydata.ResultReading;
                    dr["ExcelResult"] = surveydata.ExcelResult;
                    dr["dif"] = Math.Abs(surveydata.ExcelResult - surveydata.LoadReading).ToString("#0.0000"); 
                    if (flag)
                    {
                        dr["resultreading"] = surveydata.AfterLock;
                        int i = 1;
                        foreach (var v in surveydata.MultiDatas)
                        {
                            dr["F" + i.ToString()] = v.Value.Survey_ZorR;
                            dr["R" + i.ToString()] = v.Value.LoadReading;
                            //if (comboType.Text == "应变计组")
                            //{
                            //    dr["R" + i.ToString()] = v.Value.Survey_RorT;
                            //}
                            i++;
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }
            this.dataGridView1.DataSource = dt;
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                this.dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            Status(String.Format("读取{0}个点，总共{1}条数据",Pcount, id));
           
        }
       void loaddatagridviewExpand(List<PointSurveyData> datas)
       {
           DataTable dt = new DataTable();
           dt.TableName = "showTable";
           dt.Columns.Add("ID");
           dt.Columns.Add("Survey_point_Number");
           dt.Columns.Add("Observation_Date");
           dt.Columns.Add("Observation_Time");
           dt.Columns.Add("Temperature");
           dt.Columns.Add("Frequency");
           dt.Columns.Add("Remark");
           dt.Columns.Add("UpdateTime");
           dt.Columns.Add("Tresult");
           dt.Columns.Add("loadreading");
           dt.Columns.Add("resultreading");
           dt.Columns.Add("ExcelResult");
           dt.Columns.Add("dif");
           dt.Columns.Add("F1");
           dt.Columns.Add("F2");
           dt.Columns.Add("F3");
           dt.Columns.Add("F4");
           dt.Columns.Add("F5");
           dt.Columns.Add("F6");
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
                   dr["Survey_point_Number"] = pd.SurveyPoint;
                   dr["Observation_Date"] = surveydata.SurveyDate;
                   dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString();
                   dr["Temperature"] = surveydata.Survey_RorT;
                   dr["Frequency"] = surveydata.Survey_ZorR;
                   dr["Remark"] = surveydata.Remark;
                   dr["UpdateTime"] = DateTime.Now;
                   dr["Tresult"] = surveydata.Tempreture;
                   dr["loadreading"] = surveydata.LoadReading;
                   dr["resultreading"] = surveydata.AfterLock;
                   dr["ExcelResult"] = surveydata.ExcelResult;
                    dr["dif"] = Math.Abs(surveydata.ExcelResult - surveydata.LoadReading).ToString("#0.0000");
                    int i = 1;
                    foreach (var v in surveydata.MultiDatas)
                    {
                        dr["F" + i.ToString()] = v.Value.Survey_ZorR;
                        i++;
                    }
                   dt.Rows.Add(dr);
               }
           }
           this.dataGridView1.DataSource = dt;
           for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
           {
               this.dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
           }
           Status(String.Format("读取{0}个点，总共{1}条数据", Pcount, id));

       }
       void setEnable(bool status)
       {
           btnWrite.Enabled = status;
           btnConfig.Enabled = status;
           btnRead.Enabled = status;
           comboType.Enabled = status;
           numericLimit.Enabled = status;
           btnShowNonStress.Enabled = status;
       }

    }
}
