using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fit
{
    public class Point3D
    {
        public double X;
        public double Y;
        public double Z;

        public Point3D()
        {

        }
       public Point3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public Point3D(double x, double y)
        {
            this.X = x;
            this.Y = y;

        }
       public static  Point3D operator + (Point3D pl, Point3D point3D)
        {
            Point3D p = new Point3D();
             p.X = pl.X + point3D.X;
	         p.Y = pl.Y + point3D.Y;
	         p.Z = pl.Z + point3D.Z;
	        return p;
        }

        public static Point3D operator -(Point3D pl, Point3D point3D)
        {
            Point3D p = new Point3D();
            p.X = pl.X - point3D.X;
            p.Y = pl.Y - point3D.Y;
            p.Z = pl.Z - point3D.Z;
            return p;
        }
        public static Point3D operator /(Point3D point3D, int sum)
        {
            Point3D p = new Point3D();
            p.X = point3D.X / sum;
            p.Y = point3D.Y / sum;
            p.Z = point3D.Z / sum;
            return p;
        }



    }


    public class DisplayDoc
    {
        public IList<Point3D> m_ptVertexList;//数据列表
        public List<Point3D> m_ptArea;
        public double[] m_distance=new double[365];
        public double m_dbDistance;   //当前显示立方体半径
        public  Point3D m_ptBoxCenter;//立方体中心
        public  Point3D m_ptBoxSize;//当前立方体尺寸
        public  Point3D m_ptMax;//立方体最远点
        public  Point3D m_ptMin;//立方体最近点

        public DisplayDoc()
        {
            m_ptMax = new Point3D();
            m_ptMin = new Point3D();
            m_ptBoxSize = new Point3D();
            m_ptBoxCenter = new Point3D();

            m_ptMax.X = double.MinValue;
            m_ptMax.Y = double.MinValue;
            m_ptMax.Z = double.MinValue; 
            m_ptMin.X = double.MaxValue;
            m_ptMin.Y = double.MaxValue;
            m_ptMin.Z = double.MaxValue;
        }


        public void LoadData(string path)
        {
            try
            {
                m_ptVertexList = new List<Point3D>();
                using (StreamReader sr = new StreamReader(path))
                {
                   while(!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        var s = line.Split(',');
                        if (s.Length < 3)
                            continue;
                        double x = 0, y = 0, z = 0;
                        double.TryParse(s[0], out x);
                        double.TryParse(s[1], out y);
                        double.TryParse(s[2], out z);
                        var ptXYZ = new Point3D(x, y, z);
                        m_ptMax.X = Math.Max(m_ptMax.X, ptXYZ.X);
                        m_ptMax.Y = Math.Max(m_ptMax.Y, ptXYZ.Y);
                        m_ptMax.Z = Math.Max(m_ptMax.Z, ptXYZ.Z);
                        m_ptMin.X = Math.Min(m_ptMin.X, ptXYZ.X);
                        m_ptMin.Y = Math.Min(m_ptMin.Y, ptXYZ.Y);
                        m_ptMin.Z = Math.Min(m_ptMin.Z, ptXYZ.Z);

                        m_ptVertexList.Add(ptXYZ);

                    }

                    m_ptBoxCenter = (m_ptMax + m_ptMin) / 2;
                    m_ptBoxSize = (m_ptMax - m_ptMin) / 2;
                    m_dbDistance = Math.Sqrt(m_ptBoxSize.X * m_ptBoxSize.X +
                        m_ptBoxSize.Y * m_ptBoxSize.Y +
                        m_ptBoxSize.Z * m_ptBoxSize.Z);

                }
            }
            catch
            {

            }
        }
        
    }
}
