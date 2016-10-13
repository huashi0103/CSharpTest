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
        /// <summary>各仪器列表
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
                Files = Config.ReadFileList();
                if (this.IsDisposed) return;
                this.Invoke(new EventHandler(delegate {
                    loadTypes();
                    comboType.SelectedIndex = 0;
                    toolStripProgressLoad.Visible = false;
                    this.Text = Config.ProjectName;
                }));
            });
            loadThread.Start();
            btnRead.Click += new EventHandler(btnRead_Click);
            btnWrite.Click += (o, eg) => {
                if (MessageBox.Show("确定写入数据库?", "提示", MessageBoxButtons.OKCancel) == 
                    System.Windows.Forms.DialogResult.Cancel) return;
                loadData.Wirte();
                Status(String.Format("写入数据库完成"));
            };
            btnConfig.Click += (o, eg) => {
                FormConfigFilePath fcf = new FormConfigFilePath();
                fcf.ShowDialog();
                Files = Config.ReadFileList();
            };
        }

        void btnRead_Click(object sender, EventArgs e)
        {
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
                    loaddatagridview(loadData.SurveyDataCach);
                    toolStripProgressLoad.Visible = false;
                    Status("处理完成");
                    btnConfig.Enabled = true;
                    btnRead.Enabled = true;
                    btnWrite.Enabled = true;
                    ErrorMsg.OpenLog(1);
                    
                }));
            };
            string insname = comboType.Text;
            threadLoad = new Thread((cb) =>
            {
                var type = InsDic[insname];
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

                    dt.Rows.Add(dr);
                }
            }
            this.dataGridView1.DataSource = dt;
            Type type = dataGridView1.GetType();
            PropertyInfo pi = type.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dataGridView1, true, null);
           
        }
    }
}
