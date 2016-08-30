using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpTest
{
    public class CSort
    {
 
        //插入
        public void d_insort(int[] a)
        {
            if (a.Length < 1) return;
            int i, j, t;
            for (i = 1; i < a.Length; i++)
            {
                t = a[i];
                for (j = i - 1; j >=0 && t < a[j];j-- )//把大于t的元素后移
                {
                    a[j + 1] = a[j];
                }
                a[j + 1] = t;//插入t
            }
        }
        
        //冒泡
        public void BubbleSort(int[] a)
        {
            if (a.Length < 1) return;
            int i, j, t;
            i = a.Length;
            while (i > 1)
            {//每次循环把最大的数放入有序序列的第一个
                for (j = 0; j < i - 1; j++)
                {
                    if (a[j + 1] < a[j])
                    {
                        t = a[j];
                        a[j] = a[j + 1];
                        a[j + 1] = t;
                    }
                }
                i--;
            }

        }
        
        //快排
        public void quickSort(int[] a)
        {
            if (a.Length < 1) return;
            qSort(a, 0, a.Length - 1);
        }
        private void qSort(int[] a,int left,int right)
        {//从left到right进行按left索引值进行分块//递归到1
            if (right - left < 1) return;
            int i = left, j = right;
            int x = a[i];
            while (i < j)
            {
                while ((a[j] >=x) && (i<j))
                    j --;
                a[i] = a[j];
                while ((a[i] <= x) && (j > i))
                    i++;
                a[j] = a[i];
            }
            a[i] = x;
            qSort(a, left, i - 1);
            qSort(a, i + 1, right);
        }
        
        //选择排序
        public void SelectSort(int[] a)
        {
            if (a.Length < 1) return;
            sSort(a, 0);
        }
        private void sSort(int[] a, int start)
        {//从start开始选择最小与start交换
            if (start == a.Length) return;
            int index = start;
            for (int i = start+1; i < a.Length; i++)
            {
                if (a[index]> a[i])
                {
                    index = i;
                }
            }
            int t = a[start];
            a[start] = a[index];
            a[index] = t;
            sSort(a, start + 1);
        }
        
        //堆排序
        public void heapSort(int[] a)
        {
            int t;
            int n=a.Length;
            for(int i=n/2;i>=0;i--)//初始堆
                SIFT(a,i,n);
            for (int i = n - 1; i >= 0; i--)//排序
            {//交换
                t = a[0];
                a[0] = a[i];
                a[i] = t;
                SIFT(a, 0, i - 1);//剩下的堆重新构造
            }
        }
        private void SIFT(int[] a, int i, int n)
        {//构造堆 /逆堆
            int j, T;
            T = a[i]; j = 2 * i;
            while (j < n)
            {
                if ((j < n) && (a[j] < a[j + 1])) j++;
                if (T < a[j])
                {
                    a[i] = a[j];
                    i = j;
                    j = 2 * i;
                }
                else
                {
                    j = n;
                }
                a[i] = T;
            }
        }
        
        //基数排序
        public void  rSort(int[] a)
        {
            if (a.Length < 1) return;
           // rl_sort(a,1,3);
            rr_sort(a, 0,a.Length-1,3);

        }
        private void rl_sort(int[] a,int d,int k)
        {//lsd 低位到高位
            if (d > k) return;
            int begin = 0, end = a.Length-1;
            const int radix = 10;
            int[] count = new int[radix];//基数右索引
            int[] bucket = new int[a.Length];//缓存

            for (int i = begin; i <= end; i++)
            {
                count[get(a[i], d)]++;//每个关键码的个数
            }
            for (int i = 1; i < radix; i++)
            {
                count[i] = count[i] + count[i - 1];//累加个数，对应右边界索引
            }
            for (int i = end; i >=begin; i--)
            {
                int j = get(a[i], d);//求关键码
                bucket[count[j] - 1] = a[i];//放入对应的索引位置
                count[j]--;//放一个右索引左移一
            }
            for (int i = begin, j = 0; i <= end; i++, j++)
            {
                a[i] = bucket[j];
            }
            rl_sort(a,d + 1,k);// 高位one more try
        }
        private void rr_sort(int[] a,int begin,int end,int d)
        {//MSD//高位到低位
            const int radix = 10;
            int[] count = new int[radix];//基数索引
            int[] bucket = new int[a.Length];//缓存

            for (int i = begin; i <= end; i++)
            {
                count[get(a[i], d)]++;
            }
            for (int i = 1; i < radix; i++)
            {
                count[i] = count[i] + count[i - 1];
            }
            for (int i = end; i >= begin; i--)
            {
                int j = get(a[i], d);
                bucket[count[j] - 1] = a[i];
                count[j]--;//循环减完了就变左边界了
            }
            for (int i = begin, j = 0; i <= end; i++, j++)
            {
                a[i] = bucket[j];
            }
            for (int i = 1; i < radix; i++)//0不用循环，高位必须是1开始
            {
                int p1 = begin+ count[i-1];
                int p2 = begin+count[i] - 1;
                if (p1 < p2 && d > 1)
                {
                    rr_sort(a, p1, p2, d - 1);
                }
            }
        }
        private int get(int a,int reg)
        {
            int[] d = { 1, 1, 10, 100,1000 };
            return (a / d[reg]) % 10;

        }
      
        //归并排序
        public void mergeSort(int[] a)
        {
            if (a.Length < 1) return;
            mSortPass(a, 1);
        }
        private void mSortPass(int[] a,int len)
        {
            if (a.Length<len) return;
            
            int s = 0;
            while (s + 2 * len-1 < a.Length)
            {

                mSort(a, s, s + len, s + 2 * len - 1);
                s = s + 2 * len;
            }
            int c = 0;
            if (s - 2 * len < 0)
            {
                s = 0;
                c = len;
            }
            else
            {
                s = s - 2 * len;
                c = s + 2 * len;
            }
            if (s< a.Length - 1)//最后一个序列
            {
                mSort(a, s, c, a.Length - 1);
            }
            mSortPass(a, 2 * len);

        }
        private void mSort(int[] a,int s1,int s2,int l2)
        {
            int tlen=l2 - s1 + 1;
            int[] temp = new int[tlen];
            int i = s1, j = s2, k = 0;
            while (i < s2 && j <= l2)//把小的添加到缓冲
            {
                if (a[i] <= a[j])
                {
                    temp[k] = a[i];
                    i++;
                    k++;
                }
                else
                {
                    temp[k] = a[j];
                    j++;
                    k++;
                }
           
            }
            //剩下的添加到缓冲
            while (i < s2)
            {
                temp[k] = a[i];         
                i++;
                k++;
            }
            while (j <= l2)
            {
                temp[k] = a[j];
                j++;
                k++;
            }
            Array.Copy(temp, 0, a, s1, tlen);//复制
        }
    }


}
