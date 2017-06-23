using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(((System.ComponentModel.Component)(this)));
            this.skinEngine1.SerialNumber = "";
            this.skinEngine1.SkinDialogs = false;
            this.skinEngine1.SkinFile = null;
        }
        private Sunisoft.IrisSkin.SkinEngine skinEngine1;
        private bool IsClose = false;
        private System.Timers.Timer m_timer;
        private int index = 0;
        Form frm1 = new Form();
        Form frm2 = new Form();
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            notifyIcon1.Visible = false;

            var menu = new System.Windows.Forms.ContextMenu();
            menu.MenuItems.Add(new MenuItem("显示", new EventHandler((o, eg) => { this.Show(); this.WindowState = FormWindowState.Normal; })));
            menu.MenuItems.Add(new MenuItem("退出", new EventHandler((o, eg) => { IsClose = true; this.Close(); })));
    
            notifyIcon1.ContextMenu = menu;
            notifyIcon1.MouseDoubleClick += (o, eg) =>
            {
                if (eg.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    this.Show(); this.WindowState = FormWindowState.Normal;
                }
            };
            m_timer = new System.Timers.Timer(500);
            m_timer.Elapsed += (o, eg) =>
            {
                index++;
                if (index == imageList1.Images.Count) index = 0;
                Image img = imageList1.Images[index];
                Bitmap b = new Bitmap(img);
                Icon icon = Icon.FromHandle(b.GetHicon());
                notifyIcon1.Icon = icon;
            };
            this.SizeChanged += (o, eg) => {
                if (this.Disposing) return;
                if (this.WindowState == FormWindowState.Normal)
                {
                    notifyIcon1.Visible = false; //托盘图标隐藏
                }
                if (this.WindowState == FormWindowState.Minimized)//最小化事件
                {
                    this.Hide();//最小化时窗体隐藏
                    notifyIcon1.Visible = true;
                }
            };
            test2ToolStripMenuItem.Click += (o, eg) => {
                m_timer.Enabled = m_timer.Enabled ? false : true;
            };
            skinEngine1.SkinFile = Application.StartupPath + "\\MSN.ssk";
            frm1.Text = "frm1";
            frm2.Text = "frm2";
            form1ToolStripMenuItem.Click += delegate
            {
                if (!frm1.Visible)
                {
                    frm1.Show();
                    frm1.MdiParent = this;
                }
            };
            form2ToolStripMenuItem.Click += delegate
            {
                if (!frm2.Visible)
                {
                    frm2.Show();
                    frm2.MdiParent = this;
                }
            };
            frm1.Click +=(o,eg)=> frm1.Focus();
            frm2.Click += (o,eg)=> frm2.Focus();
            frm1.GotFocus += (o,eg) =>this.Text = "frm1";
            frm2.GotFocus +=  (o,eg) => this.Text = "frm2";
     
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!IsClose)
            {
                this.Hide();
                e.Cancel = true;
                notifyIcon1.Visible = true;
            }
            else
            {
                this.notifyIcon1.Dispose();
                this.m_timer.Dispose();
            }
        }
    
    }
}
