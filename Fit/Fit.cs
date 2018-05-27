using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;


namespace Fit
{

    public class Fit 
    {

        Point3D GetCenter(IList<Point3D> points)
        {

            Point3D sum = new Point3D();
            foreach (var p in points)
            {
                sum = sum + p;
            }

            return sum / points.Count;
            
        }

        public double[] PlaneFitSvd(IList<Point3D> points)
        {
            var center = GetCenter(points);
            var n = points.Count;
            var b_arr = new double[n, 3];
            var list = new List<Point3D>();
            foreach (var p in points)
            {
                list.Add(center - p);
            }
            for (int i = 0; i < n; i++)
            {
                b_arr[i, 0] = list[i].X;
                b_arr[i, 1] = list[i].Y;
                b_arr[i, 2] = list[i].Z;
               // b_arr[i, 2] = 1;
            }
            var m = Matrix<double>.Build;
            var mb = m.DenseOfArray(b_arr);
            var res = mb.Svd(true);
            var a = res.VT[2, 0];
            var b = res.VT[2, 1];
            var c = res.VT[2, 2];
            var d = -(a * center.X + b * center.Y + c * center.Z);
            return new double []{ a,b,c,d };

        }

        //d
        double get_d(Point3D p,Point3D pi)
        {
            return Math.Sqrt((p.X - pi.X) * (p.X - pi.X) + (p.Y - pi.Y) * (p.Y - pi.Y) + (p.Z - pi.Z) * (p.Z - pi.Z));
        }
   
        const double toleranceAngle = 0.0000001;
        const double tolerance = 0.000001;

        public double[] CylinderFit(IList<Point3D> points)
        {
            double a = 1, b = 1, c = 1, x0 = 0, y0 = 0, z0 = 0, R = 0;
            //double a0 = 1, b0 = 1, c0 = 1, x00 = 0, y00 = 0, h00 = 0, R0 = 0;

            var n = points.Count;

            var center = GetCenter(points);
            x0 = center.X;
            y0 = center.Y;
            z0 = center.Z;
            double sumr = 0;
            foreach (var p in points)
            {
                sumr += get_d(p, center);
            }
            R= sumr/n;

            var   loop_times = 0;
            var while_flag = true;
            while (while_flag)
            {
                if (a < 0)
                {
                    a = -a; b = -b; c = -c;
                }
                else if (a == 0)
                {
                    if (b < 0)
                    {
                        b = -b; c = -c;
                    }
                    else if (b == 0)
                    {
                        if (c < 0)
                            c = -c;
                    }
                }

                var s = Math.Sqrt(a*a + b*b + c*c) + 0.0000001;       //防止a b c 同时为0
                a = a / s + 0.0000001; b = b / s + 0.0000001; c = c / s + 0.0000001;
                double[,] B = new double[n, 7];
                double[,] L = new double[n, 1];
                for (int i = 0; i < n; i++)
                {
                    //数据计算
                     var D = a * (points[i].X - x0) + b * (points[i].Y - y0) + c * (points[i].Z - z0);       //D=a*(X(i)-x0)+b*(Y(i)-y0)+c*(Z(i)-z0);
                    var dx = x0 + a * D - points[i].X;
                    var dy = y0 + b * D - points[i].Y;
                    var dz = z0 + c * D - points[i].Z;//dx=x0+a*D-X(i);dy=y0+b*D-Y(i);dz=z0+c*D-Z(i);
                    var S = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2) + Math.Pow(dz, 2));    //S=sqrt(dx^2+dy^2+dz^2);
                    B[i,0] = (dx * (a * (points[i].X - x0) + D) + dy * b * (points[i].X - x0) + dz * c * (points[i].X - x0)) / S;   //b1
                    B[i,1] = (dx * a * (points[i].Y- y0) + dy * (b * (points[i].Y - y0) + D) + dz * c * (points[i].Y - y0)) / S;
                    B[i,2] = (dx * a * (points[i].Z - z0) + dy * b * (points[i].Z - z0) + dz * (c * (points[i].Z - z0) + D)) / S;
                    B[i,3] = (dx * (1 - Math.Pow(a, 2)) - dy * a * b - dz * a * c) / S;
                    B[i,4] = (-dx * a * b + dy * (1 - Math.Pow(b, 2)) - dz * b * c) / S;
                    B[i,5] = (-dx * a * c - dy * b * c + dz * (1 - Math.Pow(c, 2))) / S;
                    B[i,6] = -1;
                    L[i,0] = R - S;  //l=[R-S];  //L=[L;l];
                }
                //3. 计算矩阵C(2*7)和W(2*1)
                var d_temp1 = 1 - Math.Pow(a, 2) - Math.Pow(b, 2) - Math.Pow(c, 2);

                double[,] C = new double[2, 7];
                double[,] W = new double[2, 1];

                if (Math.Abs(a) >= Math.Abs(b) && Math.Abs(a) >= Math.Abs(c))
                {
                    C[0, 0] = 2 * a;
                    C[0, 1] = 2 * b;
                    C[0, 2] = 2 * c;
                    C[1, 3] = 1;
                    W[0, 0] = d_temp1;
                    W[1, 0] = center.X - x0;
                }
                if (Math.Abs(b) >= Math.Abs(a) && Math.Abs(b) >= Math.Abs(c))
                {
                    C[0, 0]= 2 * a;
                    C[ 0, 1]= 2 * b;
                    C[0, 2]= 2 * c;
                    C[ 1, 4]= 1;
                    W[0, 0]= d_temp1;
                    W[ 1, 0]= center.Y - y0;
                }
                if (Math.Abs(c) >= Math.Abs(a) && Math.Abs(c) >= Math.Abs(b))
                {
                    C[ 0, 0]=2 * a;
                    C[ 0, 1]= 2 * b;
                    C[0, 2]= 2 * c;
                    C[1, 5]=1;
                    W[ 0, 0]= d_temp1;
                    W[ 1, 0]= center.Z - z0;
                }
                //4. 计算para矩阵
                //Nbb=B'*B;U=B'*L;
                //N=[Nbb C';C zeros(2)];
                //UU=[U;W];
                //para=inv(N)*UU;
                var m = Matrix<double>.Build;
                var mB = m.DenseOfArray(B);
                var mL = m.DenseOfArray(L);
                var p = m.DenseIdentity(n);
                var mBT = mB.Transpose();
                var Nbb = mBT * p * mB;
                var U = mBT * p * mL;
                double[,] arr_N =new double[9, 9];

                //4.1 计算矩阵N
                // N= |Nbb(7*7)  C'(7*2)|
                //    |C  (2*7)  O(2*2) |
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        arr_N[ i, j]= Nbb[i,j];
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        arr_N[i + 7, j] = C[i, j];
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        arr_N[ j, i + 7]=C[ i, j];
                    }
                }
                var arr_U = new double[9, 1];
                for (int i = 0; i < 7; i++)
                {
                    arr_U[i, 0] = U[i, 0];
                }

                arr_U[7, 0] = W[0,0];
                arr_U[8, 0] = W[1, 0];

                //4.2 计算矩阵para
                var N = m.DenseOfArray(arr_N);
                var UU= m.DenseOfArray(arr_U);
                var N_inv = N.Inverse();       //para=inv(N)*UU;
                var para = N_inv * UU;
                a = a + para[ 0, 0];      //a=a+para(1);b=b+para(2);c=c+para(3);
                b = b + para[1, 0];
                c = c + para[ 2, 0];
                x0 = x0 +para[ 3, 0];   //x0=x0+para(4);y0=y0+para(5);z0=z0+para(6);
                y0 = y0 + para[ 4, 0];
                z0 = z0 + para[ 5, 0];
                R = R + para[ 6, 0];
                loop_times ++;     //t=t+1

                //5. 计算改正值 小于限差
                d_temp1 = para[ 0, 0];
                for (int i = 1; i < 7; i++)
                {
                    if (Math.Abs(d_temp1) < Math.Abs(para[ i, 0]))
                        d_temp1 = para[ i, 0];
                }
                if (Math.Abs(d_temp1) > tolerance)
                    while_flag = true;
                else
                    while_flag = false;

            };

            return new double[] { a, b, c, x0, y0, z0, R };
        }


    }

}