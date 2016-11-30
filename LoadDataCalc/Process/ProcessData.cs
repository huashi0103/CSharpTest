using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using System.Data.SqlClient;
using NPOI.HSSF.UserModel;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace LoadDataCalc
{
    /// <summary> 读写数据基类，项目涉及的仪器类型都要继承实现该类
    /// </summary>
    public abstract class ProcessData
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
        /// <summary>没有读取的sheet名
        /// </summary>
        public Dictionary<string, List<string>> ErrorSheetName = new Dictionary<string, List<string>>();
        /// <summary>
        /// 读取应变计和应变机组的无应力计数据缓存  多点锚杆应力计的数据
        /// </summary>
        public List<PointSurveyData> ExpandDataCach = new List<PointSurveyData>();
        /// <summary>
        /// 同时读两种仪器，第二种的仪器的点名缓存
        /// 默认为应变计和应变计组中的无应力计
        /// 苗尾项目中锚杆应力计中的多点锚杆应力计
        /// 多点位移计用来缓存从浅到深的点名
        /// </summary>
        public List<string> NonNumberDataCach = new List<string>();
        /// <summary> 点名对照表//excel中的点名和数据库中的点名不一样，key为excel中的点名
        /// </summary>
        protected Dictionary<string, string> PointNumberCach = new Dictionary<string, string>();

        protected virtual void AddErroSheetname(string filename, string errorsheetname)
        {
            if (ErrorSheetName.Keys.Contains(filename))
            {
                ErrorSheetName[filename].Add(errorsheetname);
            }
            else
            {
                ErrorSheetName.Add(filename, new List<string>() { errorsheetname });
            }
        }
        /// <summary> 从excel文件中读取数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors);

        /// <summary> 获取数据索引
        /// </summary>
        /// <param name="psheet"></param>
        /// <returns></returns>
        protected virtual DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            bool flag = true;
            bool Zflag = true;
            bool Rflag = true;
            int count = psheet.LastRowNum > 10 ? 10 : psheet.LastRowNum;
            for (int j = 0; j < count; j++)//读取前10行
            {
                IRow row = psheet.GetRow(j);
                if (row == null) continue;
                string laststr = "";
                for (int c = 0; c < row.Cells.Count; c++)
                {
                    var cell = row.Cells[c];
                    if (cell.CellType != CellType.String) continue;
                    var cellstr = cell.StringCellValue;
                    int pyhindex = row.Cells[c].ColumnIndex;
                    if (DataUtils.CheckContainStr(cellstr, "观测日期", "日期"))
                    {
                        flag = false;
                        info.DateIndex = pyhindex;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "观测时间", "时间")) info.TimeIndex = pyhindex;
                    else if (DataUtils.CheckContainStr(cellstr, "digit", "读数(L)", "电阻比", "频模", "频率", "模数"))
                    {
                        info.ZoRIndex = pyhindex;
                        Zflag = false;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "电阻", "电阻值", "温度电阻") && !DataUtils.CheckContainStr(cellstr, "电阻比"))
                    {
                        info.RorTIndex = pyhindex;
                        Rflag = false;
                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                    if (DataUtils.CheckContainStr(cellstr, "计算结果", "开合度", "张合量(mm)", "应变"))//对比用
                    {
                        info.Result = pyhindex;
                    }

                    if (Rflag)
                    {
                        if (cellstr.Contains("温度")) info.RorTIndex = pyhindex;
                    }
                    laststr = cellstr;
                }
            }
            if (flag && info.TimeIndex > 0)
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
            string sql = Config.IsAuto?"select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number and RecordMethod='人工'":
                "select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number";
            sql = String.Format(sql,Config.InsCollection[this.Instrument_Name].Measure_Table);
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
        protected virtual bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, List<SurveyData>Datas,string Survey_point_name,out ErrorMsg err)
        {
            err = new ErrorMsg();
            if (sd.Remark.Contains("已复测")) return true;
            if (lastsd == null) return true;//上一行数据为空不处理
            //先跟上一次的数据做比较，不超过限值直接return
            if (Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;

            //超过限值跟上一个月同一天作比较
            var sqlhelper = CSqlServerHelper.GetInstance();
            DateTime dt = sd.SurveyDate.Date.AddMonths(-1);
            string sql = Config.IsAuto?"select * from {0} where Survey_point_Number='{1}' and Observation_Date>='{2}' and Observation_Date<'{3}' and RecordMethod='人工'":
                "select * from {0} where Survey_point_Number='{1}' and Observation_Date>='{2}' and Observation_Date<'{3}'" ;
            var table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.AddDays(-3).ToString(), dt.AddDays(3).ToString()));
            double szr = 0;
            SurveyData lastMorYData = null;
            if (table.Rows.Count > 0)
            {
                szr = Convert.ToDouble(table.Rows[0]["Frequency"]);
            }
            else
            {
                lastMorYData = GetData(dt.AddDays(-3),dt.AddDays(3),Datas);
                if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
            }
            if (Math.Abs(sd.Survey_ZorR - szr) < Config.LimitZ) return true;
            //跟上一年的同一天做对比
            dt = sd.SurveyDate.Date.AddYears(-1);
            table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.AddDays(-3).ToString(), dt.AddDays(3)));
            if (table.Rows.Count > 0)
            {
                szr = Convert.ToDouble(table.Rows[0]["Frequency"]);
            }
            else
            {
                lastMorYData = GetData(dt.AddDays(-3), dt.AddDays(3), Datas);
                if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
            }
            if (Math.Abs(sd.Survey_ZorR - szr) < Config.LimitZ) return true;
            err.Exception = "数据误差超限";
            return false;

        }
        protected SurveyData GetData(DateTime Start, DateTime End,List<SurveyData>list)
        {
            for (int i =0; i<list.Count; i++)
            {
                if (list[i].SurveyDate.CompareTo(Start) >= 0 && list[i].SurveyDate.CompareTo(End) <= 0) return list[i];
                if (list[i].SurveyDate.CompareTo(Start) < 0) break;
            }
            return null;

        }

        /// <summary> 获取测值的基准值
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected virtual double GetZorRStandard(string point)
        {
            string sql = "select  Benchmark_Resist_Ratio from {0}  where Survey_point_Number='{1}'";
            CSqlServerHelper sqlhelper = CSqlServerHelper.GetInstance();
            var res=sqlhelper.SelectFirst(String.Format(sql,Config.InsCollection[Instrument_Name].Fiducial_Table,point));
            if(res!=DBNull.Value)
            {
                return Convert.ToDouble(res);
            }
            else
            {
                 return 0;
            }
        }

        /// <summary> 读取数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="info"></param>
        /// <param name="datas"></param>
        /// <param name="errors"></param>
        protected void LoadData(string path, DataInfo info, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            IFormatProvider culture = CultureInfo.CurrentCulture;
            var workbook = WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            //测试代码
#if TEST
            PointCach.Add(path,new List<string>());
#endif
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                string pointnumber = psheet.SheetName;
                if (!CheckName(pointnumber))
                {
                    if (PointNumberCach.ContainsKey(pointnumber.ToUpper().Trim()))
                    {
                        pointnumber = PointNumberCach[pointnumber.ToUpper().Trim()];
                    }
                    else
                    {
                        AddErroSheetname(path, psheet.SheetName);
                        continue;
                    }
                }

                //测试代码
#if TEST
                PointCach[path].Add(psheet.SheetName);
#endif
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData();
                pd.SurveyPoint = pointnumber;
                pd.ExcelPath = path;
                DataInfo dinfo = new DataInfo();
                if (info == null)
                {
                    dinfo = GetInfo(psheet, path);
                }
                else
                {
                    dinfo = (DataInfo)info.Clone();
                }
                DateTime maxDatetime = new DateTime();
                bool flag = GetMaxDate(pd.SurveyPoint, out maxDatetime);
                double ZStandard=0;
                if (!flag) ZStandard = GetZorRStandard(pd.SurveyPoint);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行
                SurveyData lastsd = null;
                int count = psheet.PhysicalNumberOfRows;
                //for (int j = count - 1; j > 0; j--)//从后往前读，没法滤掉有问题的数据
                for (int j = 1; j <count+1; j++)//从前往后读
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        DateTime dt;
                        //获取时间，不是时间进入下一次循环
                        if (!GetDateTime(row, dinfo, out dt)) continue;
                        //数据库中有数据，对比上次最大时间，比上次时间小，进入下一次循环
                        if (flag && dt.CompareTo(maxDatetime) <= 0) continue;
                        SurveyData sd = new SurveyData();
                        string errmsg = null;
                        if (!ReadRow(row, dinfo, sd, out  errmsg))//读当前行
                        {
                            if (errmsg != null)
                            {
                                ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg };
                                errors.Add(err);
                            }
                            continue;
                        }
                        //数据库没有数据，用基准值做对比，从基准值行开始读
                        if (!flag)
                        {
                            if (!FirstFlag && Math.Abs(sd.Survey_ZorR - ZStandard) <= 0.01)
                            {
                                FirstFlag = true;
                            }
                            if (!FirstFlag) continue;
                        }
                        ErrorMsg msg;
                        if (CheckData(sd, lastsd, dinfo, pd.Datas,pd.SurveyPoint, out msg))
                        {
                            pd.Datas.Add(sd);
                            lastsd = sd;
                        }
                        else
                        {
                            msg.ErrorRow = j + 1;
                            msg.PointNumber = pd.SurveyPoint;
                            errors.Add(msg);
                        }
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

        private bool ReadRow(IRow row, DataInfo info, SurveyData sd, out string err, bool last = false)
        {
            err = null;
            ICell cell;
            try
            {
                cell = row.GetCell(info.DateIndex);
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false; 
                cell = row.GetCell(info.ZoRIndex);
                if (CheckCell(cell))
                {
                    //err = "测值为空";//不提示认为没测
                    sd = null;
                    return false;
                }
                else
                {
                    if (!GetDataFromCell(cell, out sd.Survey_ZorR))//频率/基准电阻
                    {
                        err = "测值异常";
                        sd = null;
                        return false;
                    }
                    if (sd.Survey_ZorR == 0) return false;//测值为零认为没有测
                }
                if (info.RorTIndex > 0)
                {
                    cell = row.GetCell(info.RorTIndex);
                    if (!CheckCell(cell)) GetDataFromCell(cell, out sd.Survey_RorT);//温度
                }
                if (info.Result > 0)
                {
                    cell = row.GetCell(info.Result);
                    if (!CheckCell(cell)) GetDataFromCell(cell, out sd.ExcelResult);
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
            catch (Exception ex)
            {
                sd = null;
                if (!last) throw ex;
                return false;
            }
        }
        /// <summary>
        /// 检查单元格是否为空或者空字符串
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>是，返回真</returns>
        protected bool CheckCell(ICell cell)
        {
            return (cell == null || String.IsNullOrEmpty(cell.ToString().Trim()));
        }
        /// <summary>
        /// 从单元格中获取double数据
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="value"></param>
        /// <returns>读取成功返回true</returns>
        protected bool GetDataFromCell(ICell cell, out double value)
        {
            if (cell.CellType == CellType.Numeric||
                cell.CellType==CellType.Formula)
            {
                if (cell.CellType == CellType.Formula&&cell.CellFormula == "#N/A") value = 0;
                if (cell == null) value = 0;
                value = cell.NumericCellValue;
                return true;
            }
            else
            {
                return double.TryParse(cell.ToString(), out value);
            }

        }
        /// <summary>
        /// 检查点名是否在数据库/不区分大小写，去空格
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected bool CheckName(string name)
        {
            if (SurveyNumberCach.Count == 0)
            {
                var sqlhelper = CSqlServerHelper.GetInstance();
                string sql = String.Format("select Survey_point_Number from {0}", Config.InsCollection[this.Instrument_Name].Fiducial_Table);
                var dt = sqlhelper.SelectData(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SurveyNumberCach.Add(dt.Rows[i][0].ToString().ToUpper().Trim());
                }
            }
            return SurveyNumberCach.Contains(name.ToUpper().Trim());
        }
        /// <summary>
        /// 检查扩展数据(无应力计)点名是否在数据库/不区分大小写，去空格
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected bool CheckNameExpand(string number)
        {
            if (NonNumberDataCach.Count == 0)
            {//如果没有再初始化中加载数据，这里默认加载无应力计的点名
                var sqlhelper = CSqlServerHelper.GetInstance();
                string sql = "select Survey_point_Number from Fiducial_Nonstress";
                var dt = sqlhelper.SelectData(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NonNumberDataCach.Add(dt.Rows[i][0].ToString().ToUpper().Trim());
                }
            }
            return NonNumberDataCach.Contains(number.ToUpper().Trim());
        }
        /// <summary> 读取观测日期和时间
        /// </summary>
        /// <param name="row"></param>
        /// <param name="info"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        protected bool GetDateTime(IRow row,DataInfo info, out DateTime dt)
        {
            dt = new DateTime();
            DateTime DateSp = new DateTime();
            bool flag = false;
            var cell = row.GetCell(info.DateIndex);
            if (CheckCell(cell)) return false;
            if (cell.CellType != CellType.Numeric && cell.CellType != CellType.Formula)
            {
                if (cell.CellType == CellType.String)
                {
                    double dvalue;
                    if (double.TryParse(cell.StringCellValue, out  dvalue))
                    {
                        DateSp = DateTime.FromOADate(dvalue);
                        flag = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!HSSFDateUtil.IsCellDateFormatted(cell)) return false;
            }
            if (info.TimeIndex > 0 && row.GetCell(info.TimeIndex) != null && row.GetCell(info.TimeIndex).ToString() != "")
            {
                var date = flag ? DateSp.ToString("MM/dd/yyyy") : cell.DateCellValue.ToString("MM/dd/yyyy");
                string time;
                if (row.GetCell(info.TimeIndex).CellType == CellType.Numeric)
                {
                    time = row.GetCell(info.TimeIndex).DateCellValue.TimeOfDay.ToString();
                }
                else
                {
                    time = row.GetCell(info.TimeIndex).ToString();
                }
                if (!DateTime.TryParse(date + " " + time, CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out  dt))
                {
                    dt = flag ? DateSp : cell.DateCellValue;
                }
            }
            else
            {
                dt = flag ? DateSp : cell.DateCellValue;
            }
            return true;
        }
        /// <summary>加载点名对照表中对应仪器的缓存
        /// </summary>
        /// <param name="type"></param>
        public void LoadPointCach(InstrumentType type)
        {
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config\\PointNumberConfig.xls";
            if (!File.Exists(filePath)) return;
            var workbook = WorkbookFactory.Create(filePath);
            var psheet = workbook.GetSheet(Config.ProCode);
            int count = psheet.LastRowNum;
            for (int i = 1; i < count; i++)
            {
                IRow row = psheet.GetRow(i);
                string Instypename = row.GetCell(2).StringCellValue;
                if (!Config.InsCollection.InstrumentDic.ContainsKey(Instypename)) continue;
                var temptype = Config.InsCollection.InstrumentDic[Instypename];
                if (temptype != type) continue;
                string excelname = row.GetCell(0).StringCellValue.ToUpper().Trim();
                string dbname = row.GetCell(1).StringCellValue.ToUpper().Trim();
                PointNumberCach.Add(excelname, dbname);
            }
        }
        /// <summary>获取多点位移计从浅到深排序的点名
        /// </summary>
        public void LoadMultidisplacementName()
        {
            NonNumberDataCach = Config.GetMultiDisplacementOrder();
        }

#if TEST
        //测试代码：
        public Dictionary<string, List<string>> PointCach = new Dictionary<string, List<string>>();
#endif

    }

    /// <summary>单点仪器 数据在excel中的索引列
    /// 多点数据列记录的为起始列
    /// </summary>
    public class DataInfo : ICloneable
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
        ///多点位移计起始列,应变计组的起始列
        /// </summary>
        public int Findex = -1;
        /// <summary>
        /// 应变计组的电阻比总列数，与数据库中的向数做对比,
        /// 确定是否包含无应力计的数据
        /// </summary>
        public int FCount = 0;
        /// <summary>
        /// excel计算结果列
        /// </summary>
        public int Result = -1;
        /// <summary>
        /// 锚索测力计平均值列
        /// </summary>
        public int Average = 6;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

}
