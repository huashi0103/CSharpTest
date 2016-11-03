using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using NPOI.SS.UserModel;
using System.Data.SqlClient;
using System.Data;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;

namespace LoadDataCalc
{

    /// <summary> 渗压计
    /// </summary>
    public class Fiducial_Leakage_PressureProcessMW : ProcessData
    {
        /// <summary> 从excel文件中读取数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override void ReadData(string path, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            base.LoadData(path, null, out datas, out errors);
        }

        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
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
                    var cell = row.Cells[c];
                    cell.SetCellType(CellType.String);
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
                    else if (DataUtils.CheckContainStr(cellstr, "电阻值", "温度电阻")) info.RorTIndex = pyhindex;
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;

                    if (DataUtils.CheckContainStr(laststr, "频率", "频模", "模数"))
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
            if (Zflag)
            {
                return null;
            }
            return info;
        }

        public Fiducial_Leakage_PressureProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Leakage_Pressure;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "渗压计";
        }
    }

    /// <summary> 单点位移计
    /// </summary>
    public class Fiducial_Single_DisplacementProcessMW : ProcessData
    {
        public Fiducial_Single_DisplacementProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Single_Displacement;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "单点位移计";

        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            base.LoadData(path, null, out datas, out errors);
        }
   
        protected  override DataInfo GetInfo(ISheet psheet,string filePath=null)
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
                    else if(DataUtils.CheckContainStr(cellstr,"观测时间","时间"))info.TimeIndex = pyhindex;
                    else if (DataUtils.CheckContainStr(cellstr, "digit", "读数(mV)", "电阻比", "频模", "频率", "模数"))
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
            if (flag&&info.TimeIndex>0)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }
            return info;
        }

    }

    /// <summary> 测缝计
    /// </summary>
    public class Fiducial_Measure_ApertureProcessMW : ProcessData
    {
        public Fiducial_Measure_ApertureProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Measure_Aperture;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "测缝计";
        }

        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            //string filename = Path.GetFileName(path);
            //if (filename.Contains("位错计"))
            //{
            //    LoadDataExpand(path, null, out datas, out errors);
            //}
            //else
            //{
                base.LoadData(path, null, out datas, out errors);
           //}
        }
        void LoadDataExpand(string path, DataInfo info, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {//三向测缝计
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            IFormatProvider culture = CultureInfo.CurrentCulture;
            var workbook = WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                if (!CheckName(psheet.SheetName + "x"))
                {
                    AddErroSheetname(path, psheet.SheetName);
                    continue;
                }
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData();
                PointSurveyData pd1 = new PointSurveyData();
                PointSurveyData pd2 = new PointSurveyData();
                pd.SurveyPoint = psheet.SheetName + "x";
                pd1.SurveyPoint = psheet.SheetName + "y";
                pd2.SurveyPoint = psheet.SheetName + "z";

                info = new DataInfo() { DateIndex = 0, TimeIndex = 1, ZoRIndex = 2, RorTIndex = 3, RemarkIndex = 11 };
                DataInfo infoy = new DataInfo() { DateIndex = 0, TimeIndex = 1, ZoRIndex = 4, RorTIndex = 5, RemarkIndex = 11 };
                DataInfo infoz = new DataInfo() { DateIndex = 0, TimeIndex = 1, ZoRIndex = 6, RorTIndex = 7, RemarkIndex = 11 };
                info.TableName = infoy.TableName = infoz.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
                DateTime maxDatetimex = new DateTime();
                bool flagx = GetMaxDate(pd.SurveyPoint, out maxDatetimex);
                DateTime maxDatetimey = new DateTime();
                bool flagy = GetMaxDate(pd1.SurveyPoint, out maxDatetimey);
                DateTime maxDatetimez = new DateTime();
                bool flagz = GetMaxDate(pd2.SurveyPoint, out maxDatetimez);

                int count = psheet.LastRowNum;
                for (int j = count - 1; j > 0; j--)
                {
                    IRow row = psheet.GetRow(j);
                    IRow lastrow = psheet.GetRow(j - 1);
                    if (row == null) continue;
                    var cell = row.GetCell(info.DateIndex);
                    if (cell == null || String.IsNullOrEmpty(cell.ToString()) || cell.CellType != CellType.Numeric) continue;
                    AddOne(info, row, lastrow, flagx, maxDatetimex, pd, errors);
                    AddOne(infoy, row, lastrow, flagx, maxDatetimey, pd1, errors);
                    AddOne(infoz, row, lastrow, flagx, maxDatetimez, pd2, errors);
                }
                datas.Add(pd);
                datas.Add(pd1);
                datas.Add(pd2);
            }

            workbook.Close();

        }
        void AddOne(DataInfo info, IRow row, IRow lastrow, bool flag, DateTime maxDatetime, PointSurveyData pd, List<ErrorMsg> errors)
        {
            try
            {
                SurveyData sd = new SurveyData();
                string errmsg = null;
                if (!ReadRowExpand(row, info, sd, out  errmsg))//读当前行
                {
                    if (errmsg != null)
                    {
                        ErrorMsg err = new ErrorMsg() { PointNumber = pd.SurveyPoint, ErrorRow = row.RowNum + 1, Exception = errmsg };
                        errors.Add(err);
                    }
                    return;
                }
                var lastsd = new SurveyData();
                if (!ReadRowExpand(lastrow, info, lastsd, out errmsg, true)) lastsd = null;

                if (flag)
                {
                    if (sd.SurveyDate.CompareTo(maxDatetime) > 0)
                    {
                        ErrorMsg msg;
                        if (CheckData(sd, lastsd, info, out msg, pd.SurveyPoint))
                        {
                            pd.Datas.Add(sd);
                            return;
                        }
                        else
                        {
                            msg.ErrorRow = row.RowNum + 1;
                            msg.PointNumber = pd.SurveyPoint;
                            errors.Add(msg);
                            return;
                        }
                    }
                }
                pd.Datas.Add(sd);
            }
            catch (Exception ex)
            {
                ErrorMsg err = new ErrorMsg() { PointNumber = pd.SurveyPoint, ErrorRow = row.RowNum + 1, Exception = ex.Message };
                errors.Add(err);
            }
        }
        bool ReadRowExpand(IRow row, DataInfo info, SurveyData sd, out string err, bool last = false)
        {
            err = null;
            try
            {
                var cell = row.GetCell(info.DateIndex);
                if (info.TimeIndex > 0 && row.GetCell(info.TimeIndex) != null &&
                    row.GetCell(info.TimeIndex).ToString() != "" &&
                    info.DateIndex != info.TimeIndex)
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
                if (row.GetCell(info.RorTIndex) != null && !String.IsNullOrEmpty(row.GetCell(info.RorTIndex).ToString()))
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
            catch (Exception ex)
            {
                sd = null;
                if (!last) throw ex;
                return false;
            }
        }

    }
    /// <summary>裂缝计
    /// </summary>
    public class Fiducial_ApertureProcessMW : ProcessData
    {
        public Fiducial_ApertureProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Aperture;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "裂缝计";

        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            base.LoadData(path, null, out datas, out errors);
        }
    }
    /// <summary>钢筋计
    /// </summary>
    public class Fiducial_Steel_BarProcessMW : ProcessData
    {
        public Fiducial_Steel_BarProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Steel_Bar;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "钢筋计";

        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            string filename = Path.GetFileName(path);
            //if (filename != "11#机钢筋计.xlsx") return;
            LoadDataExpand(path, out datas, out errors);

        }
        //特殊的格式，sheet里边可能有两个测点的数据
        void LoadDataExpand(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
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
                string name = psheet.SheetName;
                string surveyname1 = name;
                string surveyname2 = "";
                bool Istwo = false;
                if (name.Contains("~"))
                {
                    Istwo = true;
                    var temp = name.Split('~');
                    surveyname1 = temp[0];
                    surveyname2 = temp[0].Substring(0, temp[0].Length - 1) + temp[1];
                }

                if (!CheckName(surveyname1))
                {
                    AddErroSheetname(path, psheet.SheetName);
                    continue;
                }
                //测试代码
#if TEST
                PointCach[path].Add(surveyname1);
#endif

                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData();
                PointSurveyData pd1 = new PointSurveyData();
                pd.SurveyPoint = surveyname1;
                if (Istwo) pd1.SurveyPoint = surveyname2;

                DateTime maxDatetime = new DateTime();
                string sql = String.Format("select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number",
                    Config.InsCollection[this.Instrument_Name].Measure_Table);
                var result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                bool flag = (result != DBNull.Value);
                if (flag) maxDatetime = (DateTime)result;

                DataInfo info = GetInfo(psheet, path);
                DataInfo Firstinfo = info.Clone() as DataInfo;
                if (Istwo)
                {
                    if (pd.SurveyPoint.StartsWith("RGDK"))
                    {
                        Firstinfo.ZoRIndex = Firstinfo.ZoRIndex - 3;
                        Firstinfo.RorTIndex = Firstinfo.RorTIndex - 3;
                    }
                    else
                    {
                        Firstinfo.ZoRIndex = Firstinfo.ZoRIndex - 2;
                        Firstinfo.RorTIndex = Firstinfo.RorTIndex - 2;
                    }
                }
                
                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = count - 1; j > 0; j--)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        var cell = row.GetCell(Firstinfo.DateIndex);
                        if (cell == null || String.IsNullOrEmpty(cell.ToString().Trim()))continue;
                        if(cell.CellType !=(CellType.Numeric&CellType.Formula)) continue;
                        IRow lastrow = psheet.GetRow(j - 1);
                        SurveyData sd = new SurveyData();
                        #region//读取第一个点
                        string errmsg = null;
                        //info=new DataInfo(){DateIndex=1,ZoRIndex=2,RorTIndex=3};
                        lastsd = new SurveyData();

                        if (!ReadRow(row, lastsd, Firstinfo, Istwo, out  errmsg)) lastsd = null;
                        errmsg = null;
                        if (!ReadRow(row, sd, Firstinfo, Istwo, out  errmsg))//读当前行
                        {
                            if (errmsg != null)
                            {
                                ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg };
                                errors.Add(err);
                            }
                        }
                        else
                        {
                            if (flag)
                            {
                                if (sd.SurveyDate.CompareTo(maxDatetime) > 0)
                                {
                                    ErrorMsg msg;
                                    if (CheckData(sd, lastsd, Firstinfo, out msg, pd.SurveyPoint))
                                    {
                                        pd.Datas.Add(sd);
                                    }
                                    else
                                    {
                                        msg.ErrorRow = j + 1;
                                        msg.PointNumber = pd.SurveyPoint;
                                        errors.Add(msg);
                                    }
                                }
                                else
                                {
                                    if (!Istwo) break;
                                }
                            }
                            else
                            {
                                pd.Datas.Add(sd);
                            }
                        }
                        #endregion

                        #region//读取第二个点
                        if (Istwo)
                        {
                            errmsg = null;
                            //info.ZoRIndex = 6;
                            //info.RorTIndex = 7;
                            lastsd = new SurveyData();
                            if (!ReadRow(row, lastsd, info, Istwo, out  errmsg)) lastsd = null;
                            errmsg = null;
                            if (!ReadRow(row, sd, info, Istwo, out  errmsg))//读当前行
                            {
                                if (errmsg != null)
                                {
                                    ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg };
                                    errors.Add(err);
                                }
                                continue;
                            }
                            else
                            {
                                if (flag)
                                {
                                    if (sd.SurveyDate.CompareTo(maxDatetime) > 0)
                                    {
                                        ErrorMsg msg;
                                        if (CheckData(sd, lastsd, info, out msg, pd.SurveyPoint))
                                        {
                                            pd1.Datas.Add(sd);
                                        }
                                        else
                                        {
                                            msg.ErrorRow = j + 1;
                                            msg.PointNumber = pd.SurveyPoint;
                                            errors.Add(msg);
                                        }
                                    }
                                    else { break; }
                                }
                                else
                                {
                                    pd1.Datas.Add(sd);
                                }
                            }
                        #endregion
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
                if (Istwo) datas.Add(pd1);
            }
            workbook.Close();

        }

        private bool ReadRow(IRow row, SurveyData sd, DataInfo info, bool IsTwo, out string err, bool last = false)
        {
            err = null;
            try
            {
                var cell = row.GetCell(info.DateIndex);
                if (cell == null || !HSSFDateUtil.IsCellDateFormatted(cell))return false;
                if(cell.CellType!=CellType.Numeric&&cell.CellType!=CellType.Formula) return false;
                if (info.TimeIndex > 0 && row.GetCell(info.TimeIndex) != null &&
                        row.GetCell(info.TimeIndex).ToString() != "")
                {
                    var date = row.GetCell(info.DateIndex).DateCellValue.ToString("MM/dd/yyyy");
                    string time;
                    if (row.GetCell(info.TimeIndex).CellType == CellType.Numeric)
                    {
                        time = row.GetCell(info.TimeIndex).DateCellValue.TimeOfDay.ToString();
                    }
                    else
                    {
                        time = row.GetCell(1).ToString();
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
                if (row.GetCell(info.RorTIndex) != null && !String.IsNullOrEmpty(row.GetCell(info.RorTIndex).ToString()))
                {
                    double.TryParse(row.GetCell(info.RorTIndex).ToString(), out sd.Survey_RorT);//温度
                }
                int index = 6;
                if (IsTwo) index = 10;
                sd.Remark = row.GetCell(index) == null ? "" : row.GetCell(index).ToString();
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
        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            IFormatProvider culture = CultureInfo.CurrentCulture;
            System.Collections.IEnumerator rows = psheet.GetRowEnumerator();
            int count = psheet.LastRowNum;
            bool flag = true;
            bool Zflag = true;
            bool Rflag = true;
            string filename = Path.GetFileName(filePath);
            for (int j = 0; j < 10; j++)//读取前10行
            {
                IRow row = psheet.GetRow(j);
                if (row == null) continue;
                string laststr = "";

                for (int c = 0; c < row.Cells.Count; c++)
                {
                    var cellstr = row.Cells[c].ToString();
                    int pyhindex = row.Cells[c].ColumnIndex;
                    if (DataUtils.CheckContainStr(cellstr, "观测日期", "日期", "年-月-日"))
                    {
                        flag = false;
                        info.DateIndex = pyhindex;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "观测时间", "时间", "时")) info.TimeIndex = pyhindex;
                    else if (DataUtils.CheckContainStr(cellstr, "MPa/F", "电阻比", "频模", "频率", "模数") &&
                         !DataUtils.CheckContainStr(cellstr, "电阻比变化", "反测电阻比"))
                    {
                        info.ZoRIndex = pyhindex;
                        Zflag = false;

                    }
                    else if (DataUtils.CheckContainStr(cellstr, "电阻值", "温度电阻", "电阻")&&
                       !DataUtils.CheckContainStr(cellstr, "电阻比", "电阻变化"))
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
        
        
    }

    /// <summary>钢板计
    /// </summary>
    public class Fiducial_Armor_plateProcessMW : ProcessData
    {
        public Fiducial_Armor_plateProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Armor_plate;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "钢板计";

        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            //datas = new List<PointSurveyData>();
            //errors = new List<ErrorMsg>();
            //string filename = Path.GetFileName(path);
            //if (filename != "引水洞-钢板计.xlsx") return;
            base.LoadData(path, null, out datas, out errors);
        }
        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
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
                    else if (DataUtils.CheckContainStr(cellstr, "MPa/F", "读数(F)", "电阻比", "频模", "频率", "模数"))
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
    }
    /// <summary> 锚杆应力计
    /// </summary>
    public class Fiducial_Anchor_PoleProcessMW : ProcessData
    {
        public Fiducial_Anchor_PoleProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Anchor_Pole;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "锚杆应力计";

        }
        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            bool flag = true;
            bool Rflag = true;
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
                    if (DataUtils.CheckContainStr(cellstr, "观测日期", "日期", "年-月-日"))
                    {
                        flag = false;
                        info.DateIndex = pyhindex;
                    }
                    if (DataUtils.CheckContainStr(cellstr,"观测时间","时间","时")) info.TimeIndex = pyhindex;

                   if (DataUtils.CheckContainStr(cellstr, "MPa/F", "电阻比", "频模", "频率", "模数") &&
                   !DataUtils.CheckContainStr(cellstr, "电阻比变化", "反测电阻比"))
                    {
                        info.ZoRIndex = pyhindex;
                    }
                   if (DataUtils.CheckContainStr(cellstr, "电阻值", "温度电阻", "电阻") &&
                      !DataUtils.CheckContainStr(cellstr, "电阻比", "电阻变化"))
                   {
                       info.RorTIndex = pyhindex;
                       Rflag = false;
                   }


                   if (Rflag)
                   {
                       if (cellstr.Contains("温度")) info.RorTIndex = pyhindex;
                   }
                    if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                    laststr = cellstr;
                }
            }
            if (flag)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }


            return info;
        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            base.LoadData(path, null, out datas, out errors);
        }

    }

    /// <summary>压应力计
    /// </summary>
    public class Fiducial_Press_StressProcessMW : ProcessData
    {
        public Fiducial_Press_StressProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Press_Stress;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "压应力计";

        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            DataInfo index = new DataInfo();
            base.LoadData(path, null, out datas, out errors);
        }
    }

    /// <summary> 基岩变形计
    /// </summary>
    public class Fiducial_Basic_Rock_DistortionProcessMW : ProcessData
    {
        public Fiducial_Basic_Rock_DistortionProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Basic_Rock_Distortion;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "基岩变形计";

        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            base.LoadData(path, null, out datas, out errors);
        }

    }
    /// <summary> 多点位移计
    /// </summary>
    public class Fiducial_Multi_DisplacementProcessMW : ProcessData
    {
        public Fiducial_Multi_DisplacementProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Multi_Displacement;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "多点位移计";

        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {

            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            //string filename = Path.GetFileName(path);
            //if (filename != "进水口边坡-多点位移计.xlsx") return;
            var workbook = WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            //测试代码
#if TEST
            PointCach.Add(path, new List<string>());
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
                DataInfo info = GetInfo(psheet);

                DateTime maxDatetime = new DateTime();
                string sql = String.Format("select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number", info.TableName);
                var result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                bool flag = (result != DBNull.Value);
                if (flag) maxDatetime = (DateTime)result;
                sql = String.Format("select Instrument_Serial  from {0} where Survey_point_Number=@Survey_point_Number", Config.InsCollection[this.Instrument_Name].Fiducial_Table);
                var dt = sqlhelper.SelectData(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                info.Sum = dt.Rows.Count;
                List<string> seriallist = GetSerial(dt);

                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = count - 1; j > 0; j--)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        var cell = row.GetCell(info.DateIndex);
                        if (cell == null || String.IsNullOrEmpty(cell.ToString())) continue;
                        if (cell.CellType != CellType.Numeric && cell.CellType != CellType.Formula) continue;
                        if (!HSSFDateUtil.IsCellDateFormatted(cell)) continue;
                        SurveyData sd = new SurveyData();
                        string errmsg = null;
                        if (!ReadRow(row, info, sd, seriallist, out  errmsg))//读当前行
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
                        if (!ReadRow(lastrow, info, lastsd, seriallist, out errmsg, true)) lastsd = null;

                        if (flag)
                        {
                            if (sd.SurveyDate.CompareTo(maxDatetime) > 0)
                            {
                                ErrorMsg msg;
                                if (CheckData(sd, lastsd, info, out msg, pd.SurveyPoint))
                                {
                                    pd.Datas.Add(sd);
                                    if (msg != null)
                                    {
                                        msg.ErrorRow = row.RowNum + 1;
                                        msg.PointNumber = pd.SurveyPoint;
                                        errors.Add(msg);
                                    }

                                }
                                else
                                {
                                    msg.ErrorRow = row.RowNum + 1;
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

        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            info.Findex = -1;
            bool flag = true;
            int TCount = 0;
            int TempIndex = -1;
            int count = psheet.LastRowNum > 15 ? 15 : psheet.LastRowNum;
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
                    else if (DataUtils.CheckContainStr(cellstr, "温度电阻", "电阻值") || cellstr.StartsWith("温度"))
                    {
                        info.Findex = info.Findex < 0 ? pyhindex : info.Findex;
                        if (info.RorTIndex != pyhindex)
                        {
                            info.RorTIndex = pyhindex;
                            TCount++;
                        }
                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                    laststr = cellstr;
                    if (cellstr.Contains("模数值")) TempIndex = pyhindex;
                    if (DataUtils.CheckContainStr(cellstr, "观测时间", "时间")) info.TimeIndex = pyhindex;
                }
            }
            if (flag && info.TimeIndex > 0)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }
            if (TCount > 1)
            {
                info.RorTIndex = -1;
                if (info.Findex - info.DateIndex > 2)
                {
                    info.Findex = info.DateIndex + 1;
                }
                else
                {
                    info.Findex = info.TimeIndex > 0 ? info.TimeIndex + 1 : info.DateIndex;
                }
            }
            else
            {
                info.Findex = (info.TimeIndex < 0 && TempIndex > 0) ? TempIndex : -1;

            }
            return info;

        }

        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, out ErrorMsg err, string Survey_point_name)
        {
            err = new ErrorMsg();
            if (sd.Remark.Contains("已复测")) return true;
            if (lastsd == null) return true;//上一行数据为空不处理
            //先跟上一次的数据做比较，不超过限值直接return
            int count = 0;
            foreach (var dic in sd.MultiDatas)
            {
                if (dic.Value.Survey_ZorR == 0 || lastsd.MultiDatas[dic.Key].Survey_ZorR == 0) continue;
                if (Math.Abs(dic.Value.Survey_ZorR - lastsd.MultiDatas[dic.Key].Survey_ZorR) > Config.LimitZ)
                {
                    err.Exception = "数据误差超限";
                    count++;
                }
            }
            if (count == 0) err = null;
            if (count < sd.MultiDatas.Keys.Count) return true;
            err.Exception = "所有数据误差超限";
            return false;
        }

        List<string> GetSerial(DataTable dt)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string serialnum = (string)dt.Rows[i][0];
                list.Add(serialnum);
                //bool flag = true;
                //for (int j = list.Count-1; j>=0; j--)
                //{
                //    if (Compare(serialnum, list[j]))
                //    {
                //        list.Insert(j, serialnum);
                //        flag = false;
                //        break;
                //    }
                //}
                //if (flag) list.Add(serialnum);

            }
            return list;
        }
        bool Compare(string left, string right)
        {
            double leftvalue = GetDepth(left);
            double rightvalue = GetDepth(right);
            return (leftvalue >= rightvalue);
        }
        double GetDepth(string serial)
        {
            try
            {
                var tempstr = serial.Split('-');
                int len = tempstr.Length;
                string str = tempstr[len - 1].Contains('A') ? tempstr[len - 1].Substring(0, len - 1) : tempstr[len - 1];
                return double.Parse(str);
            }
            catch
            {
                return 0;
            }
        }
        private bool ReadRow(IRow row, DataInfo info, SurveyData sd, List<string> seriallist, out string err, bool last = false)
        {
            err = null;
            try
            {
                var cell = row.GetCell(info.DateIndex);
                if (cell == null) return false;
                if (!HSSFDateUtil.IsCellDateFormatted(cell)) return false;

                if (info.TimeIndex > 0 && row.GetCell(info.TimeIndex) != null &&
                    row.GetCell(info.TimeIndex).ToString() != "")
                {
                    var timecell = row.GetCell(info.TimeIndex);
                    var date = cell.DateCellValue.ToString("MM/dd/yyyy");
                    string time;
                    if (timecell.CellType == CellType.Numeric)
                    {
                        time = timecell.DateCellValue.TimeOfDay.ToString();
                    }
                    else
                    {
                        time = timecell.ToString();
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
                bool flag = false;
                if (info.RorTIndex > 0)//只有一列温度
                {
                    if (row.GetCell(info.RorTIndex) != null && !String.IsNullOrEmpty(row.GetCell(info.RorTIndex).ToString()))
                    {
                        //温度//多点位移计只读一个温度值
                        double.TryParse(row.GetCell(info.RorTIndex).ToString(), out sd.Survey_RorT);
                    }
                    int mid = info.TimeIndex > 0 ? info.RorTIndex - info.TimeIndex : info.RorTIndex - info.DateIndex;
                    int firstindex = (mid > 1) ? (info.TimeIndex > 0 ? info.TimeIndex : info.DateIndex) : info.RorTIndex;
                    if (info.Findex > 0) firstindex = info.Findex - 1;
                    for (int i = 0; i < info.Sum; i++)
                    {
                        SurveyData temp = new SurveyData();

                        if (row.GetCell(firstindex + 1 + i) != null)
                        {
                            //频率/基准电阻
                            if (double.TryParse(row.GetCell(firstindex + 1 + i).ToString(), out temp.Survey_ZorR))
                            {
                                flag = true;
                            }
                        }
                        sd.MultiDatas.Add(seriallist[i], temp);//成不成功都添加//吧位子占了
                    }
                }
                else//有多列温度只读一列
                {
                    flag = false;
                    bool tflag = false;
                    int firstindex = info.Findex + 1;
                    for (int i = 0; i < info.Sum; i++)
                    {
                        SurveyData temp = new SurveyData();
                        if (row.GetCell(firstindex + i * 2) != null)
                        {
                            //频率/基准电阻
                            if (double.TryParse(row.GetCell(firstindex + i * 2).ToString(), out temp.Survey_ZorR))
                            {
                                flag = true;
                            }
                        }
                        sd.MultiDatas.Add(seriallist[i], temp);
                        if (!tflag && row.GetCell(firstindex + i * 2 + 1) != null)
                        {
                            //温度//多点位移计只读一个温度值
                            if (double.TryParse(row.GetCell(firstindex + i * 2 + 1).ToString(), out sd.Survey_RorT)) tflag = true;
                        }
                    }
                }
                if (!flag)
                {
                    sd = null;
                    return false;
                }
                sd.Remark = row.GetCell(info.TimeIndex + 2 * info.Sum + 4) == null ? "" : row.GetCell(info.TimeIndex + 2 * info.Sum + 4).ToString();
                return true;
            }
            catch (Exception ex)
            {
                sd = null;
                if (!last) throw ex;
                return false;
            }

        }

    }

    /// <summary> 应变计
    /// </summary>
    public class Fiducial_Strain_GaugeProcessMW : ProcessData
    {
        public Fiducial_Strain_GaugeProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Strain_Gauge;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "应变计";

        }
      
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            IFormatProvider culture = CultureInfo.CurrentCulture;
            var workbook = WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                string sheetname = psheet.SheetName;
                if (sheetname.Contains("与"))//应变计和无应力计在一个sheet
                {
                    sheetname = sheetname.Split('与')[0];
                }
                if (!CheckName(sheetname))
                {
                    if (CheckNonStress(sheetname))
                    {
                        ReadNonStressData(psheet, errors, sheetname);
                    }
                    else
                    {
                        AddErroSheetname(path, psheet.SheetName);
                    }
                    continue;
                }
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData();
                pd.SurveyPoint = sheetname;
                DataInfo info = GetInfo(psheet, path);
                if (info.FCount > 1)
                {
                    ReadNonStressData(psheet, errors);
                }

                DateTime maxDatetime = new DateTime();
                string sql = String.Format("select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number", info.TableName);
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
                                if (CheckData(sd, lastsd, info, out msg, pd.SurveyPoint))
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
            try
            {
                if (row == null) return false;
                var cell = row.GetCell(info.DateIndex);
                if (cell == null || cell.CellType != CellType.Numeric || !HSSFDateUtil.IsCellDateFormatted(cell)) return false;
                if (info.TimeIndex > 0 && row.GetCell(info.TimeIndex) != null &&
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
                if (row.GetCell(info.RorTIndex) != null && !String.IsNullOrEmpty(row.GetCell(info.RorTIndex).ToString()))
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
            catch (Exception ex)
            {
                sd = null;
                if (!last) throw ex;
                return false;
            }
        }

        //读取无应力计的数据
        private void ReadNonStressData(ISheet psheet, List<ErrorMsg> errors, string number = null)
        {
            var sqlhelper = CSqlServerHelper.GetInstance();
            if (number == null)
            {
                number = psheet.SheetName;
                if (psheet.SheetName.Contains("与"))//应变计和无应力计在一个sheet
                {
                    number = psheet.SheetName.Split('与')[1];
                }
                else
                {
                    //在应变计考证表中查无应力计的测点编号
                    string temp = string.Format("select Nonstress_Number from Fiducial_Strain_Gauge where Survey_Point_Number='{0}'", number);
                    var res = sqlhelper.SelectFirst(temp);
                    if (res != DBNull.Value) number = res.ToString();
                }
            }
            if (!CheckNonStress(number)) return;
            PointSurveyData pd = new PointSurveyData();
            pd.SurveyPoint = number;
            DataInfo info = GetNonInfo(psheet);
            DateTime maxDatetime = new DateTime();
            string sql = String.Format("select max(Observation_Date) from Survey_Nonstress where Survey_point_Number=@Survey_point_Number", info.TableName);
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
                    var cell = row.GetCell(info.DateIndex);
                    if (cell == null || String.IsNullOrEmpty(cell.ToString()) || cell.CellType != CellType.Numeric) continue;
                    SurveyData sd = new SurveyData();
                    string errmsg = null;
                    if (!ReadRow(row, info, sd, out  errmsg))//读当前行
                    {
                        if (errmsg != null)
                        {
                            ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg + "无应力计" };
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
                            if (CheckData(sd, lastsd, info, out msg, pd.SurveyPoint))
                            {
                                pd.Datas.Add(sd);
                            }
                            else
                            {
                                msg.ErrorRow = j + 1;
                                msg.PointNumber = pd.SurveyPoint;
                                msg.Exception += "无应力计";
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
                    ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = ex.Message + "无应力计" };
                    errors.Add(err);
                    continue;
                }
            }
            NonStressDataCach.Add(pd);

        }
        private DataInfo GetNonInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            info.Findex = -1;
            int TCount = 0;
            bool flag = true;
            int count = psheet.LastRowNum > 15 ? 15 : psheet.LastRowNum;
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
                    else if (DataUtils.CheckContainStr(cellstr, "观测时间", "时间")) info.TimeIndex = pyhindex;
                    else if (DataUtils.CheckContainStr(cellstr, "电阻比", "频率", "频模", "模数"))
                    {
                        info.ZoRIndex = pyhindex;
                        TCount++;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "电阻值", "温度电阻"))
                    {
                        info.RorTIndex = pyhindex;
                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                    if (DataUtils.CheckContainStr(laststr, "频率", "频模", "模数"))
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
            info.FCount = TCount;
            return info;
        }

    }

    /// <summary> 无应力计
    /// </summary>
    public class Fiducial_NonstressProcessMW: ProcessData
    {
        public Fiducial_NonstressProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Nonstress;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "无应力计";

        }

        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            info.Findex = -1;
            int TCount = 0;
            bool flag = true;
            int count = psheet.LastRowNum > 15 ? 15 : psheet.LastRowNum;
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
                    else if (DataUtils.CheckContainStr(cellstr, "观测时间", "时间")) info.TimeIndex = pyhindex;
                    else if (DataUtils.CheckContainStr(cellstr, "电阻比", "频率", "频模", "模数"))
                    {
                        info.ZoRIndex = pyhindex;
                        info.Findex = info.Findex < 0 ? pyhindex : info.Findex;
                        TCount++;

                    }
                    else if (DataUtils.CheckContainStr(cellstr, "电阻值", "温度电阻"))
                    {
                        info.RorTIndex = pyhindex;
                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                    if (DataUtils.CheckContainStr(laststr, "频率", "频模", "模数"))
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
            info.FCount = TCount;
            return info;
        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            LoadDataExpand(path, null, out datas, out errors);
        }

        void LoadDataExpand(string path, DataInfo info, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            IFormatProvider culture = CultureInfo.CurrentCulture;
            var workbook = WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                string sheetname = psheet.SheetName;

                if (!CheckName(sheetname))
                {
                    AddErroSheetname(path, psheet.SheetName);
                    continue;
                }
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData();
                pd.SurveyPoint = sheetname;
                info = GetInfo(psheet, path);

                DateTime maxDatetime = new DateTime();
                string sql = String.Format("select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number", info.TableName);
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
                                if (CheckData(sd, lastsd, info, out msg, pd.SurveyPoint))
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
            try
            {
                if (row == null) return false;
                var cell = row.GetCell(info.DateIndex);
                if (cell == null || cell.CellType != CellType.Numeric || !HSSFDateUtil.IsCellDateFormatted(cell)) return false;
                if (info.TimeIndex > 0 && row.GetCell(info.TimeIndex) != null &&
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
                    if (DateTime.TryParse(date + " " + time, CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out  sd.SurveyDate))
                    {
                        sd.SurveyDate = cell.DateCellValue;
                    }
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
                    sd.Survey_ZorRMoshu = sd.Survey_ZorR;
                }
                if (row.GetCell(info.RorTIndex) != null && !String.IsNullOrEmpty(row.GetCell(info.RorTIndex).ToString()))
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
            catch (Exception ex)
            {
                sd = null;
                if (!last) throw ex;
                return false;
            }
        }

    }
    /// <summary> 锚索测力计
    /// </summary>
    public class Fiducial_Anchor_CableProcessMW : ProcessData
    {
        public Fiducial_Anchor_CableProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Anchor_Cable;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "锚索测力计";

        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
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
                    AddErroSheetname(path, psheet.SheetName);
                    continue;
                }
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData();
                pd.SurveyPoint = psheet.SheetName;
                DataInfo info = GetInfo(psheet);

                DateTime maxDatetime = new DateTime();
                string sql = String.Format("select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number", info.TableName);
                var result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                bool flag = (result != DBNull.Value);
                if (flag) maxDatetime = (DateTime)result;
                sql = "select Read_GroupNum from Fiducial_Anchor_Cable where Survey_point_Number=@Survey_point_Number";
                result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                if (result == DBNull.Value) continue;
                info.Sum = Convert.ToInt16(result);

                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = count - 1; j > 0; j--)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        var cell = row.GetCell(info.DateIndex);
                        if (cell == null || cell.CellType != CellType.Numeric || !HSSFDateUtil.IsCellDateFormatted(cell)) continue;
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
                                if (CheckData(sd, lastsd, info, out msg, pd.SurveyPoint))
                                {
                                    pd.Datas.Add(sd);
                                    if (msg != null)
                                    {
                                        msg.ErrorRow = row.RowNum + 1;
                                        msg.PointNumber = pd.SurveyPoint;
                                        errors.Add(msg);
                                    }
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
        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, out ErrorMsg err, string Survey_point_name)
        {
            err = new ErrorMsg();
            if (sd.Remark.Contains("已复测")) return true;
            if (lastsd == null) return true;//上一行数据为空不处理
            //先跟上一次的数据做比较，不超过限值直接return
            int count = 0;
            foreach (var dic in sd.MultiDatas)
            {
                if (dic.Value.Survey_ZorR == 0 || lastsd.MultiDatas[dic.Key].Survey_ZorR == 0) continue;
                if (Math.Abs(dic.Value.Survey_ZorR - lastsd.MultiDatas[dic.Key].Survey_ZorR) > Config.LimitZ)
                {
                    err.Exception = "数据误差超限";
                    count++;
                }
            }
            if (count == 0) err = null;
            if (count < sd.MultiDatas.Keys.Count) return true;
            err.Exception = "所有数据误差超限";
            return false;
        }
        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            bool flag = true;
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
                    else if (DataUtils.CheckContainStr(cellstr, "观测时间", "时间")) info.TimeIndex = pyhindex;
                    else if (DataUtils.CheckContainStr(cellstr, "温度")) info.RorTIndex = pyhindex;
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;

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
      
        private bool ReadRow(IRow row, DataInfo info, SurveyData sd, out string err, bool last = false)
        {
            err = null;
            try
            {
                var cell = row.GetCell(info.DateIndex);
                if (cell == null || cell.CellType != CellType.Numeric || !HSSFDateUtil.IsCellDateFormatted(cell)) return false;
                if (info.TimeIndex > 0 && HSSFDateUtil.IsCellDateFormatted(row.GetCell(info.TimeIndex)))
                {
                    var timecell = row.GetCell(info.TimeIndex);
                    var date = cell.DateCellValue.ToString("MM/dd/yyyy");
                    string time;
                    if (timecell.CellType == CellType.Numeric)
                    {
                        time = timecell.DateCellValue.TimeOfDay.ToString();
                    }
                    else
                    {
                        time = timecell.ToString();
                    }
                    sd.SurveyDate = DateTime.Parse(date + " " + time, CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault);
                }
                else
                {
                    sd.SurveyDate = cell.DateCellValue;
                }
                if (row.GetCell(info.RorTIndex) != null && !String.IsNullOrEmpty(row.GetCell(info.RorTIndex).ToString()))
                {
                    double.TryParse(row.GetCell(info.RorTIndex).ToString(), out sd.Survey_RorT);
                }
                bool flag = false;
                int firstindex = info.TimeIndex > 0 ? info.TimeIndex : info.DateIndex;
                for (int i = 0; i < info.Sum; i++)
                {
                    SurveyData temp = new SurveyData();
                    if (row.GetCell(firstindex + 1 + i) != null && !String.IsNullOrEmpty(row.GetCell(firstindex + 1 + i).ToString().Trim()))
                    {
                        //频率/基准电阻
                        if (double.TryParse(row.GetCell(firstindex + 1 + i).ToString(), out temp.Survey_ZorR))
                        {
                            flag = true;
                        }
                        sd.MultiDatas.Add(i.ToString(), temp);
                    }
                }
                if (!flag)
                {
                    sd = null;
                    return false;
                }

                sd.Remark = row.GetCell(info.TimeIndex + 2 * info.Sum + 4) == null ? "" : row.GetCell(info.TimeIndex + 2 * info.Sum + 4).ToString();
                return true;
            }
            catch (Exception ex)
            {
                sd = null;
                if (!last) throw ex;
                return false;
            }

        }

    }
    /// <summary> 应变计组
    /// </summary>
    public class Fiducial_Strain_GroupProcessMW : ProcessData
    {
        public Fiducial_Strain_GroupProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Multi_Displacement;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "应变计组";

        }

        public void ReadDataExpand(string path,  out List<PointSurveyData> datas,  out List<ErrorMsg> errors)
        {
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            var workbook = WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            Dictionary<string, List<SurveyData>> SerialData = new Dictionary<string, List<SurveyData>>();
            List<string> Points = new List<string>();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                string p = Regex.Match(psheet.SheetName, @"(?<=\（).*(?=\）)").Groups[0].ToString();
                string point = Regex.Match(psheet.SheetName, @".*(?=（)").Groups[0].ToString();
                if (!CheckName(point))
                {
                    AddErroSheetname(path, psheet.SheetName);
                    continue;
                }
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                if (!Points.Contains(point)) Points.Add(point);
                
                int number = 1;
                string serialNumber = point.ToUpper();
                if(int.TryParse(p,out number))
                {
                    serialNumber = serialNumber + "-" + number.ToString();
                }
                else
                {
                    serialNumber = serialNumber + "-" + ((p == "右岸") ? 1 : 2);
                }
                DataInfo info = base.GetInfo(psheet);
                DateTime maxDatetime =new DateTime();
                bool flag = GetMaxDate(point, out maxDatetime);
                SurveyData lastsd = null;
                int count = psheet.LastRowNum;

                for (int j = count - 1; j >= 0; j--)
                {
                    #region
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        var cell = row.GetCell(info.DateIndex);
                        if (CheckCell(cell)) continue;
                        if (cell.CellType != CellType.Numeric && cell.CellType != CellType.Formula) continue;
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
                                if (CheckData(sd, lastsd, info, out msg, psheet.SheetName))
                                {
                                    if (SerialData.ContainsKey(serialNumber))
                                    {
                                        SerialData[serialNumber].Add(sd);
                                    }
                                    else
                                    {
                                        SerialData.Add(serialNumber, new List<SurveyData>() { sd });

                                    }
                                }
                                else
                                {
                                    msg.ErrorRow = j + 1;
                                    msg.PointNumber = psheet.SheetName;
                                    errors.Add(msg);
                                }
                                continue;
                            }
                            break;
                        }
                        if (SerialData.ContainsKey(serialNumber))
                        {
                            SerialData[serialNumber].Add(sd);
                        }
                        else
                        {
                            SerialData.Add(serialNumber, new List<SurveyData>() { sd });
 
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = ex.Message };
                        errors.Add(err);
                        continue;
                    }
                    #endregion
                }
            }
            datas = GetDataFromDic(Points, SerialData);
 
        }

        private List<PointSurveyData> GetDataFromDic(List<string> PointsName, Dictionary<string, List<SurveyData>> Dic)
        {
            List<PointSurveyData> Datas = new List<PointSurveyData>();
            var Sqlhelper = CSqlServerHelper.GetInstance();
            foreach (string SurveyPoint in PointsName)
            {
                string sql = String.Format("select Instrument_Serial from Fiducial_Strain_Group where Survey_point_Number='{0}' order by Instrument_Serial", SurveyPoint);
                var dt = Sqlhelper.SelectData(sql);
                if (dt.Rows.Count<1) continue;
                List<string> serials = new List<string>();
                int count = 0;
                string Matchser = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string tempser = dt.Rows[i][0].ToString().ToUpper();
                    if (Dic[tempser].Count > count)
                    {
                        count = Dic[tempser].Count;
                        Matchser = tempser;
                    }                    
                    serials.Add(tempser);

                }
                PointSurveyData pd = new PointSurveyData();
                pd.SurveyPoint = SurveyPoint;
                for (int i = 0; i < count; i++)
                {
                    var dtmath = Dic[Matchser][i].SurveyDate;
                    SurveyData oneData = new SurveyData();
                    oneData.SurveyDate = dtmath;
                    foreach (string serial in serials)
                    {
                        var sd = Dic[serial].Where(s => s.SurveyDate.Date == dtmath.Date).FirstOrDefault();
                        if (sd == null) sd = new SurveyData();
                        oneData.MultiDatas.Add(serial, sd);
                    }
                    pd.Datas.Add(oneData);
                }
                Datas.Add(pd);
            }
            return Datas;
        }
        
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            var workbook = WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                if (!CheckName(psheet.SheetName))
                {
                    if (CheckNonStress(psheet.SheetName))
                    {
                        DataInfo tempinfo = base.GetInfo(psheet);
                        ReadNonStressData(psheet, tempinfo, errors, psheet.SheetName);

                    }
                    else if (psheet.SheetName.Contains("（"))
                    {
                        ReadDataExpand(path,out  datas, out  errors);
                        return;
                    }
                    else
                    {
                        AddErroSheetname(path, psheet.SheetName);
                    }
                    continue;
                }
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData();
                pd.SurveyPoint = psheet.SheetName;
                DataInfo info = GetInfo(psheet);

                DateTime maxDatetime = new DateTime();
                string sql = String.Format("select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number", info.TableName);
                var result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                bool flag = (result != DBNull.Value);
                if (flag) maxDatetime = (DateTime)result;
                sql = String.Format("select Instrument_Serial  from {0} where Survey_point_Number=@Survey_point_Number order by Instrument_Serial", Config.InsCollection[this.Instrument_Name].Fiducial_Table);
                var dt = sqlhelper.SelectData(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                info.Sum = dt.Rows.Count;
                List<string> seriallist = new List<string>();
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    seriallist.Add(dt.Rows[k][0].ToString());
                }
                if (info.FCount > seriallist.Count)
                {
                    ReadNonStressData(psheet, info, errors);//读取无应力计的数据
                    info.Findex += 2;//包含无应力计的数据加一组索引
                }

                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = count - 1; j > 0; j--)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        var cell = row.GetCell(info.DateIndex);
                        if (cell == null || String.IsNullOrEmpty(cell.ToString()) || cell.CellType != CellType.Numeric) continue;
                        SurveyData sd = new SurveyData();
                        string errmsg = null;
                        if (!ReadRow(row, info, sd, seriallist, out  errmsg))//读当前行
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
                        if (lastrow == null || !ReadRow(lastrow, info, lastsd, seriallist, out errmsg, true)) lastsd = null;

                        if (flag)
                        {
                            if (sd.SurveyDate.CompareTo(maxDatetime) > 0)
                            {
                                ErrorMsg msg;
                                if (CheckData(sd, lastsd, info, out msg, pd.SurveyPoint))
                                {
                                    pd.Datas.Add(sd);
                                    if (msg != null)
                                    {
                                        msg.ErrorRow = row.RowNum + 1;
                                        msg.PointNumber = pd.SurveyPoint;
                                        errors.Add(msg);
                                    }
                                }
                                else
                                {
                                    msg.ErrorRow = row.RowNum + 1;
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

        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            info.Findex = -1;
            bool flag = true;
            int TCount = 0;
            int TempIndex = -1;
            int count = psheet.LastRowNum > 15 ? 15 : psheet.LastRowNum;
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
                    else if (DataUtils.CheckContainStr(cellstr, "观测时间", "时间")) info.TimeIndex = pyhindex;
                    else if (DataUtils.CheckContainStr(cellstr, "电阻比"))
                    {
                        info.Findex = info.Findex < 0 ? pyhindex : info.Findex;
                        TCount++;

                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                    laststr = cellstr;
                    if (cellstr.Contains("模数值")) TempIndex = pyhindex;
                }
            }
            if (flag && info.TimeIndex > 0)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }
            info.FCount = TCount;

            return info;

        }

        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, out ErrorMsg err, string Survey_point_name)
        {
            err = null;
            if (sd.Remark.Contains("已复测")) return true;
            if (lastsd == null) return true;//上一行数据为空不处理
            //先跟上一次的数据做比较，不超过限值直接return
            int count = 0;
            err = new ErrorMsg();
            foreach (var dic in sd.MultiDatas)
            {
                if (dic.Value.Survey_ZorR == 0 || lastsd.MultiDatas[dic.Key].Survey_ZorR == 0) continue;
                if (Math.Abs(dic.Value.Survey_ZorR - lastsd.MultiDatas[dic.Key].Survey_ZorR) > Config.LimitZ)
                {
                    err.Exception = "部分数据误差超限";
                    count++;
                }
            }
            if (count == 0) err = null;
            if (count < sd.MultiDatas.Keys.Count) return true;
            err.Exception = "所有数据误差超限";
            return false;
        }

        private bool ReadRow(IRow row, DataInfo info, SurveyData sd, List<string> seriallist, out string err, bool last = false)
        {
            err = null;
            try
            {
                var cell = row.GetCell(info.DateIndex);
                if (cell == null) return false;
                if (cell.CellType != CellType.Numeric && cell.CellType != CellType.Formula) return false;
                if (!HSSFDateUtil.IsCellDateFormatted(cell)) return false;
                if (info.TimeIndex > 0 && row.GetCell(info.TimeIndex) != null &&
                    row.GetCell(info.TimeIndex).ToString() != "")
                {
                    var timecell = row.GetCell(info.TimeIndex);
                    var date = cell.DateCellValue.ToString("MM/dd/yyyy");
                    string time;
                    if (timecell.CellType == CellType.Numeric)
                    {
                        time = timecell.DateCellValue.TimeOfDay.ToString();
                    }
                    else
                    {
                        time = timecell.ToString();
                    }
                    if (!DateTime.TryParse(date + " " + time, CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out sd.SurveyDate))
                    {
                        sd.SurveyDate = cell.DateCellValue;
                    }
                }
                else
                {
                    sd.SurveyDate = cell.DateCellValue;
                }
                bool flag = false;
                int firstindex = info.Findex;
                for (int i = 0; i < info.Sum; i++)
                {
                    SurveyData temp = new SurveyData();
                    if (row.GetCell(firstindex + i * 2) != null)
                    {
                        //频率/基准电阻
                        if (double.TryParse(row.GetCell(firstindex + i * 2).ToString(), out temp.Survey_ZorR))
                        {
                            flag = true;
                        }
                    }
                    //电阻
                    if (row.GetCell(firstindex + i * 2 + 1) != null) double.TryParse(row.GetCell(firstindex + i * 2 + 1).ToString(), out temp.Survey_RorT);
                    sd.MultiDatas.Add(seriallist[i], temp);
                }

                if (!flag)
                {
                    sd = null;
                    return false;
                }
                sd.Remark = row.GetCell(info.TimeIndex + 2 * info.Sum + 4) == null ? "" : row.GetCell(info.TimeIndex + 2 * info.Sum + 4).ToString();
                return true;
            }
            catch (Exception ex)
            {
                sd = null;
                if (!last) throw ex;
                return false;
            }

        }

        //读取无应力计的数据
        private void ReadNonStressData(ISheet psheet, DataInfo info, List<ErrorMsg> errors, string number = null)
        {
            var sqlhelper = CSqlServerHelper.GetInstance();
            if (number == null)
            {//无应力计数据和应变计数据在一个sheet里边
                number = psheet.SheetName;
                //在应变机组考证表中查无应力计的测点编号
                string temp = string.Format("select Nonstress_Number from Fiducial_Strain_Group where Survey_Point_Number='{0}'", number);
                var res = sqlhelper.SelectFirst(temp);
                if (res != DBNull.Value) number = res.ToString();
                info.ZoRIndex = info.Findex;//第一个索引
                info.RorTIndex = info.Findex + 1;//温度电阻+1
            }
            if (!CheckNonStress(number)) return;
            PointSurveyData pd = new PointSurveyData();
            pd.SurveyPoint = number;
            DateTime maxDatetime = new DateTime();
            string sql = String.Format("select max(Observation_Date) from Survey_Nonstress where Survey_point_Number=@Survey_point_Number", info.TableName);
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
                    var cell = row.GetCell(info.DateIndex);
                    if (cell == null || String.IsNullOrEmpty(cell.ToString()) || cell.CellType != CellType.Numeric) continue;
                    SurveyData sd = new SurveyData();
                    string errmsg = null;
                    if (!ReadRow(row, info, sd, out  errmsg))//读当前行
                    {
                        if (errmsg != null)
                        {
                            ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg + "无应力计" };
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
                            if (base.CheckData(sd, lastsd, info, out msg, pd.SurveyPoint))
                            {
                                pd.Datas.Add(sd);
                            }
                            else
                            {
                                msg.ErrorRow = j + 1;
                                msg.PointNumber = pd.SurveyPoint;
                                msg.Exception += "无应力计";
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
                    ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = ex.Message + "无应力计" };
                    errors.Add(err);
                    continue;
                }
            }
            NonStressDataCach.Add(pd);

        }

        private bool ReadRow(IRow row, DataInfo info, SurveyData sd, out string err, bool last = false)
        {
            err = null;
            try
            {
                if (row == null) return false;
                var cell = row.GetCell(info.DateIndex);
                if (cell == null || cell.CellType != CellType.Numeric || !HSSFDateUtil.IsCellDateFormatted(cell)) return false;
                if (info.TimeIndex > 0 && row.GetCell(info.TimeIndex) != null &&
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
                    if (DateTime.TryParse(date + " " + time, CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out  sd.SurveyDate))
                    {
                        sd.SurveyDate = cell.DateCellValue;
                    }
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
                    sd.Survey_ZorRMoshu = sd.Survey_ZorR;
                }
                if (row.GetCell(info.RorTIndex) != null && !String.IsNullOrEmpty(row.GetCell(info.RorTIndex).ToString()))
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
            catch (Exception ex)
            {
                sd = null;
                if (!last) throw ex;
                return false;
            }
        }

    }
    /// <summary>
    /// 固定测斜仪
    /// </summary>
    public class Fiducial_Survey_Slant_FixedProcessMW : ProcessData
    {
        public override void ReadData(string path, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            LoadDataExpand(path, null, out datas, out errors);
        }
        void LoadDataExpand(string path,DataInfo info, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            IFormatProvider culture = CultureInfo.CurrentCulture;
            var workbook = WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            //测试代码
#if TEST
            PointCach.Add(path, new List<string>());
#endif
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                string firstname = psheet.SheetName;
                List<string> points = new List<string>();
                int icount = 1;
                if (psheet.SheetName.Contains('～'))
                {
                    var temp = psheet.SheetName.Split('～');
                    firstname = temp[0];
                    int start = int.Parse(firstname.Split('-').Last());
                    int end = int.Parse(temp[1]);
                    string head = firstname.Split('-')[0];
                    for (int k = start; k < end + 1; k++)
                    {
                        points.Add(head + "-" + k.ToString());
                    }
                    icount = end - start + 1;
                }
                else
                {
                    points.Add(firstname);
 
                }
                if (!CheckName(firstname))//值检查第一个
                {
                    AddErroSheetname(path, psheet.SheetName);
                    continue;
                }

                //测试代码
#if TEST
                PointCach[path].Add(psheet.SheetName);
#endif
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                List<PointSurveyData> PointList = new List<PointSurveyData>();
                foreach (string name in points)
                {
                    PointSurveyData pd = new PointSurveyData();
                    pd.SurveyPoint = name;
                    PointList.Add(pd);
                }
                DataInfo dinfo =new DataInfo(){DateIndex=0,TimeIndex=-1,Findex=1,FCount=icount};
                DateTime maxDatetime = new DateTime();
                string sql = String.Format("select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number", Config.InsCollection[this.Instrument_Name].Measure_Table);
                var result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", points[0]));
                bool flag = (result != DBNull.Value);
                if (flag) maxDatetime = (DateTime)result;

                List<Survey_Slant_FixedSurveyData> lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = count - 1; j > 0; j--)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        var cell = row.GetCell(dinfo.DateIndex);
                        if (CheckCell(cell) || cell.CellType != CellType.Numeric) continue;
                        List<Survey_Slant_FixedSurveyData> sds = new List<Survey_Slant_FixedSurveyData>();
                        string errmsg = null;
                        if (!ReadRowExpand(row, dinfo, sds, out  errmsg))//读当前行
                        {
                            if (errmsg != null)
                            {
                                ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg };
                                errors.Add(err);
                            }
                            continue;
                        }
                        lastsd = new List<Survey_Slant_FixedSurveyData>();
                        IRow lastrow = psheet.GetRow(j - 1);
                        if (!ReadRowExpand(lastrow, dinfo, lastsd, out errmsg, true)) lastsd = null;

                        if (flag)
                        {
                              for (int k = 0; k < sds.Count; k++)
                              {
                                    if (sds[k].SurveyDate.CompareTo(maxDatetime) > 0)
                                    {
                                        ErrorMsg msg;
                                        if (CheckData(sds[k], lastsd[k], dinfo, out msg, PointList[k].SurveyPoint))
                                        {
                                            PointList[k].Datas.Add(sds[k]);
                                        }
                                        else
                                        {
                                            msg.ErrorRow = j + 1;
                                            msg.PointNumber = PointList[k].SurveyPoint;
                                            errors.Add(msg);
                                        }
                                        continue;
                                    }
                                    break;

                              }

                        }
                        AddTolist(PointList, sds);
                    }
                    catch (Exception ex)
                    {
                        ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = ex.Message };
                        errors.Add(err);
                        continue;
                    }
                }
                datas.AddRange(PointList);
            }
            workbook.Close();
        }
        void AddTolist(List<PointSurveyData> points, List<Survey_Slant_FixedSurveyData> datas)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].Datas.Add(datas[i]);
            }
        }

        bool ReadRowExpand(IRow row, DataInfo info, List<Survey_Slant_FixedSurveyData> sds, out string err, bool last = false)
        {
            err = null;
            ICell cell;
            try
            {
                if (row == null) return false;
                cell = row.GetCell(info.DateIndex);
                if (cell == null || cell.CellType != CellType.Numeric || !HSSFDateUtil.IsCellDateFormatted(cell)) return false;
                DateTime SurveyDate= new DateTime();
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
                    if (DateTime.TryParse(date + " " + time, CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out  SurveyDate))
                    {
                        SurveyDate = cell.DateCellValue;
                    }
                }
                else
                {
                    SurveyDate = cell.DateCellValue;
                }
                if (!DataUtils.CheckDateTime(SurveyDate))
                {
                    err = "观测日期有误";
                    return false;
                }
                int icount = 0;
                for (int i = 0; i < info.FCount; i++)
                {
                    Survey_Slant_FixedSurveyData sd = new Survey_Slant_FixedSurveyData();
                    sd.SurveyDate = SurveyDate;
                    cell = row.GetCell(info.Findex+i*2);
                    if (!CheckCell(cell))
                    {
                        if (GetDataFromCell(cell, out sd.Reading_A))//频率/基准电阻
                        {
                            icount++;
                        }
                    }
                    sds.Add(sd);
                }
                if (icount==0) return false;//一个也没读成功
                return true;
            }
            catch (Exception ex)
            {
                if (!last) throw ex;
                return false;
            }
        }
        
        public Fiducial_Survey_Slant_FixedProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Survey_Slant_Fixed;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "固定测斜仪";
        }
 
    }

    /// <summary>
    /// 土压力计
    /// </summary>
    public class Fiducial_Soil_StresProcessMW : ProcessData
    {
        public Fiducial_Soil_StresProcessMW()
        {

            base.InsType = InstrumentType.Fiducial_Leakage_Pressure;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "土压力计";
        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            base.LoadData(path, null, out datas, out errors);
        }

    }
    /// <summary>
    /// 温度计
    /// </summary>
    public class Fiducial_TemperatureProcessMW : ProcessData
    {
        public Fiducial_TemperatureProcessMW()
        {

            base.InsType = InstrumentType.Fiducial_Leakage_Pressure;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "温度计";
        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
           LoadDataExpand(path, null, out datas, out errors);
        }
        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
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
                    else if (DataUtils.CheckContainStr(cellstr, "电阻", "电阻值", "温度电阻") && !DataUtils.CheckContainStr(cellstr, "电阻比"))
                    {
                        info.ZoRIndex = pyhindex;
                        Rflag = false;
                        info.RorTIndex = -1;
                        info.Findex = pyhindex;

                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;

                    if (Rflag)
                    {
                        if (cellstr.Contains("温度"))
                        {
                            info.RorTIndex = pyhindex;
                            info.Findex = pyhindex;
                        }
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

       void LoadDataExpand(string path, DataInfo info, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            IFormatProvider culture = CultureInfo.CurrentCulture;
            var workbook = WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            //测试代码
#if TEST
            PointCach.Add(path, new List<string>());
#endif
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                string firstname = psheet.SheetName;
                List<string> points = new List<string>();
                int icount = 1;
                if (psheet.SheetName.Contains('~'))
                {
                    var temp = psheet.SheetName.Split('~');
                    firstname = temp[0];
                    int start = int.Parse(firstname.Split('-').Last());
                    int end = int.Parse(temp[1]);
                    string head = firstname.Split('-')[0];
                    for (int k = start; k < end + 1; k++)
                    {
                        points.Add(head + "-" + k.ToString());
                    }
                    icount = end - start + 1;
                }
                else
                {
                    points.Add(firstname);
                }
                if (!CheckName(firstname))
                {
                    AddErroSheetname(path, psheet.SheetName);
                    continue;
                }

                //测试代码
#if TEST
                PointCach[path].Add(psheet.SheetName);
#endif
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                List<PointSurveyData> PointList = new List<PointSurveyData>();
                for (int k = 0; k < points.Count; k++)
                {
                    PointSurveyData pd = new PointSurveyData();
                    pd.SurveyPoint = points[0];
                    PointList.Add(pd);
                }
                DataInfo dinfo = new DataInfo();
                if (info == null)
                {
                    dinfo = GetInfo(psheet, path);
                }
                else
                {
                    dinfo = (DataInfo)info.Clone();
                }
                if (icount > 1) { dinfo.Findex = 1; }
                dinfo.FCount = icount;

                DateTime maxDatetime = new DateTime();
                string sql = String.Format("select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number", dinfo.TableName);
                var result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", PointList[0].SurveyPoint));
                bool flag = (result != DBNull.Value);
                if (flag) maxDatetime = (DateTime)result;
                List<SurveyData> lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = count - 1; j > 0; j--)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        var cell = row.GetCell(dinfo.DateIndex);
                        if (CheckCell(cell) || cell.CellType != CellType.Numeric) continue;
                        List<SurveyData> sds = new List<SurveyData>();
                        string errmsg = null;
                        if (!ReadRowExpand(row, dinfo, sds, out  errmsg))//读当前行
                        {
                            if (errmsg != null)
                            {
                                ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg };
                                errors.Add(err);
                            }
                            continue;
                        }
                        lastsd = new List<SurveyData>();
                        IRow lastrow = psheet.GetRow(j - 1);
                        if (!ReadRowExpand(lastrow, dinfo, lastsd, out errmsg, true)) lastsd = null;

                        if (flag)
                        {
                            for (int k = 0; k < sds.Count; k++)
                            {
                                if (sds[k].SurveyDate.CompareTo(maxDatetime) > 0)
                                {
                                    ErrorMsg msg;
                                    if (CheckData(sds[k], lastsd[k], dinfo, out msg, PointList[k].SurveyPoint))
                                    {
                                        PointList[k].Datas.Add(sds[k]);
                                    }
                                    else
                                    {
                                        msg.ErrorRow = j + 1;
                                        msg.PointNumber = PointList[k].SurveyPoint;
                                        errors.Add(msg);
                                    }
                                    continue;
                                }
                                break;

                            }

                        }
                        for (int k = 0; k < PointList.Count; k++)
                        {
                            PointList[k].Datas.Add(sds[k]);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = ex.Message };
                        errors.Add(err);
                        continue;
                    }
                }
                datas.AddRange(PointList);
                
            }
            workbook.Close();
        }
       bool ReadRowExpand(IRow row, DataInfo info, List<SurveyData> sds, out string err, bool last = false)
        {
            err = null;
            ICell cell;
            try
            {
                if (row == null) return false;
                cell = row.GetCell(info.DateIndex);
                if (cell == null || cell.CellType != CellType.Numeric || !HSSFDateUtil.IsCellDateFormatted(cell)) return false;
                DateTime SurveyDate = new DateTime();
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
                    if (DateTime.TryParse(date + " " + time, CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out  SurveyDate))
                    {
                        SurveyDate = cell.DateCellValue;
                    }
                }
                else
                {
                    SurveyDate = cell.DateCellValue;
                }
                for (int i = 0; i < info.FCount; i++)
                {
                    SurveyData sd = new SurveyData();
                    sd.SurveyDate = SurveyDate;
                    if (info.RorTIndex < 0)
                    {
                        cell = row.GetCell(info.Findex+i);
                        if (!CheckCell(cell))
                        {
                            GetDataFromCell(cell, out sd.Survey_ZorR);//电阻
                        }
                    }
                    else
                    {
                        cell = row.GetCell(info.Findex + i);
                        if (!CheckCell(cell))
                        {
                            GetDataFromCell(cell, out sd.Survey_RorT);//温度
                        }
                    }
                    sds.Add(sd);
                    
                }
                return true;
            }
            catch (Exception ex)
            {
                if (!last) throw ex;
                return false;
            }
        }

    }

}
