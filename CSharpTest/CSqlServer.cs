﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace CSharpTest
{
    public sealed class CSqlServerHelper
    {
       //private static SqlConnection sqlConnection = null;
        private static CSqlServerHelper cSqlServer=null;
        /// <summary>
        /// 使用之前赋值
        /// </summary>
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
                //sqlConnection = new SqlConnection(Connectionstr);
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
            var sqlConnection = new SqlConnection(Connectionstr);
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
            var sqlConnection = new SqlConnection(Connectionstr);
            try
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(sql, sqlConnection);
                cmd.Parameters.AddRange(pms);
                return cmd.ExecuteScalar();
            }
            finally
            {
                sqlConnection.Close();
            }

        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="proname"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        public object DoPROCEDURE(string proname, params SqlParameter[] pms)
        {
            var sqlConnection = new SqlConnection(Connectionstr);
            try
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand(proname, sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(pms);
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                sqlConnection.Close();
            }
        }


        /// <summary>
        /// 增删查改，sql语句不一样接口一样
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int InsertDelUpdate(string sql, params SqlParameter[] parameters)
        {
            var sqlConnection = new SqlConnection(Connectionstr);
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
        public bool BulkCopy(DataTable dt,bool IsMap=true)
        {
            var sqlConnection = new SqlConnection(Connectionstr);
            try
            {
                sqlConnection.Open();
                using (SqlBulkCopy bulk = new SqlBulkCopy(Connectionstr, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    bulk.BatchSize = dt.Rows.Count;
                    bulk.DestinationTableName = dt.TableName;
                    if (IsMap)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            bulk.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                    }
                    bulk.WriteToServer(dt);
                
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
        /// <summary>
        /// 检查数据库是否可以连接
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            var sqlConnection = new SqlConnection(Connectionstr);
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
