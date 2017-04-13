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
            string name = psheet.SheetName;
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
                    //if (c> 8) continue;
                    var cell = row.Cells[c];
                    cell.SetCellType(CellType.String);
                    var cellstr = cell.StringCellValue;
                    int pyhindex = row.Cells[c].ColumnIndex;
                    if (pyhindex > 9) break;
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
                    if (DataUtils.CheckContainStr(cellstr, "计算结果", "渗透压力", "压力", "应变")&&
                        !DataUtils.CheckContainStr(cellstr,"压力变化速率"))//对比用
                    {
                        info.Result = pyhindex;
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
            info.RorTIndex = -1;
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
              base.LoadData(path, null, out datas, out errors);
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
        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            info.RorTIndex = -1;
            bool flag = true;
    
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
                    else if (DataUtils.CheckContainStr(cellstr, "测值"))
                    {
                        info.ZoRIndex = pyhindex;
                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                    if (DataUtils.CheckContainStr(cellstr, "计算结果"))//对比用
                    {
                        info.Result = pyhindex;
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
                if (name.Contains("#机"))
                {
                    name = name.Split('机')[1];
                }
                string surveyname1 = name;
                if (name.Contains("～"))
                {
                    var temp = name.Split('～');
                    surveyname1 = temp[0];
                    if (surveyname1.StartsWith("Rlpz"))
                    {
                        LoadDataRlpz(path, psheet, ref datas, ref errors);
                        continue;
                    }
                }
                else if (name.Contains("~"))
                {
                    var temp = name.Split('~');
                    surveyname1 = temp[0];
                    if (surveyname1.StartsWith("R2ZD-Z"))
                    {
                        LoadDataR2ZD(path, psheet, ref datas, ref errors);
                        continue;
                    }
                    else if (surveyname1.StartsWith("RGDK-"))
                    {
                        LoadDataRGDK(path, psheet, ref datas, ref errors);
                        continue;
                    }
                }
                else if (name == "RGD-A-1" || name == "RGD-A-3")
                {
                    LoadDataRGDK(path, psheet, ref datas, ref errors);
                    continue;
                }
                if (!CheckName(surveyname1))
                {
                    if (PointNumberCach.ContainsKey(surveyname1.ToUpper().Trim()))
                    {
                        surveyname1 = PointNumberCach[surveyname1.ToUpper().Trim()];
                    }
                    else
                    {
                        AddErroSheetname(path, psheet.SheetName);
                        continue;
                    }
                }
                //测试代码
#if TEST
                PointCach[path].Add(surveyname1);
#endif
                DataInfo info = GetInfo(psheet, path);
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = LoadOnepoint(path, surveyname1, info, psheet, ref errors);
                datas.Add(pd);
            }
            workbook.Close();

        }
        private bool ReadRow(IRow row, SurveyData sd, DataInfo info, out string err, bool last = false)
        {
            var t1 = DateTime.Now.Ticks;
            err = null;
            try
            {
                var cell = row.GetCell(info.DateIndex);
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false; 
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
                sd.Remark = row.GetCell(info.RemarkIndex) == null ? "" : row.GetCell(info.RemarkIndex).ToString();
                cell = row.GetCell(info.Result);
                if (info.Result > 0 && cell != null) GetDataFromCell(cell, out  sd.ExcelResult);
                
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
                    if (DataUtils.CheckContainStr(cellstr, "计算结果", "计算(MPa)", "应力", "压力(Mpa)")
                        && !DataUtils.CheckContainStr(cellstr, "应力变化"))//对比用
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
        protected void LoadDataRGDK(string path,ISheet sheet,ref List<PointSurveyData> datas, ref List<ErrorMsg> errors)
        {
            string surveyname1, surveyname2;
            var temp=sheet.SheetName.Split('~');
            if (sheet.SheetName == "RGD-A-1" || sheet.SheetName == "RGD-A-3")
            {
                surveyname1 = sheet.SheetName;
                surveyname2 = (sheet.SheetName == "RGD-A-1") ? "RGDK-A-2" : "RGDK-A-4";
            }
            else
            {
                surveyname1 = temp[0];
                surveyname2 = "RGDK-" + temp[1];
            }
            
            if(!CheckName(surveyname1))
            {
                AddErroSheetname(path, sheet.SheetName);
                return;
            }
            DataInfo info = GetInfo(sheet, path);
            info.RorTIndex = 1;
            info.ZoRIndex = 2;
            PointSurveyData pd1 = LoadOnepoint(path, surveyname1, info, sheet, ref errors);
            info.RorTIndex = 5;
            info.ZoRIndex = 6;
            PointSurveyData pd2 = LoadOnepoint(path, surveyname2, info, sheet, ref errors);
            datas.Add(pd1);
            datas.Add(pd2);
        }
        protected void LoadDataR2ZD(string path, ISheet sheet, ref List<PointSurveyData> datas, ref List<ErrorMsg> errors)
        {
            var temp = sheet.SheetName.Split('~');
            string surveyname1 = temp[0];
            string surveyname2 = "R2ZD-Z" + temp[1];
            if (!CheckName(surveyname1))
            {
                AddErroSheetname(path, sheet.SheetName);
                return;
            }

            DataInfo info = GetInfo(sheet, path);
            info.RorTIndex = 3;
            info.ZoRIndex = 1;
            PointSurveyData pd1 = LoadOnepoint(path, surveyname1, info, sheet, ref errors);
            info.RorTIndex =7;
            info.ZoRIndex = 5;
            PointSurveyData pd2 = LoadOnepoint(path, surveyname2, info, sheet, ref errors);
            datas.Add(pd1);
            datas.Add(pd2);
        }
        protected void LoadDataRlpz(string path, ISheet sheet, ref List<PointSurveyData> datas, ref List<ErrorMsg> errors)
        {
            var temp = sheet.SheetName.Split('～');
            List<string> points = new List<string>();
            string firstname = temp[0];
            int start = int.Parse(firstname.Split('z').Last());
            int end = int.Parse(temp[1]);
            string head = "Rlpz";
            for (int k = start; k < end + 1; k++)
            {
                points.Add(head + k.ToString());
            }
            if (!CheckName(points[0]))
            {
                AddErroSheetname(path, sheet.SheetName);
                return;
            }
            int icount = end - start + 1;
            DataInfo info = GetInfo(sheet, path);
            for (int i = 0; i < icount; i++)
            {
                info.RorTIndex = 2 + i * 2;
                info.ZoRIndex = 1 + i * 2;
                PointSurveyData pd = LoadOnepoint(path, points[i], info, sheet, ref errors);
                datas.Add(pd);
            }
        }
        protected PointSurveyData LoadOnepoint(string path, string point, DataInfo dinfo, ISheet psheet, ref List<ErrorMsg> errors)
        {
            PointSurveyData pd = new PointSurveyData(this.InsType);
            pd.SurveyPoint = point;
            pd.ExcelPath = path;
            DateTime maxDatetime = new DateTime();
            bool flag = GetMaxDate(pd.SurveyPoint, out maxDatetime);
            double ZStandard = 0;
            if (!flag) ZStandard = GetZorRStandard(pd.SurveyPoint);
            bool FirstFlag = (ZStandard == 0);//是否找到基准行
            SurveyData lastsd = null;
            int count = psheet.PhysicalNumberOfRows;
            //for (int j = count - 1; j > 0; j--)//从后往前读，没法滤掉有问题的数据
            for (int j = 1; j < count + 1; j++)//从前往后读
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
                    if (!ReadRow(row,sd, dinfo, out  errmsg))//读当前行
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
                    if (CheckData(sd, lastsd, dinfo, pd.Datas, pd.SurveyPoint, out msg))
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
            return pd;

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
                    if (DataUtils.CheckContainStr(cellstr, "应力","计算结果", "应变"))//对比用
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
                   if (DataUtils.CheckContainStr(cellstr, "计算结果", "应力")&&
                       !DataUtils.CheckContainStr(cellstr, "应力变化"))//对比用
                   {
                       info.Result = pyhindex;
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
            LoadDataExpand(path, null, out datas, out errors);
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
            PointCach.Add(path,new List<string>());
#endif
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                string point = psheet.SheetName;
                if (point == "K1YD-Ⅰ")
                {
                    point = "K1YD-I";
                }
                if (!CheckName(point))
                {
                    if(point.StartsWith("K")||point.StartsWith("k")) AddErroSheetname(path, point);
                    continue;
                }

                //测试代码
#if TEST
                PointCach[path].Add(psheet.SheetName);
#endif
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = point;
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

                double ZStandard = 0;
                if (!flag) ZStandard = GetZorRStandard(pd.SurveyPoint);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行

                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = 1; j < count + 1; j++)//从前往后读
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        DateTime dt;
                        //获取时间，不是时间进入下一次循环
                        if (!GetDateTime(row, dinfo, out dt)) continue;
                        //数据库中有数据，对比上次最大时间，比上次时间小，进入下一次循环
                        if (flag && dt.Date.CompareTo(maxDatetime) <= 0) continue;
                        SurveyData sd = new SurveyData();
                        string errmsg = null;
                        if (!ReadRowExpand(row, dinfo, sd, out  errmsg))//读当前行
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
                            if (!FirstFlag && Math.Abs(sd.Survey_ZorR - ZStandard) <= 0.01
                                ||(Config.IsMoshu && Math.Abs(sd.Survey_ZorR - Math.Sqrt(ZStandard * 1000)) < 1))
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
        bool ReadRowExpand(IRow row, DataInfo info, SurveyData sd, out string err, bool last = false)
        {
            err = null;
            ICell cell;
            try
            {
                if (row == null) return false;
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
                }

                cell = row.GetCell(info.RorTIndex);
                if (!CheckCell(cell))
                {
                    GetDataFromCell(cell, out sd.Survey_RorT);//温度
                }
                if (info.Result > 0)
                {
                    cell = row.GetCell(info.Result);
                    if (!CheckCell(cell)) GetDataFromCell(cell, out sd.ExcelResult);
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

    }


    /// <summary>
    /// 多点锚杆应力计
    /// </summary>
    public class Fiducial_Multi_Anchor_PoleProcessMW : ProcessData
    {
        public Fiducial_Multi_Anchor_PoleProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Multi_Anchor_Pole;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "多点锚杆应力计";
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
            int count = psheet.LastRowNum > 10 ? 10 : psheet.LastRowNum;
            for (int j = 0; j < count; j++)//读取前10行
            {
                IRow row = psheet.GetRow(j);
                if (row == null) continue;
                for (int c = 0; c < row.Cells.Count; c++)
                {
                    var cellstr = row.Cells[c].ToString();
                    int pyhindex = row.Cells[c].ColumnIndex;
                    if (DataUtils.CheckContainStr(cellstr, "观测日期", "日期", "年-月-日"))
                    {
                        flag = false;
                        info.DateIndex = pyhindex;
                    }
                    if (DataUtils.CheckContainStr(cellstr, "观测时间", "时间", "时")) info.TimeIndex = pyhindex;
                    if (DataUtils.CheckContainStr(cellstr, "实测频率"))
                    {
                        info.step = 4;
                        info.ZoRIndex = 1;
                        info.RorTIndex = 3;
                        info.Result = 4;
                        info.RemarkIndex = 9;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "电阻比Z"))
                    {
                        info.step = 4;
                        info.ZoRIndex = 2;
                        info.RorTIndex =1;
                        info.Result = 4;
                        info.RemarkIndex = 9;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "频率(Hz)"))
                    {
                        info.step = 2;
                        info.ZoRIndex =1;
                        info.RorTIndex = 2;
                        info.Result = 6;
                        info.RemarkIndex = 5;
                    }   
                }
            }
            if (flag)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }

            return info;
        }
        //有的多点是单个sheet
        protected  DataInfo GetInfoEx(ISheet psheet, string filePath = null)
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
                    if (DataUtils.CheckContainStr(cellstr, "观测时间", "时间", "时")) info.TimeIndex = pyhindex;

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
                    if (DataUtils.CheckContainStr(cellstr, "计算结果", "应力") &&
                        !DataUtils.CheckContainStr(cellstr, "应力变化"))//对比用
                    {
                        info.Result = pyhindex;
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
       
        protected override double GetZorRStandard(string point)
        {
            string sql = "select  Benchmark_Resist_Ratio from {0}  where Instrument_Serial='{1}'";
            CSqlServerHelper sqlhelper = CSqlServerHelper.GetInstance();
            var res = sqlhelper.SelectFirst(String.Format(sql, Config.InsCollection[Instrument_Name].Fiducial_Table, point));
            if (res != DBNull.Value)
            {
                return Convert.ToDouble(res);
            }
            else
            {
                return 0;
            }
        }
        private bool checkName(string serialName, out string PointName)
        {
            PointName = null;
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql = "select Survey_point_Number from Fiducial_Multi_Anchor_Pole where Instrument_Serial='{0}'";
            var result = sqlhelper.SelectFirst(string.Format(sql,serialName));
            if (result!=null&&result != DBNull.Value)
            {
                PointName = result.ToString();
                return true;
            }

            return false;
        }
        void LoadDataExpand(string path, DataInfo info, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            IFormatProvider culture = CultureInfo.CurrentCulture;
            var workbook = WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            Dictionary<string, List<SurveyData>> ExpandDatas = new Dictionary<string, List<SurveyData>>();
            List<string> pointName = new List<string>();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                string number = psheet.SheetName;
#region 校验sheet名是否是多点锚杆的测点
                if (number.StartsWith("r") || number.StartsWith("R")) continue;//R开头是钢筋计
                if (!CheckName(number))
                {
                    string point = "";
                    if (DataUtils.CheckContainStr(number, "~"))
                    {
                        point = number.Split('~')[0];//K2ZD2-1-1~(2)
                        if (point.Contains('('))
                        {
                            //提取点名K2YD-A4-1-(1)~(2) 用~分割后的前半部分的点名
                            point = Regex.Match(number, @".*(?=\-\()").Groups[0].ToString();
                            number = point;
                        }
                        if (!CheckName(number))
                        {
                            continue;
                        }
                    }
                    else if (checkName(number, out point))//多点锚杆分开记录
                    {
                        //number = point;//一个sheet一个锚杆，是多点锚杆中的一个特殊处理
                        if(!pointName.Contains(point)) pointName.Add(point);
                        List<SurveyData> serialDatas=new List<SurveyData>();
                        LoadDataSingle(path, psheet, point, out serialDatas, ref errors);
                        ExpandDatas.Add(number.ToUpper().Trim(), serialDatas);
                        continue;
                    }
                    else
                    {
                        if (psheet.SheetName.StartsWith("K") || psheet.SheetName.StartsWith("k")) AddErroSheetname(path, psheet.SheetName);
                        continue;
                    }
                }
#endregion
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = number;
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

                string sql = String.Format("select Instrument_Serial from  Fiducial_Multi_Anchor_Pole where survey_Point_number=@Survey_point_Number order by Instrument_Serial", Config.InsCollection[this.Instrument_Name].Fiducial_Table);
                var dt = sqlhelper.SelectData(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                dinfo.Sum = dt.Rows.Count;
                List<string> seriallist = new List<string>();
                for (int n = 0; n < dt.Rows.Count; n++) { seriallist.Add(dt.Rows[n]["Instrument_Serial"].ToString().ToUpper().Trim()); }

                double ZStandard = 0;
                if (!flag) ZStandard = GetZorRStandard(seriallist[0]);//以第一个锚杆的基准为准
                bool FirstFlag = (ZStandard == 0);//是否找到基准行
                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = 1; j < count + 1; j++)//从前往后读
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        DateTime date;
                        //获取时间，不是时间进入下一次循环
                        if (!GetDateTime(row, dinfo, out date)) continue;
                        //数据库中有数据，对比上次最大时间，比上次时间小，进入下一次循环
                        if (flag && date.Date.CompareTo(maxDatetime) <= 0) continue;
                        SurveyData sd = new SurveyData();
                        string errmsg = null;
                        if (!ReadRowExpand(row, dinfo, sd,seriallist, out  errmsg))//读当前行
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
                            if (!FirstFlag && Math.Abs(sd.MultiDatas[seriallist[0]].Survey_ZorR - ZStandard) <= 0.1)
                            {
                                FirstFlag = true;
                            }
                            if (!FirstFlag) continue;
                        }
                        ErrorMsg msg;
                        if (CheckData(sd, lastsd, dinfo, pd.Datas, pd.SurveyPoint, out msg))
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
            var pts = GetDataFromDic(pointName, ExpandDatas, path);
            datas.AddRange(pts);
            workbook.Close();
        }
        bool ReadRowExpand(IRow row, DataInfo info, SurveyData sd, List<string> seriallist, out string err, bool last = false)
        {
            err = null;
            ICell cell;
            try
            {
                if (row == null) return false;
                cell = row.GetCell(info.DateIndex);
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false;
                bool flag = false;
                for (int i = 0; i < info.Sum; i++)
                {
                    SurveyData temp = new SurveyData();
                    cell = row.GetCell(info.ZoRIndex + i*info.step);
                    if (!CheckCell(cell) && GetDataFromCell(cell, out temp.Survey_ZorR))
                    {
                        flag = true;
                    }
                    else
                    {
                        sd.MultiDatas[seriallist[i].ToUpper()] = temp;//成不成功都添加//吧位子占了
                        continue;
                    }
                    cell = row.GetCell(info.RorTIndex + i * info.step);
                    if(!CheckCell(cell)) GetDataFromCell(cell, out temp.Survey_RorT);
                    int exstep = info.step == 2 ? 1 : info.step;
                    cell = row.GetCell(info.Result + i * exstep);
                    if (!CheckCell(cell)) GetDataFromCell(cell, out temp.ExcelResult);

                    sd.MultiDatas[seriallist[i].ToUpper()] = temp;//成不成功都添加//吧位子占了
                }
                if (!flag) return false;
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
        private List<PointSurveyData> GetDataFromDic(List<string> PointsName, Dictionary<string, List<SurveyData>> Dic, string path)
        {
            List<PointSurveyData> Datas = new List<PointSurveyData>();
            var Sqlhelper = CSqlServerHelper.GetInstance();
            foreach (string SurveyPoint in PointsName)
            {
                string sql = String.Format("select Instrument_Serial from Fiducial_Multi_Anchor_Pole where Survey_point_Number='{0}' order by Instrument_Serial", SurveyPoint);
                var dt = Sqlhelper.SelectData(sql);
                if (dt.Rows.Count < 1) continue;
                List<string> serials = new List<string>();
                int count = 0;
                string Matchser = "";//用来做匹配的serial名
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string tempser = dt.Rows[i][0].ToString().ToUpper();
                    if (!Dic.ContainsKey(tempser)) continue;
                    if (Dic[tempser].Count > count)
                    {
                        count = Dic[tempser].Count;
                        Matchser = tempser;
                    }
                    serials.Add(tempser);

                }
                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = SurveyPoint;
                pd.ExcelPath = path;
                for (int i = 0; i < count; i++)
                {
                    var dtmath = Dic[Matchser][i].SurveyDate;
                    SurveyData oneData = new SurveyData();
                    oneData.SurveyDate = dtmath;
                    foreach (string serial in serials)
                    {
                        //if (serial == Matchser) continue;
                        var sd = Dic[serial].Where(s => s.SurveyDate.Date == dtmath.Date).FirstOrDefault();
                        if (sd == null) sd = new SurveyData();
                        oneData.MultiDatas.Add(serial.ToUpper().Trim(), sd);
                    }
                    pd.Datas.Add(oneData);
                }
                Datas.Add(pd);
            }
            return Datas;
        }

        bool CheckDataExpand(string point, int index,SurveyData sd,SurveyData lastsd,List<SurveyData>datas,out ErrorMsg msg)
        {
            msg = new ErrorMsg();
            if (lastsd == null)
            {
                string sql = "select * from Survey_multi_Anchor_Pole where Survey_point_Number='{0}' and {1}>0  and RecordMethod='人工' order by Observation_Date desc";
                var sqlhelper = CSqlServerHelper.GetInstance();
                string fre = "Frequency" + index.ToString();
                var data = sqlhelper.SelectData(String.Format(sql, point, fre));
                if (data.Rows.Count == 0) return true;
                double f = Convert.ToDouble(data.Rows[0][fre]);
                if (Math.Abs(sd.Survey_ZorR - f) < Config.LimitZ) return true;
                //数据库里边不确定写的是频率还是模数
                if (Config.FreOrMoshu != 1)//默认 0的时候认为写进去的是模数或者频率 
                {
                    if (Math.Abs(sd.Survey_ZorR - Math.Sqrt(f * 1000)) < Config.LimitZ) return true;
                }
                else
                {
                    if (Math.Abs(sd.Survey_ZorR - f * f / 1000) < Config.LimitZ) return true;
                }
            }
            else
            {
                //先跟上一次的数据做比较，不超过限值直接return
                if (Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;
            }
            msg.Exception = "数据超限";
            return false;

        }
        
        void LoadDataSingle(string path, ISheet psheet,string point, out  List<SurveyData> datas, ref List<ErrorMsg> errors)
        {
            datas = new List<SurveyData>();
            string serialNumber = psheet.SheetName;
            if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
            DataInfo info = GetInfoEx(psheet);
            DateTime maxDatetime =new DateTime();
            SurveyData lastsd = null;

            bool flag = GetMaxDate(point, out maxDatetime);
            int index = Convert.ToInt16(serialNumber.Split('-').LastOrDefault());
            double ZStandard = 0;
            if (!flag) ZStandard = GetZorRStandard(serialNumber);
            bool FirstFlag = (ZStandard == 0);//是否找到基准行

            int count = psheet.LastRowNum;
        
            for (int j = 0; j <=count+1; j++)
            {
                #region
                try
                {
                    IRow row = psheet.GetRow(j);
                    if (row == null) continue;
                    DateTime dt;
                    //获取时间，不是时间进入下一次循环
                    if (!GetDateTime(row, info, out dt)) continue;
                    //数据库中有数据，对比上次最大时间，比上次时间小，进入下一次循环
                    if (flag && dt.CompareTo(maxDatetime) <= 0) continue;
                    SurveyData sd = new SurveyData();
                    string errmsg = null;
                    if (!ReadRowSingle(row, info, sd, out  errmsg))//读当前行
                    {
                        if (errmsg != null)
                        {
                            ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg };
                            errors.Add(err);
                        }
                        continue;
                    }
                    if (!flag)
                    {
                        if (!FirstFlag && Math.Abs(sd.Survey_ZorR - ZStandard) <= 0.01)
                        {
                            FirstFlag = true;
                        }
                        if (!FirstFlag) continue;
                    }
                    ErrorMsg msg = new ErrorMsg();
                    if (CheckDataExpand(point,index,sd, lastsd, datas, out msg))
                    {
                        datas.Add(sd);
                        lastsd = sd;
                    }
                    else
                    {
                        msg.ErrorRow = j + 1;
                        msg.PointNumber = psheet.SheetName;
                        errors.Add(msg);
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
        bool ReadRowSingle(IRow row, DataInfo info, SurveyData sd, out string err, bool last = false)
        {
            err = null;
            ICell cell;
            try
            {
                if (row == null) return false;
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
                }

                cell = row.GetCell(info.RorTIndex);
                if (!CheckCell(cell))
                {
                    GetDataFromCell(cell, out sd.Survey_RorT);//温度
                }
                if (info.Result > 0)
                {
                    cell = row.GetCell(info.Result);
                    if (!CheckCell(cell)) GetDataFromCell(cell, out sd.ExcelResult);
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
        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, List<SurveyData> datas, string Survey_point_name, out ErrorMsg err)
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
        /// <summary>
        /// 是否是从浅到深排列
        /// </summary>
        private bool IsShallowToDeep = false;
        public Fiducial_Multi_DisplacementProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Multi_Displacement;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "多点位移计";
            base.LoadMultidisplacementName();

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
                string pointnumber = psheet.SheetName;
                if (!CheckName(pointnumber))
                {
                    if (base.PointNumberCach.ContainsKey(pointnumber.ToUpper().Trim()))
                    {
                        pointnumber = PointNumberCach[pointnumber.ToUpper().Trim()];
                    }
                    else
                    {
                        AddErroSheetname(path, psheet.SheetName);
                        continue;
                    }
                }
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = pointnumber;
                pd.ExcelPath = path;
                IsShallowToDeep = NonNumberDataCach.Contains(pd.SurveyPoint.ToUpper().Trim());
                DataInfo info = GetInfo(psheet);
                DateTime maxDatetime = new DateTime();
                bool flag = GetMaxDate(pd.SurveyPoint, out maxDatetime);


                string sql = String.Format("select Instrument_Serial  from {0} where Survey_point_Number=@Survey_point_Number", Config.InsCollection[this.Instrument_Name].Fiducial_Table);
                var dt = sqlhelper.SelectData(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                info.Sum = dt.Rows.Count;
                List<string> seriallist = GetSerial(dt);
                double ZStandard = 0;
                if (!flag) ZStandard = GetZorRStandard(seriallist[0]);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行

                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = 1; j < count + 1; j++)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        DateTime date;
                        //获取时间，不是时间进入下一次循环
                        if (!GetDateTime(row, info, out date)) continue;
                        //数据库中有数据，对比上次最大时间，比上次时间小，进入下一次循环
                        if (flag && date.CompareTo(maxDatetime) <= 0) continue;

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
                        if (!flag)
                        {
                            if (!FirstFlag && Math.Abs(sd.MultiDatas[seriallist[0]].Survey_ZorR - ZStandard) <= 0.01)
                            {
                                FirstFlag = true;
                            }
                            if (!FirstFlag) continue;
                        }

                        ErrorMsg msg;
                        if (CheckData(sd, lastsd, info, pd.Datas, pd.SurveyPoint, out msg))
                        {
                            pd.Datas.Add(sd);
                            lastsd = sd;
                        }
                        else
                        {
                            msg.ErrorRow = row.RowNum + 1;
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
                if (!flag && !FirstFlag) AddErroSheetname(path, psheet.SheetName+",-1");
                datas.Add(pd);
            }
            workbook.Close();

        }
        protected override double GetZorRStandard(string point)
        {
            string sql = "select  Benchmark_Resist_Ratio from {0}  where Instrument_Serial='{1}'";
            CSqlServerHelper sqlhelper = CSqlServerHelper.GetInstance();
            var res = sqlhelper.SelectFirst(String.Format(sql, Config.InsCollection[Instrument_Name].Fiducial_Table, point));
            if (res != DBNull.Value)
            {
                return Convert.ToDouble(res);
            }
            else
            {
                return 0;
            }
        }
        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            info.RorTIndex = -1;
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
            if (flag && info.TimeIndex >= 0)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }
            //if (TCount > 1)
            //{
            //    if (info.Findex - info.DateIndex > 2)
            //    {
            //        info.Findex = info.DateIndex + 1;
            //    }
            //    else
            //    {
            //        info.Findex = info.TimeIndex > 0 ? info.TimeIndex  : info.DateIndex;
            //    }
            //}
            //else
            //{
            //    info.Findex = info.TimeIndex > 0  ? info.TimeIndex : info.DateIndex;
            //}
            info.Findex = info.TimeIndex > 0 ? info.TimeIndex : info.DateIndex;
            info.FCount = TCount;
            
            return info;

        }

        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, List<SurveyData> datas, string Survey_point_name, out ErrorMsg err)
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
                //list.Add(serialnum);
                bool flag = true;
                for (int j = 0; j < list.Count; j++)
                {
                    if (Compare(serialnum, list[j]))
                    {
                        list.Insert(j, serialnum);
                        flag = false;
                        break;
                    }
                }
                if (flag) list.Add(serialnum);
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
                string str = tempstr[len - 1].Contains('A') ? tempstr[len - 1].Substring(0, tempstr[len-1].Length-1) : tempstr[len - 1];
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
               
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false; 
                bool flag = false;
                sd.MultiDatas.Clear();
                foreach (string serial in seriallist)
                {
                    sd.MultiDatas.Add(serial, null);
                }
                int start = this.IsShallowToDeep ? info.Sum - 1 : 0;//从前往后还是从后往前
                //if (info.FCount <=1)//只有一列温度
                {
                    if (info.RorTIndex>0)
                    {
                        cell = row.GetCell(info.RorTIndex);
                        if(cell!=null) GetDataFromCell(row.GetCell(info.RorTIndex), out sd.Survey_RorT);
                    }
                    int firstindex = info.Findex + 1;
                    for (int i = 0; i < info.Sum;i++)
                    {
                        SurveyData temp = new SurveyData();
                        temp.Survey_RorT = sd.Survey_RorT;
                        cell = row.GetCell(firstindex + i);
                        if (cell != null && GetDataFromCell(row.GetCell(firstindex + i), out temp.Survey_ZorR)) flag = true;
                        sd.MultiDatas[seriallist[this.IsShallowToDeep?info.Sum-1-i:i]] = temp;//成不成功都添加//吧位子占了
                    }
                }
                //else//有多列温度只读一列
                //{
                //    flag = false;
                //    bool tflag = false;
                //    int firstindex = info.Findex + 1;
                //    for (int i = 0; i < info.Sum;i++)
                //    {
                //        if (info.RorTIndex > 0 && !tflag)
                //        {
                //            cell = row.GetCell(firstindex + i * 2+1);
                //            if (cell != null && GetDataFromCell(row.GetCell(info.RorTIndex), out sd.Survey_RorT)) tflag = true;
                //        }
                //        SurveyData temp = new SurveyData();
                //        cell = row.GetCell(firstindex + i * 2);
                //        if (cell != null && GetDataFromCell(cell, out temp.Survey_ZorR)) flag = true;
                //        temp.Survey_RorT = sd.Survey_RorT;
                //        sd.MultiDatas[seriallist[this.IsShallowToDeep ? info.Sum - 1 - i : i]] = temp; 
                //    }
                //}
                if (!flag)
                {
                    sd = null;
                    return false;
                }
                sd.Remark = row.GetCell(info.TimeIndex + 2 * info.Sum + 4) == null ? "" : row.GetCell(info.TimeIndex + 2 * info.Sum + 4).ToString();
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
                if(sheetname.Contains("#机"))
                {
                    sheetname = sheetname.Split('机')[1];

                }
                if (!CheckName(sheetname))
                {
                    if (base.PointNumberCach.ContainsKey(sheetname.ToUpper().Trim()))
                    {
                        sheetname = PointNumberCach[sheetname.ToUpper().Trim()];
                    }
                    if (CheckNameExpand(sheetname))//sheet未单个无应力计数据读完contnue
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
                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = sheetname;
                pd.ExcelPath = path;
                DataInfo info = GetInfo(psheet, path);
                if (info.FCount > 1)
                {
                    ReadNonStressData(psheet, errors);
                }

                DateTime maxDatetime = new DateTime();
                bool flag = GetMaxDate(pd.SurveyPoint, out maxDatetime);

                double ZStandard = 0;
                if (!flag) ZStandard = GetZorRStandard(pd.SurveyPoint);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行

                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = 1; j < count + 1; j++)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        DateTime dt;
                        if (!GetDateTime(row, info, out dt)) continue;
                        if (flag && dt.CompareTo(maxDatetime) <= 0) continue;
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
                        //数据库没有数据，用基准值做对比，从基准值行开始读
                        if (!FirstFlag && !flag && Math.Abs(sd.Survey_ZorR - ZStandard) <= 0.1)
                        {
                            FirstFlag = true;
                        }
                        if ( !flag&&!FirstFlag) continue;
                        ErrorMsg msg;
                        if (CheckData(sd, lastsd, info, pd.Datas, pd.SurveyPoint, out msg))
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
            try
            {
                ICell cell;
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false; 
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
                if (info.Result > 0)
                {
                    cell = row.GetCell(info.Result);
                    GetDataFromCell(cell, out sd.ExcelResult);
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
            if (!CheckNameExpand(number)) return;
            PointSurveyData pd = new PointSurveyData(InstrumentType.Fiducial_Nonstress);
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

                    if (!flag || (flag && sd.SurveyDate.CompareTo(maxDatetime) > 0))
                    {
                        ErrorMsg msg;
                        if (CheckData(sd, lastsd, info, pd.Datas, pd.SurveyPoint, out msg))
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
                    }
                }
                catch (Exception ex)
                {
                    ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = ex.Message + "无应力计" };
                    errors.Add(err);
                    continue;
                }
            }
            ExpandDataCach.Add(pd);

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
                    else if (DataUtils.CheckContainStr(cellstr, "应变"))
                    {
                        info.Result = pyhindex;
                    
                    }
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
                    else if (DataUtils.CheckContainStr(cellstr, "电阻值", "电阻") &&
                        !DataUtils.CheckContainStr(cellstr, "电阻比"))
                    {
                        info.RorTIndex = pyhindex;
                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                    if (DataUtils.CheckContainStr(laststr, "频率", "频模", "模数"))
                    {
                        if (cellstr.Contains("温度")) info.RorTIndex = pyhindex;
                    }
                    if (DataUtils.CheckContainStr(cellstr, "应变", "开合度", "张合量(mm)"))//对比用
                    {
                        info.Result = pyhindex;
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
                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = sheetname;
                pd.ExcelPath = path;
                info = GetInfo(psheet, path);

                DateTime maxDatetime = new DateTime();
                bool flag = GetMaxDate(pd.SurveyPoint, out maxDatetime);
                double ZStandard = 0;
                if (!flag) ZStandard = GetZorRStandard(pd.SurveyPoint);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行

                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = 1; j < count + 1; j++)//从前往后读
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        DateTime dt;
                        if (!GetDateTime(row, info, out dt)) continue;
                        if (flag && dt.CompareTo(maxDatetime) <= 0) continue;
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
                        if (CheckData(sd, lastsd, info, pd.Datas, pd.SurveyPoint, out msg))
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
            try
            {
                var cell = row.GetCell(info.DateIndex);
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false; 
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
                    //sd.Survey_ZorRMoshu = sd.Survey_ZorR;
                }
                if (row.GetCell(info.RorTIndex) != null && !String.IsNullOrEmpty(row.GetCell(info.RorTIndex).ToString()))
                {
                    double.TryParse(row.GetCell(info.RorTIndex).ToString(), out sd.Survey_RorT);//温度
                }
                if (info.RemarkIndex > 0) sd.Remark = row.GetCell(info.RemarkIndex) == null ? "" : row.GetCell(info.RemarkIndex).ToString();
                if (info.Result > 0)
                {
                    cell = row.GetCell(info.Result);
                    if (!CheckCell(cell)) GetDataFromCell(cell, out sd.ExcelResult);
                }

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
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = pointnumber;
                pd.ExcelPath = path;
                DataInfo info = GetInfo(psheet);

                DateTime maxDatetime = new DateTime();
                bool flag = GetMaxDate(pd.SurveyPoint, out maxDatetime);
                string sql = "select Read_GroupNum from Fiducial_Anchor_Cable where Survey_point_Number=@Survey_point_Number";
                var result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                if (result == DBNull.Value) continue;
                info.Sum = Convert.ToInt16(result);

                double ZStandard = 0;
                if (!flag) ZStandard = GetZorRStandard(pd.SurveyPoint);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行

                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = 1; j < count + 1; j++)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        DateTime dt;
                        if (!GetDateTime(row, info, out dt)) continue;
                        if (flag && dt.CompareTo(maxDatetime) <= 0) continue; 
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
                        //if (!flag)
                        //{
                        //    if (!FirstFlag && Math.Abs(sd.Average - ZStandard) <=1)
                        //    {
                        //        FirstFlag = true;
                        //    }
                        //    if (!FirstFlag) continue;
                        //}
                        ErrorMsg msg;
                        if (CheckData(sd, lastsd, info, pd.Datas, pd.SurveyPoint, out msg))
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
        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, List<SurveyData> datas, string Survey_point_name, out ErrorMsg err)
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
                    if (DataUtils.CheckContainStr(cellstr, "观测日期", "日期","观测年月"))
                    {
                        flag = false;
                        info.DateIndex = pyhindex;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "观测时间", "时间") && !DataUtils.CheckContainStr(cellstr,"安装时间","埋设时间")) info.TimeIndex = pyhindex;
                    else if (DataUtils.CheckContainStr(cellstr, "温度")) info.RorTIndex = pyhindex;
                    else if (DataUtils.CheckContainStr(cellstr,"备注", "备   注")) info.RemarkIndex = pyhindex;
                    else if (cellstr.Contains("平均")) info.Average = pyhindex;
                    if (DataUtils.CheckContainStr(cellstr, "荷载") && !cellstr.Contains("荷载增长率")) info.Result = pyhindex;
                    
                    laststr = cellstr;
                }
            }
            if (flag && info.TimeIndex > 0)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }

            info.Findex = info.TimeIndex > 0 ? info.TimeIndex : info.DateIndex;
            return info;
        }
        private bool ReadRow(IRow row, DataInfo info, SurveyData sd, out string err, bool last = false)
        {
            err = null;
            try
            {
                var cell = row.GetCell(info.DateIndex);
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false; 
                cell=row.GetCell(info.RorTIndex);
                if (cell != null && !String.IsNullOrEmpty(cell.ToString()))
                {
                    GetDataFromCell(cell, out sd.Survey_RorT);
                }

                bool flag = false;

                for (int i = 0; i < info.Sum; i++)
                {
                    SurveyData temp = new SurveyData();
                    cell = row.GetCell(info.Findex + 1 + i);
                    if (cell != null && !String.IsNullOrEmpty(cell.ToString().Trim()))
                    {
                        //频率/基准电阻
                        if (GetDataFromCell(cell, out temp.Survey_ZorR))
                        {
                            flag = true;
                        }
                    }
                    sd.MultiDatas.Add(i.ToString(), temp);
                }
                if (!flag)
                {
                    sd = null;
                    return false;
                }
                cell = row.GetCell(info.Average);
                if (!CheckCell(cell)) GetDataFromCell(cell, out sd.Average);
                
                if (info.Result > 0)
                {
                    cell = row.GetCell(info.Result);
                    if (!CheckCell(cell)) GetDataFromCell(cell, out sd.ExcelResult);
                }
                if (info.RemarkIndex > 0)
                {
                    sd.Remark = sd.Remark = row.GetCell(info.RemarkIndex) == null ? "" : row.GetCell(info.RemarkIndex).ToString();
                }
                else
                {
                    sd.Remark = row.GetCell(info.TimeIndex + 2 * info.Sum + 4) == null ? "" : row.GetCell(info.TimeIndex + 2 * info.Sum + 4).ToString();
                }
                
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
        public override dynamic CheckStandard(string path)
        {
            IFormatProvider culture = CultureInfo.CurrentCulture;
            var workbook = WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            List<dynamic> Results = new List<dynamic>();
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
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                DataInfo dinfo = GetInfo(psheet);
                string sql = "select Read_GroupNum from Fiducial_Anchor_Cable where Survey_point_Number=@Survey_point_Number";
                var result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", pointnumber));
                if (result == DBNull.Value) continue;
                dinfo.Sum = Convert.ToInt16(result);
                double ZStandard = 0;
                ZStandard = GetZorRStandard(pointnumber);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行
                int count = psheet.PhysicalNumberOfRows;
                //for (int j = count - 1; j > 0; j--)//从后往前读，没法滤掉有问题的数据
                for (int j = 1; j < count + 1; j++)//从前往后读
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        DateTime dt;
                        //获取时间，不是时间进入下一次循环
                        if (!GetDateTime(row, dinfo, out dt)) continue;
                        SurveyData sd = new SurveyData();
                        string errmsg = null;
                        if (!ReadRow(row, dinfo, sd, out  errmsg))//读当前行
                        {
                            continue;
                        }
                        double a = 0;
                        double cn = 0;
                        foreach (var s in sd.MultiDatas)
                        {
                            if (s.Value.Survey_ZorR != 0) cn++;
                            a += s.Value.Survey_ZorR;
                        }
                        a = a / cn;
                        if (Math.Abs(sd.Average - a) >1) sd.Average = a;
                        if (!FirstFlag && Math.Abs(sd.Average - ZStandard) <= 1)
                        {
                            FirstFlag = true;
                            Results.Add(new { name = pointnumber, spath = path, index = j + 1 });
                            break;
                        }

                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                if (!FirstFlag) Results.Add(new { name = pointnumber, spath = path, index = -1 });
            }
            workbook.Close();
            return Results;
        }
        
    }
    /// <summary> 应变计组
    /// </summary>
    public class Fiducial_Strain_GroupProcessMW : ProcessData
    {
        public Fiducial_Strain_GroupProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Strain_Group;
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
                string serialNumber = point.ToUpper().Trim();
                if(int.TryParse(p,out number))//括号是数字的直接拼接
                {
                    serialNumber = serialNumber + "-" + p;
                }
                else//括号是文字的，右岸为1
                {
                    serialNumber = serialNumber + "-" + ((p == "右岸") ? 1 : 2);
                }

                DataInfo info = base.GetInfo(psheet);
                DateTime maxDatetime =new DateTime();
                bool flag = GetMaxDate(point, out maxDatetime);

                double ZStandard = 0;
                if (!flag) ZStandard = GetZorRStandard(serialNumber);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行

                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                int index = Convert.ToInt16(serialNumber.Split('-').LastOrDefault());

                for (int j = 0; j <=count+1; j++)
                {
                    #region
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        DateTime dt;
                        //获取时间，不是时间进入下一次循环
                        if (!GetDateTime(row, info, out dt)) continue;
                        //数据库中有数据，对比上次最大时间，比上次时间小，进入下一次循环
                        if (flag && dt.CompareTo(maxDatetime) <= 0) continue;
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
                        if (!flag)
                        {
                            if (!FirstFlag && Math.Abs(sd.Survey_ZorR - ZStandard) <= 0.01)
                            {
                                FirstFlag = true;
                            }
                            if (!FirstFlag) continue;
                        }
                       
                        if (!SerialData.ContainsKey(serialNumber))
                        {
                            SerialData.Add(serialNumber, new List<SurveyData>() { sd });
                            continue;
                        }
                        ErrorMsg msg = new ErrorMsg();
                        if (checkDataExpand(sd, index, lastsd, info, SerialData[serialNumber], point, out msg))
                        {
                            if (SerialData.ContainsKey(serialNumber))
                            {
                                SerialData[serialNumber].Add(sd);
                            }
                            else
                            {
                                SerialData.Add(serialNumber,new List<SurveyData>() { sd });
                            }
                            lastsd = sd;
                        }
                        else
                        {
                            msg.ErrorRow = j + 1;
                            msg.PointNumber = psheet.SheetName;
                            errors.Add(msg);
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
            datas = GetDataFromDic(Points, SerialData,path);
 
        }
        private List<PointSurveyData> GetDataFromDic(List<string> PointsName, Dictionary<string, List<SurveyData>> Dic,string path)
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
                string Matchser = "";//用来做匹配的serial名
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string tempser = dt.Rows[i][0].ToString().ToUpper().Trim();
                    if (!Dic.ContainsKey(tempser)) continue;
                    if (Dic[tempser].Count > count)
                    {
                        count = Dic[tempser].Count;
                        Matchser = tempser;
                    }                    
                    serials.Add(tempser);

                }
                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = SurveyPoint;
                pd.ExcelPath = path;
                for (int i = 0; i < count; i++)
                {
                    var dtmath = Dic[Matchser][i].SurveyDate;
                    SurveyData oneData = new SurveyData();
                    oneData.SurveyDate = dtmath;
                    foreach (string serial in serials)
                    {
                        //if (serial == Matchser) continue;
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
        private bool checkDataExpand(SurveyData sd, int index,SurveyData lastsd, DataInfo info, List<SurveyData> datas, string Survey_point_name, out ErrorMsg err)
        {
            err = new ErrorMsg();
            if (sd.Remark.Contains("已复测")) return true;
            if (lastsd == null) return true;//上一行数据为空不处理
            //先跟上一次的数据做比较，不超过限值直接return
            if (Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;

            //超过限值跟上一个月同一天作比较
            var sqlhelper = CSqlServerHelper.GetInstance();
            DateTime dt = sd.SurveyDate.Date.AddMonths(-1);
            string sql = "select * from Survey_Strain_Group where Survey_point_Number='{1}' and Observation_Date>='{2}' and Observation_Date<'{3}' and RecordMethod='人工'";
            var table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.AddDays(-3).ToString(), dt.AddDays(3).ToString()));
            double szr = 0;
            SurveyData lastMorYData = null;
            if (table.Rows.Count > 0)
            {
                szr = Convert.ToDouble(table.Rows[0]["Frequency"+index.ToString()]);
            }
            else
            {
                lastMorYData = GetData(dt.AddDays(-3), dt.AddDays(3), datas);
                if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
            }
            if (szr != 0)
            {
                if (Math.Abs(sd.Survey_ZorR - szr) < Config.LimitZ) return true;
                if (Config.FreOrMoshu != 1)//默认 0的时候认为写进去的是模数
                {
                    if (Math.Abs(sd.Survey_ZorR - Math.Sqrt(szr * 1000)) < Config.LimitZ) return true;
                }
                else
                {
                    if (Math.Abs(sd.Survey_ZorR - szr * szr / 1000) < Config.LimitZ) return true;
                }
            }
            //跟上一年的同一天做对比
            dt = sd.SurveyDate.Date.AddYears(-1);
            table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.AddDays(-3).ToString(), dt.AddDays(3)));
            if (table.Rows. Count > 0)
            {
                szr = Convert.ToDouble(table.Rows[0]["Frequency"+index.ToString()]);
            }
            else
            {
                lastMorYData = GetData(dt.AddDays(-3), dt.AddDays(3), datas);
                if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
            }
            if (szr != 0)
            {
                if (Math.Abs(sd.Survey_ZorR - szr) < Config.LimitZ) return true;
                if (Config.FreOrMoshu != 1)//默认 0的时候认为写进去的是模数
                {
                    if (Math.Abs(sd.Survey_ZorR - Math.Sqrt(szr * 1000)) < Config.LimitZ) return true;
                }
                else
                {
                    if (Math.Abs(sd.Survey_ZorR - szr * szr / 1000) < Config.LimitZ) return true;
                }
            }
            err.Exception = "数据误差超限";
            return false;
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
                    if (CheckNameExpand(psheet.SheetName))
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
                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = psheet.SheetName;
                pd.ExcelPath = path;
                DataInfo info = GetInfo(psheet);
                DateTime maxDatetime = new DateTime();
                bool flag = GetMaxDate(pd.SurveyPoint, out maxDatetime);
                
                string sql = String.Format("select Instrument_Serial  from {0} where Survey_point_Number=@Survey_point_Number order by Instrument_Serial", Config.InsCollection[this.Instrument_Name].Fiducial_Table);
                var dt = sqlhelper.SelectData(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                info.Sum = dt.Rows.Count;
                List<string> seriallist = new List<string>();
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    seriallist.Add(dt.Rows[k][0].ToString().ToUpper().Trim());
                }
                if (info.FCount > seriallist.Count)
                {
                    ReadNonStressData(psheet, info, errors);//读取无应力计的数据
                    info.Findex += 2;//包含无应力计的数据加一组索引
                }
                double ZStandard = 0;
                if (!flag) ZStandard = GetZorRStandard(seriallist[0]);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行

                SurveyData lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = 1; j < count + 1; j++)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        var cell = row.GetCell(info.DateIndex);
                        DateTime date;
                        if (!GetDateTime(row, info, out date)) continue;
                        if (flag && date.CompareTo(maxDatetime) < 0) continue;
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
                        if (!flag)
                        {
                            if (!FirstFlag && Math.Abs(sd.MultiDatas[seriallist[0]].Survey_ZorR - ZStandard) <= 0.01)
                            {
                                FirstFlag = true;
                            }
                            if (!FirstFlag) continue;
                        }
                        ErrorMsg msg;
                        if (CheckData(sd, lastsd, info, pd.Datas, pd.SurveyPoint, out msg))
                        {
                            pd.Datas.Add(sd);
                            lastsd = sd;
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
        protected override double GetZorRStandard(string point)
        {
            string sql = "select  Benchmark_Resist_Ratio from {0}  where Instrument_Serial='{1}'";
            CSqlServerHelper sqlhelper = CSqlServerHelper.GetInstance();
            var res = sqlhelper.SelectFirst(String.Format(sql, Config.InsCollection[Instrument_Name].Fiducial_Table, point));
            if (res != DBNull.Value)
            {
                return Convert.ToDouble(res);
            }
            else
            {
                return 0;
            }
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
            int count = psheet.LastRowNum > 15 ? 15 : psheet.LastRowNum;
            for (int j = 0; j < count; j++)//读取前10行
            {
                IRow row = psheet.GetRow(j);
                if (row == null) continue;
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
                    else if (DataUtils.CheckContainStr(cellstr, "频率","电阻比"))
                    {
                        info.Findex = info.Findex < 0 ? pyhindex : info.Findex;
                        TCount++;

                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
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
       
        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, List<SurveyData>datas, string Survey_point_name,out ErrorMsg err)
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
            if (!CheckNameExpand(number)) return;
            PointSurveyData pd = new PointSurveyData(InstrumentType.Fiducial_Nonstress);
            pd.SurveyPoint = number;
            DateTime maxDatetime = new DateTime();
            string sql = "select max(Observation_Date) from Survey_Nonstress where Survey_point_Number=@Survey_point_Number";
            var result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
            bool flag = (result != DBNull.Value);
            if (flag) maxDatetime = (DateTime)result;

            SurveyData lastsd = null;
            int count = psheet.LastRowNum;
            for (int j = 1; j < count + 1; j++)
            {
                try
                {
                    IRow row = psheet.GetRow(j);
                    if (row == null) continue;
                    DateTime dt;
                    if (!GetDateTime(row, info, out dt)) continue;
                    if (flag && dt.CompareTo(maxDatetime) < 0) continue; 
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
                    if (flag)
                    {
                        if (sd.SurveyDate.CompareTo(maxDatetime) > 0)
                        {
                            ErrorMsg msg;
                            if (base.CheckData(sd, lastsd, info, pd.Datas, pd.SurveyPoint, out msg))
                            {
                                pd.Datas.Add(sd);
                                lastsd = sd;
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
                    lastsd = sd;
                }
                catch (Exception ex)
                {
                    ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = ex.Message + "无应力计" };
                    errors.Add(err);
                    continue;
                }
            }
            ExpandDataCach.Add(pd);

        }

        private bool ReadRow(IRow row, DataInfo info, SurveyData sd, out string err, bool last = false)
        {
            err = null;
            try
            {
                var cell = row.GetCell(info.DateIndex);
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false; 
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
                    //sd.Survey_ZorRMoshu = sd.Survey_ZorR;
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
                    PointSurveyData pd = new PointSurveyData(this.InsType);
                    pd.SurveyPoint = name;
                    pd.ExcelPath = path;
                    PointList.Add(pd);
                }
                DataInfo dinfo =new DataInfo(){DateIndex=0,TimeIndex=-1,Findex=1,FCount=icount,TableName=Config.InsCollection[this.Instrument_Name].Measure_Table};
                DateTime maxDatetime = new DateTime();
                string sql = String.Format("select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number", Config.InsCollection[this.Instrument_Name].Measure_Table);
                var result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", points[0]));
                bool flag = (result != DBNull.Value);
                if (flag) maxDatetime = (DateTime)result;
                if (Config.IsCovery)
                {
                    maxDatetime = Config.StartTime;
                    flag = !Config.IsCoveryAll;
                }

                List<Survey_Slant_FixedSurveyData> lastsd = null;
                int count = psheet.LastRowNum;
                for (int j = 1; j < count + 1; j++)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        DateTime dt;
                        if (!GetDateTime(row, dinfo, out dt)) continue;
                        if (flag && dt.CompareTo(maxDatetime) <= 0) continue;
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
                        for (int k = 0; k < sds.Count; k++)
                        {
                            ErrorMsg msg;
                            SurveyData lsd = (lastsd == null) ? null : lastsd[k];
                            if (CheckData(sds[k], lsd, dinfo, PointList[k].Datas, PointList[k].SurveyPoint, out msg))
                            {
                                PointList[k].Datas.Add(sds[k]);
                            }
                            else
                            {
                                msg.ErrorRow = j + 1;
                                msg.PointNumber = PointList[k].SurveyPoint;
                                errors.Add(msg);
                            }
                        }
                        lastsd = sds;
                        for (int k = 0; k< PointList.Count; k++)
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

        bool ReadRowExpand(IRow row, DataInfo info, List<Survey_Slant_FixedSurveyData> sds, out string err, bool last = false)
        {
            err = null;
            ICell cell;
            try
            {
                DateTime SurveyDate;
                if (!GetDateTime(row, info, out SurveyDate)) return false; 
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
                        if (GetDataFromCell(cell, out sd.Survey_ZorR))//频率/基准电阻
                        {
                            icount++;
                        }
                    }
                    cell = cell = row.GetCell(info.Findex + i * 2+1);
                    if (!CheckCell(cell)) GetDataFromCell(cell, out sd.ExcelResult);
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

        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, List<SurveyData> Datas, string Survey_point_name, out ErrorMsg err)
        {
            err = new ErrorMsg();
            if (sd.Remark.Contains("已复测")) return true;
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql;
            if (lastsd == null)
            {
                sql = Config.IsAuto ? "select top 1 * from {0} where Survey_point_Number='{1}' and Observation_Date<'{2}' and RecordMethod='人工' Order by Observation_Date desc" :
               "select  top 1 * from {0} where Survey_point_Number='{1}' and Observation_Date<'{2}'Order by Observation_Date desc ";
                var data = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, sd.SurveyDate.Date.ToString()));
                if (data.Rows.Count == 0) return true;
                double f = Convert.ToDouble(data.Rows[0]["Reading_A"]);
                //数据库里边不确定写的是频率还是模数
                if (Math.Abs(sd.Survey_ZorR - f) < Config.LimitZ ||
                   Math.Abs(sd.Survey_ZorR - Math.Sqrt(f * 1000)) < Config.LimitZ) return true;
            }
            else
            {
                //先跟上一次的数据做比较，不超过限值直接return
                if (Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;
            }

            //超过限值跟上一个月同一天作比较
            DateTime dt = sd.SurveyDate.Date.AddMonths(-1);
            sql = Config.IsAuto ? "select * from {0} where Survey_point_Number='{1}' and abs(datediff(d,Observation_Date,'{2}'))<3 and RecordMethod='人工' Order by abs(datediff(dd,Observation_Date,'{3}'))" :
                "select * from {0} where Survey_point_Number='{1}' and abs(datediff(d,Observation_Date,'{2}'))<3 Order by abs(datediff(dd,Observation_Date,'{3}')) ";
            var table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.ToString(), dt.ToString()));
            double szr = 0;
            SurveyData lastMorYData = null;
            if (table.Rows.Count > 0)
            {
                szr = Convert.ToDouble(table.Rows[0]["Reading_A"]);
            }
            else
            {
                lastMorYData = GetData(dt.AddDays(-3), dt.AddDays(3), Datas);
                if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
            }
            if (szr != 0 && Math.Abs(sd.Survey_ZorR - szr) < Config.LimitZ) return true;
            //跟上一年的同一天做对比
            dt = sd.SurveyDate.Date.AddYears(-1);
            table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.ToString(), dt.ToString()));
            if (table.Rows.Count > 0)
            {
                szr = Convert.ToDouble(table.Rows[0]["Reading_A"]);
            }
            else
            {
                lastMorYData = GetData(dt.AddDays(-3), dt.AddDays(3), Datas);
                if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
            }
            if (szr != 0 && Math.Abs(sd.Survey_ZorR - szr) < Config.LimitZ) return true;
            err.Exception = "数据误差超限";

            return false;
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

            base.InsType = InstrumentType.Fiducial_Soil_Stres;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "土压力计";
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
                    if (row.Cells[c].ColumnIndex > 8) break;
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
                    if (DataUtils.CheckContainStr(cellstr, "计算结果", "开合度", "压力", "应变"))//对比用
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

            base.InsType = InstrumentType.Fiducial_Temperature  ;
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
                    if (DataUtils.CheckContainStr(cellstr, "温度")) info.Result = pyhindex;
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
                else if (psheet.SheetName.Contains('、'))
                {
                    var temp = psheet.SheetName.Split('、');
                    firstname = temp[0];
                    if (!firstname.Contains('-')) return;
                    int start = int.Parse(firstname.Split('-').Last());
                    int end = int.Parse(temp[1]);
                    string head = firstname.Split('-')[0];
                    points.Add(firstname);
                    points.Add(head + "-" + end);
                    icount = 2;
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
                    PointSurveyData pd = new PointSurveyData(this.InsType);
                    pd.SurveyPoint = points[0];
                    pd.ExcelPath = path;
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
                for (int j = 1; j < count + 1; j++)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        DateTime dt;
                        //获取时间，不是时间进入下一次循环
                        if (!GetDateTime(row, dinfo, out dt)) continue;
                        //数据库中有数据，对比上次最大时间，比上次时间小，进入下一次循环
                        if (flag && dt.Date.CompareTo(maxDatetime) <= 0) continue;
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
                        for (int k = 0; k < sds.Count; k++)
                        {
                            ErrorMsg msg;
                            SurveyData ls = (lastsd != null && lastsd.Count > k) ? lastsd[k] : null;
                            if (CheckData(sds[k], ls, dinfo, PointList[k].Datas, PointList[k].SurveyPoint, out msg))
                            {
                                PointList[k].Datas.Add(sds[k]);
                            }
                            else
                            {
                                msg.ErrorRow = j + 1;
                                msg.PointNumber = PointList[k].SurveyPoint;
                                errors.Add(msg);
                            }
                        }
                        lastsd = sds;
                        //for (int k = 0; k < PointList.Count; k++)
                        //{
                        //    PointList[k].Datas.Add(sds[k]);
                        //}
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
                        if (!CheckCell(cell)) GetDataFromCell(cell, out sd.Survey_ZorR);//电阻
                        if(info.Result>0)
                        cell = row.GetCell(info.Result);
                        if (!CheckCell(cell)) GetDataFromCell(cell, out sd.ExcelResult);//表格结果

                    }
                    else
                    {
                        cell = row.GetCell(info.Findex + i);
                        if (!CheckCell(cell))
                        {
                            GetDataFromCell(cell, out sd.Survey_RorT);//温度
                            sd.Tempreture = sd.Survey_RorT;
                            sd.ExcelResult = sd.Survey_RorT;
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
       /// <summary>检查数据正确性
       /// </summary>
       /// <param name="sd"></param>
       /// <param name="lastsd"></param>
       /// <param name="tablename"></param>
       /// <param name="err"></param>
       /// <returns></returns>
       protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, List<SurveyData> Datas, string Survey_point_name, out ErrorMsg err)
       {
           err = new ErrorMsg();
           if (sd.Remark.Contains("已复测")) return true;
           var sqlhelper = CSqlServerHelper.GetInstance();
           string sql;
           if (lastsd == null)
           {
               sql = Config.IsAuto ? "select top 1 * from {0} where Survey_point_Number='{1}' and Observation_Date<'{2}' and RecordMethod='人工' Order by Observation_Date desc" :
              "select  top 1 * from {0} where Survey_point_Number='{1}' and Observation_Date<'{2}'Order by Observation_Date desc ";
               var data = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, sd.SurveyDate.Date.ToString()));
               if (data.Rows.Count == 0) return true;
               double f = Convert.ToDouble(data.Rows[0]["Reading"]);
               //数据库里边不确定写的是频率还是模数
               if (Math.Abs(sd.Survey_ZorR - f) < Config.LimitZ ||
                  Math.Abs(sd.Survey_ZorR - Math.Sqrt(f * 1000)) < Config.LimitZ) return true;
           }
           else
           {
               //先跟上一次的数据做比较，不超过限值直接return
               if (Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;
           }

           //超过限值跟上一个月同一天作比较
           DateTime dt = sd.SurveyDate.Date.AddMonths(-1);
           sql = Config.IsAuto ? "select * from {0} where Survey_point_Number='{1}' and abs(datediff(d,Observation_Date,'{2}'))<3 and RecordMethod='人工' Order by abs(datediff(dd,Observation_Date,'{3}'))" :
               "select * from {0} where Survey_point_Number='{1}' and abs(datediff(d,Observation_Date,'{2}'))<3 Order by abs(datediff(dd,Observation_Date,'{3}')) ";
           var table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.ToString(), dt.ToString()));
           double szr = 0;
           SurveyData lastMorYData = null;
           if (table.Rows.Count > 0)
           {
               szr = Convert.ToDouble(table.Rows[0]["Reading"]);
           }
           else
           {
               lastMorYData = GetData(dt.AddDays(-3), dt.AddDays(3), Datas);
               if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
           }
           if (szr != 0 && Math.Abs(sd.Survey_ZorR - szr) < Config.LimitZ) return true;
           //跟上一年的同一天做对比
           dt = sd.SurveyDate.Date.AddYears(-1);
           table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.ToString(), dt.ToString()));
           if (table.Rows.Count > 0)
           {
               szr = Convert.ToDouble(table.Rows[0]["Reading"]);
           }
           else
           {
               lastMorYData = GetData(dt.AddDays(-3), dt.AddDays(3), Datas);
               if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
           }
           if (szr != 0 && Math.Abs(sd.Survey_ZorR - szr) < Config.LimitZ) return true;
           err.Exception = "数据误差超限";

           return false;

       }
    }
    public class Fiducial_Earth_MeasureProcessMW : ProcessData
    {
        public Fiducial_Earth_MeasureProcessMW()
        {

            base.InsType = InstrumentType.Fiducial_Earth_Measure;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "大地测量";
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
            //测试代码
#if TEST
            PointCach.Add(path,new List<string>());
#endif
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                string pointnumber = psheet.SheetName;
                if (pointnumber.Contains("计算表"))
                {
                    pointnumber = pointnumber.Replace("计算表", "");
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
                }

                //测试代码
#if TEST
                PointCach[path].Add(psheet.SheetName);
#endif
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = pointnumber;
                pd.ExcelPath = path;
                DataInfo dinfo = new DataInfo();
                dinfo.DateIndex = 3;
                dinfo.RemarkIndex = 16;
                DateTime maxDatetime = new DateTime();
                bool flag = GetMaxDate(pd.SurveyPoint, out maxDatetime);
                double ZStandard = 0;
                if (!flag) ZStandard = GetZorRStandard(pd.SurveyPoint);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行
                SurveyData lastsd = null;
                int count = psheet.PhysicalNumberOfRows;
                //for (int j = count - 1; j > 0; j--)//从后往前读，没法滤掉有问题的数据
                for (int j = 1; j < count + 1; j++)//从前往后读
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
                        Earth_MeasureData sd = new Earth_MeasureData();
                        string errmsg = null;
                        if (!ReadRowExpand(row, dinfo, sd, out  errmsg))//读当前行
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
                        if (CheckData(sd, lastsd, dinfo, pd.Datas, pd.SurveyPoint, out msg))
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
       bool ReadRowExpand(IRow row, DataInfo info, Earth_MeasureData sd, out string err, bool last = false)
        {
            err = null;
            ICell cell;
            try
            {
                cell = row.GetCell(info.DateIndex);
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false;
                cell = row.GetCell(6);
                GetDataFromCell(cell, out sd.X);
                GetDataFromCell(cell, out sd.Y);
                GetDataFromCell(cell, out sd.H);
                GetDataFromCell(cell, out sd.DeltX);
                GetDataFromCell(cell, out sd.DeltY);
                GetDataFromCell(cell, out sd.DeltH);
                GetDataFromCell(cell, out sd.SumX);
                GetDataFromCell(cell, out sd.SumY);
                GetDataFromCell(cell, out sd.SumH);
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

    }
    /// <summary>
    /// 量水堰
    /// </summary>
    public class Fiducial_MeasureWater_WeirProcessMW : ProcessData
    {
        public Fiducial_MeasureWater_WeirProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_MeasureWater_Weir;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "量水堰";
        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            base.LoadData(path, null, out datas, out errors);
            //LoadDataExpand(path, null, out datas, out errors);
        }
        void LoadDataExpand(string path, DataInfo info, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();

        }
        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, List<SurveyData> Datas, string Survey_point_name, out ErrorMsg err)
        {
            string keystr = "reading";
            err = new ErrorMsg();
            if (sd.Remark.Contains("已复测")) return true;
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql;
            if (lastsd == null)
            {
                sql = Config.IsAuto ? "select top 1 * from {0} where Survey_point_Number='{1}' and Observation_Date<'{2}' and RecordMethod='人工' Order by Observation_Date desc" :
               "select  top 1 * from {0} where Survey_point_Number='{1}' and Observation_Date<'{2}'Order by Observation_Date desc ";
                var data = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, sd.SurveyDate.Date.ToString()));
                if (data.Rows.Count == 0) return true;
                double f = Convert.ToDouble(data.Rows[0][keystr]);
                //数据库里边不确定写的是频率还是模数
                if (Math.Abs(sd.Survey_ZorR - f) < Config.LimitZ ||
                   Math.Abs(sd.Survey_ZorR - Math.Sqrt(f * 1000)) < Config.LimitZ) return true;
            }
            else
            {
                //先跟上一次的数据做比较，不超过限值直接return
                if (Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;
            }

            //超过限值跟上一个月同一天作比较
            DateTime dt = sd.SurveyDate.Date.AddMonths(-1);
            sql = Config.IsAuto ? "select * from {0} where Survey_point_Number='{1}' and abs(datediff(d,Observation_Date,'{2}'))<3 and RecordMethod='人工' Order by abs(datediff(dd,Observation_Date,'{3}'))" :
                "select * from {0} where Survey_point_Number='{1}' and abs(datediff(d,Observation_Date,'{2}'))<3 Order by abs(datediff(dd,Observation_Date,'{3}')) ";
            var table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.ToString(), dt.ToString()));
            double szr = 0;
            SurveyData lastMorYData = null;
            if (table.Rows.Count > 0)
            {
                szr = Convert.ToDouble(table.Rows[0][keystr]);
            }
            else
            {
                lastMorYData = GetData(dt.AddDays(-3), dt.AddDays(3), Datas);
                if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
            }
            if (szr != 0 && Math.Abs(sd.Survey_ZorR - szr) < Config.LimitZ) return true;
            //跟上一年的同一天做对比
            dt = sd.SurveyDate.Date.AddYears(-1);
            table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.ToString(), dt.ToString()));
            if (table.Rows.Count > 0)
            {
                szr = Convert.ToDouble(table.Rows[0][keystr]);
            }
            else
            {
                lastMorYData = GetData(dt.AddDays(-3), dt.AddDays(3), Datas);
                if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
            }
            if (szr != 0 && Math.Abs(sd.Survey_ZorR - szr) < Config.LimitZ) return true;
            err.Exception = "数据误差超限";

            return false;

        }
        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            info.RorTIndex = -1;
            bool flag = true;
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
                    else if (DataUtils.CheckContainStr(cellstr, "读数", "水位标尺"))
                    {
                        info.ZoRIndex = pyhindex;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "L/s"))
                    {
                        info.Result=pyhindex;
                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                }
            }
            if (flag && info.TimeIndex > 0)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }

            return info;
        }
        protected override double GetZorRStandard(string point)
        {
            string sql = "select  Elevation from {0}  where Survey_point_Number='{1}'";
            CSqlServerHelper sqlhelper = CSqlServerHelper.GetInstance();
            var res = sqlhelper.SelectFirst(String.Format(sql, Config.InsCollection[Instrument_Name].Fiducial_Table, point));
            if (res != DBNull.Value)
            {
                return Convert.ToDouble(res);
            }
            else
            {
                return 0;
            }
        }
    
    }
    public class Fiducial_Hose_Settlement_GaugeProcessMW : ProcessData
    {
        public Fiducial_Hose_Settlement_GaugeProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Hose_Settlement_Gauge;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "水管式沉降仪";
        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
           // base.LoadData(path, null, out datas, out errors);
            LoadDataExpand(path, null, out datas, out errors);
        }
        private bool CheckNameEx(string point)
        {
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql = String.Format("select  * from {0} where Instrument_Serial='{1}' ", Config.InsCollection[this.Instrument_Name].Fiducial_Table,point);
            var dt = sqlhelper.SelectData(sql);
            return dt.Rows.Count > 0;
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
                string pointnumber = psheet.SheetName;
                if (!CheckNameEx(pointnumber))
                {
                    AddErroSheetname(path, psheet.SheetName);
                    continue;
                }

                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData(this.InsType);
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
                double ZStandard = 0;
                if (!flag) ZStandard = GetZorRStandard(pd.SurveyPoint);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行
                SurveyData lastsd = null;
                int count = psheet.PhysicalNumberOfRows;
                //for (int j = count - 1; j > 0; j--)//从后往前读，没法滤掉有问题的数据
                for (int j = 1; j < count + 1; j++)//从前往后读
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        DateTime dt;
                        //获取时间，不是时间进入下一次循环
                        if (!GetDateTime(row, dinfo, out dt)) continue;
                        //数据库中有数据，对比上次最大时间，比上次时间小，进入下一次循环
                        if (flag && dt.Date.CompareTo(maxDatetime) <= 0) continue;
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
                            if (!FirstFlag && Math.Abs(sd.Survey_ZorR - ZStandard) <= 0.1
                                || (Config.IsMoshu && Math.Abs(sd.Survey_ZorR - Math.Sqrt(ZStandard * 1000)) < 1))
                            {
                                FirstFlag = true;
                            }
                            if (!FirstFlag) continue;
                        }
                        ErrorMsg msg;
                        if (CheckData(sd, lastsd, dinfo, pd.Datas, pd.SurveyPoint, out msg))
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
                if (!flag && !FirstFlag) AddErroSheetname(path, psheet.SheetName + ",-1");
                datas.Add(pd);
            }
            workbook.Close();

        }
        bool ReadRow(IRow row, DataInfo info, SurveyData sd, out string err, bool last = false)
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
        
        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            info.RorTIndex = -1;
            bool flag = true;
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
                    else if (DataUtils.CheckContainStr(cellstr, "读数"))
                    {
                        info.ZoRIndex = pyhindex;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "沉降量"))
                    {
                        info.Result=pyhindex;
                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                }
            }
            if (flag && info.TimeIndex > 0)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }

            return info;
        }
        protected override double GetZorRStandard(string point)
        {
            string sql = "select  BData from {0}  where Survey_point_Number='{1}'";
            CSqlServerHelper sqlhelper = CSqlServerHelper.GetInstance();
            var res = sqlhelper.SelectFirst(String.Format(sql, Config.InsCollection[Instrument_Name].Fiducial_Table, point));
            if (res != DBNull.Value)
            {
                return Convert.ToDouble(res);
            }
            else
            {
                return 0;
            }
        }
        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, List<SurveyData> Datas, string Survey_point_name, out ErrorMsg err)
        {
            string keystr = "Frequency";
            err = new ErrorMsg();
            if (sd.Remark.Contains("已复测")) return true;
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql;
            if (lastsd == null)
            {
                sql = Config.IsAuto ? "select top 1 * from {0} where Survey_point_Number='{1}' and Observation_Date<'{2}' and RecordMethod='人工' Order by Observation_Date desc" :
               "select  top 1 * from {0} where Survey_point_Number='{1}' and Observation_Date<'{2}'Order by Observation_Date desc ";
                var data = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, sd.SurveyDate.Date.ToString()));
                if (data.Rows.Count == 0) return true;
                double f = Convert.ToDouble(data.Rows[0][keystr]) * 10;
                //数据库里边不确定写的是频率还是模数
                if (Math.Abs(sd.Survey_ZorR - f) < Config.LimitZ ||
                   Math.Abs(sd.Survey_ZorR - Math.Sqrt(f * 1000)) < Config.LimitZ) return true;
            }
            else
            {
                //先跟上一次的数据做比较，不超过限值直接return
                if (Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;
            }

            //超过限值跟上一个月同一天作比较
            DateTime dt = sd.SurveyDate.Date.AddMonths(-1);
            sql = Config.IsAuto ? "select * from {0} where Survey_point_Number='{1}' and abs(datediff(d,Observation_Date,'{2}'))<3 and RecordMethod='人工' Order by abs(datediff(dd,Observation_Date,'{3}'))" :
                "select * from {0} where Survey_point_Number='{1}' and abs(datediff(d,Observation_Date,'{2}'))<3 Order by abs(datediff(dd,Observation_Date,'{3}')) ";
            var table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.ToString(), dt.ToString()));
            double szr = 0;
            SurveyData lastMorYData = null;
            if (table.Rows.Count > 0)
            {
                szr = Convert.ToDouble(table.Rows[0][keystr]);
            }
            else
            {
                lastMorYData = GetData(dt.AddDays(-3), dt.AddDays(3), Datas);
                if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
            }
            if (szr != 0 && Math.Abs(sd.Survey_ZorR - szr * 10) < Config.LimitZ) return true;
            //跟上一年的同一天做对比
            dt = sd.SurveyDate.Date.AddYears(-1);
            table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.ToString(), dt.ToString()));
            if (table.Rows.Count > 0)
            {
                szr = Convert.ToDouble(table.Rows[0][keystr]) * 10;
            }
            else
            {
                lastMorYData = GetData(dt.AddDays(-3), dt.AddDays(3), Datas);
                if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
            }
            if (szr != 0 && Math.Abs(sd.Survey_ZorR - szr) < Config.LimitZ) return true;
            err.Exception = "数据误差超限";

            return false;
        }
    
    }

    public class Fiducial_Flex_DisplacementProcessMW : ProcessData
    {
        public Fiducial_Flex_DisplacementProcessMW()
        {
            base.InsType = InstrumentType.Fiducial_Flex_Displacement;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "引张线式水平位移计";
        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            base.LoadData(path, null, out datas, out errors);
            //LoadDataExpand(path, null, out datas, out errors);
        }
        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            info.RorTIndex = -1;
            bool flag = true;
            int count = psheet.LastRowNum > 10 ? 10 : psheet.LastRowNum;
            for (int j = 0; j < count; j++)//读取前10行
            {
                IRow row = psheet.GetRow(j);
                if (row == null) continue;
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
                    else if (DataUtils.CheckContainStr(cellstr, "读数"))
                    {
                        info.ZoRIndex = pyhindex;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "水平位移"))
                    {
                        info.Result=pyhindex;
                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                }
            }
            if (flag && info.TimeIndex > 0)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }

            return info;
        }
        protected override double GetZorRStandard(string point)
        {
            string sql = "select  BData from {0}  where Survey_point_Number='{1}'";
            CSqlServerHelper sqlhelper = CSqlServerHelper.GetInstance();
            var res = sqlhelper.SelectFirst(String.Format(sql, Config.InsCollection[Instrument_Name].Fiducial_Table, point));
            if (res != DBNull.Value)
            {
                return Convert.ToDouble(res);
            }
            else
            {
                return 0;
            }
        }
        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, List<SurveyData> Datas, string Survey_point_name, out ErrorMsg err)
        {
            string keystr = "Frequency";
            err = new ErrorMsg();
            if (sd.Remark.Contains("已复测")) return true;
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql;
            if (lastsd == null)
            {
                sql = Config.IsAuto ? "select top 1 * from {0} where Survey_point_Number='{1}' and Observation_Date<'{2}' and RecordMethod='人工' Order by Observation_Date desc" :
               "select  top 1 * from {0} where Survey_point_Number='{1}' and Observation_Date<'{2}'Order by Observation_Date desc ";
                var data = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, sd.SurveyDate.Date.ToString()));
                if (data.Rows.Count == 0) return true;
                double f = Convert.ToDouble(data.Rows[0][keystr]) * 10;
                //数据库里边不确定写的是频率还是模数
                if (Math.Abs(sd.Survey_ZorR - f) < Config.LimitZ ||
                   Math.Abs(sd.Survey_ZorR - Math.Sqrt(f * 1000)) < Config.LimitZ) return true;
            }
            else
            {
                //先跟上一次的数据做比较，不超过限值直接return
                if (Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;
            }

            //超过限值跟上一个月同一天作比较
            DateTime dt = sd.SurveyDate.Date.AddMonths(-1);
            sql = Config.IsAuto ? "select * from {0} where Survey_point_Number='{1}' and abs(datediff(d,Observation_Date,'{2}'))<3 and RecordMethod='人工' Order by abs(datediff(dd,Observation_Date,'{3}'))" :
                "select * from {0} where Survey_point_Number='{1}' and abs(datediff(d,Observation_Date,'{2}'))<3 Order by abs(datediff(dd,Observation_Date,'{3}')) ";
            var table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.ToString(), dt.ToString()));
            double szr = 0;
            SurveyData lastMorYData = null;
            if (table.Rows.Count > 0)
            {
                szr = Convert.ToDouble(table.Rows[0][keystr]);
            }
            else
            {
                lastMorYData = GetData(dt.AddDays(-3), dt.AddDays(3), Datas);
                if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
            }
            if (szr != 0 && Math.Abs(sd.Survey_ZorR - szr*10) < Config.LimitZ) return true;
            //跟上一年的同一天做对比
            dt = sd.SurveyDate.Date.AddYears(-1);
            table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.ToString(), dt.ToString()));
            if (table.Rows.Count > 0)
            {
                szr = Convert.ToDouble(table.Rows[0][keystr]) * 10;
            }
            else
            {
                lastMorYData = GetData(dt.AddDays(-3), dt.AddDays(3), Datas);
                if (lastMorYData != null) szr = lastMorYData.Survey_ZorR;
            }
            if (szr != 0 && Math.Abs(sd.Survey_ZorR - szr) < Config.LimitZ) return true;
            err.Exception = "数据误差超限";

            return false;
        }
    }
}
