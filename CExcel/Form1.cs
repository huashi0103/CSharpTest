using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LoadDataCalc;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;

namespace CExcel
{
    public partial class Form1 : Form
    {
        Dictionary<string, List<string>> Files = new Dictionary<string, List<string>>();
        private object Lockobj = new object();
        private List<DataTable> Datas = new List<DataTable>();
        private int index = 0;
        public Form1()
        {
            InitializeComponent();
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            loadFiles();
            test1ToolStripMenuItem.Click += delegate
            {
                var files = Files["渗压计"];
                CExcel m_excel = new CExcel();
                // files.AsParallel().ForAll(f =>
                Task task = new Task(() =>
                {
                    foreach (string f in files)
                    {
                        var tables = m_excel.GettablesFromexcel(f);
                        Console.WriteLine(f);
                        lock (Lockobj)
                        {
                            Datas.AddRange(tables);
                        }
                    }
                });
                task.ContinueWith(new Action<Task>(newtask =>
                {
                    this.dataGridView1.DataSource = Datas[index];
                    m_excel.Dispose();
                }));
                task.Start();
            };
            test2ToolStripMenuItem.Click += delegate {
                if (index >= Datas.Count) index = 0;
                this.dataGridView1.DataSource = Datas[index];
                index++;
            };

        }

        private void loadFiles()
        {
            string path = Environment.CurrentDirectory + "\\FileList.xml";

            XmlDocument xml = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            using (XmlReader reader = XmlReader.Create(path, settings))
            {
                xml.Load(reader);
                var root = xml.DocumentElement;
                var nodes = root.ChildNodes;
                foreach (XmlNode node in nodes)
                {
                    var ent = node as XmlElement;
                    string insname = ent.GetAttribute("Name");
                    List<string> fs = new List<string>();
                    foreach (XmlNode nd in node.ChildNodes)
                    {
                        fs.Add((nd as XmlElement).InnerText);
                    }
                    Files.Add(insname, fs);
                }
                reader.Close();
            }

        }

        
    }
}
