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

namespace LoadDataCalc
{
    public partial class FormLoadCalc : Form
    {
        private LoadDataClass loadData;
        private List<string> loadFiles = new List<string>();

        /// <summary> 所有仪器名词典
        /// </summary>
        private Dictionary<string, InstrumentType> InsDic = new Dictionary<string, InstrumentType>();
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
            Status("初始化...");
            Thread loadThread = new Thread(() =>
            {
                loadData = LoadDataClass.GetInstance();
                int res = loadData.Init();
                if (res != 3)
                {
                    Status(String.Format("加载失败,{0}", res.ToString()));
                }
                else
                {
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
                }));
            });
            loadThread.Start();

            btnRead.Click += new EventHandler(btnRead_Click);
            btnWrite.Click += (o, eg) => {
                if (MessageBox.Show("确定写入数据库?", "提示", MessageBoxButtons.OKCancel) == 
                    System.Windows.Forms.DialogResult.Cancel) return;
                var instype = InsDic[ comboType.Text];
                loadData.Wirte(instype);
                Status(String.Format("写入数据库完成"));
            };
            btnConfig.Click += (o, eg) => {
                FormConfigFilePath fcf = new FormConfigFilePath();
                fcf.ShowDialog();
                Files = Config.ReadFileList();
            };
            numericLimit.ValueChanged += (o, eg) => { Config.LimitZ = (double)numericLimit.Value;
            Config.LimitT = (double)numericLimit.Value;
            };

            Type type = dataGridView1.GetType();
            PropertyInfo pi = type.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dataGridView1, true, null);
    
        }

       
        void btnRead_Click(object sender, EventArgs e)
        {
           string insname = comboType.Text;
           string errfile;
           if (!loadData.CheckFiles(Files[insname], out errfile))
           {
               MessageBox.Show(errfile+"被占用!");
               return;
           }
            btnWrite.Enabled = false;
            btnConfig.Enabled = false;
            btnRead.Enabled = false;
            Status("读取数据");
            toolStripProgressLoad.Visible = true;
            Action callback = () =>
            {
                if (this.IsDisposed) return;
                this.Invoke(new EventHandler(delegate
                {
                    toolStripProgressLoad.Visible = false;
                    btnConfig.Enabled = true;
                    btnRead.Enabled = true;
                    btnWrite.Enabled = true;
                    loaddatagridview(loadData.SurveyDataCach);
                    ErrorMsg.OpenLog(1);
                }));
            };

            threadLoad = new Thread((cb) =>
            {
                var type = InsDic[insname];
                loadData.ClearCach();
                loadData.ReadData(type,Files[insname]);
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
            this.Invoke(new EventHandler(delegate{
                statuslbl.Text = msg;
            }));
        }
       void loadTypes()
        {
            InsDic.Clear();
            foreach (int myCode in Enum.GetValues(typeof(InstrumentType)))
            {
                var instype = (InstrumentType)myCode;
                string insname = instype.GetDescription();
                InsDic.Add(insname, (InstrumentType)myCode);
            }
            comboType.Items.Clear();
            foreach (var ins in Config.Instruments) {
                comboType.Items.Add(ins.InsName);         
            }
        }
       void loaddatagridview(List<PointSurveyData> datas)
        {
            bool flag = comboType.Text == "多点位移计" || comboType.Text == "锚索测力计";
            DataTable dt = new DataTable();
            dt.TableName = "Survey_Leakage_Pressure";
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("Frequency");
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
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            dt.Columns.Add("Tresult");
            dt.Columns.Add("loadreading");
            dt.Columns.Add("resultreading");

            int id = 0;
            foreach (PointSurveyData pd in datas)
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
                    if (flag)
                    {
                        int i = 1;
                        foreach (var v in surveydata.MultiDatas)
                        {
                            dr["F" + i.ToString()] = v.Value.Survey_ZorR;
                            dr["R" + i.ToString()] = v.Value.LoadReading;
                            i++;
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }
            this.dataGridView1.DataSource = dt;
            Status(String.Format("读取{0}条数据", id));
           
        }
    }
}
