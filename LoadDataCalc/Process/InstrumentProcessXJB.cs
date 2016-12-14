﻿using System;
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

namespace LoadDataCalc
{

    /// <summary> 渗压计
    /// </summary>
    public class Fiducial_Leakage_PressureProcessXJB : ProcessData
    {
        /// <summary> 从excel文件中读取数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override void ReadData(string path, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            base.LoadData(path, null, out datas, out errors);
        }
        //判断文件格式//渗压计有四种格式
        protected override DataInfo  GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            bool flag = true;
            bool Zflag = true;
            int count = psheet.LastRowNum > 10 ? 10 : psheet.LastRowNum;
            string laststr = "";
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
                    else if (DataUtils.CheckContainStr(cellstr, "电阻比", "频模", "频率", "模数","f2"))
                    {
                        info.ZoRIndex = pyhindex;
                        Zflag = false;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "电阻值", "温度电阻", "T")) info.RorTIndex = pyhindex;
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;

                    if (DataUtils.CheckContainStr(laststr, "频率", "频模", "模数"))
                    {
                        if (DataUtils.CheckContainStr(cellstr,"温度")) info.RorTIndex = pyhindex;
                    }
                    if (DataUtils.CheckContainStr(cellstr, "渗透压力", "水压力", "渗压")) info.Result = pyhindex;
                  
                    laststr = cellstr;
                }
            }
            #region//特殊
            IRow rowt = psheet.GetRow(34);
            if (rowt != null)
            {
                for (int c = 0; c < rowt.Cells.Count; c++)
                {
                    var cellstr = rowt.Cells[c].ToString();
                    int pyhindex = rowt.Cells[c].ColumnIndex;
                    if (DataUtils.CheckContainStr(cellstr, "观测日期", "日期"))
                    {
                        flag = false;
                        info.DateIndex = pyhindex;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "观测时间", "时间")) info.TimeIndex = pyhindex;
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
            #endregion
            if (flag && info.TimeIndex > 0)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }
            return info;
        }
        public Fiducial_Leakage_PressureProcessXJB()
        {
            base.InsType = InstrumentType.Fiducial_Leakage_Pressure;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "渗压计";
        }
    }

    /// <summary> 单点位移计
    /// </summary>
    public class Fiducial_Single_DisplacementProcessXJB : ProcessData
    {
        public Fiducial_Single_DisplacementProcessXJB()
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
        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            bool flag = true;
            int count = psheet.LastRowNum > 10 ? 10 : psheet.LastRowNum;
            bool NullFlag = true;
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
                    if (DataUtils.CheckContainStr(cellstr, "观测时间", "时间")) info.TimeIndex = pyhindex;
                    if (DataUtils.CheckContainStr(cellstr, "电阻比", "频模", "频率", "模数"))
                    {
                        NullFlag = false;
                        info.ZoRIndex = pyhindex;
                    }
                    if (DataUtils.CheckContainStr(cellstr, "电阻值", "温度电阻")) info.RorTIndex = pyhindex;
                    if (DataUtils.CheckContainStr(laststr, "频率", "频模", "模数"))
                    {
                        if (cellstr.Contains("温度")) info.RorTIndex = pyhindex;
                    }
                    if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                    if (DataUtils.CheckContainStr(cellstr, "位移", "开合度")) info.Result = pyhindex;
                    laststr = cellstr;
                }
            }
            if (flag && info.TimeIndex > 0)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }
            #region//没有匹配到直接读数据,识别里面的的数据
            if (NullFlag)//特殊
            {
                for (int j = 0; j < count; j++)//读取前10行
                {
                    if (!NullFlag) break;
                    IRow row = psheet.GetRow(j);
                    if (row == null) continue;
                    for (int c = 0; c < row.Cells.Count; c++)
                    {
                        var cell = row.Cells[c];
                        if (cell.CellType != CellType.Numeric) continue;
                        if (HSSFDateUtil.IsCellDateFormatted(cell))
                        {
                            info.DateIndex = cell.ColumnIndex;
                            cell = row.Cells[c + 1];
                            if (HSSFDateUtil.IsCellDateFormatted(cell))
                            {
                                info.TimeIndex = cell.ColumnIndex;
                                cell = row.Cells[c + 2];
                            }
                            double tempvalue;
                            if (double.TryParse(cell.ToString(), out  tempvalue))
                            {
                                if (tempvalue > 200)
                                {
                                    info.ZoRIndex = cell.ColumnIndex;
                                    info.RorTIndex = cell.ColumnIndex + 1;
                                }
                                else
                                {
                                    info.RorTIndex = cell.ColumnIndex;
                                    info.ZoRIndex = cell.ColumnIndex + 1;

                                }
                                NullFlag = false;
                                break;
                            }
                        }
                    }
                }
            }
            #endregion
            return info;
        }
    }

    /// <summary> 测缝计
    /// </summary>
    public class Fiducial_Measure_ApertureProcessXJB : ProcessData
    {
        public Fiducial_Measure_ApertureProcessXJB()
        {
            base.InsType = InstrumentType.Fiducial_Measure_Aperture;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "测缝计";
        }

        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            string filename = Path.GetFileName(path);
            if (filename.Contains("位错计"))
            {
                LoadDataExpand(path, null, out datas, out errors);
            }
            else
            {
                base.LoadData(path, null, out datas, out errors);
            }
        }
        void LoadDataExpand(string path, DataInfo info, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {//三向测缝计/位错计
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            IFormatProvider culture = CultureInfo.CurrentCulture;
            var workbook =WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                string pointname = psheet.SheetName;
                if (!CheckName(psheet.SheetName + "x"))
                {
                    if (CheckName(psheet.SheetName + "-x"))
                    {
                        pointname +="-";
                    }
                    else
                    {
                        AddErroSheetname(path, psheet.SheetName);
                        continue;
                    }
                }
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData(this.InsType);
                PointSurveyData pd1 = new PointSurveyData(this.InsType);
                PointSurveyData pd2 = new PointSurveyData(this.InsType);
                pd.SurveyPoint = pointname + "x";
                pd1.SurveyPoint = pointname + "y";
                pd2.SurveyPoint = pointname + "z";
                pd.ExcelPath = path;
                pd1.ExcelPath = path;
                pd2.ExcelPath = path;

                info = new DataInfo() { DateIndex = 0, TimeIndex = 1, ZoRIndex = 2, RorTIndex = 3,RemarkIndex=11,Result=8};
                DataInfo infoy = new DataInfo() { DateIndex = 0, TimeIndex = 1, ZoRIndex = 4, RorTIndex = 5, RemarkIndex = 11,Result=9 };
                DataInfo infoz = new DataInfo() { DateIndex = 0, TimeIndex = 1, ZoRIndex = 6, RorTIndex = 7, RemarkIndex = 11,Result=10 };
                info.TableName = infoy.TableName = infoz.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
                DateTime maxDatetimex = new DateTime();
                bool flagx = GetMaxDate(pd.SurveyPoint, out maxDatetimex);
                DateTime maxDatetimey = new DateTime();
                bool flagy = GetMaxDate(pd1.SurveyPoint, out maxDatetimey);
                DateTime maxDatetimez = new DateTime();
                bool flagz = GetMaxDate(pd2.SurveyPoint, out maxDatetimez);

                double ZStandard = flagx ? 0 : GetZorRStandard(pd.SurveyPoint);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行

                double ZStandardy = flagy ? 0 : GetZorRStandard(pd1.SurveyPoint);
                bool FirstFlagy = (ZStandardy == 0);//是否找到基准行

                double ZStandardz = flagz ? 0 : GetZorRStandard(pd2.SurveyPoint);
                bool FirstFlagz = (ZStandardz== 0);//是否找到基准行
                SurveyData lastsd = null;
                SurveyData lastsd1 = null;
                SurveyData lastsd2 = null;
                int count = psheet.LastRowNum;
               for (int j = 1; j <count+1; j++)
                {
                    IRow row = psheet.GetRow(j);
                    AddOne(info, row, flagx, FirstFlag, ZStandard, maxDatetimex, lastsd, pd, errors);
                    AddOne(infoy, row, flagx, FirstFlagy, ZStandardy, maxDatetimey, lastsd1, pd1, errors);
                    AddOne(infoz, row, flagx, FirstFlagz, ZStandardz, maxDatetimez, lastsd2, pd2, errors);
                }
                datas.Add(pd);
                datas.Add(pd1);
                datas.Add(pd2);
            }

            workbook.Close();
            
        }
        void AddOne(DataInfo info, IRow row, bool flag, bool FirstFlag, double ZStandard, DateTime maxDatetime,SurveyData lastsd, 
            PointSurveyData pd, List<ErrorMsg> errors)
        {
            try
            {
                if (row == null) return;
                DateTime dt;
                //获取时间，不是时间进入下一次循环
                if (!GetDateTime(row, info, out dt)) return;
                //数据库中有数据，对比上次最大时间，比上次时间小，进入下一次循环
                if (flag && dt.Date.CompareTo(maxDatetime) <= 0) return;
                SurveyData sd = new SurveyData();
                string errmsg = null;
                if (!ReadRowExpand(row, info, sd, out  errmsg))//读当前行
                {
                    if (errmsg != null)
                    {
                        ErrorMsg err = new ErrorMsg() { PointNumber =pd.SurveyPoint , ErrorRow = row.RowNum+ 1, Exception = errmsg };
                        errors.Add(err);
                    }
                    return;
                }
                //数据库没有数据，用基准值做对比，从基准值行开始读
                if (!flag)
                {
                    if (!FirstFlag && Math.Abs(sd.Survey_ZorR - ZStandard) <= 0.01)
                    {
                        FirstFlag = true;
                    }
                    if (!FirstFlag) return;
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
                ErrorMsg err = new ErrorMsg() { PointNumber = pd.SurveyPoint, ErrorRow = row.RowNum + 1, Exception = ex.Message };
                errors.Add(err);
                return;
            }
        }
        bool ReadRowExpand(IRow row, DataInfo info, SurveyData sd, out string err, bool last = false)
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
         
    }
    /// <summary> 裂缝计//算在测缝计
    /// </summary>
    public class Fiducial_ApertureProcessXJB : ProcessData
    {
        public Fiducial_ApertureProcessXJB()
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
    public class Fiducial_Steel_BarProcessXJB : ProcessData
    {
        public Fiducial_Steel_BarProcessXJB()
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
            if (filename.Contains("钢筋桩"))
            {
                  LoadData(path, out datas, out errors);
            }
            else
            {
                base.LoadData(path, null, out datas, out errors);
            }
        }
        //特殊的格式，sheet里边可能有两个测点的数据
        void LoadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            IFormatProvider culture = CultureInfo.CurrentCulture;
            var workbook = WorkbookFactory.Create(path);
            var sqlhelper = CSqlServerHelper.GetInstance();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                string name=psheet.SheetName;
                string surveyname1 = name;
                string surveyname2="";
                bool Istwo=false;
                if(name.Contains(","))
                {
                    Istwo=true;
                    var temp=name.Split(',');
                    surveyname1=temp[0];
                    surveyname2=temp[0].Split('-')[0]+"-"+temp[1];

                }
                else if(name.Contains("."))
                {
                    Istwo=true;
                     var temp=name.Split('.');
                    surveyname1=temp[0];
                    surveyname2=temp[0].Split('-')[0]+"-"+temp[1];
                }
                else if (name.Contains("，"))
                {
                    Istwo = true;
                    var temp = name.Split('，');
                    surveyname1 = temp[0];
                    surveyname2 = temp[0].Split('-')[0] + "-" + temp[1];
                }

                if (!CheckName(surveyname1))
                {
                    AddErroSheetname(path, psheet.SheetName);
                    continue;
                }
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData(this.InsType);
                PointSurveyData pd1 = new PointSurveyData(this.InsType);
                pd.SurveyPoint = surveyname1;
                pd.ExcelPath = path;
                if (Istwo)
                {
                    pd1.SurveyPoint = surveyname2;
                    pd1.ExcelPath = path;
                }

                DateTime maxDatetime = new DateTime();
                bool flag=GetMaxDate(pd.SurveyPoint,out maxDatetime);

                double ZStandard = 0;
                if (!flag) ZStandard = GetZorRStandard(pd.SurveyPoint);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行

                DataInfo info = GetInfo(psheet, path);
                if (info.Result < 0)
                {
                    info.Result = info.RorTIndex + 2;
                }
                
                DataInfo info1 = (DataInfo)info.Clone();
                info1.ZoRIndex = 6;
                info1.RorTIndex = 7;
                info1.Result = info1.RorTIndex + 2;
                System.Collections.IEnumerator rows = psheet.GetRowEnumerator();
                SurveyData lastsd = null;
                SurveyData lastsd1 = null;
                int count = psheet.LastRowNum;
                for (int j = 1; j <count+1; j++)
                {
                    try
                    {
                        IRow row = psheet.GetRow(j);
                        if (row == null) continue;
                        DateTime dt;
                        //获取时间，不是时间进入下一次循环
                        if (!GetDateTime(row, info, out dt)) continue;
                        //数据库中有数据，对比上次最大时间，比上次时间小，进入下一次循环
                        if (flag && dt.Date.CompareTo(maxDatetime) <= 0) continue;

                        SurveyData sd = new SurveyData();
                        #region//读取第一个点
                        string errmsg = null;
       
                        if (!ReadRow(row, sd, info, Istwo, out  errmsg))//读当前行
                        {
                            if (errmsg != null)
                            {
                                ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg };
                                errors.Add(err);
                            }
                        }
                        else
                        {

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
                            if (!Istwo) continue;
                        }
                        #endregion
                       
                        #region//读取第二个点
                        if (Istwo)
                        {
                            errmsg = null;
                            sd = new SurveyData();
                            if (!ReadRow(row, sd, info1, Istwo, out  errmsg))//读当前行
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

                                ErrorMsg msg;
                                if (CheckData(sd, lastsd1, info1, pd1.Datas, pd1.SurveyPoint, out msg))
                                {
                                    pd1.Datas.Add(sd);
                                    lastsd1 = sd;
                                }
                                else
                                {
                                    msg.ErrorRow = j + 1;
                                    msg.PointNumber = pd1.SurveyPoint;
                                    errors.Add(msg);
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

        private bool ReadRow(IRow row, SurveyData sd, DataInfo info,bool IsTwo, out string err, bool last = false)
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
                int index = IsTwo ? 10 : 6;
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
            int count = psheet.LastRowNum;
            bool flag = true;
            bool Zflag = true;
            bool Rflag = true;
            string filename = Path.GetFileName(filePath);
            bool isCheck = filename.Contains("钢筋桩");
            for (int j = 0; j < 10; j++)//读取前10行
            {
                IRow row = psheet.GetRow(j);
                if (row == null) continue;
                string laststr = "";

                for (int c = 0; c < row.Cells.Count; c++)
                {
                    var cellstr = row.Cells[c].ToString();
                    int pyhindex = row.Cells[c].ColumnIndex;
                    if (cellstr.Contains("观测日期") || cellstr.Contains("日期"))
                    {
                        flag = false;
                        info.DateIndex = pyhindex;
                    }
                    else if (cellstr.Contains("观测时间") || cellstr.Contains("时间")) info.TimeIndex = pyhindex;
                    else if (DataUtils.CheckContainStr(cellstr, "电阻比", "频模", "频率", "模数"))
                    {
                        if (isCheck&&!Zflag) continue;
                        info.ZoRIndex = pyhindex;
                        Zflag = false;
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "电阻值", "温度电阻", "电 阻"))
                    {
                        if (isCheck&&!Rflag) continue;
                        info.RorTIndex = pyhindex;
                        Rflag = false;
                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;

                    if (laststr.Contains("频率") || laststr.Contains("频模") || laststr.Contains("模数"))
                    {
                        if (cellstr.Contains("温度")) info.RorTIndex = pyhindex;
                    }
                    if (DataUtils.CheckContainStr(cellstr, "电 阻")) info.RorTIndex = pyhindex;//特殊电阻中间带空格
                    if (DataUtils.CheckContainStr(cellstr, "应力")) info.Result = pyhindex;//特殊电阻中间带空格
                    laststr = cellstr;
                }
            }
            if (flag && info.TimeIndex > 0)
            {
                info.DateIndex = info.TimeIndex;
                info.TimeIndex = -1;
            }
            if (info.Result < 0) info.Result = 5;
      
            
            return info;
        }
    }

    /// <summary>钢板计
    /// </summary>
    public class Fiducial_Armor_plateProcessXJB : ProcessData
    {
        public Fiducial_Armor_plateProcessXJB()
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
        
    }
    /// <summary> 锚杆应力计
    /// </summary>
    public class Fiducial_Anchor_PoleProcessXJB : ProcessData
    {
        public Fiducial_Anchor_PoleProcessXJB()
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
                    if (cellstr.Contains("观测日期") || cellstr.Contains("日期"))
                    {
                        flag = false;
                        info.DateIndex = pyhindex;
                    }
                    if (cellstr.Contains("观测时间") || cellstr.Contains("时间")) info.TimeIndex = pyhindex;
                    if (cellstr.Contains("电阻比") || cellstr.Contains("频模") ||cellstr.Contains("频率") || 
                        cellstr.Contains("模数") || cellstr.Contains("线性读数")) info.ZoRIndex = pyhindex;
                    if (cellstr.Contains("电阻值") || cellstr.Contains("温度电阻")) info.RorTIndex = pyhindex;
                    if (laststr.Contains("频率") || laststr.Contains("频模") ||
                        laststr.Contains("模数") || laststr.Contains("线性读数"))
                    {
                        if (cellstr.Contains("温度")) info.RorTIndex = pyhindex;
                    }
                    if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                    if (cellstr.Contains("应力")&&!cellstr.Contains("锚杆应力计")) info.Result = pyhindex; 
                    laststr = cellstr;
                }
            }
            if (info.Result < 0) info.Result = 5;
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
    public class Fiducial_Press_StressProcessXJB : ProcessData
    {
        public Fiducial_Press_StressProcessXJB()
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
    public class Fiducial_Basic_Rock_DistortionProcessXJB : ProcessData
    {
        public Fiducial_Basic_Rock_DistortionProcessXJB()
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
    public class Fiducial_Multi_DisplacementProcessXJB : ProcessData
    {
        public Fiducial_Multi_DisplacementProcessXJB()
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
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var psheet = workbook.GetSheetAt(i);
                if (!CheckName(psheet.SheetName))
                {
                    AddErroSheetname(path, psheet.SheetName);
                    continue;
                }
                if (StatusAction != null) StatusAction(path + "-" + psheet.SheetName);
                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = psheet.SheetName;
                pd.ExcelPath = path;
                DataInfo info = GetInfo(psheet);

                DateTime maxDatetime = new DateTime();
                bool flag = GetMaxDate(pd.SurveyPoint, out maxDatetime);

                string  sql = String.Format("select Instrument_Serial  from {0} where Survey_point_Number=@Survey_point_Number", Config.InsCollection[this.Instrument_Name].Fiducial_Table);
                var dt = sqlhelper.SelectData(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                info.Sum = dt.Rows.Count;
                if (pd.SurveyPoint == "M45-1")
                {

                }
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
                        if (flag && date.Date.CompareTo(maxDatetime) <= 0) continue;
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
                    else if (DataUtils.CheckContainStr(cellstr,"温度电阻","电阻值")||cellstr.StartsWith("温度"))
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

        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, List<SurveyData> datas, string Survey_point_name, out ErrorMsg err)
        {
            err = new ErrorMsg();
            if (sd.Remark.Contains("已复测")) return true;
            int count = 0;
            if (lastsd == null)
            {
                return true;
                var sqlhelper=CSqlServerHelper.GetInstance();
                string sql = "select top 1 * from Survey_Multi_Displacement  where Survey_point_Number='{0}'and Observation_Date<'{1}' order by Observation_Date desc";
                var data = sqlhelper.SelectData(String.Format(sql, Survey_point_name, sd.SurveyDate.Date.ToString()));
                if (data.Rows.Count == 0) return true;
                int index=0;
                foreach (var dic in sd.MultiDatas)
                {
                    index++;
                    double f = Convert.ToDouble(data.Rows[0]["Frequency" + index.ToString()]);
                    if (dic.Value.Survey_ZorR == 0 || f== 0) continue;
                    if (Math.Abs(dic.Value.Survey_ZorR - f) > Config.LimitZ&&
                        Math.Abs(dic.Value.Survey_ZorR - Math.Sqrt(f*1000)) > Config.LimitZ)
                    {
                        err.Exception = "部分数据误差超限";
                         count++;
                    }
                }    
            }
            else
            {
                //先跟上一次的数据做比较，不超过限值直接return
                foreach (var dic in sd.MultiDatas)
                {
                    if (dic.Value.Survey_ZorR == 0 || lastsd.MultiDatas[dic.Key].Survey_ZorR == 0) continue;
                    if (Math.Abs(dic.Value.Survey_ZorR - lastsd.MultiDatas[dic.Key].Survey_ZorR) > Config.LimitZ)
                    {
                        err.Exception = "部分数据误差超限";
                        count++;
                    }
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
            return (leftvalue >=rightvalue);
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
        private bool ReadRow(IRow row, DataInfo info, SurveyData sd,List<string>seriallist, out string err, bool last = false)
        {
            err = null;
            try
            {
                var cell = row.GetCell(info.DateIndex);
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false; 
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
                    if (info.Findex > 0) firstindex = info.Findex-1;
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
                    int firstindex = info.Findex+1;
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
                        if (!tflag && row.GetCell(firstindex + i * 2+1) != null)
                        {
                            //温度//多点位移计只读一个温度值
                            if (double.TryParse(row.GetCell(firstindex + i * 2+1).ToString(), out sd.Survey_RorT)) tflag = true;
                        }
                    }
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
        
    }

    /// <summary> 应变计
    /// </summary>
    public class Fiducial_Strain_GaugeProcessXJB : ProcessData
    {
        public Fiducial_Strain_GaugeProcessXJB()
        {
            base.InsType = InstrumentType.Fiducial_Strain_Gauge;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "应变计";

        }
        protected override DataInfo GetInfo(ISheet psheet, string filePath = null)
        {
            DataInfo info = new DataInfo();
            info.TableName = Config.InsCollection[this.Instrument_Name].Measure_Table;
            //这两个可能没有
            info.TimeIndex = -1;
            info.RemarkIndex = -1;
            bool flag = true;
            bool Zflag = false, Rflag = false;
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
                    else if (DataUtils.CheckContainStr(cellstr, "电阻比", "频模", "频率", "模数"))
                    {
                        if (!Zflag)
                        {
                            info.ZoRIndex = pyhindex;
                            Zflag = true;
                        }
                    }
                    else if (DataUtils.CheckContainStr(cellstr, "温度电阻"))
                    {
                        if (!Rflag)
                        {
                            info.RorTIndex = pyhindex;
                            Rflag = true;
                        }
                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;

                    if (DataUtils.CheckContainStr(laststr, "频率", "频模", "模数"))
                    {
                        if (!Rflag && cellstr.Contains("温度"))
                        {
                            info.RorTIndex = pyhindex;
                            Rflag = true;
                        }
                    }
                    if (DataUtils.CheckContainStr(cellstr, "应变"))//对比用
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
                    if (CheckNameExpand(sheetname))
                    {
                        ReadNonStressData(psheet,path, errors, sheetname);
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
                    ReadNonStressData(psheet,path, errors);
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
                        if (!flag && !FirstFlag) continue;
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
       
        //读取无应力计的数据
        private void ReadNonStressData(ISheet psheet,string path, List<ErrorMsg> errors,string number=null)
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
            pd.ExcelPath = path;
            DataInfo info = GetNonInfo(psheet);

            DateTime maxDatetime = new DateTime();
            bool flag = GetMaxDate(pd.SurveyPoint, out maxDatetime, "Survey_Nonstress");
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
                    //获取时间，不是时间进入下一次循环
                    if (!GetDateTime(row, info, out dt)) continue;
                    //数据库中有数据，对比上次最大时间，比上次时间小，进入下一次循环
                    if (flag && dt.Date.CompareTo(maxDatetime) <= 0) continue;

                    SurveyData sd = new SurveyData();
                    string errmsg = null;
                    if (!ReadRow(row, info, sd, out  errmsg))//读当前行
                    {
                        if (errmsg != null)
                        {
                            ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg+"无应力计" };
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
                    ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = ex.Message+"无应力计"};
                    errors.Add(err);
                    continue;
                }
            }
            ExpandDataCach.Add(pd);
           
        }
        private  DataInfo GetNonInfo(ISheet psheet, string filePath = null)
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
    public class Fiducial_NonstressProcessXJB : ProcessData
    {
        public Fiducial_NonstressProcessXJB()
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
                    if (DataUtils.CheckContainStr(cellstr, "应变"))//对比用
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
                    //sd.Survey_ZorRMoshu = sd.Survey_ZorR;
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

    }
    /// <summary> 锚索测力计
    /// </summary>
    public class Fiducial_Anchor_CableProcessXJB : ProcessData  
    {
        public Fiducial_Anchor_CableProcessXJB()
        {
            base.InsType = InstrumentType.Fiducial_Anchor_Cable;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "锚索测力计";

        }
        //测试，生成锚索测力计的每根弦的初始值
        public void ReadDataTEST(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
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
                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = psheet.SheetName;
                pd.ExcelPath = path;
                DataInfo info = GetInfo(psheet);

                DateTime maxDatetime = new DateTime();
                bool flag = GetMaxDate(pd.SurveyPoint, out maxDatetime);
                string sql = "select Read_GroupNum from Fiducial_Anchor_Cable where Survey_point_Number=@Survey_point_Number";
                var result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                if (result == DBNull.Value) continue;
                info.Sum = Convert.ToInt16(result);
                double ZStandard = 0;
                ZStandard = GetZorRStandard(pd.SurveyPoint);
                bool FirstFlag = false;    
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
                        SurveyData sd = new SurveyData();
                        string errmsg = null;

                        if (pd.SurveyPoint == "Dpu-4" || pd.SurveyPoint == "DPd-4")
                        {
                            if (!ReadRow(row, info, sd, out  errmsg, pd.SurveyPoint))//读当前行
                            {
                                if (errmsg != null)
                                {
                                    ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg };
                                    errors.Add(err);
                                }
                                continue;
                            }
                        }
                        else
                        {
                            if (!ReadRow(row, info, sd, out  errmsg))//读当前行
                            {
                                if (errmsg != null)
                                {
                                    ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg };
                                    errors.Add(err);
                                }
                                continue;
                            }
                        }

                        if (Math.Abs(sd.Average - ZStandard) <= 1)
                        {
                            using (StreamWriter sw = new StreamWriter("D:\\1.txt", true))
                            {
                                string s = "";
                                foreach (var dic in sd.MultiDatas)
                                {
                                    s += "," + dic.Value.Survey_ZorR.ToString();
                                }
                                sw.WriteLine(pd.SurveyPoint + s);
                            }
                            FirstFlag = true;
                            break;
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
                if(!FirstFlag)datas.Add(pd);
            }
            workbook.Close();
        }
        
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            //ReadDataTEST(path, out datas, out errors);
            //return;
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
                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = psheet.SheetName;
                pd.ExcelPath = path;
                DataInfo info = GetInfo(psheet);

                DateTime maxDatetime = new DateTime();
                bool flag = GetMaxDate(pd.SurveyPoint, out maxDatetime);
                string sql ="select Read_GroupNum from Fiducial_Anchor_Cable where Survey_point_Number=@Survey_point_Number";
                var  result = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                if (result == DBNull.Value) continue;
                info.Sum = Convert.ToInt16(result);
                double ZStandard = 0;
                if (!flag)ZStandard = GetZorRStandard(pd.SurveyPoint);
                bool FirstFlag = (ZStandard == 0);//是否找到基准行
                if (info.Findex < 0)
                {
                    info.Findex = info.TimeIndex > 0 ? info.TimeIndex+1 : info.DateIndex + 1;
                }

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
                        if (flag && dt.Date.CompareTo(maxDatetime) <= 0) continue; 
                        SurveyData sd = new SurveyData();
                        string errmsg = null;

                        if (pd.SurveyPoint == "Dpu-4" || pd.SurveyPoint == "DPd-4")
                        {
                            if (!ReadRow(row, info, sd, out  errmsg, pd.SurveyPoint))//读当前行
                            {
                                if (errmsg != null)
                                {
                                    ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg };
                                    errors.Add(err);
                                }
                                continue;
                            }
                        }
                        else
                        {
                            if (!ReadRow(row, info, sd, out  errmsg))//读当前行
                            {
                                if (errmsg != null)
                                {
                                    ErrorMsg err = new ErrorMsg() { PointNumber = psheet.SheetName, ErrorRow = j + 1, Exception = errmsg };
                                    errors.Add(err);
                                }
                                continue;
                            }
                        }

                        if (!flag)
                        {
                            if (!FirstFlag && Math.Abs(sd.Average - ZStandard) <= 1)
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
        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info, List<SurveyData> datas, string Survey_point_name, out ErrorMsg err)
        {
            err = new ErrorMsg();
            if (sd.Remark.Contains("已复测")) return true;
            int count = 0;
            if (Survey_point_name == "DP-1CP")
            {

            }
            if (lastsd == null)
            {
                var sqlhelper = CSqlServerHelper.GetInstance();
                string[] sbStrlist =new string[]{ "Reading_Red", "Reading_Black", "Reading_Yellow", "Reading_Blue", "Reading_Ash", "Reading_Purple" };
                string sql = "select top 1* from Survey_Anchor_Cable  where Survey_point_Number='{0}'and Observation_Date<'{1}' order by ID desc";
                var data = sqlhelper.SelectData(String.Format(sql, Survey_point_name, sd.SurveyDate.Date.ToString()));
                if (data.Rows.Count == 0) return true;
                int index = 0;
                foreach (var dic in sd.MultiDatas)
                {
                    double f = Convert.ToDouble(data.Rows[0][sbStrlist[index]]);
                    if (dic.Value.Survey_ZorR == 0 || f == 0) continue;
                    if (Math.Abs(dic.Value.Survey_ZorR - f) > Config.LimitZ &&
                        Math.Abs(dic.Value.Survey_ZorR - Math.Sqrt(f * 1000)) > Config.LimitZ)
                    {
                        err.Exception = "部分数据误差超限";
                        count++;
                    }
                    index++;
                }    
            }
            else
            {
                //先跟上一次的数据做比较，不超过限值直接return
                foreach (var dic in sd.MultiDatas)
                {
                    if (dic.Value.Survey_ZorR == 0 || lastsd.MultiDatas[dic.Key].Survey_ZorR == 0) continue;
                    if (Math.Abs(dic.Value.Survey_ZorR - lastsd.MultiDatas[dic.Key].Survey_ZorR) > Config.LimitZ)
                    {
                        err.Exception = "数据误差超限";
                        count++;
                    }
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
            info.Findex = -1;
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
                    else if (DataUtils.CheckContainStr(cellstr, "平均", "频模均值")) info.Average = pyhindex;
                    else if (DataUtils.CheckContainStr(cellstr, "测力计读数", "仪器读数")) info.Findex = pyhindex;
                    if (DataUtils.CheckContainStr(cellstr, "荷载", "测力计", "拉力")) info.Result = pyhindex;
                    

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
        //特殊点
        private bool ReadRow(IRow row, DataInfo info, SurveyData sd, out string err, string surveypoint, bool last = false)
        {
            err = null;
            try
            {
                var cell = row.GetCell(info.DateIndex);
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false;
                if (row.GetCell(info.RorTIndex) != null && !String.IsNullOrEmpty(row.GetCell(info.RorTIndex).ToString()))
                {
                    double.TryParse(row.GetCell(info.RorTIndex).ToString(), out sd.Survey_RorT);
                }
                bool flag = false;
                for (int i = 0; i < info.Sum; i++)
                {
                    int dec = i;
                    if (surveypoint == "Dpu-4")
                    {
                        if (i > 0) dec += 2;//中间空了两列
                    }
                    else if (surveypoint == "DPd-4")
                    {
                        if (i > 1) dec += 1;//中间空了1列
                    }
                    SurveyData temp = new SurveyData();
                    cell = row.GetCell(info.Findex + dec);
                    if (cell != null && !String.IsNullOrEmpty(cell.ToString().Trim()))
                    {
                        if (GetDataFromCell(cell, out temp.Survey_ZorR))
                        {
                            if (temp.Survey_ZorR != 0) flag = true;
                        }
                    }
                    sd.MultiDatas.Add(i.ToString(), temp);
                }
                cell = row.GetCell(info.Average);
                if (!CheckCell(cell)) GetDataFromCell(cell, out sd.Average);
                if (info.Result > 0)
                {
                    cell = row.GetCell(info.Result);
                    if (!CheckCell(cell)) GetDataFromCell(cell, out sd.ExcelResult);
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
        private bool ReadRow(IRow row, DataInfo info, SurveyData sd, out string err, bool last = false)
        {
            err = null;
            try
            {
                var cell = row.GetCell(info.DateIndex);
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false; 
                if (row.GetCell(info.RorTIndex) != null && !String.IsNullOrEmpty(row.GetCell(info.RorTIndex).ToString()))
                {
                    double.TryParse(row.GetCell(info.RorTIndex).ToString(), out sd.Survey_RorT);
                }
                bool flag = false;

                for (int i = 0; i < info.Sum; i++)
                {
                    SurveyData temp = new SurveyData();
                    cell = row.GetCell(info.Findex+i);
                    if (cell != null && !String.IsNullOrEmpty(cell.ToString().Trim()))
                    {
                        if (GetDataFromCell(cell, out temp.Survey_ZorR))
                        {
                            if (temp.Survey_ZorR != 0) flag = true;
                        }
                    }
                    sd.MultiDatas.Add(i.ToString(), temp);
                }

                cell = row.GetCell(info.Average);
                if (!CheckCell(cell)) GetDataFromCell(cell, out sd.Average);
                if (info.Result > 0)
                {
                    cell = row.GetCell(info.Result);
                    if (!CheckCell(cell)) GetDataFromCell(cell, out sd.ExcelResult);
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

    }
    /// <summary> 应变计组
    /// </summary>
    public class Fiducial_Strain_GroupProcessXJB : ProcessData
    {
        public Fiducial_Strain_GroupProcessXJB()
        {
            base.InsType = InstrumentType.Fiducial_Multi_Displacement;
            base.ErrorLimitRT = 20;
            base.ErrorLimitZR = 20;
            base.Instrument_Name = "应变计组";

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
                    if (CheckNameExpand(psheet.SheetName))//读无应力计
                    {
                        DataInfo tempinfo = base.GetInfo(psheet);
                        ReadNonStressData(psheet,path, tempinfo, errors,psheet.SheetName);
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
                for (int k=0;k<dt.Rows.Count;k++)
                {
                    seriallist.Add(dt.Rows[k][0].ToString());
                }
                if (info.FCount > seriallist.Count)
                {
                    ReadNonStressData(psheet, path, info, errors);//读取无应力计的数据
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
                    else if (DataUtils.CheckContainStr(cellstr,"电阻比"))
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
        protected override bool CheckData(SurveyData sd, SurveyData lastsd, DataInfo info,List<SurveyData>datas, string Survey_point_name, out ErrorMsg err)
        {
            err = null;
            if (sd.Remark.Contains("已复测")) return true;
            //先跟上一次的数据做比较，不超过限值直接return
            int count = 0;
            err = new ErrorMsg();
            if (lastsd == null)
            {
                var sqlhelper = CSqlServerHelper.GetInstance();
                string sql = "select top 1 * from Survey_Strain_Group  where Survey_point_Number='{0}'and Observation_Date<'{1}' order by Observation_Date desc";
                var data = sqlhelper.SelectData(String.Format(sql, Survey_point_name, sd.SurveyDate.Date.ToString()));
                if (data.Rows.Count == 0) return true;
                int index = 0;
                foreach (var dic in sd.MultiDatas)
                {
                    index++;
                    double f = Convert.ToDouble(data.Rows[0]["Frequency" + index.ToString()]);
                    if (dic.Value.Survey_ZorR == 0 || f == 0) continue;
                    if (Math.Abs(dic.Value.Survey_ZorR - f) > Config.LimitZ &&
                        Math.Abs(dic.Value.Survey_ZorR - Math.Sqrt(f * 1000)) > Config.LimitZ)
                    {
                        err.Exception = "部分数据误差超限";
                        count++;
                    }
                }    
            }
            else
            {
 
                foreach (var dic in sd.MultiDatas)
                {
                    if (dic.Value.Survey_ZorR == 0 || lastsd.MultiDatas[dic.Key].Survey_ZorR == 0) continue;
                    if (Math.Abs(dic.Value.Survey_ZorR - lastsd.MultiDatas[dic.Key].Survey_ZorR) > Config.LimitZ)
                    {
                        err.Exception = "部分数据误差超限";
                        count++;
                    }
                }

            }
            if (count == 0) err = null;
            if (count < sd.MultiDatas.Keys.Count) return true;
            err.Exception = "所有数据误差超限";
            return false;
        }
        private bool ReadRow(IRow row, DataInfo info, SurveyData sd,List<string>seriallist, out string err, bool last = false)
        {
            err = null;
            try
            {
                var cell = row.GetCell(info.DateIndex);
                cell = row.GetCell(info.DateIndex);
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false;
                cell = row.GetCell(info.ZoRIndex);

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
        private void ReadNonStressData(ISheet psheet,string path,DataInfo info,  List<ErrorMsg> errors,string number=null)
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
            pd.ExcelPath = path;
            DateTime maxDatetime = new DateTime();
            bool flag = GetMaxDate(pd.SurveyPoint, out maxDatetime, "Survey_Nonstress");
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
                    //获取时间，不是时间进入下一次循环
                    if (!GetDateTime(row, info, out dt)) continue;
                    //数据库中有数据，对比上次最大时间，比上次时间小，进入下一次循环
                    if (flag && dt.Date.CompareTo(maxDatetime) <= 0) continue;

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
                    if (base.CheckData(sd, lastsd, info, pd.Datas, pd.SurveyPoint, out msg))
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
                cell = row.GetCell(info.ZoRIndex);
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
    /// 温度计
    /// </summary>
    public class Fiducial_TemperatureProcessXJB : ProcessData
    {
        public Fiducial_TemperatureProcessXJB()
        {

            base.InsType = InstrumentType.Fiducial_Temperature;
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
                       // Rflag = false;
                        info.RorTIndex = -1;
                        info.Findex = pyhindex;

                    }
                    else if (cellstr.Contains("备注")) info.RemarkIndex = pyhindex;
                    if (DataUtils.CheckContainStr(cellstr, "温度")) info.Result = pyhindex;
                    
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
            string sql = "select  Zero_Resistance from {0}  where Survey_point_Number='{1}'";
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

                PointSurveyData pd = new PointSurveyData(this.InsType);
                pd.SurveyPoint = firstname;
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
                SurveyData lastsd = null;
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
                        SurveyData sd  = new SurveyData();
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
        bool ReadRowExpand(IRow row, DataInfo info, SurveyData sd, out string err, bool last = false)
        {
            err = null;
            ICell cell;
            try
            {
                if (row == null) return false;
                cell = row.GetCell(info.DateIndex);
                if (!GetDateTime(row, info, out sd.SurveyDate)) return false; 

                if (info.RorTIndex < 0)
                {
                    cell = row.GetCell(info.ZoRIndex);
                    if (!CheckCell(cell))
                    {
                        GetDataFromCell(cell, out sd.Survey_ZorR);//电阻
                    }
                }
                if (sd.Survey_ZorR == 0) return false;
                if (info.Result > 0)
                {
                    cell = row.GetCell(info.Result);
                    if (!CheckCell(cell)) GetDataFromCell(cell, out sd.ExcelResult);
                }
                if (info.RemarkIndex > 0) sd.Remark = row.GetCell(info.RemarkIndex) == null ? "" : row.GetCell(info.RemarkIndex).ToString();
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
            if (lastsd == null) return true;//上一行数据为空不处理
            //先跟上一次的数据做比较，不超过限值直接return
            if (Math.Abs(sd.Survey_ZorR - lastsd.Survey_ZorR) < Config.LimitZ) return true;

            //超过限值跟上一个月同一天作比较
            var sqlhelper = CSqlServerHelper.GetInstance();
            DateTime dt = sd.SurveyDate.Date.AddMonths(-1);
            string sql = Config.IsAuto ? "select * from {0} where Survey_point_Number='{1}' and Observation_Date>='{2}' and Observation_Date<'{3}' and RecordMethod='人工'" :
                "select * from {0} where Survey_point_Number='{1}' and Observation_Date>='{2}' and Observation_Date<'{3}'";
            var table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.AddDays(-3).ToString(), dt.AddDays(3).ToString()));
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
            if (Math.Abs(sd.Survey_ZorR - szr) < Config.LimitZ) return true;
            //跟上一年的同一天做对比
            dt = sd.SurveyDate.Date.AddYears(-1);
            table = sqlhelper.SelectData(String.Format(sql, info.TableName, Survey_point_name, dt.AddDays(-3).ToString(), dt.AddDays(3)));
            if (table.Rows.Count > 0)
            {
                szr = Convert.ToDouble(table.Rows[0]["Reading"]);
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
    }


}
