using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WinDraw
{
    public class CalcCls
    {

        /// <summary>
        /// 根据两点计算距离两点连线为R的两点,默认垂足为A
        /// </summary>
        /// <param name="pointa">A 已知点</param>
        /// <param name="pointb">B 已知点</param>
        /// <param name="R">距离</param>
        /// <param name="pointc">C 待求点</param>
        /// <param name="pointd">D 待求点</param>
        /// <returns>AB两点重合返回false 正常为true</returns>
        protected bool Vertical(PointXY pointa, PointXY pointb, double R,
            ref PointXY pointc, ref PointXY pointd)
        {
            try
            {
                //（X-xa)^2+(Y-ya)^2=R*R    距离公式
                //(X-xa)*(xb-xa)+(Y-ya)*(yb-ya)=0   垂直
                //解方程得两点即为所求点
                pointc.X = pointa.X - (pointb.Y - pointa.Y) * R / Distance(pointa, pointb);
                pointc.Y = pointa.Y + (pointb.X - pointa.X) * R / Distance(pointa, pointb);

                pointd.X = pointa.X + (pointb.Y - pointa.Y) * R / Distance(pointa, pointb);
                pointd.Y = pointa.Y - (pointb.X - pointa.X) * R / Distance(pointa, pointb);

                return true;
            }
            catch
            {
                //如果A,B两点重合会报错，那样就返回false
                return false;
            }
        }
        /// <summary> 判断pt在pa到pb的左侧</summary>
        /// <returns></returns>
        protected bool IsLeft(PointXY pa, PointXY pb, PointXY pt)
        {
            //ax+by+c=0
            double a = pb.Y - pa.Y;
            double b = pa.X - pb.X;
            double c = pb.X * pa.Y - pa.X * pb.Y;
            double d = a * pt.X + b * pt.Y + c;
            if (d < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary> 计算P1P2和P3P4两条线段所在直线的交点
        /// 如果两条线段在同一直线，返回较短的线段的端点,p2或P3</summary>
        /// <param name="pt">输出交点</param>
        /// <returns>有交点返回true 否则返回false</returns>
        protected void calcIntersectionPoint(PointXY pt1, PointXY pt2, PointXY pt3, PointXY pt4, ref PointXY pt)
        {   //ax+by=c
            double a1, b1, c1, a2, b2, c2;
            a1 = pt1.Y - pt2.Y;
            b1 = pt2.X - pt1.X;
            c1 = pt1.Y * pt2.X - pt2.Y * pt1.X;

            a2 = pt3.Y - pt4.Y;
            b2 = pt4.X - pt3.X;
            c2 = pt3.Y * pt4.X - pt4.Y * pt3.X;
            double db = a1 * b2 - a2 * b1;
            if (db == 0)//平行或共线
            {
                if (a1 * pt3.X + b1 * pt3.Y == c1)//共线
                {
                    pt = (Distance(pt1, pt2) > Distance(pt3, pt4)) ? pt3 : pt2;
                }
            }
            else
            {
                pt.X = (b2 * c1 - b1 * c2) / db;
                pt.Y = (a1 * c2 - a2 * c1) / db;
            }
        }
        /// <summary>判断P1P2和P3P4线段是否相交</summary>
        protected bool IsIntersect(PointXY pt1, PointXY pt2, PointXY pt3, PointXY pt4)
        {
            //return ((max(u.s.x, u.e.x) >= min(v.s.x, v.e.x)) && //排斥实验 
            //(max(v.s.x, v.e.x) >= min(u.s.x, u.e.x)) &&
            //(max(u.s.y, u.e.y) >= min(v.s.y, v.e.y)) &&
            //(max(v.s.y, v.e.y) >= min(u.s.y, u.e.y)) &&
            //(multiply(v.s, u.e, u.s) * multiply(u.e, v.e, u.s) >= 0) && //跨立实验 
            //(multiply(u.s, v.e, v.s) * multiply(v.e, u.e, v.s) >= 0)); 
            //判断矩形是否相交
            if (Math.Max(pt1.X, pt2.X) >= Math.Min(pt3.X, pt4.X) &&
                Math.Max(pt3.X, pt4.X) >= Math.Min(pt3.X, pt4.X) &&
                Math.Max(pt1.Y, pt2.Y) >= Math.Min(pt3.Y, pt4.Y) &&
                Math.Max(pt3.Y, pt4.Y) >= Math.Min(pt1.Y, pt2.Y))
            {
                //线段跨立
                if (multiply(pt3, pt2, pt1) * multiply(pt2, pt4, pt1) >= 0 &&
                    multiply(pt1, pt4, pt3) * multiply(pt4, pt2, pt3) >= 0)
                {
                    return true;
                }

            }
            return false;
        }
        /****************************************************************************** 
        r=multiply(sp,ep,op),得到(sp-op)*(ep-op)的叉积 
        r>0:ep在矢量opsp的逆时针方向； 
        r=0：opspep三点共线； 
        r<0:ep在矢量opsp的顺时针方向 
        *******************************************************************************/
        /// <summary> 计算p1p0和p2p0叉积 </summary>
        /// <returns></returns>
        protected double multiply(PointXY pt1, PointXY pt2, PointXY p0)
        {
            //return ((sp.x - op.x) * (ep.y - op.y) - (ep.x - op.x) * (sp.y - op.y)); 
            double mult = (pt1.X - p0.X) * (pt2.Y - p0.Y) - (pt2.X - p0.X) * (pt1.Y - p0.Y);
            return mult;

        }
        /// <summary>按照距离将交点排序 </summary>
        /// <param name="temline"></param>
        /// <param name="ps">起点</param>
        /// <returns></returns>
        protected PointXY[] sortPoint(PointXY[] temline, PointXY ps)
        {
            List<PointXY> lp = new List<PointXY>();
            List<PointXY> sortlp = new List<PointXY>();
            lp.AddRange(temline);
            if (temline.Length < 1) return sortlp.ToArray();
            for (; lp.Count > 1; )
            {
                PointXY pd = lp[0];
                for (int i = 0; i < lp.Count - 1; i++)
                {
                    if (Distance(ps, pd) > Distance(ps, lp[i + 1]))
                    {
                        pd = lp[i + 1];
                    }
                }
                lp.Remove(pd);
                sortlp.Add(pd);
            }
            sortlp.Add(lp[0]);
            return sortlp.ToArray();
        }

        /// <summary>
        /// 根据两点计算两点连线上距离A点L的点 输出C点为距离B近的点上的点 D为距离B远的点
        /// </summary>
        /// <param name="pointa">A点 默认为计算距离A点L的点</param>
        /// <param name="pointb">B点</param>
        /// <param name="L">距离</param>
        /// <param name="pointc">返回点C</param>
        /// <param name="pointd">返回点D 有时候没用</param>
        /// <returns></returns>
        protected bool GapP(PointXY pointa, PointXY pointb, double L, ref PointXY pointc, ref PointXY pointd)
        {
            try
            {
                PointXY pc = new PointXY();
                PointXY pd = new PointXY();
                //(north-xa)^2+(east-ya)^2=L    两点距离公式
                //(north-xa)*(ya-yb)-(east-ya)(xa-xb)=0   直线平行条件，注意，是CA和AB平行（实际是C点在AB上）
                pc.X = pointa.X +
                    (pointa.X - pointb.X) * L / Distance(pointa, pointb);
                pc.Y = pointa.Y +
                    (pointa.Y - pointb.Y) * L / Distance(pointa, pointb);

                pd.X = pointa.X -
                    (pointa.X - pointb.X) * L / Distance(pointa, pointb);
                pd.Y = pointa.Y -
                    (pointa.Y - pointb.Y) * L / Distance(pointa, pointb);

                pointc = Distance(pc, pointb) > Distance(pd, pointb) ? pd : pc; //取距离B近的点
                pointd = Distance(pc, pointb) > Distance(pd, pointb) ? pc : pd;//取距离B远的点

                return true;
            }
            catch
            {
                //如果A,B两点重合会报错，那样就返回false
                return false;
            }
        }

        /// <summary> 返回两点的距离 </summary>
        /// <param name="pa"></param>
        /// <param name="pb"></param>
        /// <returns></returns>
        public double Distance(double xa, double ya, double xb, double yb)
        {
            double L;
            L = Math.Sqrt(Math.Pow(xa - xb, 2) + Math.Pow(ya - yb, 2));
            return L;
        }
        public double Distance(PointXY pa, PointXY pb)
        {
            return Distance(pa.X, pa.Y, pb.X, pb.Y);
        }

        /// <summary> 用VS自带算法判断点是否在区域内 </summary>
        /// <param name="pt"></param>
        /// <param name="pointsArray"></param>
        /// <returns></returns>
        public bool PtInPolygon(PointXY pd, PointXY[] pdsArray)
        {
            PointF pt = new PointF();
            pt.X = (float)pd.X;
            pt.Y = (float)pd.Y;
            List<Point> pl = new List<Point>();
            for (int i = 0; i < pdsArray.Length; i++)
            {
                Point p = new Point();
                p.X = (int)pdsArray[i].X;
                p.Y = (int)pdsArray[i].Y;
                pl.Add(p);
            }
            if (pl.Count < 3) return false;
            using (System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath())
            {
                gp.AddPolygon(pl.ToArray());
                return gp.IsVisible(pt);
            }
        }

        /// <summary>
        /// 求线段的方位角0-360
        /// </summary>
        /// <param name="pa">线段起点</param>
        /// <param name="pb">线段终点</param>
        /// <returns>从0度顺时针偏转到该线段所划过的角度</returns>
        protected double Angle(PointXY pa, PointXY pb)
        {
            double Ang = 0;
            double a;
            try
            {
                a = Math.Acos((pb.X - pa.X) / Distance(pa, pb));

                if (pb.Y - pa.Y < 0)
                {
                    a = 2 * Math.PI - a;
                }
                Ang = a * 180d / Math.PI;
                return Ang;
            }
            catch
            {
                Ang = 0;
                return Ang;
            }
        }

        /// <summary>角度到弧度</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double AngleToRadian(double value)
        {
            return value * Math.PI / 180d;
        }

        /// <summary> 弧度到角度</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double RadianToAngle(double value)
        {
            return value * 180d / Math.PI;
        }

    }

    public struct PointXY
    {
        public double Y;
        public double X;
 
    }
}
