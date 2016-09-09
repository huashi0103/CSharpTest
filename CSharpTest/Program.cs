using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpTest
{
    class Program
    {
        static void Main(string[] args) 
        {
            NExcel excel = new NExcel();
            string filepath = @"D:\WORK\Project\三峡\三峡工程自动化文件\XIN三峡枢纽考证表格式.xls";
            excel.Open(filepath);
            var data = excel.getData(excel.getSheet(0));
            for (int i = 0; i < data.Rows.Count; i++)
            {
                for (int j = 0; j < data.Columns.Count; j++)
                {
                    Console.Write(data.Rows[i][j].ToString());
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.ReadLine();
        }
    }
}
