using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using System.Data.SqlClient;
using NPOI.HSSF.UserModel;
using System.Globalization;

namespace LoadDataCalc
{
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
        /// 读取应变计和应变机组的无应力计数据缓存
        /// </summary>
        public List<PointSurveyData> NonStressDataCach = new List<PointSurveyData>();
        public List<string> NonNumberDataCach = new List<string>();

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
        protected virtual bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, out ErrorMsg err, string Survey_point_name)
        {
            err = new ErrorMsg();
            if (sd.Remark.Contains("已复测")) return true;
            if (lastsd == null) return true;//上一行数据为空不处理
            //先跟上一次的数据做比较，不超过限值直接return
            if (Math.Abs(sd.Survey_RorT - lastsd.Survey_RorT) < Config.LimitT &&
                Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;
            //超过限值跟上一个月同一天作比较
            var sqlhelper = CSqlServerHelper.GetInstance();
            DateTime dt = sd.SurveyDate.Date.AddMonths(-1);
            string sql = "select * from {0} where Survey_point_Number='{1}' and Observation_Date>='{2}' and Observation_Date<'{3}'";
            var table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.ToString(), dt.AddDays(1).ToString()));
            if (table.Rows.Count > 0)
            {
                double srt = Convert.ToDouble(table.Rows[0]["Temperature"]);
                double szr = Convert.ToDouble(table.Rows[0]["Frequency"]);
                if (Math.Abs(sd.Survey_RorT - lastsd.Survey_RorT) < Config.LimitT &&
                    Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;
            }
            //跟上一年的同一天做对比
            dt = sd.SurveyDate.Date.AddYears(-1);
            table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.ToString(), dt.AddDays(1).ToString()));
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
                if (!CheckName(psheet.SheetName))
                {
                    AddErroSheetname(path, psheet.SheetName);
                    continue;
                }

                //测试代码
#if TEST
                PointCach[path].Add(psheet.SheetName);
#endif
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData();
                pd.SurveyPoint = psheet.SheetName;
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
                string sql = String.Format("select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number", dinfo.TableName);
                var result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                bool flag = (result != DBNull.Value);
                if (flag) maxDatetime = (DateTime)result;

                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = count - 1; j > 0; j--)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        var cell = row.GetCell(dinfo.DateIndex);
                        if (CheckCell(cell)) continue;
                        if (cell.CellType != CellType.Numeric && cell.CellType != CellType.Formula) continue;
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
                        lastsd = new SurveyData();
                        IRow lastrow = psheet.GetRow(j - 1);
                        if (!ReadRow(lastrow, dinfo, lastsd, out errmsg, true)) lastsd = null;

                        if (flag)
                        {
                            if (sd.SurveyDate.CompareTo(maxDatetime) > 0)
                            {
                                ErrorMsg msg;
                                if (CheckData(sd, lastsd, dinfo, out msg, pd.SurveyPoint))
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

        private bool ReadRow(IRow row, DataInfo info, SurveyData sd, out string err, bool last = false)
        {
            err = null;
            ICell cell;
            try
            {
                if (row == null) return false;
                cell = row.GetCell(info.DateIndex);
                if (cell == null) return false;
                if (cell.CellType != CellType.Numeric && cell.CellType != CellType.Formula) return false;
                if (!HSSFDateUtil.IsCellDateFormatted(cell)) return false;
                if (info.TimeIndex > 0 && row.GetCell(info.TimeIndex) != null && row.GetCell(info.TimeIndex).ToString() != "")
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
                    if (DateTime.TryParse(date + " " + time, CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out  sd.SurveyDate))
                    {
                        sd.SurveyDate = cell.DateCellValue;
                    }
                }
                else
                {
                    sd.SurveyDate = cell.DateCellValue;
                }

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
                }

                cell = row.GetCell(info.RorTIndex);
                if (!CheckCell(cell))
                {
                    GetDataFromCell(cell, out sd.Survey_RorT);//温度
                }

                cell = row.GetCell(info.RemarkIndex);
                if (info.RemarkIndex > 0) sd.Remark = cell == null ? "" : cell.ToString();
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
            if (cell.CellType == CellType.Numeric ||
                cell.CellType == CellType.Formula)
            {
                value = cell.NumericCellValue;
                return true;
            }
            else
            {
                return double.TryParse(cell.ToString(), out value);
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
                    SurveyNumberCach.Add(dt.Rows[i][0].ToString().ToUpper().Trim());
                }
            }
            return SurveyNumberCach.Contains(name.ToUpper().Trim());
        }

        protected bool CheckNonStress(string number)
        {
            if (NonNumberDataCach.Count == 0)
            {
                var sqlhelper = CSqlServerHelper.GetInstance();
                string sql = "select Survey_point_Number from Fiducial_Nonstress";
                var dt = sqlhelper.SelectData(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NonNumberDataCach.Add(dt.Rows[i][0].ToString().ToUpper());
                }
            }
            return NonNumberDataCach.Contains(number.ToUpper());
        }

#if TEST
        //测试代码：
        public Dictionary<string, List<string>> PointCach = new Dictionary<string, List<string>>();
#endif

    }

}
