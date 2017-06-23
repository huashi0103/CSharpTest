using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mine
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private TabControl tabControl1 = new TabControl();
        private bool StartMove = false;
        private bool flag = true;
        private TabPage MovePage = null;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            tabControl1.Dock = DockStyle.Fill;
            
            this.Controls.Add(tabControl1);
            this.Controls.SetChildIndex(tabControl1, 0);
            frm1ToolStripMenuItem.Click += delegate {
                var frm1 = new Form();
                frm1.Text = "frm1";
                var btn=new Button();
                btn.Text = "btn";
                btn.Click += delegate { MessageBox.Show("this is frm1"); btn.Text = "frm1"; };
                frm1.Controls.Add(btn);
                addfrm(frm1);
            };
            frm2ToolStripMenuItem.Click += delegate
            {
                var frm1 = new Form();
                frm1.Text = "frm2";
                var btn = new Button();
                btn.Text = "btn";
                btn.Click += delegate { MessageBox.Show("this is frm2"); };
                frm1.Controls.Add(btn);
                addfrm(frm1);
            };
            frm3ToolStripMenuItem.Click += delegate
            {
                var frm1 = new Form();
                frm1.Text = "frm3";
                var btn = new Button();
                btn.Text = "btn";
                btn.Click += delegate { MessageBox.Show("this is frm3"); };
                frm1.Controls.Add(btn);
                addfrm(frm1);
            };
            tabControl1.AllowDrop = true;
            Func<Point, TabPage> GetTabPageByTab = (point) =>
            {
                for (int i = 0; i < this.tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.GetTabRect(i).Contains(point))
                    {
                        return this.tabControl1.TabPages[i];
                    }
                }
                return null;
            };
            tabControl1.MouseDown += (o, eg) => {
                if (flag)
                {
                    StartMove = true;
                    MovePage=GetTabPageByTab(new Point(eg.X, eg.Y));
                }
                flag = true;
            };
            tabControl1.MouseUp += (o, eg) => {
                StartMove = false;
            };
            tabControl1.SelectedIndexChanged += (o, eg) => {
                flag = false;//切换的时候因为失去焦点的原因，会触发down事件不触发up事件这里做屏蔽
            };
            tabControl1.MouseMove += (o, eg) => {
                if (StartMove && flag)
                {
                    if (MovePage != null)
                    {
                        this.DoDragDrop(MovePage, DragDropEffects.None);
                    }
                }
            };
            tabControl1.ControlAdded += delegate {
                StartMove = false;
                flag = false;
            };
            tabControl1.DragOver += (o, eg) => {
                eg.Effect = DragDropEffects.None;
                if (tabControl1.TabPages.Count < 2) return;
                TabPage page = (TabPage)eg.Data.GetData(typeof(TabPage));
                var frm1 = page.Controls[0] as Form;
                frm1.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                frm1.Parent = null;
                frm1.TopLevel = true;
                frm1.Owner = this;
                frm1.Location = new Point(eg.X, eg.Y);
                frm1.Show();
                tabControl1.TabPages.Remove(page);
                MovePage = null;
                frm1.Move += (oo, ee) =>
                {
                    for (int i = 0; i < tabControl1.TabPages.Count; i++)
                    {
                        if (tabControl1.GetTabRect(i).Contains(this.PointToClient(frm1.Location)))
                        {
                            addfrm(frm1);
                        }
                    }
                };

            };
           
        }
        private void addfrm(Form frm)
        {
            if (tabControl1.Visible == false) tabControl1.Visible = true;
            bool flag = true;
            frm.TopLevel = false;
            frm.Dock = DockStyle.Fill;
            frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            frm.Owner = null;
            foreach (TabPage page in tabControl1.TabPages)
            {
                if (page.Controls.Count < 1)
                {
                    page.Controls.Add(frm);
                    page.Text=frm.Text;
                    flag = false;
                    frm.Show();
                    return;
                }
            }
            if (flag)
            {
                var page=new TabPage(frm.Text);
                page.Controls.Add(frm);
                tabControl1.TabPages.Add(page);
                frm.Show();
            }
          
        }
    }    
}
