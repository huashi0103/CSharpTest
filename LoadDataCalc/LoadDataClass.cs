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
        public Action<string> StatusAction;
        private object DataLock = new object();

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
        public string Init()
        {
            string res = "";
            try
            {
                Config.WriteDifaultConfig();//写一个默认模板
                res = "读取项目配置文件";
                //读取本项目配置文件
                if (!Config.LoadConfig()) return res;
                res = "初始化数据库";
                CSqlServerHelper.Connectionstr = Config.DataBase;
                sqlhelper = CSqlServerHelper.GetInstance();
                if (sqlhelper.Check())
                {
                    res = "OK";
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
                    if (DataUtils.CheckContainStr(filename, keyword))
                    {
                        if (insname == "应变计")
                        {
                            string[] multikey= Config.Instruments.Where(x => x.InsName == "应变计组").FirstOrDefault().KeyWord.ToArray();
                            if (!DataUtils.CheckContainStr(filename, multikey))
                            {
                                refiles.Add(file);
                            }
                        }
                        else if (insname == "无应力计")
                        {
                            string[] multikey = Config.Instruments.Where(x => x.InsName == "应变计组").FirstOrDefault().KeyWord.ToArray();
                            string[] multikey1 = Config.Instruments.Where(x => x.InsName == "应变计").FirstOrDefault().KeyWord.ToArray();
                            if (!DataUtils.CheckContainStr(filename, multikey) && !DataUtils.CheckContainStr(filename, multikey1))
                            {
                                refiles.Add(file);
                            }
 
                        }
                        else
                        {
                            refiles.Add(file);
                        }
                    }
                    
                }
                return refiles;
            }
 
        }
        
        //数据缓存
        public List<PointSurveyData> SurveyDataCach = new List<PointSurveyData>();
        public Dictionary<string, List<ErrorMsg>> ErrorMsgCach = new Dictionary<string, List<ErrorMsg>>();
        //数据缓存,缓存应变计对应的无应力计数据
        public List<PointSurveyData> SurveyDataCachExpand = new List<PointSurveyData>();
     

        /// <summary> 当前处理仪器类
        /// </summary>
        private ProcessData process = null;

        /// <summary> 清空缓存数据
        /// </summary>
        public void ClearCach()
        {
            SurveyDataCach.Clear();
            ErrorMsgCach.Clear();
            SurveyDataCachExpand.Clear();
        }
        /// <summary>
        /// 检查文件是否被占用
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public bool CheckFiles(List<string> files, out string errfile)
        {
            errfile = null;
            foreach (string file in files)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);

                }
                catch
                {
                    errfile = file;
                    return false;
                }
                finally
                {
                    if (fs != null)fs.Close();
                }
            }
            return true;

 
        }
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
            process.StatusAction = this.StatusAction;
            process.ErrorLimitZR = Config.LimitZ;
            process.ErrorLimitZR = Config.LimitT;
            files.AsParallel().ForAll((file) => {
                List<PointSurveyData> datas = new List<PointSurveyData>();
                List<ErrorMsg> msgs = new List<ErrorMsg>();
                process.ReadData(file, out datas, out msgs);
                lock (DataLock)
                {
                    SurveyDataCach.AddRange(datas);
                    if (ErrorMsgCach.Keys.Contains(file))
                    {
                        ErrorMsgCach[file].AddRange(msgs);
                    }
                    else
                    {
                        ErrorMsgCach.Add(file, msgs);
                    }
                }
            
            });
            if (instype==InstrumentType.Fiducial_Strain_Gauge)
            {
                ReadDataExpand(files);
            }
            if (StatusAction != null) StatusAction("读取完成,正在写入日志文件");
            ErrorMsg.Log(ErrorMsgCach);
            ErrorMsg.LogSheetErr(process.ErrorSheetName);
        }
        public void ReadDataExpand(List<string> files)
        {
            process = ProcessFactory.CreateInstance(InstrumentType.Fiducial_Nonstress);
            if (process == null) return;
            process.StatusAction = this.StatusAction;
            process.ErrorLimitZR = Config.LimitZ;
            process.ErrorLimitZR = Config.LimitT;
            files.AsParallel().ForAll((file) =>
            {
                List<PointSurveyData> datas = new List<PointSurveyData>();
                List<ErrorMsg> msgs = new List<ErrorMsg>();
                process.ReadData(file, out datas, out msgs);
                lock (DataLock)
                {
                    SurveyDataCachExpand.AddRange(datas);
                    if (ErrorMsgCach.Keys.Contains(file))
                    {
                        ErrorMsgCach[file].AddRange(msgs);
                    }
                    else
                    {
                        ErrorMsgCach.Add(file, msgs);
                    }
                }
            });
 
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
        public void Wirte(InstrumentType instype)
        {
            process = ProcessFactory.CreateInstance(instype);
            if (process == null) return;
            int result= process.WriteSurveyToDB(SurveyDataCach);
            ErrorMsg.Log(String.Format("写入{0}行测值",result));
            result=process.WriteResultToDB(SurveyDataCach);
            ErrorMsg.Log(String.Format("写入{0}行成果值", result));
            
        }
        void getFiles(List<string> list, string path, string pattern)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            var allfiles = di.GetFiles(pattern);
            foreach (FileInfo fi in allfiles)
            {
                if ((fi.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    list.Add(fi.FullName);
                }

            }
        }
        public void getDir(string path, List<string> FileList)
        {
            getFiles(FileList, path, "*.xls");
            getFiles(FileList, path, "*.xlsx");
            //FileList.AddRange(Directory.GetFiles(path,"*.xls"));
            //FileList.AddRange(Directory.GetFiles(path, "*.xlsx"));
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
       public Action<string> StatusAction = null;
        /// <summary>仪器类型
        /// </summary>
        public InstrumentType InsType { get; set; }
        public double ErrorLimitZR = 20;
        public double ErrorLimitRT = 20;
        public string Instrument_Name = "";
        /// <summary>
        /// 测点缓存//每次读数据从数据库查询一次，所有文件读完清空
        /// </summary>
        public List<string> SurveyNumberCach = new List<string>();

        public List<string> ErrorSheetName = new List<string>();

        private static object lockobject = new object();

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
       
       /// <summary> 获取数据索引
        /// </summary>
        /// <param name="psheet"></param>
        /// <returns></returns>
        protected  virtual DataInfo GetInfo(ISheet psheet,string filePath=null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            bool flag = true;
            bool Zflag = true;
            int count = psheet.LastRowNum > 10 ? 10 : psheet.LastRowNum;
            for (int j = 0; j < count; j++)//读取前10行
            {
                IRow row = psheet.GetRow(j);
                if (row == null) continue;
                string laststr = "";
                for (int c = 0; c < row.Cells.Count; c++)
                {
                    var cellstr = row.Cells[c].ToString();
                    int pyhindex = row.Cells[c].ColumnIndex;
                    if (DataUtils.CheckContainStr(cellstr, "观测日期", "日期"))
                    {
                        flag = false;
                        info.DateIndex = pyhindex;
                    }
                    else if(DataUtils.CheckContainStr(cellstr,"观测时间","时间"))info.TimeIndex = pyhindex;
                    else if (DataUtils.CheckContainStr(cellstr, "电阻比", "频模", "频率", "模数"))
                    {
                        info.ZoRIndex = pyhindex;
                        Zflag = false;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "电阻值", "温度电阻")) info.RorTIndex = pyhindex;
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;

                    if (DataUtils.CheckContainStr(laststr, "频率", "频模", "模数"))
                    {
                        if (cellstr.Contains("温度")) info.RorTIndex = pyhindex;
                    }

                    laststr = cellstr;
                }
            }
            if (flag&&info.TimeIndex>0)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }
            return info;
        }
       /// <summary>
       /// 获取测值表中最大日期
       /// </summary>
       /// <returns></returns>
        protected bool GetMaxDate(string surveyNumber, out DateTime maxDatetime)
        {
            var sqlhelper = CSqlServerHelper.GetInstance();
            maxDatetime = new DateTime();
            string sql = String.Format("select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number",
                Config.InsCollection[this.Instrument_Name].Measure_Table);
            var result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", surveyNumber));
            bool flag = (result != DBNull.Value);
            if (flag) maxDatetime = (DateTime)result;
            return flag;
        }

       /// <summary>检查数据正确性
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="lastsd"></param>
        /// <param name="tablename"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        protected virtual bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, out ErrorMsg err,string Survey_point_name)
        {
            err = new ErrorMsg();
            if (sd.Remark.Contains("已复测")) return true;
            if (lastsd==null) return true;//上一行数据为空不处理
            //先跟上一次的数据做比较，不超过限值直接return
            if (Math.Abs(sd.Survey_RorT - lastsd.Survey_RorT) <Config.LimitT &&
                Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;
            //超过限值跟上一个月同一天作比较
            var sqlhelper = CSqlServerHelper.GetInstance();
            DateTime dt = sd.SurveyDate.Date.AddMonths(-1);
            string sql = "select * from {0} where Survey_point_Number='{1}' and Observation_Date>='{2}' and Observation_Date<'{3}'";
            var table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name,dt.ToString(),dt.AddDays(1).ToString()));
            if (table.Rows.Count > 0)
            {
                double srt = Convert.ToDouble(table.Rows[0]["Temperature"]);
                double szr = Convert.ToDouble(table.Rows[0]["Frequency"]);
                if (Math.Abs(sd.Survey_RorT - lastsd.Survey_RorT) < Config.LimitT &&
                    Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;
            }
            //跟上一年的同一天做对比
            dt = sd.SurveyDate.Date.AddYears(-1);
            table=sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.ToString(), dt.AddDays(1).ToString()));
            if (table.Rows.Count > 0)
            {
                double srt = Convert.ToDouble(table.Rows[0]["Temperature"]);
                double szr = Convert.ToDouble(table.Rows[0]["Frequency"]);
                if (Math.Abs(sd.Survey_RorT - lastsd.Survey_RorT) < Config.LimitT &&
                    Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;
            }
            err.Exception = "数据误差超限";
            return false;

        }
       
       /// <summary> 读取数据
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
                if (!CheckName(psheet.SheetName))
                {
                    ErrorSheetName.Add(psheet.SheetName);
                    continue;
                    
                }
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData();
                pd.SurveyPoint = psheet.SheetName;
                info = GetInfo(psheet,path);

                DateTime maxDatetime = new DateTime();
                string sql = String.Format("select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number", info.TableName);
                var result=sqlhelper.SelectFirst(sql,new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                bool flag = (result != DBNull.Value);
                if (flag) maxDatetime = (DateTime)result;
                
                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = count - 1; j>0; j--)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        var cell = row.GetCell(info.DateIndex);
                        if (cell == null || String.IsNullOrEmpty(cell.ToString()) || cell.CellType != CellType.Numeric) continue;
                        SurveyData sd = new SurveyData();
                        string errmsg = null;
                        if (!ReadRow(row, info, sd, out  errmsg))//读当前行
                        {
                            if (errmsg != null)
                            {
                                ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg };
                                errors.Add(err);
                            }
                            continue;
                        }
                        lastsd = new SurveyData();
                        IRow lastrow = psheet.GetRow(j - 1);
                        if (!ReadRow(lastrow, info, lastsd, out errmsg, true)) lastsd = null;

                        if (flag)
                        {
                            if (sd.SurveyDate.CompareTo(maxDatetime) > 0)
                            {
                                ErrorMsg msg;
                                if (CheckData(sd, lastsd, info, out msg,pd.SurveyPoint))
                                {
                                    pd.Datas.Add(sd);
                                }
                                else
                                {
                                    msg.ErrorRow = j + 1;
                                    msg.PointNumber = pd.SurveyPoint;
                                    errors.Add(msg);
                                }
                                continue;
                            }
                            break;
                        }
                        pd.Datas.Add(sd);
                    }
                    catch (Exception ex)
                    {
                        ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = ex.Message };
                        errors.Add(err);
                        continue;
                    }
                }
                datas.Add(pd);
            }
            workbook.Close();
           
        }

        private bool ReadRow(IRow row, DataInfo info, SurveyData sd,out string err,bool last=false)
        {
            err = null;
            try
            {
                if (row==null) return false;
                var cell = row.GetCell(info.DateIndex);
                if (cell == null || cell.CellType != CellType.Numeric || !HSSFDateUtil.IsCellDateFormatted(cell)) return false;
                if (info.TimeIndex>0&&row.GetCell(info.TimeIndex) != null &&
                    row.GetCell(info.TimeIndex).ToString() != "")
                {
                    var date = cell.DateCellValue.ToString("MM/dd/yyyy");
                    string time;
                    if (row.GetCell(info.TimeIndex).CellType == CellType.Numeric)
                    {
                        time = row.GetCell(info.TimeIndex).DateCellValue.TimeOfDay.ToString();
                    }
                    else
                    {
                        time = row.GetCell(info.TimeIndex).ToString();
                    }
                    sd.SurveyDate = DateTime.Parse(date + " " + time, CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault);
                }
                else
                {
                    sd.SurveyDate = cell.DateCellValue;
                }
                if (row.GetCell(info.ZoRIndex) == null || String.IsNullOrEmpty(row.GetCell(info.ZoRIndex).ToString().Trim()))
                {
                    //err = "测值为空";//不提示认为没测
                    sd = null;
                    return false;
                }
                else
                {
                    if (!double.TryParse(row.GetCell(info.ZoRIndex).ToString(), out sd.Survey_ZorR))//频率/基准电阻
                    {
                        err = "测值异常";
                        sd = null;
                        return false;
                    }
                }
                if(row.GetCell(info.RorTIndex)!=null&&!String.IsNullOrEmpty(row.GetCell(info.RorTIndex).ToString()))
                {
                   double.TryParse(row.GetCell(info.RorTIndex).ToString(), out sd.Survey_RorT);//温度
                }
                if (info.RemarkIndex > 0) sd.Remark = row.GetCell(info.RemarkIndex) == null ? "" : row.GetCell(info.RemarkIndex).ToString();
                if (!DataUtils.CheckDateTime(sd.SurveyDate))
                {
                    err = "观测日期有误";
                    sd = null;
                    return false;
                }
                return true;
            }
            catch(Exception ex)
            {
                sd = null;
                if (!last) throw ex;
                return false;
            }
        }
         
        protected bool CheckName(string name)
        {
            if (SurveyNumberCach.Count == 0)
            {
                var sqlhelper = CSqlServerHelper.GetInstance();
                string sql = String.Format("select Survey_point_Number from {0}", Config.InsCollection[this.Instrument_Name].Fiducial_Table);
                var dt = sqlhelper.SelectData(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SurveyNumberCach.Add((string)dt.Rows[i][0]);
                }
            }
            return SurveyNumberCach.Contains(name);
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

   /// <summary>
   /// 处理数据类的工厂类
   /// </summary>
   public class ProcessFactory
   {
       public static ProcessData CreateInstance(InstrumentType type)
       {
           Assembly ass = Assembly.GetExecutingAssembly();
           string completeName = String.Format("LoadDataCalc.{0}Process{1}", type.ToString(),Config.ProCode);
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
       public static string temp= dir + "\\log\\temp_error.log";
       public static string tempsheeterror = dir + "\\log\\sheet_error.log";
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
       public static void Log(Dictionary<string, List<ErrorMsg>> ErrorMsgCach)
       {
           string filepath = dir + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
           using (StreamWriter sw = new StreamWriter(filepath,true, Encoding.UTF8))
           {
               foreach (var dic in ErrorMsgCach)
               {
                   sw.WriteLine(String.Format("FILE:{0},{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), dic.Key));
                   foreach (var msg in dic.Value)
                   {
                       sw.WriteLine(msg.ToString());
                   }
               }
               
           }

           //临时日志文件,每次刷新
           using (StreamWriter sw = new StreamWriter(temp, false, Encoding.UTF8))
           {
               foreach (var dic in ErrorMsgCach)
               {
                   sw.WriteLine(String.Format("FILE:{0},{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), dic.Key));
                   foreach (var msg in dic.Value)
                   {
                       sw.WriteLine(msg.ToString());
                   }
               }
           }

       }
       /// <summary>
       /// 记下没有读取的sheetname名
       /// </summary>
       /// <param name="sheetnames"></param>
       public static void LogSheetErr(List<string> sheetnames)
       {
           using (StreamWriter sw = new StreamWriter(tempsheeterror, false, Encoding.UTF8))
           {
               foreach (string sheetname in sheetnames)
               {
                   sw.WriteLine(sheetname);
               }
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
                   if (File.Exists(temp))
                   {
                       Process.Start(new ProcessStartInfo("notepad", temp));
                   }
                   if (File.Exists(tempsheeterror))
                   {
                       Process.Start(new ProcessStartInfo("notepad", tempsheeterror));
                   }
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
    /// 多点数据列记录的为起始列
    /// </summary>
   public class DataInfo
   {
       public string TableName = "";
       public int DateIndex = 0;
       public int TimeIndex = 1;
       public int ZoRIndex = 2;
       public int RorTIndex = 3;
       public int RemarkIndex = 6;
       /// <summary>应变机组中的无应力计
       /// </summary>
       public int NZoRIndex = 2;
       public int NRorTIndex = 3;
       public int Sum = 4;
       /// <summary>
       ///多点位移计起始列
       /// </summary>
       public int Findex = -1;

   }


}
