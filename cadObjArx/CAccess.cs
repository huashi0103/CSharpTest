using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using  System.Data;
using System.Data.OleDb;
using System.Windows.Media.Media3D;
using Autodesk.AutoCAD.DatabaseServices;
using DataTable = System.Data.DataTable;


namespace test
{
    public class CAccess
    {
     
        /// <summary>数据库表名</summary>
        public struct TableName
        {
            /// <summary>测点表</summary>
            public const string RAW_POINT = "raw_point";
            /// <summary>测线表</summary>
            public const string POINT_LINE = "point_line";
            /// <summary>房屋属性表</summary>
            public const string FW_INFO = "fw_info";
            /// <summary>照片表</summary>
            public const string PICTURES = "pictures";
            /// <summary>编码缓存表</summary>
            public const string CODES = "codes";
            public const string DJBASEINFO = "djbaseinfo";
            public const string XZQHCODE = "xzqhcode";
        }

        /// <summary> 连接字符串 </summary>
        private const string connectionString="Provider=Microsoft.Jet.OLEDB.4.0 ;Data Source={0}";
        private string mdbPath;
        /// <summary> 连接字符串 </summary>
        public string ConnectionString
        {
            get { return string.Format(connectionString, mdbPath); }
        }

        /// <summary>存储数据库连接</summary>
        protected OleDbConnection Connection;

        /// <summary>
        /// 构造函数：带有参数的数据库连接
        /// </summary>
        /// <param name="mdbpath"></param>
        public CAccess(string mdbpath)
        {
            mdbPath = mdbpath;
            Connection = new OleDbConnection(string.Format(connectionString,mdbpath));
        }

        /// <summary>执行SQL语句没有返回结果，如：执行删除、更新、插入等操作
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns>操作成功标志</returns>
        public bool ExecuteNonQuery(string strSQL)
        {
            bool resultState = false;

            Connection.Open();
            OleDbTransaction myTrans = Connection.BeginTransaction();
            OleDbCommand command = new OleDbCommand(strSQL, Connection, myTrans);

            try
            {
                command.ExecuteNonQuery();
                myTrans.Commit();
                resultState = true;
            }
            catch
            {
                myTrans.Rollback();
                resultState = false;
            }
            finally
            {
                Connection.Close();
            }
            return resultState;
        }

        /// <summary>执行SQL语句返回结果到DataReader中
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns>dataReader</returns>
        public OleDbDataReader ExecuteDataReader(string strSQL)
        {
            Connection.Open();
            OleDbCommand command = new OleDbCommand(strSQL, Connection);
            OleDbDataReader dataReader = command.ExecuteReader();
            Connection.Close();
            return dataReader;
        }

        /// <summary>执行SQL语句返回结果到DataSet中
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSet(string strSQL)
        {
            Connection.Open();
            DataSet dataSet = new DataSet();
            OleDbDataAdapter OleDbDA = new OleDbDataAdapter(strSQL, Connection);
            OleDbDA.Fill(dataSet, "myDataSet");

            Connection.Close();
            return dataSet;
        }

        /// <summary>执行SQL语句返回结果到DataTable中
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public System.Data.DataTable ExecuteDataTable(string strSQL)
        {
            return this.ExecuteDataSet(strSQL).Tables[0];
        }

        /// <summary>
        /// 取所有表名
        /// </summary>
        /// <returns></returns>
        public List<string> GetTableNameList()
        {
            List<string> list = new List<string>();
            try
            {
                Connection.Open();
                DataTable dt = Connection.GetSchema("Tables");
                foreach (DataRow row in dt.Rows)
                {
                    if (row[3].ToString() == "TABLE")
                        list.Add(row[2].ToString());
                }
                Connection.Close();
                return list;

            }
            catch (Exception e)
            {
                Connection.Close();
                return null;
            }
        }

        /// <summary> 添加点 </summary>
        public int AddPoint( int p_no, string p_code,Point3D pt,int p_type, int p_id, int p_status, ObjectId id)
        {
            string sql = "insert into raw_point values({0},{1},{2},{3},{4},{5},{6},{7},{8},{9})";
            sql = string.Format(sql, p_no, p_code, pt.X, pt.Y, pt.Z, p_type, p_id, p_status, id.Handle.Value);
            Connection.Open();
            OleDbTransaction myTrans = Connection.BeginTransaction();
            OleDbCommand command = new OleDbCommand(sql, Connection, myTrans);
           
            int resultState = 0;
            try
            {
                resultState= command.ExecuteNonQuery();
                myTrans.Commit();
            }
            catch
            {
                myTrans.Rollback();
                resultState = 0;
            }
            finally
            {
                Connection.Close();
            }
            return resultState;

        }
        /// <summary> 添加点</summary>
        /// <param name="pt">点</param>
        /// <param name="id">objectid</param>
        /// <param name="p_type">类型 1 外业点，2解析点</param>
        public void AddPoint(Point3D pt, ObjectId id, int p_type)
        {
            AddPoint(0,"", pt, p_type, 0, 1, id);
        }

        public int Addline(Polyline line, ObjectId id)
        {
            int result = 0;

            return result;
        }
    }

    /// <summary>图层名</summary>
    public struct LayerName
    {
        /// <summary>测点图层</summary>
        public const string RAW_POINT_LAYER = "raw_point";
        /// <summary>测线图层</summary>
        public const string POINT_LINE = "point_line";
        /// <summary>房屋图层</summary>
        public const string FW_LAYER = "JMD";
        /// <summary>界址线图层</summary>
        public const string JZX = "JZX";
        /// <summary>界址点图层</summary>
        public const string JZD = "JZD";
        /// <summary>。。</summary>
        public const string KZD = "KZD";
    }

}
