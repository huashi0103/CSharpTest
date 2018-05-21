using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using WinDraw;
using System.Diagnostics;
using System.Threading;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Reflection;

namespace CSharpTest
{

    public class Mine
    {
        public int[,] Set(int r,int c,int hard)
        {
            int[,] data = new int[r,c];
            Random rd=new Random();
            for (int i = 0; i < r*c; i++)
            {
                data[i / c, i % c] = rd.Next(hard) == 1 ? 1 : 0;
            }
            return data;

        }
        public int[,] GetNewData(int[,] data)
        {
            int r = data.GetLength(0);
            int c= data.GetLength(1);
            int[,] newdata = new int[r, c];
            for (int i = 0; i < r * c; i++)
            {
                if (data[i / c, i % c] == 1)
                {
                    newdata[i / c, i % c] = 9;
                }
                else
                {
                    int d = 0;
                    for (int j = 0; j < 9; j++)
                    {
                        if (i / c + (j / 3 - 1)>=0 && i % c + (j % 3 - 1) >= 0 &&
                            i / c + (j / 3 - 1) < r && i % c + (j % 3 - 1) < c &&
                            data[i / c + (j / 3 - 1), i % c + (j % 3 - 1)] == 1)
                        {
                            d++;
                        }
                    }
                    newdata[i / c, i % c] = d;
                }
            }
            return newdata;
        }

    }
   
    delegate void testDelegate(object s);
    [Serializable]
    class Program
    {

        public class Node
        {
            public int NodeData;
            public Node LeftChild;
            public Node RightChild;
        }
        public class TreeNode
        {
            public int val;
            public TreeNode left;
            public TreeNode right;
            public TreeNode (int x)
            {
                val = x;
            }
        }

        static bool check()
        {
            int[][] array = new int[4][] { new[] { 1, 5, 9, 13 }, new[] { 2, 6, 10, 14, 20, 30 }, new[] { 3, 7, 11, 15 }, new[] { 4, 8, 12, 16, 21 } };
            int a = 20;
            Node tree = null;
            Action<int> func = null;
            func = (b) =>
            {
                Node Parent;
                var newNode = new Node() { NodeData = b };
                if (tree == null)
                {
                    tree = newNode;
                }
                else
                {
                    Node Current = tree;
                    while (true)
                    {
                        Parent = Current;
                        if (newNode.NodeData < Current.NodeData)
                        {
                            Current = Current.LeftChild;
                            if (Current == null)
                            {
                                Parent.LeftChild = newNode;
                                break;
                            }
                        }
                        else
                        {
                            Current = Current.RightChild;
                            if (Current == null)
                            {
                                Parent.RightChild = newNode;
                                //插入叶子后跳出循环
                                break;
                            }
                        }
                    }
                }
            };

            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    func(array[i][j]);
                }
            }
            var current = tree;
            if (current == null) return false;
            while (true)
            {
                if (a < current.NodeData)
                {
                    if (current.LeftChild == null) break;
                    current = current.LeftChild;
                }
                else if (a > current.NodeData)
                {
                    if (current.RightChild == null) break;
                    current = current.RightChild;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }


       static  TreeNode reConstructBinaryTree(int [] pre,int [] tin) {
           TreeNode root = reConstructBinaryTree(pre, 0, pre.Length - 1, tin, 0, tin.Length - 1);
            return root;
        }
         //前序遍历{1,2,4,7,3,5,6,8}和中序遍历序列{4,7,2,1,5,3,8,6}
        static  TreeNode reConstructBinaryTree(int [] pre,int startPre,int endPre,int [] tin,int startIn,int endIn) {
         
            if(startPre>endPre||startIn>endIn)
                return null;
            TreeNode root=new TreeNode(pre[startPre]);

            for (int i = startIn; i <= endIn; i++)
            {
                if (tin[i] == pre[startPre])
                {
                    root.left = reConstructBinaryTree(pre, startPre + 1, startPre + i - startIn, tin, startIn, i - 1);
                    root.right = reConstructBinaryTree(pre, i - startIn + startPre + 1, endPre, tin, i + 1, endIn);
                }
            }
            return root;
        }

        static double calc()
        {
            double b = 1.4;
            double p = 0.25;
            double g = 0.39526;
            double zs = 0.2070;
            double H0 = 0.2720;
            double Q1 = 50;
            while(true)
            {
                double H = (zs - 0.4361) * g + H0;
                double deltg = b * (H + p);
                double v = (Q1 / 1000.0) / deltg;
                double I=H + Math.Pow(v, 2) / (2 * 9.81);
                double F = 0.627 + 0.018 * I / p;
                double Q2 = F * 2 * Math.Sqrt(2 * 9.81) * b * Math.Pow(I, 1.5) * 1000 / 3;
                if (Math.Abs(Q2 - Q1) < 0.01)
                {
                    Q1 = Q2;
                    break;
                }
                else
                {
                    Q1 = Q2;
                }
            }
            return Q1;

        }
        public class ListNode
        {
            public int val;
            public ListNode next;
            public ListNode(int x)
            {
                val = x;
            }
        }

        static void func(string s)
        {
            Console.WriteLine(s);
        }
        static  int sum(int n)
        {
            if (n <= 0) return 0;
            return n + sum(n - 1);
        }
        static void OldTest()
        {
            string filepath = @"D:\Desktop\MWXC08配置表.doc";
            CSWord csword = null;
            List<MCU> datas = new List<MCU>();
            try
            {
                csword = new CSWord(filepath, true);
                datas = csword.test();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (csword != null) csword.Dispose();
            }
            Console.WriteLine("read over");
            CSqlServerHelper.Connectionstr = @"Data Source =DESKTOP-9SCR28N\SQLEXPRESS12;Initial Catalog = MWDatabase;User ID = sa;Password = 123;";
            CSqlServerHelper sqlhelper = CSqlServerHelper.GetInstance();

            foreach (MCU mcu in datas)
            {
                Console.Write(mcu.ToString());
                foreach (var point in mcu.SurveyPoint)
                {
                    if (string.IsNullOrEmpty(point.Value)) continue;
                    sqlhelper.DoPROCEDURE("update_Auto_Points",
                        new SqlParameter("Survey_point_number", point.Value),
                        new SqlParameter("MCU_Station", mcu.mcustation.Split('-')[0]),
                        new SqlParameter("MCU_Number", mcu.mcustation),
                        new SqlParameter("CJ_Number", point.Key),
                        new SqlParameter("IP", mcu.IP));
                }
            }
        }

        static void TestDom()
        {
            AppDomain appDomain = AppDomain.CreateDomain("testdom");

            string path = "DomainTest.dll";
            byte[] fsContent;
            using (FileStream fs = File.OpenRead(path))
            {
                fsContent = new byte[fs.Length];
                fs.Read(fsContent, 0, fsContent.Length);
            }
            appDomain.DoCallBack(() => {
                AppDomain cur = AppDomain.CurrentDomain;
                Console.WriteLine(cur.FriendlyName);
                Object obj = cur.CreateInstanceAndUnwrap("DomainTest", "DomainTest.TestClass");
                var type = obj.GetType();
                MethodInfo method = type.GetMethod("SayHi");
                var res = method.Invoke(obj, new string[] { });
                Console.WriteLine(res as string);
                var data = cur.GetData("AppData");
                Console.WriteLine(data as string);
            });
            AppDomain.Unload(appDomain);
        }
        static void w(string msg)
        {
            Console.WriteLine(msg);
        }
        static void w(string msg, params object[] msgs)
        {
            Console.WriteLine(string.Format(msg, msgs));
        }
        static void Main(string[] args) 
        {
            AutoResetEvent autoReset = new AutoResetEvent(false);
            Thread t1 = new Thread(() =>
            {
                autoReset.WaitOne();
                w("{0}:{1}","t1", DateTime.Now.ToString());
            });
            t1.Start();
            Thread t2 = new Thread(() =>
            {
                w("{0}:{1}", "t2", DateTime.Now.ToString());
                Thread.Sleep(1000);
                autoReset.Set();

            });
            t2.Start();

            int stat = 0;
            if(Interlocked.Exchange(ref stat, 1)==0)
            {
                
            }



            Console.WriteLine("OK");
            Console.ReadLine();

        }

    }
}
