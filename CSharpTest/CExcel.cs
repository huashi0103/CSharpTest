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
                var sheet = book.Worksheets.Add() as Worksheet;
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
                ////合并竖着的单元格
                //Range rangeLecture = sheet.get_Range(sheet.Cells[1, 1], sheet.Cells[2,  1]);
                //rangeLecture.Application.DisplayAlerts = false;
                //rangeLecture.Merge(Missing.Value);
                //rangeLecture.Application.DisplayAlerts = true;
                ////合并横着的单元格

                //Range rangeProgram = sheet.get_Range(sheet.Cells[1, 1], sheet.Cells[1, 2]);//获取需要合并的单元格的范围
                //rangeProgram.Application.DisplayAlerts = false;
                //rangeProgram.Merge(Missing.Value);
                //rangeProgram.Application.DisplayAlerts = true
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
    }
}
