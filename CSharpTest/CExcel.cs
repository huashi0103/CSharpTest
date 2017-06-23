using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;



using SDataTable = System.Data.DataTable;
using Microsoft.Office.Interop.Excel;

namespace CSharpTest
{
    /// <summary>
    /// 调用com接读写excel
    /// </summary>
    public class CExcel
    {
        
        private string filepath = "";
        private Microsoft.Office.Interop.Excel.Application excel;
        private Workbook workbook = null;
        public CExcel()
        {
            excel = new Application();
            excel.Visible = false;
            excel.DisplayAlerts = false;
        }
       /// <summary>
       /// 打开Excel文件 
       /// </summary>
       /// <param name="filename">文件名</param>
        public void OpenExcel(string filename)
        {
            workbook = excel.Application.Workbooks.Open(filename);
        }
        /// <summary>
        /// 获取一个sheet
        /// </summary>
        /// <param name="index">sheet索引</param>
        /// <param name="sheetname">sheet名,默认为空，为空以第一个参数为准，否则以名为准</param>
        /// <returns>返回worksheet对象</returns>
        public Worksheet GetSheet(int index,string sheetname=null)
        {
            if (workbook == null) return null;
            var sheet = workbook.Worksheets.get_Item(index);
            if (sheetname == null) return sheet;
            foreach (var sh in workbook.Worksheets)
            {
                sheet = sh as Worksheet;
                if (sheet.Name == sheetname) break;
            }
            return sheet;

        }
        /// <summary>
        /// 关闭workbook
        /// </summary>
        public void Closeworkbook()
        {
            if (workbook != null) workbook.Close();
        }

        /// <summary>
        /// 释放excel对象
        /// </summary>
        public void Dispose()
        {
            excel.Quit();
            int ProID = 0;
            GetWindowThreadProcessId(new IntPtr(excel.Hwnd), out ProID);
            var pro = Process.GetProcessById(ProID);
            if (pro != null) pro.Kill();
        }
        /// <summary>
        /// 读取一个sheet内容
        /// </summary>
        /// <param name="psheet"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public SDataTable GetTableFromSheet(Worksheet psheet, int index = 0)
        {
            int rowcount = psheet.UsedRange.Cells.Rows.Count;
            int colcount = psheet.UsedRange.Columns.Count;
            Range range;
            SDataTable dt = new SDataTable();
            dt.TableName = psheet.Parent.Name + psheet.Name;
            DataColumn dc;
            int ColumnID = 1;
            int col = 0;
            while (col <= colcount)
            {
                dc = new DataColumn();
                dc.DataType = typeof(string);
                dc.ColumnName = col.ToString();
                dt.Columns.Add(dc);
                col++;
            }
            for (int iRow = 1; iRow <= rowcount; iRow++)
            {
                var dr = dt.NewRow();
                for (int iCol = 1; iCol <= colcount; iCol++)
                {
                    range = psheet.Cells[iRow, iCol] as Range;
                    string content = "";
                    if (range.Value != null)
                    {
                        content = range.Value.ToString();
                    }
                    dr[iCol - 1] = content;
                }
                dt.Rows.Add(dr);
            }
            return dt;

        }
        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="table"></param>
        /// <param name="filename"></param>
        public void Datatabletoexcel(string filename, params SDataTable[] tables)
        {
            excel.DefaultFilePath = "";
            excel.DisplayAlerts = true;
            excel.SheetsInNewWorkbook = 1;
            var book = excel.Workbooks.Add(true);
            foreach (var table in tables)
            {
                var result = book.Worksheets.Add();
                var sheet=book.ActiveSheet;
                sheet.Name = table.TableName;
                int rownum = table.Rows.Count;
                int colnum = table.Columns.Count;
                int rowindex = 1;
                int columnindex = 0;
                int row = 0;
                int col = 0;
                for (int i = 0; i < rownum; i++)
                {
                    col = 0;
                    for (int j = 0; j < colnum; j++)
                    {
                        sheet.Cells[row + 1, col + 1] = table.Rows[i][j] == null ? "" : table.Rows[i][j].ToString();
                        col++;
                    }
                    row++;
                }
                #region//合并单元格
                //Range rangeLecture = sheet.Range[sheet.Cells[1, 1], sheet.Cells[2, 1]];//左上和右下
                //rangeLecture.MergeCells = true;

                #endregion
            }
            excel.DisplayAlerts = false;
            book.SaveAs(filename);
            book.Close();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
            book = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        /// <summary>设置字体
        /// </summary>
        /// <param name="sheet"></param>
        void SetFont(Worksheet sheet)
        {
            excel.StandardFont = "宋体";
            excel.StandardFontSize = 9;
            //sheet.Range.AutoFit();//自适应
            //sheet.Range[sheet.Cells[1, 1], sheet.Cells[4, 27]].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;//居中对齐
            //sheet.Range[sheet.Cells[1, 1], sheet.Cells[rowindex, 27]].Borders.LineStyle = 1;//设置边框

        }

        [DllImport("user32", EntryPoint = "GetWindowThreadProcessId")]
        static extern int GetWindowThreadProcessId(IntPtr hwnd, out int pid);
        public static void test()
        {
            string path = @"D:\AWORK\向家坝\向家坝11月数据\11月数据(2016.11)\11月数据\渗压计、测压管、水文孔、量水堰\水位孔\水位孔（一期）.xlsx";
            Application excel1 = new Application();
            excel1.Visible = false;
            excel1.DisplayAlerts = false;
            Workbook workbook = excel1.Application.Workbooks.Open(path);
            try
            {
                //zOH1-1
                //foreach (Worksheet psheet in workbook.Worksheets)
                {
                    Worksheet psheet = workbook.Worksheets["zOH1-1"];
                    int rowcount = psheet.UsedRange.Cells.Rows.Count;
                    int colcount = psheet.UsedRange.Columns.Count;
                    for (int iRow = 1; iRow <= rowcount; iRow++)
                    {
                        for (int iCol = 1; iCol <= colcount; iCol++)
                        {
                            var range = psheet.Cells[iRow, iCol] as Range;
                            if (range == null || range.Value == null) continue;
                            Console.Write(range.Value.ToString());
                            if (iCol != colcount) Console.Write(",");
                        }
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                workbook.Close();
                excel1.Quit();
                int ProID = 0;
                GetWindowThreadProcessId(new IntPtr(excel1.Hwnd), out ProID);
                var pro = Process.GetProcessById(ProID);
                if (pro != null) pro.Kill();
            }
        }

     
    }
}
