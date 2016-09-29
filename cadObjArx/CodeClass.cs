using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cadObjArx
{

    public class Family
    {
        public string code;
        public ZDinfo ZD;
        public string Owner;
    }

   public class ZDinfo
    {
       public string Code { get;set; }
       public string Owner { get; set; }
       public double Area { get; set; }
       public string Group { get; set; }
       public List<House> Houses = new List<House>();
       public List<Balcony> Balconyes = new List<Balcony>();
       public int PartCount = 0;
    }
   public class House
   {
       /// <summary>结构 </summary>
       public string Structure;
       /// <summary>层数</summary>
       public int FloorCount = 1;
       public double Area;
       public bool Isbalcony = false;
   }
    public class Balcony:House
    {
         public double Area;
    }
   public class Tools
   {
       public static void Sort(List<ZDinfo> list)
       {
           if (list.Count < 1) return;
           int i, j;
           ZDinfo t;
           i = list.Count;
           while (i > 1)
           {//每次循环把最大的数放入有序序列的第一个
               for (j = 0; j < i - 1; j++)
               {
                   if (IsBig(list[j ],list[j+1]))
                   {
                       t = list[j];
                       list[j] = list[j + 1];
                       list[j + 1] = t;
                   }
               }
               i--;
           }
       }
       private static bool IsBig(ZDinfo zd1,ZDinfo zd2)
       {
           double number1 = double.Parse(zd1.Code.Substring(14));
           double number2 = double.Parse(zd2.Code.Substring(14));
           return number1 > number2;
    
       }
   }
}
