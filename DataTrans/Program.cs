using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoadDataCalc;
using System.Data;

namespace DataTrans
{
    class Program
    {

        class a
        {
            public string name;
            public string test;
        }
        static  void Main(string[] args)
        {
            DataMove dm = new DataMove();
            //dm.ChangeDatabase();//改数据库字段长度
            //dm.MoveSubject();//添加组织架构
            //AddFiducial();//添加考证表

            List<a> testa = new List<a>();
           
            testa.ForEach(a => { a.name.Replace(",", ""); });
            testa.AsParallel().ForAll(a => { a.name.Replace(",", ""); });
        }
        static void AddFiducial()
        {
            DataMove dm = new DataMove();

            string sql = @"SELECT * FROM Table_PointTableName";
            CSqlServerHelper.Connectionstr = @"Data Source =localhost\MSSQLSERVER08;Initial Catalog = WHDT;User ID = sa;Password = 12345678;";
            var sqlhelper = CSqlServerHelper.GetInstance();
            var table = sqlhelper.SelectData(sql);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string ins = table.Rows[i]["Point_Name"].ToString();
                if (ins != "温度计")
                {
                    dm.AddDatas(ins);
                }
                else
                {
                    dm.AddTemprequre(ins);
                }
            }
        }
    }

    class DataMove
    {
        const string connectstr1 = @"Data Source =localhost\MSSQLSERVER08;Initial Catalog = WHDT;User ID = sa;Password = 12345678;";
        const string connectstr2 = @"Data Source =localhost\MSSQLSERVER08;Initial Catalog = XJBDatabase;User ID = sa;Password = 12345678;";
        const string connectstr3 = @"Data Source =localhost\MSSQLSERVER08;Initial Catalog = MWDatabase;User ID = sa;Password = 12345678;";

        public void AddDatas(string ins)
        {
            CSqlServerHelper.Connectionstr = connectstr2;
            var sqlhelper = CSqlServerHelper.GetInstance();
 
            Console.WriteLine(string.Format("加载源表数据{0}", ins));
            var t1 = sqlhelper.SelectFirst(string.Format(@"select Fiducial_Table from InstrumentTable where Instrument_Name='{0}'", ins));
            if (t1 == DBNull.Value) { Console.WriteLine("仪器类型不存在"); Console.Read(); }
            string tablename = t1 .ToString();
            Console.WriteLine("old tablename:" + tablename);
            if (!CheckTable(tablename))
            {
                Console.WriteLine("表不存在");
                Console.Read();
                return;
            }
            var table = sqlhelper.SelectData(string.Format(@"select * from {0}", tablename));
            Console.WriteLine(string.Format("查询到{0}条数据", table.Rows.Count));
            CSqlServerHelper.Connectionstr = connectstr1;
            tablename = sqlhelper.SelectFirst(string.Format(@"select Info_Number from Table_PointTableName where Point_Name='{0}'", ins)).ToString();
            Console.WriteLine("new tablename:" + tablename);
            Console.WriteLine("delete xjb data");
            int res = sqlhelper.InsertDelUpdate("delete FROM " + tablename + " where DZ_Name='向家坝'");
            Console.WriteLine("delete :" + res.ToString());
            var count = sqlhelper.SelectFirst(string.Format(@"select MAX(id) from {0}", tablename));
            int id = (count != DBNull.Value) ? Convert.ToInt32(count) : 1;
            Console.WriteLine("转换数据格式");
            DataTable dt = new DataTable();
            Console.WriteLine("按顺序添加列");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i].ColumnName == "Calculate_Coeffi_G")
                {
                    if (ins == "应变计") dt.Columns.Add("Childnum");
                    dt.Columns.Add("GA");
                    dt.Columns.Add("GB");
                    dt.Columns.Add("GC");
                }
                else if (table.Columns[i].ColumnName == "Remark")
                {
                    dt.Columns.Add("Auto");
                    dt.Columns.Add("Broken");

                }
                else if (table.Columns[i].ColumnName == "Lock_Value" ||
                    table.Columns[i].ColumnName =="Conversion_C")
                {
                    continue;
                }
                dt.Columns.Add(table.Columns[i].ColumnName);

            }
            Console.WriteLine("add data");
            for (int j = 0; j < table.Rows.Count; j++)
            {
                DataRow dr = dt.NewRow();
                id++;
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (table.Columns[i].ColumnName == "Lock_Value"||
                         table.Columns[i].ColumnName == "Conversion_C") continue;
                    
                    dr[table.Columns[i].ColumnName] = table.Rows[j][table.Columns[i].ColumnName];
                }
                dr["Project_Code"] = "向家坝";
                dr["Point_Code"] = DateTime.Now;
                dr["ID"] = id;
                if(dt.Columns.Contains("GA"))
                {
                    dr["GA"] = 0;
                    dr["GB"] = 0;
                    dr["GC"] = 0;
                }
                dr["Auto"] = "否";
                dr["Broken"] = "否";
                if (ins == "应变计") dr["Childnum"] = 1;

                dt.Rows.Add(dr);
            }
            dt.TableName = tablename;

            Console.WriteLine("wirte to database? y or n");
            string wono = Console.ReadLine();
            if (wono == "y")
            {
                Console.WriteLine(sqlhelper.BulkCopy(dt, false) ? "写入成功" : "写入失败");
            }
            else
            {
                Console.WriteLine("未写入");
            }
            Console.ReadLine();

        }
        public void AddTemprequre(string ins)
        {
            CSqlServerHelper.Connectionstr = connectstr2;
            var sqlhelper = CSqlServerHelper.GetInstance();

            Console.WriteLine(string.Format("加载源表数据{0}", ins));
            string tablename = sqlhelper.SelectFirst(string.Format(@"select Fiducial_Table from InstrumentTable where Instrument_Name='{0}'", ins)).ToString();
            Console.WriteLine("old tablename:" + tablename);
            if (!CheckTable(tablename))
            {
                Console.WriteLine("表不存在");
                Console.Read();
                return;
            }
            var table = sqlhelper.SelectData(string.Format(@"select * from {0}", tablename));
            Console.WriteLine(string.Format("查询到{0}条数据", table.Rows.Count));
            CSqlServerHelper.Connectionstr = connectstr1;
            tablename = sqlhelper.SelectFirst(string.Format(@"select Info_Number from Table_PointTableName where Point_Name='{0}'", ins)).ToString();
            Console.WriteLine("new tablename:" + tablename);
            Console.WriteLine("delete xjb data");
            int res = sqlhelper.InsertDelUpdate("delete FROM Table_Info_P where DZ_Name='向家坝'");
            Console.WriteLine("delete :" + res.ToString());
            int id = Convert.ToInt32(sqlhelper.SelectFirst(string.Format(@"select MAX(id) from {0}", tablename)));
            Console.WriteLine("转换数据格式");
            DataTable dt = new DataTable();
            Console.WriteLine("按顺序添加列");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i].ColumnName == "Temperature_Read")
                {
                    dt.Columns.Add("GA");
                    dt.Columns.Add("GB");
                    dt.Columns.Add("GC");
                    dt.Columns.Add("GG");
                    dt.Columns.Add("GK");
                    dt.Columns.Add("BRR");
                    dt.Columns.Add("BR");
                }
                else if (table.Columns[i].ColumnName == "Remark")
                {
                    dt.Columns.Add("Auto");
                    dt.Columns.Add("Broken");

                }
                dt.Columns.Add(table.Columns[i].ColumnName);
            }
            Console.WriteLine("add data");
            for (int j = 0; j < table.Rows.Count; j++)
            {
                DataRow dr = dt.NewRow();
                id++;
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    dr[table.Columns[i].ColumnName] = table.Rows[j][table.Columns[i].ColumnName];
                }
                dr["Project_Code"] = "向家坝";
                dr["Point_Code"] = DateTime.Now;
                dr["ID"] = id;
                dr["GA"] = 0;
                dr["GB"] = 0;
                dr["GC"] = 0;
                dr["GG"] = 0;
                dr["GK"] = 0;
                dr["BRR"] = 0;
                dr["BR"] = 0;
                dr["Auto"] = "否";
                dr["Broken"] = "否";

                dt.Rows.Add(dr);
            }
            dt.TableName = tablename;
            Console.WriteLine("wirte to database? y or n");
            string wono = Console.ReadLine();
            if (wono == "y")
            {
                Console.WriteLine(sqlhelper.BulkCopy(dt, false) ? "写入成功" : "写入失败");
            }
            else
            {
                Console.WriteLine("未写入");
            }
            Console.ReadLine();

        }
        
        public void ChangeDatabase()
        {
            CSqlServerHelper.Connectionstr = connectstr1;
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql = @"SELECT Info_Number FROM  Table_PointTableName";
            var res = sqlhelper.SelectData(sql);
            for (int i = 0; i < res.Rows.Count; i++)
            {
                string table = res.Rows[i][0].ToString();
                sql = @"alter table {0} alter column ZH varchar(120)";//修改桩号字段长度为120
                var en = sqlhelper.InsertDelUpdate(string.Format(sql, table));
                Console.WriteLine(en);
            }
            Console.ReadLine();

        }
        private string gettable(string ins,string connect)
        {
            CSqlServerHelper.Connectionstr = connect;
            var sqlhelper = CSqlServerHelper.GetInstance();
            return sqlhelper.SelectFirst(string.Format(@"select Fiducial_Table from InstrumentTable where Instrument_Name='{0}'", ins)).ToString();

        }
        public void checkName(string ins)
        {
            string table = gettable("", connectstr1);
            CSqlServerHelper.Connectionstr = connectstr1;
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql = "";
            sql = @"Select name from syscolumns Where ID=OBJECT_ID('{0}') ";
            var res = sqlhelper.SelectData(string.Format(sql, table));
            
            
        }
        bool CheckTable(string table)
        {
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql = "select count(1) from sys.objects where name = '{0}'";
            var result = sqlhelper.SelectFirst(string.Format(sql, table));
            bool flag = ((int)result) == 1;
            return flag;
        }
        public void MoveSubject()
        {
            Console.WriteLine("拷贝树的组织架构");
            CSqlServerHelper.Connectionstr = connectstr2;
            var sqlhelper = CSqlServerHelper.GetInstance();

            Console.WriteLine("加载源表数据{0}");
            var table = sqlhelper.SelectData(@"SELECT * FROM InstrumentTypeTable");
            Console.WriteLine(string.Format("查询到{0}条数据", table.Rows.Count));
            
            CSqlServerHelper.Connectionstr = connectstr1;
            Console.WriteLine("delete xjb data");
            int res = sqlhelper.InsertDelUpdate("delete FROM Table_InstrucmentType where DZ_Name='向家坝'");
            Console.WriteLine("delete :" + res.ToString());
            var count = sqlhelper.SelectFirst(@"select MAX(id) from Table_InstrucmentType");
            int id = (count != DBNull.Value) ? Convert.ToInt32(count) : 1;
            Console.WriteLine("转换数据格式");
            DataTable dt = new DataTable();
            Console.WriteLine("按顺序添加列");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i].ColumnName == "Instrument_Code")
                {
                    dt.Columns.Add("Unit");
                    dt.Columns.Add("Remark");
                }
                else if (table.Columns[i].ColumnName == "SubProject_Name")
                {
                    dt.Columns.Add("DZ_Name");
                }
                dt.Columns.Add(table.Columns[i].ColumnName);

            }
            Console.WriteLine("add data");
            for (int j = 0; j < table.Rows.Count; j++)
            {
                DataRow dr = dt.NewRow();
                id++;
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    dr[table.Columns[i].ColumnName] = table.Rows[j][table.Columns[i].ColumnName];
                }
                dr["DZ_Name"] = "向家坝";
                dr["Instrument_Code"] = DateTime.Now;
                dr["ID"] = id;
                if (dr["Instrument_Name"] == "应变计组")
                {
                    dr["Unit"] = "套";
                }
                else
                {
                    dr["Unit"] = "支";
                }
                dt.Rows.Add(dr);
            }
            dt.TableName = "Table_InstrucmentType";
            Console.WriteLine("wirte to database? y or n");
            string wono = Console.ReadLine();
            if (wono == "y")
            {
                Console.WriteLine(sqlhelper.BulkCopy(dt, false) ? "写入成功" : "写入失败");
            }
            else
            {
                Console.WriteLine("未写入");
            }
            Console.ReadLine();

        }


        public void AddSurveyData(string ins)
        {
            CSqlServerHelper.Connectionstr = connectstr2;
            var sqlhelper = CSqlServerHelper.GetInstance();

            Console.WriteLine(string.Format("加载源表数据{0}", ins));
            var t1 = sqlhelper.SelectData(string.Format(@"select * from InstrumentTable where Instrument_Name='{0}'", ins));
            if (t1.Rows.Count<1) { Console.WriteLine("仪器类型不粗在"); Console.Read(); }

            string surveyTable = t1.Rows[0]["Measure_Table"].ToString();
            string resTable = t1.Rows[0]["Result_Table"].ToString();
            Console.WriteLine("old tablename:" + surveyTable);
            if (!CheckTable(surveyTable))
            {
                Console.WriteLine("表不存在");
                Console.Read();
                return;
            }
            string sql = @"SELECT * FROM {0} as s JOIN {1} as r ON 
                            (s.Survey_point_Number=r.Survey_point_Number and s.Observation_Date=r.Observation_Date and s.Observation_Time=s.Observation_Time)";

            var sdata = sqlhelper.SelectData(string.Format(sql,surveyTable,resTable));

            Console.WriteLine(string.Format("查询到{0}条数据", sdata.Rows.Count));

           
            Console.ReadLine();
        }




    }
}
