using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Autodesk.AutoCAD.Interop;
using System.Diagnostics;
using System.IO;
using CSharpTest;

namespace WinDraw
{
    public partial class Form1 : Form
    {
        string[] parameters = { "A", "B", "C", "K", "G", "x", "y" };
        Dictionary<string, double> Datas = new Dictionary<string, double>() { { "A", 10 },
        { "B", 10 } ,{ "C", 10 },{ "K", 10 },{ "G", 10 },{ "x", 10 },{ "y", 10 } };

        string[] Oporetions = { "+", "-", "*", "/", "(", ")", "平方" };
        public Form1()
        {
            InitializeComponent();

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            OpencadToolStripMenuItem.Click += (s, ea) =>
            {
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    OpenCad();
                });
            };
            showpic1ToolStripMenuItem.Click += delegate {
               Pic pic=new Pic();
               pictureBox1.Image = pic.pic1(1);
            };
            showpic2ToolStripMenuItem.Click += delegate
            {
                Pic pic = new Pic();
                pictureBox1.Image = pic.pic1(2);
            };
            showpic3ToolStripMenuItem.Click += delegate
            {
                Pic pic = new Pic();
                var bmp= pic.pic1(3);
                pictureBox1.Image = bmp;
                bmp.Save(@"D:\1.jpg");
                
            };
            Load();
            btnClear.Click += new EventHandler((o, eg) => { textBox1.Clear(); });
            btncalc.Click+=new EventHandler((o,eg)=>{
                string exp = textBox1.Text;
                foreach (string s in Datas.Keys)
                {
                    exp = exp.Replace(s, Datas[s].ToString());

                }
                var result = new DataTable().Compute(exp, "");
                textBox1.AppendText(String.Format("={0}={1}", exp, result));

            });
        }

        void Load()
        {
            foreach (string p in Datas.Keys)
            {
                Button btn = new Button();
                btn.Text = p;
                btn.Click += new EventHandler((o, e) => {
                    var obj = o as Button;
                    textBox1.AppendText(obj.Text);
                });
                flowLayoutPanel1.Controls.Add(btn);
            }

            foreach (string p in Oporetions)
            {
                Button btn = new Button();
                btn.Text = p;
                btn.Click += new EventHandler((o, e) =>
                {
                    var obj = o as Button;
                    if (obj.Text == "平方")
                    {
                        string exp = textBox1.Text;
                        if (exp.EndsWith(")"))
                        {
                            if (exp.LastIndexOf('(') >= 0)
                            {
                                string s = exp.Substring(exp.LastIndexOf('('));
                                textBox1.AppendText("*" + s);
                            }
                        }
                        else
                        {
                            textBox1.AppendText("*"+exp[exp.Length - 1]);
                        }
                        //textBox1.AppendText("^2");
                        
                    }
                    else
                    {
                        textBox1.AppendText(obj.Text);
                    }
                });
                flowLayoutPanel1.Controls.Add(btn);
            }
        }



        public void CloseAllInstance()
        {
            Process[] aCAD = Process.GetProcessesByName("acad");
            foreach (Process aCADPro in aCAD)
            {
                aCADPro.CloseMainWindow();
            }
        }

        private void log(string msg)
        {
            if (InvokeRequired)
            {
                this.Invoke(new EventHandler(delegate
                {
                    textBox1.AppendText(msg + Environment.NewLine);
                }));
            }
            else
            {
                textBox1.AppendText(msg + Environment.NewLine);
            }
        }
 
        //启动CAD
        private void OpenCad()
        {
            this.Invoke(new EventHandler(delegate { this.TopMost = true; }));
            //CloseAllInstance();
            string sAppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "AutoCAD文件(*.dwg)|*.dwg";
            ofd.Title = "选择CAD文件";
            //if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;
            string sPath = @"D:\Desktop\糯扎渡\坝基渗压监测重要点A断面.dwg";// 
            string sProgID = "AutoCAD.Application.18";
            AcadApplication acApp = null;
            axAcCtrl1.Src =null;
            try
            {
                log("构造cad对象");
                Process[] aCAD = Process.GetProcessesByName("acad");
                if (aCAD.Length > 0)
                {
                    acApp = (AcadApplication)Marshal.GetActiveObject(sProgID);
                }
                else
                {
                    acApp = new AcadApplication();
                    log("启动CAD中....");
                    ThreadPool.QueueUserWorkItem((state) =>
                    {
                        for (int i = 1; i < 6; i++)
                        {

                            log(i.ToString());
                            Thread.Sleep(1000);
                        }
                    });
                    Thread.Sleep(5000);
                }

                acApp.Visible = false;
                log("打开文件:" + sPath);
                acApp.Documents.Open(sPath);
                log("加载arx文件");
                string dllPath = Environment.CurrentDirectory + @"\cadObjArx.dll";
                dllPath = dllPath.Replace("\\", "\\\\");
                string sCommand = string.Format("(command \"netload\" \"{0}\")\n", dllPath);
                log(sCommand);
                acApp.ActiveDocument.SendCommand(sCommand);


                log("do SB command");
                acApp.ActiveDocument.SendCommand("SB\n");
                string newPath = Path.GetDirectoryName(sPath) + "\\" + Path.GetFileNameWithoutExtension(sPath) + "_new.dwg";
                acApp.ActiveDocument.SaveAs(newPath);
                acApp.ActiveDocument.Close(false, null);
                log("OK");
                axAcCtrl1.Src = newPath;

            }
            catch (COMException ex)
            {
                log(ex.Message);
            }
            catch (Exception ex)
            {
                log(ex.Message);
            }
            finally
            {
                acApp = null;
            }
            this.Invoke(new EventHandler(delegate { this.TopMost = false; }));
        }
        
        //winform中嵌入cad绘图控件
        private void 打开dwgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axAcCtrl1.Src=@"D:\Desktop\坝基渗压监测重要点A断面.dwg";            
        }

        private void loadarxToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void loadexcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NExcel excel = new NExcel();
            string filepath = @"D:\WORK\Project\三峡\三峡工程自动化文件\XIN三峡枢纽考证表格式.xls";
            excel.Open(filepath);
            var data = excel.getData(excel.getSheet(0));
            this.dataGridView1.DataSource = data;
        }
    }
}
