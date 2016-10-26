using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using NPOI.SS.UserModel;

namespace LoadDataCalc
{
   public class MultiDisplacementCalc
    {
       /// <summary>
       /// 测点点号
       /// </summary>
       public string Survey_point_Number;
       /// <summary>
       /// 单只编号
       /// </summary>
       public string Ins_serial;
       /// <summary>
       /// 是否是模数，模数直接相减，非模数要平方除以1000
       /// 是模数为1
       /// </summary>
       public int  IsMoshu = 0;
       /// <summary>
       /// 是否作为基准值
       /// 是基准为1
       /// </summary>
       public int IsStanderd = 0;
       /// <summary>
       /// 是否是被减，是就是其他点减去该点，不是就是基准减去其他点
       /// 被减为1--（其他点-基准）  0--（基准-其他点）
       /// </summary>
       public int IsBySubtract = 1;
       /// <summary>
       /// 特殊系数，默认为1
       /// </summary>
       public double Coefficient = 1;
       /// <summary>
       /// 计算方法为0就用默认的方法计算，其他为特殊
       /// </summary>
       public int MCalcType = 0;
       /// <summary>
       /// 孔口是否为0，孔口为0有基准值的情况下，基准值取自己
       /// 孔口不为0的情况下基准值取0，孔口取基准值的值
       /// 该值为0 表示孔口为0
       /// </summary>
       public int R0IsZero = 0;

       public MultiDisplacementCalc this[string Ins_Serial]
       {
           get { return MultiDisplacementCalcs.Where(calc => calc.Ins_serial == Ins_serial).FirstOrDefault(); }
       }
       public MultiDisplacementCalc[] GetBySurveypoint(string SurveyPoint)
       {
            return MultiDisplacementCalcs.Where(calc => calc.Survey_point_Number == SurveyPoint).ToArray(); 
       }

       public List<MultiDisplacementCalc> MultiDisplacementCalcs = new List<MultiDisplacementCalc>();
 

       public List<MultiDisplacementCalc> GetCalclist(List<PointSurveyData> Mdata)
       {
           List<MultiDisplacementCalc> calcs = new List<MultiDisplacementCalc>();
           foreach (PointSurveyData pd in Mdata)
           {
               ParamData mpd = GetParam(pd.SurveyPoint);
               if (pd.Datas.Count< 1) continue;
               SurveyData msd = pd.Datas[0];
               string insserial = null;
               foreach (var di in msd.MultiDatas)
               {
                   if (di.Key.EndsWith("A")) insserial = di.Key;
               }
               insserial = (insserial != null) ? insserial : msd.MultiDatas.Keys.FirstOrDefault();
               foreach (var dic in msd.MultiDatas)
               {
                   MultiDisplacementCalc mc = new MultiDisplacementCalc();
                   SurveyData  sd= dic.Value;
                   mc.Survey_point_Number = pd.SurveyPoint;
                   mc.Ins_serial = dic.Key;
                   mc.IsMoshu = (sd.Survey_ZorR > 3500) ? 1 : 0;
                   if (mc.Ins_serial == insserial)  mc.IsStanderd = 1;
                   calcs.Add(mc);
               }
           }
           FillList(calcs);

           return calcs;
       }

       private  ParamData GetParam(string Survey_point_Number)
       {
           string sql = @"select Instrument_Type,Calculate_Coeffi_G,Tempera_Revise_K,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance,Instrument_Serial
                                from Fiducial_Multi_Displacement where Survey_point_Number='{0}'";
           sql = string.Format(sql, Survey_point_Number);
           var SqlHelper = CSqlServerHelper.GetInstance();
           var dt = SqlHelper.SelectData(sql);
           if (dt.Rows.Count < 1) return null;
           ParamData Mpd = new ParamData();
           for (int i = 0; i < dt.Rows.Count; i++)
           {
               try
               {
                   ParamData pd = new ParamData();
                   pd.SurveyPoint = Survey_point_Number;
                   string insSerial = dt.Rows[i][7].ToString();
                   if (dt.Rows[i][1] == null || dt.Rows[i][3] == null) return null;//G和Z必须有
                   pd.Gorf = Convert.ToDouble(dt.Rows[i][1]);
                   pd.ZorR = Convert.ToDouble(dt.Rows[i][3]);
                   if (dt.Rows[i][2] == null)
                   {
                       pd.Korb = 0;
                   }
                   else
                   {
                       pd.Korb = Convert.ToDouble(dt.Rows[i][2]);
                       pd.RorT = Convert.ToDouble(dt.Rows[i][4]);
                   }
                   string instype = dt.Rows[i][0].ToString();
                   pd.TemperatureRead = Convert.ToDouble(dt.Rows[i][5]);
                   pd.ZeroR = Convert.ToDouble(dt.Rows[i][6]);
                   if (instype.Contains("差阻") || (pd.TemperatureRead != 1 && pd.ZeroR > 0))//默认是振弦
                   {
                       pd.InsCalcType = CalcType.DifBlock;
                   }
                   Mpd.MParamData.Add(insSerial, pd);
               }
               catch { }
           }
           return Mpd;

       }
      
       public void FillList(List<MultiDisplacementCalc> calclist)
       {
           string sql = "select * from Fiducial_Multi_Displacement";
           var sqlhelper = CSqlServerHelper.GetInstance();
           var dt = sqlhelper.SelectData(sql);
           List<string> list = new List<string>();
           for (int j = 0; j < calclist.Count; j++)
           {
               list.Add(calclist[j].Survey_point_Number);
           }
           for (int i = 0; i < dt.Rows.Count; i++)
           {
                string surveynumber = (string)dt.Rows[i][1];
                if (list.Contains(surveynumber)) continue;
                MultiDisplacementCalc mc = new MultiDisplacementCalc();
                mc.Survey_point_Number = surveynumber;
                mc.Ins_serial = (string)dt.Rows[i][2];
                if (mc.Ins_serial.EndsWith("A")) mc.IsStanderd = 1;
                calclist.Add(mc);
           }
       }

       /// <summary>
       /// 写入数据库
       /// </summary>
       /// <param name="calclist"></param>
       public void WriteDB(List<MultiDisplacementCalc> calclist)
       {
           string sql = @"use XJBDatabase
                                if exists ( select * 
                                 from  sysobjects
                                 where name = 'Fiducial_Multi_Displacement_Expand'
                                 and type = 'U')
                                 drop table Fiducial_Multi_Displacement_Expand
                                go
                                create table Fiducial_Multi_Displacement_Expand
                                (
	                                [Survey_point_Number] [varchar](30) NULL,
	                                [Instrument_Serial] [varchar](30) primary key,
	                                [IsMoshu] [int] NULL,
	                                [IsStanderd] [int] NULL,
	                                [IsBySubtract] [int] NULL,
	                                [Coefficient] [int] NULL
                                )
                                go";
           var sqlhelper = CSqlServerHelper.GetInstance();
           var res = sqlhelper.InsertDelUpdate(sql);
           DataTable dt = new DataTable();
           dt.TableName = "Fiducial_Multi_Displacement_Expand";
           dt.Columns.Add("Survey_point_Number");
           dt.Columns.Add("Instrument_Serial");
           dt.Columns.Add("IsMoshu");
           dt.Columns.Add("IsStanderd");
           dt.Columns.Add("IsBySubtract");
           dt.Columns.Add("Coefficient");
           for (int i = 0; i < calclist.Count; i++)
           {
               DataRow dr = dt.NewRow();
               dr["Survey_point_Number"] = calclist[i].Survey_point_Number;
               dr["Instrument_Serial"] = calclist[i].Ins_serial; 
               dr["IsMoshu"] = calclist[i].IsMoshu; 
               dr["IsStanderd"] = calclist[i].IsStanderd; 
               dr["IsBySubtract"] = calclist[i].IsBySubtract; 
               dr["Coefficient"] = calclist[i].Coefficient;
               dt.Rows.Add(dt);
           }
           sqlhelper.BulkCopy(dt);
       }

       /// <summary>
       /// 写入xml
       /// </summary>
       /// <param name="calclist"></param>
       /// <param name="path"></param>
       public void Wirtexml(List<MultiDisplacementCalc> calclist,string path)
       {
           if (File.Exists(path)) File.Delete(path);
           XmlDocument xml = new XmlDocument();
           XmlDeclaration xmlDeclaration = xml.CreateXmlDeclaration("1.0", "utf-8", "yes");
           var root = xml.CreateElement("MultiDisplacment");
           xml.AppendChild(xmlDeclaration);
           xml.AppendChild(root);
           foreach (var mc in calclist)
           {
               var ent = xml.CreateElement("MC");
               ent.SetAttribute("Survey_point_Number", mc.Survey_point_Number);
               ent.SetAttribute("Ins_serial", mc.Ins_serial);
               ent.SetAttribute("IsBySubtract", mc.IsBySubtract.ToString());
               ent.SetAttribute("IsMoshu", mc.IsMoshu.ToString());
               ent.SetAttribute("IsStanderd", mc.IsStanderd.ToString());
               ent.SetAttribute("Coefficient", mc.Coefficient.ToString());
               ent.SetAttribute("MCalcType", mc.MCalcType.ToString());
               ent.SetAttribute("R0IsZero", mc.R0IsZero.ToString());
               root.AppendChild(ent);
           }
           xml.Save(path);
       }

       /// <summary>
       /// 从xml加载
       /// </summary>
       /// <param name="path"></param>
       /// <returns></returns>
       public void Loadxml(string path)
       {
           XmlDocument xml = new XmlDocument();
           xml.Load(path);
           var root = xml.DocumentElement;
           var inslist = root.ChildNodes;
           foreach (var node in inslist)
           {
               var ent = node as XmlElement;
               MultiDisplacementCalc info = new MultiDisplacementCalc();
               info.Survey_point_Number = ent.GetAttribute("Survey_point_Number");
               info.Ins_serial = ent.GetAttribute("Ins_serial");
               info.IsBySubtract = (int)Convert.ToDouble(ent.GetAttribute("IsBySubtract"));
               info.IsMoshu = (int)Convert.ToDouble(ent.GetAttribute("IsMoshu"));
               info.IsStanderd = (int)Convert.ToDouble(ent.GetAttribute("IsStanderd"));
               info.Coefficient = Convert.ToDouble(ent.GetAttribute("Coefficient"));
               info.MCalcType = (int)Convert.ToDouble(ent.GetAttribute("MCalcType"));
               info.R0IsZero = (int)Convert.ToDouble(ent.GetAttribute("R0IsZero"));
               MultiDisplacementCalcs.Add(info);
           }
       }
       public void loadexcel(string path)
       {
           var workbook = WorkbookFactory.Create(path);
           var psheet = workbook.GetSheetAt(0);              
           int count = psheet.LastRowNum;
           for (int i = 1; i<count; i++)
           {
               IRow row = psheet.GetRow(i);
               MultiDisplacementCalc info = new MultiDisplacementCalc();
               info.Survey_point_Number = row.GetCell(0).StringCellValue;
               info.Ins_serial = row.GetCell(1).StringCellValue;
               info.IsBySubtract = (int)row.GetCell(2).NumericCellValue;
               info.IsMoshu = (int)row.GetCell(3).NumericCellValue;
               info.IsStanderd = (int)row.GetCell(4).NumericCellValue;
               info.Coefficient = row.GetCell(5).NumericCellValue;
               info.MCalcType = (int)row.GetCell(6).NumericCellValue;
               info.R0IsZero = (int)row.GetCell(7).NumericCellValue;
               MultiDisplacementCalcs.Add(info);
 
           }
 
       }
   }
}
