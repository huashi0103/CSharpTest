using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fit
{
    class Program
    {

        static void Main(string[] args)
        {

            FitTest ft = new FitTest();
            ft.LogEvent += L;
            ft.PlaneFitTest();
            ft.CylinderFitTest();


            Console.ReadLine();
        }

        static void  L(string msg)
        {
            Console.WriteLine($"{DateTime.Now.ToString()}:{msg}");
        }
    }



    public class FitTest
    {

        public event  Action<string> LogEvent;

        void l(string msg)
        {
            LogEvent?.Invoke(msg);
        }
        string l(double[] d)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in d)
            {
                sb.Append($"{b:F6}" + " ");
            }
            return sb.ToString();
        }

        public IList<Point3D> LoadData(string path)
        {
            List<Point3D> res = new List<Point3D>();

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var ss = line.Split(' ');
                        Point3D p = new Point3D();
                        int index = 0;
                        while (string.IsNullOrWhiteSpace(ss[index]))
                        {
                            index++;
                        }
                        double.TryParse(ss[index],out  p.X);
                        index++;
                        while (string.IsNullOrWhiteSpace(ss[index]))
                        {
                            index++;
                        }
                        double.TryParse(ss[index], out p.Y);
                        index++;
                        while (string.IsNullOrWhiteSpace(ss[index]))
                        {
                            index++;
                        }
                        double.TryParse(ss[index], out p.Z);
                        res.Add(p);
                    }
                }

                return res;
            }
            catch(Exception ex)
            {
                l($"LoadData err:{ex.Message}");
            }
            return null;

        }
        Fit fit = new Fit();
        string planePath = @"D:\matlab\plane_data.txt";
        public void PlaneFitTest()
        {
            l("测试平面拟合: PlaneFitTest start ========");
            var data = LoadData(planePath);
            var res = fit.PlaneFitSvd(data);
            l("计算结果:");
            l(l(res));

            l("测试平面拟合:end PlaneFitTest end======");
        }

        string cylinder = @"D:\matlab\cylinder_data.txt";
        public void CylinderFitTest()
        {
            l("测试圆柱拟合: CylinderFitTest start ========");
            var data = LoadData(cylinder);
            var res = fit.CylinderFit(data);
            l("计算结果:");
            l(l(res));

            l("测试圆柱拟合拟合:end CylinderFitTest end======");

        }
    }
}
