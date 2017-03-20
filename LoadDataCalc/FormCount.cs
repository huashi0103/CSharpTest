using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;

namespace LoadDataCalc
{
    public partial class FormCount : Form
    {
        public FormCount()
        {
            InitializeComponent();
        }
        private CSqlServerHelper sqlhelper = CSqlServerHelper.GetInstance();
        private InstrumentType currentType = InstrumentType.BaseInstrument;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            btnChangeName.Visible = (Config.ProCode == "XJB");
            loadTypes();
            comboType.SelectedIndexChanged += ((send, arg) => {
                currentType = Config.InsCollection.InstrumentDic[comboType.Text];
            });
            if (comboType.Items.Count > 0) comboType.SelectedIndex = 0;
            Type type = dataGridView1.GetType();
            PropertyInfo pi = type.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dataGridView1, true, null);

            btnCheckName.Click += (send, arg) => {
                string table=Config.InsCollection[currentType.GetDescription()].Fiducial_Table;
                updateData(table);
                
            };
            btnSurvey.Click += (send, arg) => {
                string table = Config.InsCollection[currentType.GetDescription()].Measure_Table;
                updateData(table);
            };
            btnResult.Click += (send, arg) =>{
                string table = Config.InsCollection[currentType.GetDescription()].Result_Table;
                updateData(table);
            };
            btnChkAllName.Click += (send, arg) => {
                List<dynamic> Counts = new List<dynamic>();
                string sql = "SELECT COUNT(ID) from {0}";
                int sum = 0;
                //foreach (var ins in Config.Instruments)
                foreach (int code in Enum.GetValues(typeof(InstrumentType)))
                {
                    var t = (InstrumentType)code;
                    string insname = t.GetDescription();
                    if (Config.InsCollection[insname] == null) continue;
                    string table = Config.InsCollection[insname].Fiducial_Table;
                    if (!CheckTable(table)) continue;
                    var result = sqlhelper.SelectFirst(String.Format(sql,table));
                    sum += (int)result;
                    var data = new
                    {
                        name = insname,
                        count = (int)result
                    };
                    Counts.Add(data);
                }
                this.dataGridView1.DataSource = Counts;
                this.dataGridView1.Update();
                AddMsg(string.Format("总共{0}个测点",sum));

            };
            btnChkAllData.Click += (send, arg) => {
                List<dynamic> Counts = new List<dynamic>();
                string sql = "SELECT COUNT(ID) from {0}";
                string AddName = String.Format("Survey_point_Number ='{0}'", txName.Text);
                string AddTime = String.Format(" Observation_Date>'{0}' AND Observation_Date<'{1}'", DateTimeStart.Value.Date.ToString(),
                    DateTimeEnd.Value.Date.ToString());
                if (chkDateTime.Checked && chkName.Checked)
                {
                    sql += " where ";
                    if (chkName.Checked) sql += AddName;
                    sql += " and ";
                    if (chkName.Checked) sql += AddTime;
                }
                else
                {
                    if (chkName.Checked) sql += " where " + AddName;
                    if (chkDateTime.Checked) sql += " where " + AddTime;
                }
                int sum = 0;
                foreach (var ins in Config.Instruments)
                {
                    string table = Config.InsCollection[ins.InsName].Measure_Table;
                    if (!CheckTable(table)) continue;
                    var result = sqlhelper.SelectFirst(String.Format(sql, table));
                    sum += (int)result;
                    var data = new
                    {
                        name = ins.InsName,
                        count = (int)result
                    };
                    Counts.Add(data);
                }
                this.dataGridView1.DataSource = Counts;
                this.dataGridView1.Update();
                AddMsg(string.Format("总共{0}条数据", sum));
            };
            btnChangeName.Click += (send, arg) => {
                if (Config.ProCode != "XJB") return;
                string table = "InstrumentPointNameTransitions";
                if (!CheckTable(table)) return;
                var result = sqlhelper.SelectData("SELECT * FROM InstrumentPointNameTransitions");
                this.dataGridView1.DataSource = result;
                this.dataGridView1.Update();
                AddMsg(string.Format("总共{0}个点", result.Rows.Count));
                 
            };
            
        }
        void updateData(string table)
        {
            string sql = "select * from {0}";
            string AddName = String.Format("Survey_point_Number ='{0}'", txName.Text);
            string AddTime=String.Format(" Observation_Date>'{0}' AND Observation_Date<'{1}'",DateTimeStart.Value.Date.ToString(),
                DateTimeEnd.Value.Date.ToString());
            if (chkDateTime.Checked && chkName.Checked)
            {
                sql +=" where ";
                if (chkName.Checked) sql += AddName;
                sql += " and ";
                if (chkName.Checked) sql += AddTime;
            }
            else
            {
                if (chkName.Checked) sql  +=" where " + AddName;
                if (chkDateTime.Checked) sql += " where " + AddTime;
            }

            if (!CheckTable(table))
            {
                AddMsg("表不存在");
                return;
            }
            ThreadPool.QueueUserWorkItem((obj) => {
                var dt = sqlhelper.SelectData(string.Format(sql, table));
                this.Invoke(new EventHandler(delegate {
                    this.dataGridView1.DataSource = dt;
                    this.dataGridView1.Update();
                    int count = dt.Rows.Count;
                    SetMsg(String.Format("查询到{0}条记录",count));
                }));
            
            });

        }        
        void AddMsg(string msg)
        {
            if (txMsg.Text.Length > 5000) txMsg.Clear();
            txMsg.AppendText(Environment.NewLine+msg);
        }
        void SetMsg(string msg)
        {
            txMsg.Text = msg;
        }
        bool CheckTable(string table)
        {
            string sql = "select count(1) from sys.objects where name = '{0}'";
            var result = sqlhelper.SelectFirst(string.Format(sql, table));
            bool flag = ((int)result) == 1;
            return flag;
        }
        void loadTypes()
        {
            comboType.Items.Clear();
            foreach (var ins in Config.Instruments)
            {
                comboType.Items.Add(ins.InsName);
            }
        }
    }
}
