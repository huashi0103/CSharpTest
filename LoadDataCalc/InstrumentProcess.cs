using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Reflection;
using System.IO;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Globalization;
using System.Data.SqlClient;


namespace LoadDataCalc
{
    /// <summary>
    /// 渗压计
    /// </summary>
    public class Fiducial_Leakage_PressureProcess : ProcessData
    {

        /// <summary>仪器类型
        /// </summary>
        public InstrumentType InsType { get; set; }
        /// <summary> 从excel文件中读取数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override void ReadData(string path, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            datas = new List<PointSurveyData>();
            errors = new List<ErrorMsg>();
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                IFormatProvider culture = CultureInfo.CurrentCulture;
                var workbook = WorkbookFactory.Create(path);
                 var sqlhelper=CSqlServerHelper.GetInstance();
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    var psheet = workbook.GetSheetAt(i);
                    PointSurveyData pd = new PointSurveyData();
                    pd.SurveyPoint = psheet.SheetName;
                    var dt = sqlhelper.SelectData("select * from Survey_Leakage_Pressure where Survey_point_Number=@Survey_point_Number",
                        new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                    bool flag = dt.Rows.Count > 0 ? true : false;
                    DateTime maxDatetime = new DateTime();
                    if (flag)//有数据就查数据
                    {
                        maxDatetime = (DateTime)sqlhelper.SelectFirst("select max(Observation_Date) from Survey_Leakage_Pressure where Survey_point_Number=@Survey_point_Number",
                            new SqlParameter("@Survey_point_Number", pd.SurveyPoint));
                    }
                    System.Collections.IEnumerator rows = psheet.GetRowEnumerator();
                    int rowcn = 0;//行计数
                    while (rows.MoveNext())
                    {
                        try
                        {
                            rowcn++;
                            if (rows.Current == null) continue;
                            IRow row = (IRow)rows.Current;
                            var cell = row.GetCell(0);
                            if (!cell.IsMergedCell && String.IsNullOrEmpty(cell.ToString())) break;//用第一列的值是否是数字来判断，时间也是数字
                            if (cell.CellType != CellType.Numeric) continue;//用第一列的值是否是数字来判断，时间也是数字
                            var date = cell.DateCellValue.ToString("MM/dd/yyyy");
                            SurveyData sd = new SurveyData();
                            sd.Survey_ZorR = double.Parse(row.GetCell(2).ToString());//频率/基准电阻
                            sd.Survey_RorT = double.Parse(row.GetCell(3).ToString());//温度
                            sd.Remark = row.GetCell(6) == null ? "" : row.GetCell(6).ToString();
                            sd.SurveyDate = DateTime.Parse(date + " " + row.GetCell(1).ToString(), culture, DateTimeStyles.NoCurrentDateDefault);
                            if(flag)
                            {
                                if(sd.SurveyDate.CompareTo(maxDatetime)>0)
                                {
                                      pd.Datas.Add(sd);
                                }
                            }
                            else
                            {
                                    pd.Datas.Add(sd);
                            }
                        }
                        catch
                        {
                            ErrorMsg err = new ErrorMsg();
                            err.PointNumber = psheet.SheetName;
                            err.ErrorRow = rowcn;
                            errors.Add(err);
                            continue;
                        }
                    }
                    datas.Add(pd);
                }
               
            }
            base.Log(path, errors);            
        }
        /// <summary>把测量数据写入到数据库
        /// </summary>
        /// <returns></returns>
        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            dt.TableName = "Survey_Leakage_Pressure";
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("Frequency");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            var sqlhelper = CSqlServerHelper.GetInstance();
            int id = (int)sqlhelper.SelectFirst("select max(ID) as sid  from Survey_Leakage_Pressure ");
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
                    dr["Remark"] =surveydata.Remark;                   
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }

            sqlhelper.BulkCopy(dt);
            return datas.Count;
        }
        /// <summary>把计算后的结果数据写入数据库
        /// </summary>
        /// <returns></returns>
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            dt.TableName = "Result_Leakage_Pressure";
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
            int id = (int)sqlhelper.SelectFirst("select max(ID) as sid  from Survey_Leakage_Pressure ");
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
                    dr["Temperature"] = surveydata.Tempreture;
                    dr["loadReading"] = surveydata.LoadReading;
                    dr["ResultReading"] = surveydata.ResultReading;
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }

            sqlhelper.BulkCopy(dt);
            return datas.Count;
        }

        public  Fiducial_Leakage_PressureProcess()
        {
            InsType = InstrumentType.Fiducial_Leakage_Pressure;
           
        }
    }
}
