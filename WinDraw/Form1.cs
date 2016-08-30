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
                OpenCad(); 
                log("程序5秒后退出");
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    Thread.Sleep(5000);
                    this.Invoke(new EventHandler(delegate { this.Close(); }));
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

            string sAppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "AutoCAD文件(*.dwg)|*.dwg";
            ofd.Title = "选择CAD文件";
            ofd.ShowDialog();
            string sPath = ofd.FileName;

            string sProgID = "AutoCAD.Application.18";
            AcadApplication acApp = null;
            try
            {
                if (System.Diagnostics.Process.GetProcessesByName("acad").Count() > 0)
                {
                    log("已经启动cad，获取cad对象");
                    acApp = (AcadApplication)Marshal.GetActiveObject(sProgID);

                }
                else
                {
                    log("未启动cad,新建cad对象，启动cad");
                    //Type acType = Type.GetTypeFromProgID(sProgID);
                    //acApp = (AcadApplication)Activator.CreateInstance(acType, true);
                    acApp = new AcadApplication();

                }

                string sCommand = "";
                if (System.IO.File.Exists(sPath))
                {
                    log("打开文件:" + sPath);
                    System.Threading.Thread.Sleep(300);
                    acApp.Documents.Open(sPath, null, null);
                }
                else
                {
                    //保存
                    while (true)
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "AutoCAD文件(*.dwg)|*.dwg";
                        sfd.Title = "新建CAD文件";
                        if (sfd.ShowDialog() == DialogResult.Cancel) return;
                        sPath = sfd.FileName;
                        if (System.IO.File.Exists(sPath))
                        {
                            if (MessageBox.Show("文件已经存在，要覆盖吗", "确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                try
                                {
                                    System.IO.File.Delete(sPath);
                                    acApp.ActiveDocument.SaveAs(sPath);
                                    break;
                                }
                                catch
                                {
                                    MessageBox.Show("该文件不能覆盖，请重新指定输出路径");
                                }
                            }
                        }
                        else
                        {
                            log("新建文件");
                            acApp.ActiveDocument.SaveAs(sPath);
                            log("关闭文件");
                            acApp.Documents.Close();
                            log("打开刚刚新建的文件");
                            acApp.Documents.Open(sPath);
                            break;
                        }
                    }
                }
                acApp.Visible = true;
                log("打开cad完成");
            }
            catch (Exception ex)
            {
                log(ex.Message);
            }
        }
        
        //winform中嵌入cad绘图控件
        private void 打开dwgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axAcCtrl1.Src=@"D:\Desktop\坝基渗压监测重要点A断面.dwg";            
        }
    }
}
