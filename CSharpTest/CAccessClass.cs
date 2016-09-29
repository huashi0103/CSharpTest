using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.IO;
using System.Data;

namespace CSharpTest
{
    public class CAccessClass
    {
        private OleDbConnection DbConnection;
        private const string connect = "provider=microsoft.jet.oledb.4.0;Data Source=";
        private static void log(string msg)
        {
            using (StreamWriter sw = new StreamWriter("log.txt", true))
            {
                sw.WriteLine(String.Format("{0}:{1}", DateTime.Now.ToString(), msg));

            }

        }

        public CAccessClass(string Path)
        {
            DbConnection = new OleDbConnection(connect + Path);
        }
        public DataTable SelectData(string sql)
        {
            try
            {
                DbConnection.Open();
                var myadapter = new OleDbDataAdapter();
                myadapter.SelectCommand = new OleDbCommand(sql, DbConnection);
                DataSet ds = new DataSet();
                myadapter.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                return null;

            }
            finally
            {
                DbConnection.Close();
            }


        }
      
        public int Delete(string sql)
        {
            try
            {
                DbConnection.Open();
                var cmd = new OleDbCommand(sql, DbConnection);
                return cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                DbConnection.Close();
            }


        }


        public static void Test(string path=null)
        {
            if(path==null) path = @"D:\WORK\Project\苗尾\BGKDB.MDB";
            CAccessClass cac = new CAccessClass(path);
            string sql = "select * from Sensor where SensorType='钢筋计'";
            var table = cac.SelectData(sql);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    Console.Write(table.Rows[i][j]+" ");
                }
                Console.WriteLine();
            }

            Console.WriteLine(String.Format("获取到{0}条数据", table.Rows.Count));

            
        }


    }
}
