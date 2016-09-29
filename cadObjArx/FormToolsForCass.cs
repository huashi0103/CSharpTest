using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using System.Threading;

namespace cadObjArx
{
    public partial class FormToolsForCass : Form
    {
        public List<ZDinfo> ZDDATA = new List<ZDinfo>();
        public FormToolsForCass(List<ZDinfo> data)
        {
            InitializeComponent();
            ZDDATA = data;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.dataGridView1.DataSource = ZDDATA;
        }

        private void ToExceltool_Click(object sender, EventArgs e)
        {
            string filename = @"D:\Desktop\cadTest\test.xlsx";
            Thread thread = new Thread(() => {
                excel = new Microsoft.Office.Interop.Excel.Application();
                ExportExcel(filename);
                excel.Quit();
                MessageBox.Show("OK");
            });
            thread.Start();
        }
        private Microsoft.Office.Interop.Excel.Application excel;

        void setSheetHead(_Worksheet sheet)
        {
            excel.DisplayAlerts = false;
            Range rangeLecture;
            sheet.Cells[1, 1] = "序号";
            sheet.Cells[1, 2] = "户号";
            sheet.Cells[1, 3] = "宗地代码";
            sheet.Cells[1, 4] = "权利人";
            sheet.Cells[1, 5] = "指界人";
            sheet.Cells[1, 6] = "户籍情况";
            sheet.Cells[1, 7] = "家庭人口";
            for (int i = 1; i < 8; i++)
            {

                rangeLecture = sheet.Range[sheet.Cells[1, i], sheet.Cells[4, i]];
                rangeLecture.MergeCells = true;
            }
            sheet.Cells[1, 8] = "权属来源证明";
            rangeLecture = sheet.Range[sheet.Cells[1, 8], sheet.Cells[1, 13]];

            rangeLecture.MergeCells = true;
            sheet.Cells[2, 8] = "类别";
            rangeLecture = sheet.Range[sheet.Cells[2, 8], sheet.Cells[2, 13]];
            rangeLecture.MergeCells = true;

            sheet.Cells[3, 8] = "土地";
            sheet.Cells[3, 11] = "房屋";
            sheet.Cells[4, 8] = "权属来源名称";
            sheet.Cells[4, 9] = "权利人";
            sheet.Cells[4, 10] = "面积";
            rangeLecture = sheet.Range[sheet.Cells[3, 8], sheet.Cells[3, 10]];
            rangeLecture.MergeCells = true;

            sheet.Cells[4, 11] = "权属来源名称";
            sheet.Cells[4, 12] = "权利人";
            sheet.Cells[4, 13] = "面积";
            rangeLecture = sheet.Range[sheet.Cells[3, 11], sheet.Cells[3, 13]];
            rangeLecture.MergeCells = true;

            sheet.Cells[1, 14] = "调查面积";
            rangeLecture = sheet.Range[sheet.Cells[1, 14], sheet.Cells[1, 26]];
            rangeLecture.MergeCells = true;

            sheet.Cells[2, 14] = "土地面积";
            sheet.Cells[2, 18] = "房屋面积";

            sheet.Cells[3, 14] = "小计";
            sheet.Cells[3, 15] = "主体建筑占地";
            sheet.Cells[3, 16] = "附属建筑占地";
            sheet.Cells[3, 17] = "院落";
            for (int i = 14; i < 18; i++)
            {
                sheet.Range[sheet.Cells[3, i], sheet.Cells[4, i]].MergeCells = true;
            }

            rangeLecture = sheet.Range[sheet.Cells[2, 14], sheet.Cells[2, 17]];
            rangeLecture.MergeCells = true;
            rangeLecture = sheet.Range[sheet.Cells[2, 18], sheet.Cells[2, 26]];
            rangeLecture.MergeCells = true;

            sheet.Cells[3, 18] = "自然幢建筑面积";
            sheet.Cells[4, 18] = "小计";
            sheet.Cells[4, 19] = "幢号";
            sheet.Cells[4, 20] = "面积";
            sheet.Range[sheet.Cells[3, 18], sheet.Cells[3, 20]].MergeCells = true;

            sheet.Cells[3, 21] = "逻辑幢建筑面积";
            sheet.Cells[4, 21] = "幢号";
            sheet.Cells[4, 22] = "建筑结构";
            sheet.Cells[4, 23] = "层数";
            sheet.Cells[4, 24] = "建成年份";
            sheet.Cells[4, 25] = "建筑面积";
            sheet.Cells[3, 26] = "分摊使用建筑面积";
            sheet.Range[sheet.Cells[3, 21], sheet.Cells[3, 25]].MergeCells = true;

            sheet.Range[sheet.Cells[3, 26], sheet.Cells[4, 26]].MergeCells = true;
            sheet.Cells[1, 27] = "备注";
            sheet.Range[sheet.Cells[1, 27], sheet.Cells[4, 27]].MergeCells = true;

            sheet.Range[sheet.Cells[1, 1], sheet.Cells[4, 27]].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;//居中对齐
        }
        void SetFont(Worksheet sheet)
        {
            excel.StandardFont = "宋体";
            excel.StandardFontSize = 9;
            //sheet.Range.AutoFit();//自适应
            //sheet.Range[sheet.Cells[1, 1], sheet.Cells[4, 27]].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;//居中对齐
            //sheet.Range[sheet.Cells[1, 1], sheet.Cells[rowindex, 27]].Borders.LineStyle = 1;//设置边框

        }
        void AddSheet(string sheetname,Workbook book,List<ZDinfo> list)
        {
            var result = book.Worksheets.Add();
            var sheet = book.ActiveSheet as Worksheet;
            sheet.Name = sheetname;
            SetFont(sheet);
            setSheetHead(sheet);
            double area1 = 0;
            double area2 = 0;
            int rowindex = 5;
            for (int i = 0; i < list.Count; i++)
            {
                sheet.Cells[rowindex, 1] = i+1;
                sheet.Cells[rowindex, 2] = i+1;
                sheet.Cells[rowindex, 3] = list[i].Code;
                sheet.Cells[rowindex, 4] = list[i].Owner;
                sheet.Cells[rowindex, 14] =Math.Round(list[i].Area,2);
                area1+=list[i].Area;
                double ar = 0;
                int icount = 0;
                for (int j = 0; j < list[i].Houses.Count; j++)
                {
                    ar += list[i].Houses[j].Area;
                    if (list[i].Houses[j].Isbalcony) continue;
                    sheet.Cells[rowindex + j, 21] = j + 1;
                    sheet.Cells[rowindex + j, 22] = list[i].Houses[j].Structure;
                    sheet.Cells[rowindex + j, 23] = list[i].Houses[j].FloorCount;
                    sheet.Cells[rowindex + j, 25] = Math.Round(list[i].Houses[j].Area,2);
                    icount++;

                }
                area2 += ar;
                if(icount==0)icount=1;
                sheet.Cells[rowindex, 17] = Math.Round((list[i].Area - ar) < 0 ? 0 : list[i].Area - ar,2);
                //合并
                if (icount > 1)
                {
                    for (int j = 1; j < 18; j++)
                    {
                        sheet.Range[sheet.Cells[rowindex, j], sheet.Cells[rowindex + icount - 1, j]].MergeCells = true;
                    }
                }
                rowindex += icount;
            }
            sheet.Cells[rowindex, 1] = "小计";
            sheet.Cells[rowindex, 2] = list.Count;
            sheet.Cells[rowindex, 14] = Math.Round(area1,2);
            sheet.Cells[rowindex, 25] = Math.Round(area2,2);

            sheet.Range[sheet.Cells[1, 1], sheet.Cells[4, 27]].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;//居中对齐
            sheet.Range[sheet.Cells[1, 1], sheet.Cells[rowindex, 27]].Borders.LineStyle = 1;//设置边框
            

        }
        public void ExportExcel(string filename)
        {
            excel.Visible = false;
            excel.DisplayAlerts = false;
            excel.DefaultFilePath = "";
            excel.DisplayAlerts = true;
            excel.SheetsInNewWorkbook = 1;
            var book = excel.Workbooks.Add(true);

            try
            {
                List<List<ZDinfo>> Datas = new List<List<ZDinfo>>();
                List<ZDinfo> temp = new List<ZDinfo>();
                for (int i = 0; i < ZDDATA.Count; i++)
                {
                    if (temp.Count == 0)
                    {
                        temp.Add(ZDDATA[i]);
                    }
                    else
                    {
                        if (temp[0].Code[14] == ZDDATA[i].Code[14])
                        {
                            temp.Add(ZDDATA[i]);
                        }
                        else
                        {
                            Datas.Add(temp);
                            temp = new List<ZDinfo>();
                            temp.Add(ZDDATA[i]);
                        }
                    }
                    
                }
                for (int i=0;i<Datas.Count;i++)
                {
                    AddSheet((i + 1).ToString(), book, Datas[i]);
                }
                
                book.SaveAs(filename);

            }
            catch { }
            finally
            {
                book.Close();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
                book = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

            }

        }

       
    }
}
