using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SDataTable = System.Data.DataTable;
using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Reflection;

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

        public static void test()
        {
            string path = @"D:\WORK\Project\苗尾\昆明院苗尾监测资料\内观资料\PD45探洞（渗压计）\PD45探洞渗压计.xls";
            var excel1 = new Application();
            excel1.Visible = false;
            
            var workbook = excel1.Application.Workbooks.Open(path);
            //foreach (var sh in workbook.Worksheets)
            {
                //Worksheet psheet = sh as Worksheet;
                Worksheet psheet = workbook.Worksheets[1];
                int rowcount = psheet.UsedRange.Cells.Rows.Count;
                int colcount = psheet.UsedRange.Columns.Count;
                Range range;
                for (int iRow = 11; iRow <= rowcount; iRow++)//从第11行开始读
                {
                    Range startCell = (Range)psheet.Cells[iRow, 1];
                    Range endCell = (Range)psheet.Cells[iRow, 4];
                    range = psheet.Range[startCell, endCell];
                    var value = (object[,])range.get_Value(Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault);
                    if (value[1,1]==null||string.IsNullOrEmpty(value[1, 1].ToString())) break;
                    //for (int i = 1; i < value.GetLength(1); i++)
                    //{
                    //    Console.Write(value[1,i]);
                    //    Console.Write(":");
                    //}
                    if (value[1, 1] != null && value[1, 2] != null)
                    {
                        string data = value[1, 1].ToString();
                        string time = DateTime.FromOADate(Convert.ToDouble(value[1, 2])).ToString();
                        DateTime dt = DateTime.Parse(data.Split(' ')[0] + " " + time.Split(' ')[1]);
                        Console.WriteLine("{0}:{1}", iRow, dt.ToString());
                    }
                    else
                    {
                        Console.WriteLine("{0}", iRow);
                    }
                }

            }
            excel1.DisplayAlerts = false;
            excel1.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excel1);
            GC.Collect();
            excel1 = null;
 
        }

     
    }
}
