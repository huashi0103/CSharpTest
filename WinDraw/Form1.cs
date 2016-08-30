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

namespace WinDraw
{
    public partial class Form1 : Form
    {
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
                pictureBox1.Image = pic.pic1(3);
            };



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
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;
            string sPath = ofd.FileName; //@"D:\Desktop\坝基渗压监测重要点A断面.dwg";// 
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
    }
}
