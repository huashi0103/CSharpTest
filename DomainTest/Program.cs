using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;

namespace DomainTest
{
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        double c = 10;
    //        double b = 11;
    //        TestClass t = new TestClass();
    //        Console.WriteLine(t.Sub(c, b));
    //    }
    //}

    [Serializable]
    public class TestClass:MarshalByRefObject
    {

        public double Sub(double a,double b)
        {
            return a + b;
        }

       public string SayHi()
        {

            var dom = AppDomain.CurrentDomain;
            string version = Environment.Is64BitProcess ? "(64)" : "(86)";
            Console.WriteLine(version);
            dom.SetData("AppData", "Test");
            return "hello world!";
        }

        
    }
}
