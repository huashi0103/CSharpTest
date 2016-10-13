﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace LoadDataCalc
{
    public partial class FormConfigFilePath : Form
    {
        private LoadDataClass loadData;
        Dictionary<string, List<string>> Files = new Dictionary<string, List<string>>();
        private int fileCount = 0;//总文件数
        private int discernFilCount = 0;//识别的文件数
        public FormConfigFilePath()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            loadData=LoadDataClass.GetInstance();
            txPath.Text = Config.DataRoot;
            listviewFiles.Columns.Add("文件");
            listviewFiles.ShowGroups = true;
            
            loadTypes();
            loadAll();
            Files = Config.ReadFileList();
            if (Files != null) loadListView();

            btnChekAll.Click += new EventHandler(btnChekAll_Click);
            btnPath.Click += new EventHandler(btnPath_Click);
            listviewFiles.MouseClick += new MouseEventHandler(listviewFiles_MouseClick);
            treeViewDir.MouseClick += new MouseEventHandler(treeViewDir_MouseClick);
            btnSave.Click+=new EventHandler(delegate{
                Config.WriteFilesList(Files);
                status("保存成功");
            });
            
        }

        void treeViewDir_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right) return;
            Point ClickPoint = new Point(e.X, e.Y);
            TreeNode CurrentNode = treeViewDir.GetNodeAt(ClickPoint);
            treeViewDir.SelectedNode = CurrentNode;
            if (treeViewDir.SelectedNode == null||treeViewDir.SelectedNode.Tag==null) return;
            ContextMenuStrip strip = new ContextMenuStrip();
            strip.Items.Add("添加", null, (o, es) =>
            {
                string file = (string)treeViewDir.SelectedNode.Tag;
                ListViewGroup owngroup = null;
                foreach (ListViewGroup group in listviewFiles.Groups)
                {
                    if (group.Header == comboType.Text)
                    {
                        owngroup = group;
                        break;
                    }
                }
                if (owngroup == null)
                {
                    owngroup = new ListViewGroup(comboType.Text);
                    listviewFiles.Groups.Add(owngroup);
                }
                if (!owngroup.Items.ContainsKey(file))
                {
                    ListViewItem item = new ListViewItem(file, owngroup);
                    item.Name = file;
                    listviewFiles.Items.Add(item);
                    discernFilCount++;
                    status();
                }
            });
            strip.Show(treeViewDir, e.Location);
           
        }

        void listviewFiles_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right) return;
            if (listviewFiles.SelectedItems.Count == 0) return;
            ContextMenuStrip strip = new ContextMenuStrip();
            strip.Items.Add("移除", null, (o, es) =>
            {
                listviewFiles.BeginUpdate();
                foreach (ListViewItem item in listviewFiles.SelectedItems)
                {
                    string insname = item.Group.Header;
                    Files[insname].Remove(item.Text);
                    listviewFiles.Items.Remove(item);
                    discernFilCount--;
                    status();
                }
                listviewFiles.EndUpdate();
            });
            strip.Show(listviewFiles,e.Location);
            
        }

        void btnPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                if (Config.DataRoot != fbd.SelectedPath)
                {
                    Config.DataRoot = fbd.SelectedPath;
                    txPath.Text = fbd.SelectedPath;
                    loadAll();
                    Config.WriteConfig();
                }
            }
        }
        void btnChekAll_Click(object sender, EventArgs e)
        {
            Files = new Dictionary<string, List<string>>();
            foreach (string insname in comboType.Items)
            {
                var list = loadData.GetFiles(insname);
                Files.Add(insname, list);
            }
            loadListView();
        }

        void loadAll()
        {
            if (!Directory.Exists(Config.DataRoot))
            {
                MessageBox.Show(String.Format("路径‘{0}’不存在", Config.DataRoot)); 
                return;
            }
            treeViewDir.Nodes.Clear();
            TreeNode root = new TreeNode();
            root.Text = Config.DataRoot;
            treeViewDir.Nodes.Add(root);
            getDir(Config.DataRoot, root);
            treeViewDir.Update();
            treeViewDir.ExpandAll();
            status();
           }
        void status(string msg = null)
        {
            string ss = msg == null ? String.Format("目录下总文件数:{0},识别后的文件数:{1}", fileCount, discernFilCount) : msg;
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(delegate {
                    statuslbl.Text = ss;
                }));
                return;
            }
            statuslbl.Text = ss;
           
        }
        void getDir(string path,TreeNode node)
        {
            List<string> list = new List<string>();
            list.AddRange(Directory.GetFiles(path, "*.xls"));
            list.AddRange(Directory.GetFiles(path, "*.xlsx"));
            foreach (string file in list)
            {
                TreeNode nd = new TreeNode();
                nd.Text = Path.GetFileName(file);
                nd.Tag = file;
                fileCount++;
                node.Nodes.Add(nd);
            }
            var dirs = Directory.GetDirectories(path);
            if (dirs.Length > 0)
            {
                foreach (var d in dirs)
                {
                    TreeNode nd = new TreeNode();
                    nd.Text = d;
                    node.Nodes.Add(nd);
                    getDir(d,nd);
                }
            }
            else
            {
                return;
            }
        }
        void loadTypes()
        {
            comboType.Items.Clear();
            foreach (var ins in Config.Instruments)
            {
                comboType.Items.Add(ins.InsName);
            }
            if (comboType.Items.Count > 1) comboType.SelectedIndex = 0;
        }
        void loadListView()
        {
            listviewFiles.Groups.Clear();
            listviewFiles.Items.Clear();
            listviewFiles.BeginUpdate();
            
            foreach (var d in Files)
            {
                ListViewGroup group = new ListViewGroup(d.Key);
                listviewFiles.Groups.Add(group);
                foreach (var f in d.Value)
                {
                    ListViewItem item = new ListViewItem(f, group);
                    item.Name = f;
                    listviewFiles.Items.Add(item);
                    discernFilCount++;
                }
            }
            listviewFiles.Columns[0].Width = -1;
            listviewFiles.EndUpdate();
            status();
        }
    }
}