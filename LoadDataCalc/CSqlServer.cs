using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace LoadDataCalc
{
    public  class CSqlServerHelper
    {
        private static SqlConnection sqlConnection = null;
        private static CSqlServerHelper cSqlServer=null;
        public static string Connectionstr = "";
        private CSqlServerHelper()
        { }
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static  CSqlServerHelper GetInstance()
        {
            if (cSqlServer == null)
            {
                cSqlServer = new CSqlServerHelper();
                sqlConnection = new SqlConnection(Connectionstr);
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
                cmd.ExecuteScalar();
            }
            finally
            {
                sqlConnection.Close();
            }

            return dt;
            
        }
        /// <summary>查询返回查询结果的第一行的第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public object SelectFirst(string sql, params SqlParameter[] pms)
        {
            DataTable dt = new DataTable();
            try
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(sql, sqlConnection);
                cmd.Parameters.AddRange(pms);
                cmd.ExecuteScalar();
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
        public void BulkCopy(DataTable dt)
        {
            try
            {
                sqlConnection.Open();
                using (SqlBulkCopy bulk = new SqlBulkCopy(sqlConnection))
                {
                    bulk.BatchSize = 1000;
                    bulk.DestinationTableName = dt.TableName;
                    for (int i=0;i<dt.Columns.Count;i++)
                    {
                        bulk.ColumnMappings.Add(dt.Columns[0].ColumnName, dt.Columns[0].ColumnName);
                    }
                    bulk.WriteToServer(dt);
                }
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public bool Check()
        {
            try
            {
                sqlConnection.Open();
                bool res = (sqlConnection.State == ConnectionState.Open) ? true : false;
                sqlConnection.Close();
                return res;
            }
            catch
            {
                return false;

            }
        }
    }
}
