using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cadObjArx
{
    public partial class FormToolsForCass : Form
    {
        public List<ZDinfo> list = new List<ZDinfo>();
        public FormToolsForCass(List<ZDinfo> data)
        {
            InitializeComponent();
            list = data;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
          
            this.dataGridView1.DataSource = list;
        }
    }
}
