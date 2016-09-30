using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using System.Data;
using NPOI.XSSF.UserModel;
using System.Globalization;


namespace CSharpTest
{
    /// <summary>
    /// 调用NPOI读写EXCEL
    /// </summary>
   public class NExcel
    {
       private HSSFWorkbook hssfworkbook;
       private string filepath;

       public void Open(string file)
       {
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))  
            {  
                hssfworkbook = new HSSFWorkbook(fs);  
            } 
       }
       /// <summary>
       /// 获取sheet
       /// </summary>
       /// <param name="index"></param>
       /// <param name="sheetname"></param>
       /// <returns></returns>
       public ISheet getSheet(int index,string sheetname=null)
       {
           if(hssfworkbook==null)return null;
           ISheet sheet;
           if(sheetname==null)
           {
                sheet =hssfworkbook.GetSheetAt(index);
           }else
           {
               sheet=hssfworkbook.GetSheet(sheetname);
           }
           return sheet;

       }
       /// <summary>
       /// 读取excel文件
       /// </summary>
       /// <param name="sheet"></param>
       /// <returns></returns>
       public DataTable getData(ISheet sheet)
       {
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();  
            DataTable dt = new DataTable();  
            for (int j = 0; j < (sheet.GetRow(0).LastCellNum); j++)  
            {  
                dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());  
            }  
            while (rows.MoveNext())  
            {
                if (rows.Current == null) continue ;
                HSSFRow row = (HSSFRow)rows.Current;
                DataRow dr = dt.NewRow();  
                for (int i = 0; i < row.LastCellNum-1; i++)  
                {
                    var cell = row.GetCell(i);
                    if (cell != null) cell.SetCellType(CellType.String);
                    dr[i] = cell == null ? null : cell.ToString();
                        
                }  
                dt.Rows.Add(dr);  
            }  
            return dt;  
       }
       
       /// <summary>
       /// 写excel文件
       /// </summary>
       /// <param name="filePath"></param>
       /// <param name="dts"></param>
       public void WriteExcel( string filePath,params DataTable[] dts)
       {
           if (string.IsNullOrEmpty(filePath)) return;
           NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
           foreach (var dt in dts)
           {
               if (null == dt && dt.Rows.Count==0)continue;
                NPOI.SS.UserModel.ISheet sheet = book.CreateSheet(dt.TableName);
                NPOI.SS.UserModel.IRow row = sheet.CreateRow(0);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    row.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NPOI.SS.UserModel.IRow row2 = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        row2.CreateCell(j).SetCellValue(Convert.ToString(dt.Rows[i][j]));
                    }
                }
           }
           // 写入到客户端  
           using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
           {
               book.Write(ms);
               using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
               {
                   byte[] data = ms.ToArray();
                   fs.Write(data, 0, data.Length);
                   fs.Flush();
               }
               book = null;
           }
       }

       private static void log(string msg)
       {
           string path="D:\\test1.txt";
           using (StreamWriter sw = new StreamWriter(path, true))
           {
               sw.WriteLine(msg);
           }
       }
       
       public static void test(string path)
       {
           //path = @"D:\WORK\Project\白鹤滩\数字边坡数据下红岩渗压计.xlsx";
           using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
           {
               IFormatProvider culture = CultureInfo.CurrentCulture;
                var workbook = WorkbookFactory.Create(path);
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    var psheet = workbook.GetSheetAt(i);
                    System.Collections.IEnumerator rows = psheet.GetRowEnumerator();
                    int rowcn = 1;//行计数
                    while (rows.MoveNext())
                    {
                        try
                        {
                            rowcn++;
                            if (rows.Current == null) continue;
                            IRow row = (IRow)rows.Current;
                            var cell = row.GetCell(0);
                            if (!cell.IsMergedCell && String.IsNullOrEmpty(cell.ToString())) break;
                            if (cell.CellType != CellType.Numeric) continue;//用第一列的值是否是数字来判断，时间也是数字
                            var date = cell.DateCellValue.ToString("MM/dd/yyyy");
                            double Survey_ZorR = double.Parse(row.GetCell(2).ToString());//频率/基准电阻
                            double Survey_RorT = double.Parse(row.GetCell(3).ToString());//温度
                            string Remark = row.GetCell(6) == null ? "" : row.GetCell(6).ToString();
                            DateTime SurveyDate = DateTime.Parse(date + " " + row.GetCell(1).ToString(), culture, DateTimeStyles.NoCurrentDateDefault);
                            string msg=String.Format("{0},{1},{2},{3},{4}", SurveyDate, Survey_ZorR, Survey_RorT, Remark,rowcn);
                            //log(msg)
                            Console.WriteLine(msg);
                        }
                        catch(Exception ex)
                        {
                            string msg=string.Format("{0},{1},{2},{3}",psheet.SheetName,rowcn,rowcn,path);
                            log(msg);
                            Console.WriteLine(msg);
                            continue;
                        }
                    }
                }
              
           }


       }
            

    }
}
