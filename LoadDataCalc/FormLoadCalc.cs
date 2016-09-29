using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

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
            comboType.SelectedIndex = 0;
            string root=@"D:\WORK\Project\苗尾\昆明院苗尾监测资料\内观资料";
            loadData = LoadDataClass.GetInstance();
            loadData.DataRoot = root;
            loadData.Init();

            loadTypes();
            btnLoad.Click += new EventHandler((o, es) => {
                loadFiles = loadData.GetFiles(comboType.Text);
                listFiles.Items.Clear();
                foreach (var file in loadFiles)
                {
                    listFiles.Items.Add(Path.GetFileName(file));
                }
            });
            listFiles.SelectedIndexChanged += new EventHandler((o, es) => {
                if (listFiles.SelectedIndex < 0 || listFiles.SelectedIndex >= loadFiles.Count) return;
                textBox1.Text = loadFiles[listFiles.SelectedIndex];
            });
            btnRead.Click += new EventHandler((o, es) => {

            });
        }

        private void loadTypes()
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
                comboType.Items.Add(ins);
                        
            }
        }

        private void btncalc_Click(object sender, EventArgs e)
        {
            var type=InsDic["渗压计"];
            var calc=CalcFactoryClass.CreateInstCalc(type);
            ParamData  param=new ParamData();
            param.Gorf = 2;
            param.Korb = 2;
            param.ZeroR = 2;
            param.ZorR = 2;
            SurveyData data = new SurveyData();
            this.textBox1.Text = calc.DifBlock(param,data).ToString();
           
        }
    }
}
