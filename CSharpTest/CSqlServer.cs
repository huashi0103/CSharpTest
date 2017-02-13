using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace CSharpTest
{
    public  class CSqlServer
    {
        private static SqlConnection sqlConnection = null;
        private static CSqlServer cSqlServer=null;
        private CSqlServer()
        { }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="connectionstr"></param>
        /// <returns></returns>
        public static  CSqlServer GetInstance(string connectionstr)
        {
            if (cSqlServer == null)
            {
                cSqlServer = new CSqlServer();
                sqlConnection = new SqlConnection(connectionstr);
            }
            return cSqlServer;
 
        }
        /// <summary>查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public DataTable SelectData(string sql, params SqlParameter[] pms)
        {
            DataTable dt = new DataTable();
            try
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(sql, sqlConnection);
                cmd.Parameters.AddRange(pms);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            finally
            {
                sqlConnection.Close();
            }

            return dt;
            
        }
        /// <summary>
        /// 增删查改，sql语句不一样接口一样
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int InsertDelUpdate(string sql, params SqlParameter[] parameters)
        {
            int result = 0;
            try
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(sql, sqlConnection);
                cmd.Parameters.AddRange(parameters);
                result = cmd.ExecuteNonQuery();
            }
            finally
            {
                sqlConnection.Close();
            }
            return result;
        }
       
        /// <summary> 批量插入数据
        /// </summary>
        /// <param name="dt">插入的数据,根据dt的表名和列名进行匹配</param>
        public void BulkCopy(DataTable dt,bool IsMap=true)
        {
            try
            {
                sqlConnection.Open();
                using (SqlBulkCopy bulk = new SqlBulkCopy(sqlConnection))
                {
                    bulk.BatchSize = 1000;
                    bulk.DestinationTableName = dt.TableName;
                    if (IsMap)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            bulk.ColumnMappings.Add(dt.Columns[0].ColumnName, dt.Columns[0].ColumnName);
                        }
                    }
                    bulk.WriteToServer(dt);
                }
            }
            finally
            {
                sqlConnection.Close();
            }
        }
        public static void test()
        {
            string connstr = " Data Source = 10.6.179.9,1433;Network Library = DBMSSOCN;Initial Catalog = MWDatabase;User ID = sa;Password = sa;";
            var sqlserver = CSqlServer.GetInstance(connstr);
            string sql = @"insert into Survey_Leakage_Pressure (ID,Survey_point_Number,Observation_Date,Observation_Time,Temperature,Frequency,Remark,UpdateTime) values
             (@ID,@Survey_point_Number,@Observation_Date,@Observation_Time,@Temperature,@Frequency,@Remark,@UpdateTime)";
            SqlParameter[] sps = new SqlParameter[8]{new SqlParameter("@ID",1),
                new SqlParameter("@Survey_point_Number","test1"),
                new SqlParameter("@Observation_Date",DateTime.Now),
                new SqlParameter("@Observation_Time","10:20"),
                new SqlParameter("@Temperature",20),
                new SqlParameter("@Frequency",3000),
                new SqlParameter("@Remark","test1"),
                new SqlParameter("@UpdateTime",DateTime.Now),  
            };
            int a = sqlserver.InsertDelUpdate(sql, sps);
            Console.WriteLine(a);

            //string sql = "select * from Survey_Leakage_Pressure ";
            //var dt = sqlserver.SelectData(sql);
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    for (int j = 0; j < dt.Columns.Count; j++)
            //    {
            //        Console.Write(dt.Rows[i][j] + " ");
            //    }
            //    Console.WriteLine();
            //}
            //sql = "update Survey_Leakage_Pressure set Temperature = 30 where Survey_point_Number='test1' ";
            //int a = sqlserver.InsertData(sql);
            //Console.WriteLine(a);
            //sql = "select * from Survey_Leakage_Pressure ";
            //dt = sqlserver.SelectData(sql);
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    for (int j = 0; j < dt.Columns.Count; j++)
            //    {
            //        Console.Write(dt.Rows[i][j] + " ");
            //    }
            //    Console.WriteLine();
            //}

            //sql = "delete from Survey_Leakage_Pressure where  Survey_point_Number='test1'";
            //int a = sqlserver.InsertDelUpdate(sql);
            //Console.WriteLine(a);


        }


    }
}
