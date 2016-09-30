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

        private Dictionary<string, InstrumentType> InsDic = new Dictionary<string, InstrumentType>();

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
                if (this.IsDisposed) return;
                this.Invoke(new EventHandler(delegate {
                    loadTypes();
                    comboType.SelectedIndex = 0;
                    toolStripProgressLoad.Visible = false;
                    this.Text = loadData.ProjectName;
                
                }));

            });
            loadThread.Start();


            btnRead.Click += new EventHandler((o, es) => {
                btnRead.Enabled = false;
                Status("读取数据");
                toolStripProgressLoad.Visible = true;
                Action callback = () => {
                    if (this.IsDisposed) return;
                    this.Invoke(new EventHandler(delegate{
                        
                        btnRead.Enabled = true;
                        loaddatagridview(loadData.SurveyDataCach);
                        toolStripProgressLoad.Visible = false;
                        Status("处理完成");
                        ErrorMsg.OpenLog(1);
                    }));
                };
                Thread thread = new Thread((cb) => {
                    var type = InsDic["渗压计"];
                    loadData.ReadData(type);
                    Status("计算数据");
                    loadData.Calc(type);
                    Action call = cb as Action;
                    call();
                });
                thread.Start(callback);
            });
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
            foreach (var ins in loadData.Instruments) {
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
