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
            changeBackColor();


            btnChekAll.Click += new EventHandler(btnChekAll_Click);
            btnPath.Click += new EventHandler(btnPath_Click);
            listviewFiles.MouseClick += new MouseEventHandler(listviewFiles_MouseClick);
            treeViewDir.MouseClick += new MouseEventHandler(treeViewDir_MouseClick);
            listviewFiles.MouseDoubleClick += new MouseEventHandler(listviewFiles_MouseDoubleClick);
            treeViewDir.MouseDoubleClick += new MouseEventHandler(treeViewDir_MouseDoubleClick);
            btnSave.Click+=new EventHandler(delegate{
                Config.WriteFilesList(Files);
                status("保存成功");
            });
            
        }

        void treeViewDir_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            Point ClickPoint = new Point(e.X, e.Y);
            TreeNode CurrentNode = treeViewDir.GetNodeAt(ClickPoint);
            treeViewDir.SelectedNode = CurrentNode;
            if (treeViewDir.SelectedNode == null || treeViewDir.SelectedNode.Tag == null) return;
            string path = (string)CurrentNode.Tag;
            System.Diagnostics.Process.Start(path);
        }

        void listviewFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            if (listviewFiles.SelectedItems.Count == 0) return;
            string path = listviewFiles.SelectedItems[0].Text;
            System.Diagnostics.Process.Start(path);
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
                ListViewGroup owngroup = GetGroup(comboType.Text);
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
        ListViewGroup GetGroup(string insname)
        {
            ListViewGroup owngroup = null;
            foreach (ListViewGroup group in listviewFiles.Groups)
            {
                if (group.Header == insname)
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
            return owngroup;
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
                    string insname = item.Group.Header.Split(':')[0];
                    Files[insname].Remove(item.Text);
                    listviewFiles.Items.Remove(item);
                    discernFilCount--;
                    status();
                }
                listviewFiles.EndUpdate();
            });
            strip.Items.Add("添加", null, (o, es) =>
            {
                listviewFiles.BeginUpdate();

                foreach (ListViewItem item in listviewFiles.SelectedItems)
                {
                    string insname = item.Group.Header;
                    string targetinsname = this.comboType.Text;
                    if (insname == targetinsname) continue;
                    Files[insname].Remove(item.Text);
                    Files[targetinsname].Add(item.Text);
                    ListViewGroup owngroup = GetGroup(targetinsname);
                    if (owngroup.Items.ContainsKey(item.Name))
                    {
                        listviewFiles.Items.Remove(item);
                        continue;
                    }
                    item.Group = owngroup;
                }
                listviewFiles.EndUpdate();
            });

            strip.Show(listviewFiles,e.Location);
            
        }

        void btnPath_Click(object sender, EventArgs e)
        {
            fileCount = 0;
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Config.DataRoot;
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
            discernFilCount = 0;
            Files = new Dictionary<string, List<string>>();
            foreach (string insname in comboType.Items)
            {
                var list = loadData.GetFiles(insname);
                Files.Add(insname, list);
            }
            loadListView();
            changeBackColor();
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
        void getFiles(List<string> list,string path,string pattern)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            //var allfiles = di.GetFiles(pattern,SearchOption.TopDirectoryOnly);
            var allfiles = di.GetFiles();
            foreach (FileInfo fi in allfiles)
            {
                if ((fi.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden &&
                         (fi.FullName.EndsWith(".xls") || fi.FullName.EndsWith(".xlsx")||
                         fi.FullName.EndsWith(".xlsm")))
                {
                    list.Add(fi.FullName);
                }

            }
        }
        void getDir(string path,TreeNode node)
        {
            List<string> list = new List<string>();
            getFiles(list, path, "*.xls");
            //getFiles(list, path, "*.xlsx");

            //list.AddRange(Directory.GetFiles(path, "*.xls"));
            //list.AddRange(Directory.GetFiles(path, "*.xlsx"));
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
                    nd.Text = d.Split('\\').Last();
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
                ListViewGroup group = new ListViewGroup(d.Key+":"+d.Value.Count);
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
        void changeBackColor()
        {
            if (Files.Count < 1) return;
            var nodes = treeViewDir.Nodes;
            foreach (TreeNode node in nodes)
            {
                checkNode(node);
            }
        }
        void checkNode(TreeNode node)
        {
            bool flag = true;
            if (node.Tag != null)
            {
                string file = node.Tag as string;
                foreach (var dic in Files)
                {
                    if (dic.Value.Contains(file)) flag = false;
                }
                if (flag) node.BackColor = Color.Yellow;
            }
            foreach (TreeNode tnode in node.Nodes)
            {
                checkNode(tnode);
            }
        }
        
         
    }
}
