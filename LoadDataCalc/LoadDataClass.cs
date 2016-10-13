/*
 * 读取文件、读数据、写数据
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Diagnostics;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;


namespace LoadDataCalc
{
    /// <summary>
    /// 读取计算表格和配置文件
    /// </summary>
    public class LoadDataClass
    {
       //唯一实例
        private static  LoadDataClass loadData = null;
        //数据库实例
        private CSqlServerHelper sqlhelper = null;
        //程序集目录
        private string Assemblydir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private LoadDataClass()
        {
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static LoadDataClass GetInstance()
        {
            if (loadData == null)
            {
                loadData = new LoadDataClass();
            }
            return loadData;
        }
        /// <summary>初始化,返回进行到第几步
        /// 0-写默认模板 1-读取配置文件，2-初始化数据库 3-全部完成
        /// </summary>
        /// <returns></returns>
        public int Init()
        {
            int res = 0;
            try
            {
                Config.WriteDifaultConfig();//写一个默认模板
                res = 1;
                //读取本项目配置文件
                if (!Config.LoadConfig()) return res;
                res = 2;
                CSqlServerHelper.Connectionstr = Config.DataBase;
                sqlhelper = CSqlServerHelper.GetInstance();
                if (sqlhelper.Check())
                {
                    res = 3;
                }
            }
            catch
            { 
            }
            return res;

        }

        /// <summary>
        /// 获取包含关键词的文件，关键词为空，返回所有文件
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>文件</returns>
        public List<string> GetFiles(string insname)
        {
            string[] keyword = Config.Instruments.Where(x => x.InsName == insname).FirstOrDefault().KeyWord.ToArray();

            List<string> files = new List<string>();
            getDir(Config.DataRoot, files);
            if (keyword == null)
            {
                return files;
            }
            else
            {
                List<string> refiles = new List<string>();
                foreach (var file in files)
                {
                    string filename = Path.GetFileName(file);
                    bool flag = false;
                    foreach (string key in keyword)
                    {
                        if (filename.Contains(key))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag) refiles.Add(file);
                    
                }
                return refiles;
            }
 
        }
     
        //数据缓存
        public List<PointSurveyData> SurveyDataCach = new List<PointSurveyData>();
        public List<ErrorMsg> ErrorMsgCach = new List<ErrorMsg>();
        /// <summary> 当前处理仪器类
        /// </summary>
        private ProcessData process = null;

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="instype"></param>
        public void ReadData(InstrumentType instype,List<string>files)
        {
            string temppath = Assemblydir + "\\log\\temp_error.log";
            if (File.Exists(temppath)) File.Delete(temppath);//临时文件每次读取清空
            process = ProcessFactory.CreateInstance(instype);
            if (process == null) return;            
            SurveyDataCach.Clear();
            ErrorMsgCach.Clear();
            foreach (string file in files)
            {
                List<PointSurveyData> datas = new List<PointSurveyData>();
                List<ErrorMsg> msgs = new List<ErrorMsg>();
                process.ReadData(file, out datas, out msgs);
                SurveyDataCach.AddRange(datas);
                ErrorMsgCach.AddRange(msgs);
            }
           
        }
        /// <summary>
        /// 计算
        /// </summary>
        public void Calc(InstrumentType instype, string expression=null)
        {
            BaseInstrument inscalc = CalcFactoryClass.CreateInstCalc(instype);
            foreach (var pd in SurveyDataCach)
            {

                ParamData param = inscalc.GetParam(pd.SurveyPoint, instype.ToString());
                if (param == null)
                {
                    ErrorMsg.Log(String.Format("PARAM:{0}",pd.SurveyPoint));//保存读取参数的错误
                    pd.IsCalc = false;
                    continue;
                }
                if (expression != null)
                {
                    param.InsCalcType = CalcType.AutoDefine;
                }

                switch (param.InsCalcType)
                {
                    case CalcType.DifBlock:
                        foreach (SurveyData sd in pd.Datas)
                        {
                            inscalc.DifBlock(param, sd);
                        }
                        break;
                    case CalcType.ShakeString:
                        foreach (SurveyData sd in pd.Datas)
                        {
                            inscalc.ShakeString(param, sd);
                        }
                        break;
                    case CalcType.AutoDefine:
                        foreach (SurveyData sd in pd.Datas)
                        {
                            inscalc.AutoDefined(param, sd, expression);
                        }
                        break;
                }
                pd.IsCalc = true;
            }
        }
        /// <summary>写数据
        /// </summary>
        /// <returns></returns>
        public void Wirte()
        {
            if (process != null)
            {
                int result= process.WriteSurveyToDB(SurveyDataCach);
                ErrorMsg.Log(String.Format("写入{0}行测值",result));
                result=process.WriteResultToDB(SurveyDataCach);
                ErrorMsg.Log(String.Format("写入{0}行成果值", result));
            }
        }
        
        private void getDir(string path, List<string> FileList)
        {
            FileList.AddRange(Directory.GetFiles(path,"*.xls"));
            FileList.AddRange(Directory.GetFiles(path, "*.xlsx"));
            var dirs = Directory.GetDirectories(path);
            if (dirs.Length > 0)
            {
                foreach (var d in dirs)
                {
                    getDir(d, FileList);
                }
            }
            else
            {
                return;
            }
        }

    }

    /// <summary> 读写数据基类，项目涉及的仪器类型都要继承实现该类
    /// </summary>
   public  abstract class  ProcessData
    {
        /// <summary>仪器类型
        /// </summary>
        public InstrumentType InsType { get; set; }
        public double ErrorLimitZR = 10;
        public double ErrorLimitRT = 10;
        public string SurveyTableName = "";
        public string ResultTableName = "";

        /// <summary> 从excel文件中读取数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract  void ReadData(string path, out List<PointSurveyData> datas,out List<ErrorMsg>errors);
        /// <summary>把测量数据写入到数据库
        /// </summary>
        /// <returns></returns>
        public abstract int WriteSurveyToDB(List<PointSurveyData> datas);
        /// <summary>把计算后的结果数据写入数据库
        /// </summary>
        /// <returns></returns>
        public abstract int WriteResultToDB(List<PointSurveyData> datas);

       /// <summary> 保存读取单个文件的错误信息
       /// </summary>
       /// <param name="filepath"></param>
       /// <param name="msgs"></param>
        public void Log(string filepath,List<ErrorMsg> msgs)
        {
            ErrorMsg.Log(String.Format("FILE:{0},{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),filepath));
            foreach (var msg in msgs)
            {
                ErrorMsg.Log(msg.ToString());
            }

        }

        /// <summary>检查数据正确性
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="lastsd"></param>
        /// <param name="tablename"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        protected bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, out ErrorMsg err)
        {
            err = new ErrorMsg();
            if (lastsd != null) return true;//第一行数据不处理
            //先跟上一次的数据做比较，不超过限值直接return
            if (Math.Abs(sd.Survey_RorT - lastsd.Survey_RorT) < info.ErrorLimitRT &&
                Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < info.ErrorLimitZR) return true;
            //超过限值跟上一个月同一天作比较
            var sqlhelper = CSqlServerHelper.GetInstance();
            DateTime dt = sd.SurveyDate.Date.AddMonths(-1);
            string sql = "select * from {0} where Observation_Date>='{1}' and Observation_Date<'{2}'";
            var table = sqlhelper.SelectData(String.Format(sql, info.TableName,sd.SurveyDate.Date.ToString(), dt.ToString()));
            if (table.Rows.Count > 0)
            {
                double srt = Convert.ToDouble(table.Rows[0]["Temperature"]);
                double szr = Convert.ToDouble(table.Rows[0]["Frequency"]);
                if (Math.Abs(sd.Survey_RorT - lastsd.Survey_RorT) < info.ErrorLimitRT &&
                    Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < info.ErrorLimitZR) return true;
            }
            //跟上一年的同一天做对比
            dt = sd.SurveyDate.Date.AddYears(-1);
            table = sqlhelper.SelectData(String.Format(sql, info.TableName, sd.SurveyDate.Date.ToString(), dt.ToString()));
            if (table.Rows.Count > 0)
            {
                double srt = Convert.ToDouble(table.Rows[0]["Temperature"]);
                double szr = Convert.ToDouble(table.Rows[0]["Frequency"]);
                if (Math.Abs(sd.Survey_RorT - lastsd.Survey_RorT) < ErrorLimitRT &&
                    Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < ErrorLimitZR) return true;
            }
            err.Exception = "数据误差超限";
            return false;

        }
       /// <summary>
       /// 读取数据
       /// </summary>
       /// <param name="path"></param>
       /// <param name="info"></param>
       /// <param name="datas"></param>
       /// <param name="errors"></param>
        protected void LoadData(string path,DataInfo info, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            IFormatProvider culture = CultureInfo.CurrentCulture;
            var workbook = WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                PointSurveyData pd = new PointSurveyData();
                pd.SurveyPoint = psheet.SheetName;
                string sql = String.Format("select * from {0} where Survey_point_Number=@Survey_point_Number", info.TableName);
                var dt = sqlhelper.SelectData(sql,
                    new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                bool flag = dt.Rows.Count > 0 ? true : false;
                DateTime maxDatetime = new DateTime();
                if (flag)//有数据就查数据
                {
                    sql = String.Format("select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number", info.TableName);
                    maxDatetime = (DateTime)sqlhelper.SelectFirst(sql,new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                }
                System.Collections.IEnumerator rows = psheet.GetRowEnumerator();
                SurveyData lastsd = null;
                int rowcn = 0;//行计数
                while (rows.MoveNext())
                {
                    try
                    {
                        rowcn++;
                        if (rows.Current == null) continue;
                        IRow row = (IRow)rows.Current;
                        var cell = row.GetCell(info.DateIndex);
                        if (!cell.IsMergedCell && String.IsNullOrEmpty(cell.ToString())) break;
                        if (cell.CellType != CellType.Numeric) continue;//用第一列的值是否是数字来判断，时间也是数字
                        var date = cell.DateCellValue.ToString("MM/dd/yyyy");
                        SurveyData sd = new SurveyData();
                        if (info.ZoRIndex>0) sd.Survey_ZorR = double.Parse(row.GetCell(info.ZoRIndex).ToString());//频率/基准电阻
                        if (info.ZoRIndex > 0) sd.Survey_RorT = double.Parse(row.GetCell(info.RorTIndex).ToString());//温度
                        sd.Remark = row.GetCell(info.RemarkIndex) == null ? "" : row.GetCell(info.RemarkIndex).ToString();
                        sd.SurveyDate = DateTime.Parse(date + " " + row.GetCell(info.TimeIndex).ToString(), culture, DateTimeStyles.NoCurrentDateDefault);
                        if (flag)
                        {
                            if (sd.SurveyDate.CompareTo(maxDatetime) > 0)
                            {
                                ErrorMsg msg;
                                if (CheckData(sd, lastsd, info, out msg))
                                {
                                    pd.Datas.Add(sd);
                                }
                                else
                                {
                                    msg.ErrorRow = rowcn;
                                    msg.PointNumber = pd.SurveyPoint;
                                    errors.Add(msg);
                                }
                            }
                        }
                        else
                        {
                            pd.Datas.Add(sd);
                        }
                        lastsd = sd;
                    }
                    catch (Exception ex)
                    {
                        ErrorMsg err = new ErrorMsg();
                        err.PointNumber = psheet.SheetName;
                        err.ErrorRow = rowcn;
                        err.Exception = ex.Message;
                        errors.Add(err);
                        continue;
                    }
                }
                datas.Add(pd);
            }

            Log(path, errors);     

        }

        /// <summary> 写测值数据
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        protected int WriteSurveyToDB(List<PointSurveyData> datas,String TableName)
        {
            DataTable dt = new DataTable();
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("Frequency");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            var sqlhelper = CSqlServerHelper.GetInstance();
            var sid = sqlhelper.SelectFirst("select max(ID) as sid  from "+TableName);
            int id = sid == DBNull.Value ? 0 : Convert.ToInt32(sid);
            foreach (PointSurveyData pd in datas)
            {
                foreach (var surveydata in pd.Datas)
                {
                    id++;
                    DataRow dr = dt.NewRow();
                    dr["ID"] = id;
                    dr["Survey_point_Number"] = pd.SurveyPoint;
                    dr["Observation_Date"] = surveydata.SurveyDate;
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString();
                    dr["Temperature"] = surveydata.Survey_RorT;
                    dr["Frequency"] = surveydata.Survey_ZorR;
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }

       /// <summary>写结果数据
       /// </summary>
       /// <param name="datas"></param>
       /// <param name="TableName"></param>
       /// <returns></returns>
        protected int WriteResultToDB(List<PointSurveyData> datas, String TableName)
        {
            DataTable dt = new DataTable();
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("loadReading");
            dt.Columns.Add("ResultReading");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            var sqlhelper = CSqlServerHelper.GetInstance();
            var sid = sqlhelper.SelectFirst("select max(ID) as sid  from  " + TableName);
            int id = sid == DBNull.Value ? 0 : Convert.ToInt32(sid);
            foreach (PointSurveyData pd in datas)
            {
                foreach (var surveydata in pd.Datas)
                {
                    id++;
                    DataRow dr = dt.NewRow();
                    dr["ID"] = id;
                    dr["Survey_point_Number"] = pd.SurveyPoint;
                    dr["Observation_Date"] = surveydata.SurveyDate;
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString();
                    dr["Temperature"] = Convert.ToDecimal(surveydata.Tempreture);
                    dr["loadReading"] = Convert.ToDecimal(surveydata.LoadReading);
                    dr["ResultReading"] = Convert.ToDecimal(surveydata.ResultReading);
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
   }

   public class ProcessFactory
   {
       public static ProcessData CreateInstance(InstrumentType type)
       {
           Assembly ass = Assembly.GetExecutingAssembly();
           string completeName = String.Format("LoadDataCalc.{0}Process", type.ToString());
           var process = ass.CreateInstance(completeName) as ProcessData;
           return process; 
 
       }
   }


   /// <summary>日志类
   /// </summary>
   public class ErrorMsg
   {
       public string PointNumber;
       public int ErrorRow;
       public string Exception = "";
       private static string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
       private static string temp= dir + "\\log\\temp_error.log";
       /// <summary>
       /// 写文件，默认程序目录下新建当天文件，每天一个
       /// </summary>
       /// <param name="msg"></param>
       public  static void Log(string msg)
       {
           string filepath = dir + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
           //总日志文件 每天一个
           using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
           {
               sw.WriteLine(msg);
           }
           //临时日志文件,每次刷新
           using (StreamWriter sw = new StreamWriter(temp, true, Encoding.UTF8))
           {
               sw.WriteLine(msg);
           }
       }
      
       /// <summary>
       /// 打开日志文件
       /// </summary>
       /// <param name="fileindex">1-打开临时，2-打开当天日志</param>
       public static void OpenLog(int fileindex)
       {
           switch (fileindex)
           {
               case 1:
                   Process.Start(new ProcessStartInfo("notepad", temp));
                   break;
               case 2:
                   string filepath = dir + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                   Process.Start(new ProcessStartInfo("notepad", filepath));
                   break;
           }
       }

       public override string ToString()
       {
           return String.Format("DATA:{0},{1},{2}", this.PointNumber, this.ErrorRow,this.Exception);
       }

   }
    /// <summary>单点仪器 数据在excel中的索引列
    /// </summary>
   public class DataInfo
   {
       public string TableName = "";
       public int DateIndex = 0;
       public int TimeIndex = 1;
       public int ZoRIndex = 2;
       public int RorTIndex = 3;
       public int RemarkIndex = 6;
       public double ErrorLimitZR = 10;
       public double ErrorLimitRT = 10;
       

   }


}
