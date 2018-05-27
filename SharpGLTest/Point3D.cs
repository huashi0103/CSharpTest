using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpGLTest
{
    public class Point3D
    {
        public double x;
        public double y;
        public double z;

        public Point3D()
        {

        }
       public Point3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Point3D(double x, double y)
        {
            this.x = x;
            this.y = y;

        }
       public static  Point3D operator + (Point3D pl, Point3D point3D)
        {
            Point3D p = new Point3D();
             p.x = pl.x + point3D.x;
	         p.y = pl.y + point3D.y;
	         p.z = pl.z + point3D.z;
	        return p;
        }

        public static Point3D operator -(Point3D pl, Point3D point3D)
        {
            Point3D p = new Point3D();
            p.x = pl.x - point3D.x;
            p.y = pl.y - point3D.y;
            p.z = pl.z - point3D.z;
            return p;
        }
        public static Point3D operator /(Point3D point3D, int sum)
        {
            Point3D p = new Point3D();
            p.x = point3D.x / sum;
            p.y = point3D.y / sum;
            p.z = point3D.z / sum;
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

            m_ptMax.x = double.MinValue;
            m_ptMax.y = double.MinValue;
            m_ptMax.z = double.MinValue; 
            m_ptMin.x = double.MaxValue;
            m_ptMin.y = double.MaxValue;
            m_ptMin.z = double.MaxValue;
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
                        m_ptMax.x = Math.Max(m_ptMax.x, ptXYZ.x);
                        m_ptMax.y = Math.Max(m_ptMax.y, ptXYZ.y);
                        m_ptMax.z = Math.Max(m_ptMax.z, ptXYZ.z);
                        m_ptMin.x = Math.Min(m_ptMin.x, ptXYZ.x);
                        m_ptMin.y = Math.Min(m_ptMin.y, ptXYZ.y);
                        m_ptMin.z = Math.Min(m_ptMin.z, ptXYZ.z);

                        m_ptVertexList.Add(ptXYZ);

                    }

                    m_ptBoxCenter = (m_ptMax + m_ptMin) / 2;
                    m_ptBoxSize = (m_ptMax - m_ptMin) / 2;
                    m_dbDistance = Math.Sqrt(m_ptBoxSize.x * m_ptBoxSize.x +
                        m_ptBoxSize.y * m_ptBoxSize.y +
                        m_ptBoxSize.z * m_ptBoxSize.z);

                }
            }
            catch(Exception ex)
            {
                

            }
        }
        
    }
}
