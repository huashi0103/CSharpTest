using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CSharpTest
{
    class Program
    {
        static void Main(string[] args) 
        {
            //NExcel excel = new NExcel();
            //string filepath = @"D:\WORK\Project\三峡\三峡工程自动化文件\XIN三峡枢纽考证表格式.xls";
            //excel.Open(filepath);
            //var data = excel.getData(excel.getSheet(0));
            //for (int i = 0; i < data.Rows.Count; i++)
            //{
            //    for (int j = 0; j < data.Columns.Count; j++)
            //    {
            //        Console.Write(data.Rows[i][j].ToString());
            //        Console.Write(" ");
            //    }
            //    Console.WriteLine();
            //}

            //Console.WriteLine("导出OK");
            //List<string> list = new List<string>();
            //string root = @"D:\AWORK\苗尾\昆明院苗尾监测资料\内观资料";
            //Action<string> fileaction = (dir) =>
            //{
            //    var files = Directory.GetFiles(dir, "*.xls");
            //    foreach (var file in files)
            //    {
            //        string filename = Path.GetFileName(file);
            //        if (filename.Contains("渗压计"))
            //        {
            //            Console.WriteLine(file);
            //            list.Add(file);
            //        }
            //    }
            //};
            //Action<string> newAction = null;
            //newAction = (dir) =>
            //{
            //    fileaction(dir);
            //    var dirs = Directory.GetDirectories(dir);
            //    if (dirs.Length > 0)
            //    {
            //        foreach (var d in dirs)
            //        {
            //            //Console.WriteLine(d);
            //            newAction(d);
            //        }
            //    }
            //    else
            //    {
            //        return;
            //    }
            //};
            //newAction(root);
            //CXML xml = new CXML();
            //string path = @"D:\WORK\Project\苗尾\typelist.xml";
            //xml.readxml(path);
            //CExcel.test();
            //string path = @"D:\WORK\Project\苗尾\昆明院苗尾监测资料\内观资料\PD45探洞（渗压计）\PD45探洞渗压计.xls";
            //foreach (var file in list)
            //{
            //    NExcel.test(file);
            //}
            List<int> a = new List<int> { 1231,0,2,100,2,4,53,23,123,4,53,7,9};
            List<int> b = new List<int>();
            for (int i = 0; i < a.Count; i++)
            {
                bool flag = true;
                for (int j = 0; j < b.Count; j++)
                {
                    if (a[i] > b[j])
                    {
                        b.Insert(j,a[i]);
                        flag = false;
                        break;
                    }
                }
                if(flag)b.Add(a[i]);
            }
            foreach (int v in b)
            {
                Console.WriteLine(v);
            }
            //string r = Regex.Replace(DateTime.Now.TimeOfDay.ToString(), @"\.\d+$", string.Empty);
            //var ts = TimeSpan.Parse(r);
            //Console.WriteLine(ts.ToString());
            //CSqlServer.test();
            //CAccessClass.Test();
            Console.ReadLine();
        }
    }
}
