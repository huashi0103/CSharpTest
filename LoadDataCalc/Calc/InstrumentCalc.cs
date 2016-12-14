/*
 * 各种仪器的实现类
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LoadDataCalc
{
    /// <summary>
    /// 钢丝位移计
    /// </summary>
    public class Fiducial_SteelWire_Displacement : BaseInstrument
    {
        public Fiducial_SteelWire_Displacement()
        {
            base.InsType = InstrumentType.Fiducial_SteelWire_Displacement;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 双金属标
    /// </summary>
    public class Fiducial_Double_Metal_Target : BaseInstrument
    {
        public Fiducial_Double_Metal_Target()
        {
            base.InsType = InstrumentType.Fiducial_Double_Metal_Target;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 大地测量
    /// </summary>
    public class  Fiducial_Earth_Measure : BaseInstrument
    {
        public Fiducial_Earth_Measure()
        {
            base.InsType = InstrumentType.Fiducial_Earth_Measure;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>正倒垂线
    /// </summary>
    public class Fiducial_Inverst_Vertical : BaseInstrument
    {
        public Fiducial_Inverst_Vertical()
        {
            base.InsType = InstrumentType.Fiducial_Inverst_Vertical;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 心墙填筑高程
    /// </summary>
    public class Fiducial_Environment_Elevation : BaseInstrument
    {
        public Fiducial_Environment_Elevation()
        {
            base.InsType = InstrumentType.Fiducial_Environment_Elevation;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 视准线
    /// </summary>
    public class Fiducial_Watch_Directrix : BaseInstrument
    {
        public Fiducial_Watch_Directrix()
        {
            base.InsType = InstrumentType.Fiducial_Watch_Directrix;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    /// <summary>
    /// 引张线
    /// </summary>
    public class Fiducial_Flex_Line : BaseInstrument
    {
        public Fiducial_Flex_Line()
        {
            base.InsType = InstrumentType.Fiducial_Flex_Line;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 激光准直
    /// </summary>
    public class Fiducial_Laser_Collimation : BaseInstrument
    {
        public Fiducial_Laser_Collimation()
        {
            base.InsType = InstrumentType.Fiducial_Laser_Collimation;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 静力水准
    /// </summary>
    public class Fiducial_Statics_Level : BaseInstrument
    {
        public Fiducial_Statics_Level()
        {
            base.InsType = InstrumentType.Fiducial_Statics_Level;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 伸缩仪
    /// </summary>
    public class Fiducial_Extension : BaseInstrument
    {
        public Fiducial_Extension()
        {
            base.InsType = InstrumentType.Fiducial_Extension;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 测斜仪
    /// </summary>
    public class Fiducial_Survey_Slant : BaseInstrument
    {
        public Fiducial_Survey_Slant()
        {
            base.InsType = InstrumentType.Fiducial_Survey_Slant;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 单点位移计
    /// </summary>
    public class Fiducial_Single_Displacement : BaseInstrument
    {
        public Fiducial_Single_Displacement()
        {
            base.InsType = InstrumentType.Fiducial_Single_Displacement;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }

        //特殊的计算方式/
        private double calcOnePointExpand(ParamData pd, SurveyData sd)
        {
            if (Math.Abs(sd.Survey_ZorR) < 1)
            {
                return 0;
            }
            return ((int)(pd.Gorf * (sd.Survey_ZorR - pd.ZorR) * 100 + 0.5)) / 100.0 + pd.Korb * (sd.Survey_RorT - pd.RorT);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            if (Config.ProCode == "XJB"&&param.SurveyPoint.StartsWith("ID-"))//特殊的计算方式
            {
                data.LoadReading=calcOnePointExpand(param, data);
                data.Tempreture = data.Survey_RorT;
                return data.LoadReading;
            }

            double result = 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 5000||param.IsPotentiometer)
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
                    //data.Survey_ZorRMoshu = data.Survey_ZorR;
                }
                else
                {
                    if (Config.IsMoshu)
                    {
                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000.0 - param.ZorR) +
                            param.Korb * (data.Survey_RorT - param.RorT);
                    }
                    else
                    {
                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) +
                                              param.Korb * (data.Survey_RorT - param.RorT);
                    }
                    //data.Survey_ZorRMoshu = Math.Pow(data.Survey_ZorR, 2) / 1000;
                }
            }
            data.ResultReading = result;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            return result;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Tempera_Revise_K,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                ParamData pd = new ParamData();
                pd.SurveyPoint = Survey_point_Number;
                if (dt.Rows[0]["Calculate_Coeffi_G"] == null || dt.Rows[0]["Benchmark_Resist_Ratio"] == null) return null;//G和Z必须有
                pd.Gorf = ConvetToData(dt.Rows[0]["Calculate_Coeffi_G"]);
                pd.ZorR = ConvetToData(dt.Rows[0]["Benchmark_Resist_Ratio"]);

                if (dt.Rows[0]["Tempera_Revise_K"] == null)
                {
                    pd.Korb = 0;
                }
                else
                {
                    pd.Korb = ConvetToData(dt.Rows[0]["Tempera_Revise_K"]);
                    pd.RorT = ConvetToData(dt.Rows[0]["Benchmark_Resist"]);
                }
                string instype = dt.Rows[0]["Instrument_Type"].ToString();
                pd.TemperatureRead = ConvetToData(dt.Rows[0]["Temperature_Read"]);
                pd.ZeroR = ConvetToData(dt.Rows[0]["Zero_Resistance"]);
                if (instype.Contains("差阻") || (pd.TemperatureRead != 1 && pd.ZeroR > 0))//默认是振弦
                {
                    pd.InsCalcType = CalcType.DifBlock;
                }
                else 
                {
                    pd.IsPotentiometer = instype.Contains("电位器");
                }
                return pd;
            }
            catch
            {
                return null;
            }
        }
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Result_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("loadReading");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            if (Config.IsAuto) dt.Columns.Add("RecordMethod");
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
                    dr["Temperature"] = Math.Round(surveydata.Tempreture,2);
                    dr["loadReading"] = Math.Round(surveydata.LoadReading,4);
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        
        }
    }

    /// <summary>
    /// 滑动测微计
    /// </summary>
    public class Fiducial_Glide_Micrometer : BaseInstrument
    {
        public Fiducial_Glide_Micrometer()
        {
            base.InsType = InstrumentType.Fiducial_Glide_Micrometer;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 基岩变形计
    /// </summary>
    public class Fiducial_Basic_Rock_Distortion : BaseInstrument
    {
        public Fiducial_Basic_Rock_Distortion()
        {
            base.InsType = InstrumentType.Fiducial_Basic_Rock_Distortion;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 电磁沉降环
    /// </summary>
    public class Fiducial_Elect_Settlement_Gauge : BaseInstrument
    {
        public Fiducial_Elect_Settlement_Gauge()
        {
            base.InsType = InstrumentType.Fiducial_Elect_Settlement_Gauge;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 水管式沉降仪
    /// </summary>
    public class Fiducial_Hose_Settlement_Gauge : BaseInstrument
    {
        public Fiducial_Hose_Settlement_Gauge()
        {
            base.InsType = InstrumentType.Fiducial_Hose_Settlement_Gauge;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 引张线式水平位移计
    /// </summary>
    public class Fiducial_Flex_Displacement : BaseInstrument
    {
        public Fiducial_Flex_Displacement()
        {
            base.InsType = InstrumentType.Fiducial_Flex_Displacement;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 土体位移计
    /// </summary>
    public class Fiducial_Soil_Displacement : BaseInstrument
    {
        public Fiducial_Soil_Displacement()
        {
            base.InsType = InstrumentType.Fiducial_Soil_Displacement;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 测缝计
    /// </summary>
    public class Fiducial_Measure_Aperture : BaseInstrument
    {
        public Fiducial_Measure_Aperture()
        {
            base.InsType = InstrumentType.Fiducial_Measure_Aperture;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            double result = 0;
            if (CheckIsMoshu(param.Gorf))
            {
                result = (param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT));
                //data.Survey_ZorRMoshu = data.Survey_ZorR;
            }
            else
            {
                //录入的是频率或者模数
                if (Config.IsMoshu)
                {
                    result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000.0 - param.ZorR) +
                        param.Korb * (data.Survey_RorT - param.RorT);
                }
                else
                {
                    result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) +
                                          param.Korb * (data.Survey_RorT - param.RorT);
                }
                //data.Survey_ZorRMoshu = Math.Pow(data.Survey_ZorR, 2) / 1000;
            }
     
            data.ResultReading = result;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            return result;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Result_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("loadReading");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            if (Config.IsAuto) dt.Columns.Add("RecordMethod");
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
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
    }

    /// <summary>
    /// 渗压计
    /// </summary>
    public class Fiducial_Leakage_Pressure : BaseInstrument
    {
        public Fiducial_Leakage_Pressure():base()
        {
            base.InsType = InstrumentType.Fiducial_Leakage_Pressure;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {

            double result = (param.Gorf * (data.Survey_ZorR - param.ZorR) + 
                param.Korb * (param.TemperatureRead * (data.Survey_RorT - param.ZeroR) -
                param.TemperatureRead * (param.RorT - param.ZeroR))) *param.KpaToMpa;

            data.ResultReading = result * 102.0408 ;//最终结果水头ResultReading  Kpa
            data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);//温度
            data.LoadReading = result;//渗透压力LoadReading
            return result;
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            double result = 0;
            //录入的是频率或者模数
            if (!param.Special)
            {
                if (Math.Abs(param.Gorf * 10000) > 1)
                {
                    result = (param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT));
                    // data.Survey_ZorRMoshu = data.Survey_ZorR;
                }
                else
                {
                    if (Config.IsMoshu)
                    {
                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000.0 - param.ZorR) +
                            param.Korb * (data.Survey_RorT - param.RorT);
                    }
                    else
                    {
                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) +
                                              param.Korb * (data.Survey_RorT - param.RorT);
                    }
                }
                if (Math.Abs(param.Gorf * 100) > 1) param.KpaToMpa = 0.001;
            }
            else
            {
                if (param.IsMoshu)
                {
                    result = (param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT));
                }
                else
                {
                    result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) +
                                               param.Korb * (data.Survey_RorT - param.RorT);
                }
            }
            result += param.Constant_b;
            result = result * param.KpaToMpa;
            data.LoadReading = result;
            data.ResultReading = result * param.WaterHead_Coeffi_C;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            return result;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename=null)
        {

            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Tempera_Revise_K,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance,WaterHead_Coeffi_C from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                ParamData pd = new ParamData();
                pd.SurveyPoint = Survey_point_Number;
                if (dt.Rows[0]["Calculate_Coeffi_G"] == null || dt.Rows[0]["Benchmark_Resist_Ratio"] == null) return null;//G和Z必须有
                pd.Gorf = ConvetToData(dt.Rows[0]["Calculate_Coeffi_G"]);
                pd.WaterHead_Coeffi_C = ConvetToData(dt.Rows[0]["WaterHead_Coeffi_C"]);
                pd.ZorR = ConvetToData(dt.Rows[0]["Benchmark_Resist_Ratio"]);
                if (dt.Rows[0]["Tempera_Revise_K"] == null)
                {
                    pd.Korb = 0;
                }
                else
                {
                    pd.Korb = ConvetToData(dt.Rows[0]["Tempera_Revise_K"]);
                    pd.RorT = ConvetToData(dt.Rows[0]["Benchmark_Resist"]);
                }
                string instype = dt.Rows[0]["Instrument_Type"].ToString();
                pd.TemperatureRead = ConvetToData(dt.Rows[0]["Temperature_Read"]);
                pd.ZeroR = ConvetToData(dt.Rows[0]["Zero_Resistance"]);
                if (instype.Contains("差阻") || (pd.TemperatureRead != 1 && pd.ZeroR > 0))//默认是振弦
                {
                    pd.InsCalcType = CalcType.DifBlock;
                }                
                return pd;
            }
            catch
            {
                return null;
            }
        }

    }

    /// <summary>
    /// 测压管
    /// </summary>
    public class Fiducial_MeasureStress_Hose : BaseInstrument
    {
        public Fiducial_MeasureStress_Hose()
        {
            base.InsType = InstrumentType.Fiducial_MeasureStress_Hose;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 量水堰
    /// </summary>
    public class Fiducial_MeasureWater_Weir : BaseInstrument
    {
        public Fiducial_MeasureWater_Weir()
        {
            base.InsType = InstrumentType.Fiducial_MeasureWater_Weir;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 水位计
    /// </summary>
    public class Fiducial_Water_Level : BaseInstrument
    {
        public Fiducial_Water_Level()
        {
            base.InsType = InstrumentType.Fiducial_Water_Level;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 应变计
    /// </summary>
    public class Fiducial_Strain_Gauge : BaseInstrument
    {
        public Fiducial_Strain_Gauge()
        {
            base.InsType = InstrumentType.Fiducial_Strain_Gauge;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            double result = 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) +
                    (param.Korb - param.Concrete_Expansion_ac) * (param.TemperatureRead * (data.Survey_RorT - param.ZeroR) - param.TemperatureRead * (param.RorT - param.ZeroR));
            }
            if (param.ZeroR > 1)
            {
                data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);
            }
            else
            {
                data.Tempreture = 0;
            }
            data.ResultReading = result;
            data.LoadReading = result;
            if (data.NonStressSurveyData != null)
            {
                data.LoadReading = data.LoadReading - data.NonStressSurveyData.LoadReading;
            }

            return result;
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            double result = 0;
            double Tcorrect = (data.Survey_RorT != 0) ?(param.Korb - param.Concrete_Expansion_ac) * (data.Survey_RorT - param.RorT) : 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 5000)
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + Tcorrect;
                    //data.Survey_ZorRMoshu = data.Survey_ZorR;
                }
                else
                {
                    if (Config.IsMoshu)
                    {

                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000 - param.ZorR) + Tcorrect;
                    }
                    else
                    {
                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) + Tcorrect;
                    }
                    //data.Survey_ZorRMoshu = Math.Pow(data.Survey_ZorR, 2) / 1000;
                }
            }
            data.ResultReading = result;
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            if (param.IsHasNonStress&&data.NonStressSurveyData != null)
            {
                data.LoadReading = data.LoadReading - data.NonStressSurveyData.LoadReading;
            }
            return result;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        /// <summary>从数据库读取参数
        /// </summary>
        /// <param name="Survey_point_Number">测点名称</param>
        /// <param name="表名"></param>
        /// <returns></returns>
        public override ParamData GetParam(string Survey_point_Number, string tablename)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Instru_Expansion_b,Concrete_Expansion_ac,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance,Nonstress_Number from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename,Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                ParamData pd = new ParamData();
                pd.SurveyPoint = Survey_point_Number;
                if (dt.Rows[0]["Calculate_Coeffi_G"] == null || dt.Rows[0]["Benchmark_Resist_Ratio"] == null) return null;//G和Z必须有
                pd.Gorf =ConvetToData(dt.Rows[0]["Calculate_Coeffi_G"]);
                pd.ZorR =ConvetToData(dt.Rows[0]["Benchmark_Resist_Ratio"]);
                pd.Concrete_Expansion_ac =ConvetToData(dt.Rows[0]["Concrete_Expansion_ac"]);
                pd.NonStressNumber = dt.Rows[0]["Nonstress_Number"].ToString();
                pd.IsHasNonStress = !String.IsNullOrEmpty(pd.NonStressNumber.Trim());
                if (dt.Rows[0]["Instru_Expansion_b"] == null)
                {
                    pd.Korb = 0;
                }
                else
                {
                    pd.Korb =ConvetToData(dt.Rows[0]["Instru_Expansion_b"]);
                    pd.RorT =ConvetToData(dt.Rows[0]["Benchmark_Resist"]);
                }
                string instype = dt.Rows[0]["Instrument_Type"].ToString();
                pd.TemperatureRead =ConvetToData(dt.Rows[0]["Temperature_Read"]);
                pd.ZeroR =ConvetToData(dt.Rows[0]["Zero_Resistance"]);
                if (instype.Contains("差阻") || (pd.TemperatureRead != 1 && pd.ZeroR > 0))//默认是振弦
                {
                    pd.InsCalcType = CalcType.DifBlock;
                }
                var calcEx = CalcExpandList.Where(ce => ce.Survey_point_Number.ToUpper().Trim() == Survey_point_Number.ToUpper().Trim()).FirstOrDefault();
                if (calcEx != null) pd.InsCalcType = calcEx.Calc_Type;
                return pd;
            }
            catch
            {
                return null;
            }
        }
        public override double Expand1(ParamData param, SurveyData data, string expression)
        {
            double result=0;
            result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) + param.Korb;
            data.ResultReading = result;
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            return result;
        }

        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("Frequency");
            dt.Columns.Add("Non_Temperature");
            dt.Columns.Add("Non_Frequency");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            dt.Columns.Add("RecordMethod");
            var sqlhelper = CSqlServerHelper.GetInstance();
            var sid = sqlhelper.SelectFirst("select max(ID) as sid  from " + TableName);
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
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString(@"hh\:mm\:ss");
                    dr["Temperature"] = Math.Round(surveydata.Survey_RorT, 4);
                    dr["Frequency"] = Math.Round(surveydata.Survey_ZorR, 4);
                    if (surveydata.NonStressSurveyData != null)
                    {
                        dr["Non_Temperature"] = Math.Round(surveydata.NonStressSurveyData.Tempreture, 4);
                        dr["Non_Frequency"] = Math.Round(surveydata.NonStressSurveyData.LoadReading, 4);
                    }
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Result_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("loadReading");
            dt.Columns.Add("Non_Temperature");
            dt.Columns.Add("Non_loadReading");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            dt.Columns.Add("RecordMethod");
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
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString(@"hh\:mm\:ss");
                    dr["Temperature"] = Math.Round(surveydata.Tempreture, 2);
                    dr["loadReading"] = Math.Round(surveydata.LoadReading, 4);
                    if (surveydata.NonStressSurveyData != null)
                    {
                        dr["Non_Temperature"] = Math.Round(surveydata.NonStressSurveyData.Tempreture, 4);
                        dr["Non_loadReading"] = Math.Round(surveydata.NonStressSurveyData.LoadReading, 4);
                    }
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }


    }

    /// <summary>
    /// 无应力计
    /// </summary>
    public class Fiducial_Nonstress : BaseInstrument
    {
        public Fiducial_Nonstress()
        {
            base.InsType = InstrumentType.Fiducial_Nonstress;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            double result = 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) +
                    (param.Korb - param.Concrete_Expansion_ac) * (param.TemperatureRead * (data.Survey_RorT - param.ZeroR) - param.TemperatureRead * (param.RorT - param.ZeroR));
            }
            data.ResultReading = result;
            data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);
            data.LoadReading = result;
            return result;
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            double result = 0;
            double Tcorrect = (data.Survey_RorT != 0) ? param.Korb * (data.Survey_RorT - param.RorT) : 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 5000)
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
                    //data.Survey_ZorRMoshu = data.Survey_ZorR;
                }
                else
                {
                    if (Config.IsMoshu)
                    {
                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000.0 - param.ZorR) + Tcorrect;
                    }
                    else
                    {
                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) + Tcorrect;
                    }
                }
            }
            data.ResultReading = result;
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            return result;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        /// <summary>从数据库读取参数
        /// </summary>
        /// <param name="Survey_point_Number">测点名称</param>
        /// <param name="表名"></param>
        /// <returns></returns>
        public override ParamData GetParam(string Survey_point_Number, string tablename)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Instru_Expansion_b,Concrete_Expansion_ac,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance  from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, "Fiducial_Nonstress", Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                ParamData pd = new ParamData();
                pd.SurveyPoint = Survey_point_Number;
                if (dt.Rows[0]["Calculate_Coeffi_G"] == null || dt.Rows[0]["Benchmark_Resist_Ratio"] == null) return null;//G和Z必须有
                pd.Gorf =ConvetToData(dt.Rows[0]["Calculate_Coeffi_G"]);
                pd.ZorR =ConvetToData(dt.Rows[0]["Benchmark_Resist_Ratio"]);
                pd.Concrete_Expansion_ac =ConvetToData(dt.Rows[0]["Concrete_Expansion_ac"]);
                if (dt.Rows[0]["Instru_Expansion_b"] == null)
                {
                    pd.Korb = 0;
                }
                else
                {
                    pd.Korb =ConvetToData(dt.Rows[0]["Instru_Expansion_b"]);
                    pd.RorT =ConvetToData(dt.Rows[0]["Benchmark_Resist"]);
                }
                string instype = dt.Rows[0]["Instrument_Type"].ToString();
                pd.TemperatureRead =ConvetToData(dt.Rows[0]["Temperature_Read"]);
                pd.ZeroR =ConvetToData(dt.Rows[0]["Zero_Resistance"]);
                if (instype.Contains("差阻") || (pd.TemperatureRead != 1 && pd.ZeroR > 0))//默认是振弦
                {
                    pd.InsCalcType = CalcType.DifBlock;
                }
                return pd;
            }
            catch
            {
                return null;
            }
        }
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Result_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("loadReading");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            if (Config.IsAuto) dt.Columns.Add("RecordMethod");
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
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString(@"hh\:mm\:ss");
                    dr["Temperature"] = Math.Round(surveydata.Tempreture, 2);
                    dr["loadReading"] = Math.Round(surveydata.LoadReading, 4);
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
    }

    /// <summary>
    /// 钢筋计
    /// </summary>
    public class Fiducial_Steel_Bar : BaseInstrument
    {
        public Fiducial_Steel_Bar()
        {
            base.InsType = InstrumentType.Fiducial_Steel_Bar;

        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(TemperatureRead*(Survey_RorT-ZeroR)-RorT)
            double result = 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) +
                    param.Korb * (param.TemperatureRead * (data.Survey_RorT - param.ZeroR) - param.TemperatureRead * (param.RorT - param.ZeroR));
            }
            result += param.Constant_b;
            data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);
            data.LoadReading = result;
            data.ResultReading =  result *param.KpaToMpa;
            return result;
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            double result = 0;
            double Tcorrect = (data.Survey_RorT != 0) ? param.Korb * (data.Survey_RorT - param.RorT) : 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 3000)//模数
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + Tcorrect;
                    //data.Survey_ZorRMoshu = data.Survey_ZorR;
                }
                else//频率
                {
                    //录入的是频率或者模数
                    if (Config.IsMoshu)
                    {

                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000.0 - param.ZorR) + Tcorrect;
                    }
                    else
                    {
                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) + Tcorrect;
                    }
                    //data.Survey_ZorRMoshu = Math.Pow(data.Survey_ZorR, 2) / 1000;
                }
            }
            result += param.Constant_b;
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            data.ResultReading = result * 1000 / (Math.PI * Math.Pow(param.Steel_Diameter_L / 2.0, 2));

            return result;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Tempera_Revise_K,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance, Steel_Diameter_L from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                ParamData pd = new ParamData();
                pd.SurveyPoint = Survey_point_Number;
                if (dt.Rows[0]["Calculate_Coeffi_G"] == null || dt.Rows[0]["Benchmark_Resist_Ratio"] == null) return null;//G和Z必须有
                pd.Gorf = ConvetToData(dt.Rows[0]["Calculate_Coeffi_G"]);
                pd.ZorR = ConvetToData(dt.Rows[0]["Benchmark_Resist_Ratio"]);

                if (dt.Rows[0]["Tempera_Revise_K"] == null)
                {
                    pd.Korb = 0;
                }
                else
                {
                    pd.Korb = ConvetToData(dt.Rows[0]["Tempera_Revise_K"]);
                    pd.RorT = ConvetToData(dt.Rows[0]["Benchmark_Resist"]);
                }
                string instype = dt.Rows[0]["Instrument_Type"].ToString();
                pd.TemperatureRead = ConvetToData(dt.Rows[0]["Temperature_Read"]);
                pd.ZeroR = ConvetToData(dt.Rows[0]["Zero_Resistance"]);
                pd.Steel_Diameter_L = ConvetToData(dt.Rows[0]["Steel_Diameter_L"]);
                if (instype.Contains("差阻") || (pd.TemperatureRead != 1 && pd.ZeroR > 0))//默认是振弦
                {
                    pd.InsCalcType = CalcType.DifBlock;
                }
                return pd;
            }
            catch
            {
                return null;
            }
        }
    
    }

    /// <summary>
    /// 锚杆应力计
    /// </summary>
    public class Fiducial_Anchor_Pole : BaseInstrument
    {
        public Fiducial_Anchor_Pole()
        {
            base.InsType = InstrumentType.Fiducial_Anchor_Pole;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {

            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            double result = 0;
            double Tcorrect = (data.Survey_RorT != 0) ? param.Korb * (data.Survey_RorT - param.RorT) : 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) + Tcorrect;
                //data.Survey_ZorRMoshu = data.Survey_ZorR;
            }
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            data.ResultReading = result * 1000 / (Math.PI * Math.Pow(param.Steel_Diameter_L / 2.0, 2));

            return result;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Tempera_Revise_K,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance,Steel_Diameter_L from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                ParamData pd = new ParamData();
                pd.SurveyPoint = Survey_point_Number;
                if (dt.Rows[0]["Calculate_Coeffi_G"] == null || dt.Rows[0]["Benchmark_Resist_Ratio"] == null) return null;//G和Z必须有
                pd.Gorf = ConvetToData(dt.Rows[0]["Calculate_Coeffi_G"]);
                pd.ZorR = ConvetToData(dt.Rows[0]["Benchmark_Resist_Ratio"]);

                if (dt.Rows[0]["Tempera_Revise_K"] == null)
                {
                    pd.Korb = 0;
                }
                else
                {
                    pd.Korb = ConvetToData(dt.Rows[0]["Tempera_Revise_K"]);
                    pd.RorT = ConvetToData(dt.Rows[0]["Benchmark_Resist"]);
                }
                string instype = dt.Rows[0]["Instrument_Type"].ToString();
                pd.TemperatureRead = ConvetToData(dt.Rows[0]["Temperature_Read"]);
                pd.ZeroR = ConvetToData(dt.Rows[0]["Zero_Resistance"]);
                pd.Steel_Diameter_L = ConvetToData(dt.Rows[0]["Steel_Diameter_L"]);
                if (instype.Contains("差阻") || (pd.TemperatureRead != 1 && pd.ZeroR > 0))//默认是振弦
                {
                    pd.InsCalcType = CalcType.DifBlock;
                }
                return pd;
            }
            catch
            {
                return null;
            }
        }

        /// <summary> 写测值数据
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            return base.WriteSurveyToDB(datas);
        }

        /// <summary>写结果数据
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Result_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("loadReading");
            dt.Columns.Add("ResultReading");
            dt.Columns.Add("AfterLock");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            if (Config.IsAuto) dt.Columns.Add("RecordMethod");
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
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString(@"hh\:mm\:ss");
                    dr["Temperature"] = Math.Round(surveydata.Tempreture, 2);
                    dr["loadReading"] = Math.Round(surveydata.LoadReading, 4);
                    dr["ResultReading"] = Math.Round(surveydata.ResultReading, 4);
                    dr["AfterLock"] = surveydata.AfterLock;
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }


    }

    /// <summary>
    /// 压应力计
    /// </summary>
    public class Fiducial_Press_Stress : BaseInstrument
    {
        public Fiducial_Press_Stress()
        {
            base.InsType = InstrumentType.Fiducial_Press_Stress;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            return base.WriteSurveyToDB(datas);
        }
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            return base.WriteResultToDB(datas);
        }

    }

    /// <summary>土压力计
    /// </summary>
    public class Fiducial_Soil_Stres : BaseInstrument
    {
        public Fiducial_Soil_Stres()
        {
            base.InsType = InstrumentType.Fiducial_Soil_Stres;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            ParamData pd = base.GetParam(Survey_point_Number, this.InsType.ToString());
            var calcEx = CalcExpandList.Where(ce => ce.Survey_point_Number.ToUpper().Trim() == Survey_point_Number.ToUpper().Trim()).FirstOrDefault();
            if (calcEx != null) pd.InsCalcType = calcEx.Calc_Type;
            return pd;
        }
        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            return base.WriteSurveyToDB(datas);
        }
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Result_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("loadReading");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            if (Config.IsAuto) dt.Columns.Add("RecordMethod");
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
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString(@"hh\:mm\:ss");
                    dr["Temperature"] = Math.Round(surveydata.Tempreture, 2);
                    dr["loadReading"] = Math.Round(surveydata.LoadReading, 4);
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
        /// <summary>
        /// 特殊计算方法P=K(fn2-f02)+A，A对应考证表中的第二个参数，K对应G
        /// </summary>
        /// <param name="param"></param>
        /// <param name="data"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override double Expand1(ParamData param, SurveyData data, string expression)
        {
            //P=K(fn2-f02)+A
            double result = 0;
            result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) +param.Korb ;
            //data.Survey_ZorRMoshu = Math.Pow(data.Survey_ZorR, 2) / 1000;
            data.ResultReading = result;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            return result;
        }
    }

    /// <summary>
    /// 锚索测力计
    /// </summary>
    public class Fiducial_Anchor_Cable : BaseInstrument
    {

        /// <summary>
        ///掉弦后的改正系数，如果掉弦索引不变，系数每次只计算一次
        ///换点的时候重置为1
        /// </summary>
        public double coefficient_K = 1;


        public Fiducial_Anchor_Cable()
        {
            base.InsType = InstrumentType.Fiducial_Anchor_Cable;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        /// <summary>
        /// 计算掉弦的系数
        /// </summary>
        /// <param name="SurveyPoint"></param>
        /// <returns></returns>
        public virtual double GetC(ParamData param, SurveyData data,PointSurveyData pd)
        {
            return 1;
        }
        public override double ShakeString(ParamData paramd,SurveyData data, params double[] expand)
        {
            var param = paramd as Anchor_CableParam;
            double result = 0;
            int count = 0;
            double value = 0;
            foreach (var dic in data.MultiDatas)
            {
                if (dic.Value.Survey_ZorR <= 0) continue;
                count++;
                if (dic.Value.Survey_ZorR >= 4000)
                {
                    value += dic.Value.Survey_ZorR;
                    //dic.Value.Survey_ZorRMoshu = dic.Value.Survey_ZorR;
                }
                else
                {
                    //dic.Value.Survey_ZorRMoshu = Math.Pow(dic.Value.Survey_ZorR, 2) / 1000.0;
                    value += Math.Pow(dic.Value.Survey_ZorR, 2) / 1000.0;
                }
            }
            if (count == data.MultiDatas.Keys.Count)
            {
                value = value / count;
            }
            else
            {//掉弦
                value = value / count;
            }
           
            result = param.Gorf * (value - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
            result += param.Constant_b;
            //data.Survey_ZorRMoshu = value;
            data.LoadReading = result;
            data.AfterLock = ((param.Lock_Value - result) / param.Lock_Value) * 100;
            data.PlanLock = ((param.Plan_Loading - result) / param.Plan_Loading) * 100;
            data.ResultLock = ((param.MAX_Loading - result) / param.MAX_Loading) * 100;
            
            data.ResultReading = result;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;

            return result;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Tempera_Revise_K,Benchmark_Resist_Ratio, Benchmark_Resist,Temperature_Read,Zero_Resistance,Plan_Loading,MAX_Loading,Lock_Value,Read_GroupNum from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                Anchor_CableParam pd = new Anchor_CableParam();
                pd.SurveyPoint = Survey_point_Number;
                if (dt.Rows[0]["Calculate_Coeffi_G"] == null || dt.Rows[0]["Benchmark_Resist_Ratio"] == null) return null;//G和Z必须有
                pd.Gorf =ConvetToData(dt.Rows[0]["Calculate_Coeffi_G"]);
                pd.ZorR =ConvetToData(dt.Rows[0]["Benchmark_Resist_Ratio"]);
                pd.Plan_Loading =ConvetToData(dt.Rows[0]["Plan_Loading"]);
                pd.MAX_Loading =ConvetToData(dt.Rows[0]["MAX_Loading"]);
                pd.Lock_Value =ConvetToData(dt.Rows[0]["Lock_Value"]);
                pd.Sum = Convert.ToInt16(dt.Rows[0]["Read_GroupNum"]);
                if (dt.Rows[0]["Tempera_Revise_K"] == null)
                {
                    pd.Korb = 0;
                }
                else
                {
                    pd.Korb =ConvetToData(dt.Rows[0]["Tempera_Revise_K"]);
                    pd.RorT =ConvetToData(dt.Rows[0]["Benchmark_Resist"]);
                }
                string instype = dt.Rows[0]["Instrument_Type"].ToString();
                pd.TemperatureRead =ConvetToData(dt.Rows[0]["Temperature_Read"]);
                pd.ZeroR =ConvetToData(dt.Rows[0]["Zero_Resistance"]);
                if (instype.Contains("差阻") || (pd.TemperatureRead != 1 && pd.ZeroR > 0))//默认是振弦
                {
                    pd.InsCalcType = CalcType.DifBlock;
                }
                return pd;
            }
            catch
            {
                return null;
            }
        }

        /// <summary> 写测值数据
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string[] list = new string[] { "Reading_Red", "Reading_Black", "Reading_Yellow", "Reading_Blue", "Reading_Ash", "Reading_Purple" };
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("Average");
            foreach (string s in list)
            {
                dt.Columns.Add(s);
            }
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            if (Config.IsAuto) dt.Columns.Add("RecordMethod");
            var sqlhelper = CSqlServerHelper.GetInstance();
            var sid = sqlhelper.SelectFirst("select max(ID) as sid  from " + TableName);
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
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString(@"hh\:mm\:ss");
                    dr["Temperature"] = Math.Round(surveydata.Survey_RorT, 2);
                    dr["Average"] = Math.Round(surveydata.Survey_ZorR, 4);
                    int index = 0;
                    foreach (var dic in surveydata.MultiDatas)
                    {
                        dr[list[index]] = Math.Round(dic.Value.Survey_ZorR, 4);
                        index++;
                    }
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
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
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Result_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("loadReading");
            dt.Columns.Add("AfterLock");
            dt.Columns.Add("PlanLock");
            dt.Columns.Add("ResultLock");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            if (Config.IsAuto) dt.Columns.Add("RecordMethod");
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
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString(@"hh\:mm\:ss");
                    dr["Temperature"] = Convert.ToDecimal(surveydata.Tempreture);
                    dr["loadReading"] = Convert.ToDecimal(surveydata.LoadReading);
                    dr["AfterLock"] = Convert.ToDecimal(surveydata.AfterLock);
                    dr["PlanLock"] = Convert.ToDecimal(surveydata.PlanLock);
                    dr["ResultLock"] = Convert.ToDecimal(surveydata.ResultLock);
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }

    }

    /// <summary>
    /// 钢板计
    /// </summary>
    public class Fiducial_Armor_plate : BaseInstrument
    {
        public Fiducial_Armor_plate()
        {
            base.InsType = InstrumentType.Fiducial_Armor_plate;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(TemperatureRead*(Survey_RorT-ZeroR)-RorT)
            double result = param.Gorf * (data.Survey_ZorR - param.ZorR) +
                param.Korb * (param.TemperatureRead * (data.Survey_RorT - param.ZeroR) - param.TemperatureRead * (param.RorT - param.ZeroR));
            data.ResultReading = result * param.Elastic_Modulus_E;//这里要乘以系数
            data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);
            data.LoadReading = result * param.Elastic_Modulus_E;
            return result;
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            double result = 0;
            if (Config.IsMoshu)
            {
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
            }
            else
            {
                result = param.Gorf * (data.Survey_ZorR * data.Survey_ZorR - param.ZorR * param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
            }

  
            data.ResultReading = result*param.Elastic_Modulus_E;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result * param.Elastic_Modulus_E;
            return result;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            ParamData pd = new ParamData();
            pd=base.GetParam(Survey_point_Number, this.InsType.ToString());
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql = String.Format("select Elastic_Modulus_E from Fiducial_Armor_plate where Survey_point_Number='{0}'", 
                Survey_point_Number);
            var result = sqlhelper.SelectFirst(sql);
            if(result!=DBNull.Value)pd.Elastic_Modulus_E=Convert.ToDouble(result);

            return pd;
        }
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Result_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("loadReading");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            if (Config.IsAuto) dt.Columns.Add("RecordMethod");
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
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }

    }

    /// <summary>
    /// 温度计
    /// </summary>
    public class Fiducial_Temperature : BaseInstrument
    {
        public Fiducial_Temperature()
        {
            base.InsType = InstrumentType.Fiducial_Temperature;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            double result = 0;
            if (param.TemperatureRead != 0 && data.Survey_ZorR!=0)
            {
                result = param.TemperatureRead * (data.Survey_ZorR - param.ZeroR);
                data.LoadReading = result;
                data.Tempreture = result;
            }
            else
            {
                result = data.Survey_RorT;
                data.LoadReading = result;
                data.Tempreture = result;
            }
           
            return result;
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            double result = 0;
            if (param.TemperatureRead != 0 && data.Survey_ZorR != 0)
            {
                result = param.TemperatureRead * (data.Survey_ZorR - param.ZeroR);
                data.LoadReading = result;
                data.Tempreture = result;
            }
            else
            {
                result = data.Survey_RorT;
                data.LoadReading = result;
                data.Tempreture = result;
            }
            return result;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            string sql = @"select Instrument_Type,Temperature_Read,Zero_Resistance from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                ParamData pd = new ParamData();
                pd.SurveyPoint = Survey_point_Number;
                string instype = dt.Rows[0]["Instrument_Type"].ToString();
                pd.TemperatureRead = ConvetToData(dt.Rows[0]["Temperature_Read"]);
                pd.ZeroR = ConvetToData(dt.Rows[0]["Zero_Resistance"]);
                if (instype.Contains("电阻") || (pd.TemperatureRead != 1 && pd.ZeroR > 0))//默认是振弦
                {
                    pd.InsCalcType = CalcType.DifBlock;
                }
                return pd;
            }
            catch
            {
                return null;
            }
        }
        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            return base.WriteSurveyToDB(datas);
        }
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Result_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("loadReading");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            if (Config.IsAuto) dt.Columns.Add("RecordMethod");
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
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString(@"hh\:mm\:ss");
                    dr["loadReading"] = Math.Round(surveydata.LoadReading,4);
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }

    }

    /// <summary>
    /// 裂缝计
    /// </summary>
    public class Fiducial_Aperture : BaseInstrument
    {
        public Fiducial_Aperture()
        {
            base.InsType = InstrumentType.Fiducial_Aperture;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 竖直传高
    /// </summary>
    public class Fiducial_Vertical_Height : BaseInstrument
    {
        public Fiducial_Vertical_Height()
        {
            base.InsType = InstrumentType.Fiducial_Vertical_Height;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 日平均气温
    /// </summary>
    public class Fiducial_Environment_Temperature : BaseInstrument
    {
        public Fiducial_Environment_Temperature()
        {
            base.InsType = InstrumentType.Fiducial_Environment_Temperature;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    /// <summary>
    /// 上游水位
    /// </summary>
    public class Fiducial_Environment_UW_level : BaseInstrument
    {
        public Fiducial_Environment_UW_level()
        {
            base.InsType = InstrumentType.Fiducial_Environment_UW_level;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    /// <summary>
    /// 日降雨量
    /// </summary>
    public class Fiducial_Environment_Rainfall : BaseInstrument
    {
        public Fiducial_Environment_Rainfall()
        {
            base.InsType = InstrumentType.Fiducial_Environment_Rainfall;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 姊妹杆
    /// </summary>
    public class Fiducial_Sisters_Pole : BaseInstrument
    {
        public Fiducial_Sisters_Pole()
        {
            base.InsType = InstrumentType.Fiducial_Sisters_Pole;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    /// <summary>
    /// 位错计
    /// </summary>
    public class Fiducial_Dislocation : BaseInstrument
    {
        public Fiducial_Dislocation()
        {
            base.InsType = InstrumentType.Fiducial_Dislocation;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    

    /// <summary>
    /// 表面测缝计
    /// </summary>
    public class Fiducial_Surface_Measure_Aperture : BaseInstrument
    {
        public Fiducial_Surface_Measure_Aperture()
        {
            base.InsType = InstrumentType.Fiducial_Surface_Measure_Aperture;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    /// <summary>
    /// 弦式沉降仪
    /// </summary>
    public class Fiducial_VST_Settlement_Gauge : BaseInstrument
    {
        public Fiducial_VST_Settlement_Gauge()
        {
            base.InsType = InstrumentType.Fiducial_VST_Settlement_Gauge;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    /// <summary>
    /// 多向土压力计
    /// </summary>
    public class Fiducial_Multi_Soil_Stres : BaseInstrument
    {
        public Fiducial_Multi_Soil_Stres()
        {
            base.InsType = InstrumentType.Fiducial_Multi_Soil_Stres;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    /// <summary>
    /// 双向测缝计
    /// </summary>
    public class Fiducial_TwoWays_Measure_Aperture : BaseInstrument
    {
        public Fiducial_TwoWays_Measure_Aperture()
        {
            base.InsType = InstrumentType.Fiducial_TwoWays_Measure_Aperture;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    /// <summary>
    /// 横梁式沉降仪
    /// </summary>
    public class Fiducial_CBT_Settlement_Gauge : BaseInstrument
    {
        public Fiducial_CBT_Settlement_Gauge()
        {
            base.InsType = InstrumentType.Fiducial_CBT_Settlement_Gauge;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    /// <summary>
    /// 剪变形计
    /// </summary>
    public class Fiducial_Shear_Deformation_Meter : BaseInstrument
    {
        public Fiducial_Shear_Deformation_Meter()
        {
            base.InsType = InstrumentType.Fiducial_Shear_Deformation_Meter;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    /// <summary>
    /// 弦式沉降系统
    /// </summary>
    public class Fiducial_VST_Settlement_System : BaseInstrument
    {
        public Fiducial_VST_Settlement_System()
        {
            base.InsType = InstrumentType.Fiducial_VST_Settlement_System;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    /// <summary>
    /// 土体位移计组
    /// </summary>
    public class Fiducial_Soil_Displacement_Group : BaseInstrument
    {
        public Fiducial_Soil_Displacement_Group()
        {
            base.InsType = InstrumentType.Fiducial_Soil_Displacement_Group;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    /// <summary>
    /// 固定测斜仪
    /// </summary>
    public class Fiducial_Survey_Slant_Fixed : BaseInstrument
    {
        public Fiducial_Survey_Slant_Fixed()
        {
            base.InsType = InstrumentType.Fiducial_Survey_Slant_Fixed;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData sdata, params double[] expand)
        {
            var data = sdata as Survey_Slant_FixedSurveyData;
            var fparam=param as Survey_Slant_FixedParam;
            double result = fparam.A_C0 * (fparam.A_US * (data.Reading_A - fparam.A_DS) - fparam.A_US * (fparam.A_BS - fparam.A_DS));
            data.loadReading_A = data.ResultReading = result;
            return result;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
        //    public double A_US;
        //public double A_DS;
        //public double A_BS;
        //public double A_C0;
        //public double A_C1;
        //public double A_C2;
        //public double A_C3;
        //public double A_F0;
        //public double A_F1;
        //public double B_US;
        //public double B_DS;
        //public double B_BS;
        //public double B_C0;
        //public double B_C1;
        //public double B_C2;
        //public double B_C3;
        //public double B_F0;
        //public double AB_F1;
        //public double T_US;
        //public double T_DS;
        //public double T_BS;

            string sql = @"select Instrument_Type,A_US,A_DS,A_BS,A_C0 from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                Survey_Slant_FixedParam pd = new Survey_Slant_FixedParam();
                pd.SurveyPoint = Survey_point_Number;
                pd.A_US = ConvetToData(dt.Rows[0]["A_US"]);
                pd.A_DS = ConvetToData(dt.Rows[0]["A_DS"]);
                pd.A_BS = ConvetToData(dt.Rows[0]["A_BS"]);
                pd.A_C0 = ConvetToData(dt.Rows[0]["A_C0"]);
                string instype = dt.Rows[0]["Instrument_Type"].ToString();
                if (instype.Contains("差阻") || (pd.TemperatureRead != 1 && pd.ZeroR > 0))//默认是振弦
                {
                    pd.InsCalcType = CalcType.DifBlock;
                }
                return pd;
            }
            catch
            {
                return null;
            }
        }
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Result_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("loadReading_A");
            dt.Columns.Add("loadReading_B");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            var sqlhelper = CSqlServerHelper.GetInstance();
            var sid = sqlhelper.SelectFirst("select max(ID) as sid  from  " + TableName);
            int id = sid == DBNull.Value ? 0 : Convert.ToInt32(sid);
            foreach (PointSurveyData pd in datas)
            {
                foreach (var sd in pd.Datas)
                {
                    var surveydata = sd as Survey_Slant_FixedSurveyData;
                    id++;
                    DataRow dr = dt.NewRow();
                    dr["ID"] = id;
                    dr["Survey_point_Number"] = pd.SurveyPoint;
                    dr["Observation_Date"] = surveydata.SurveyDate;
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString();
                    dr["Temperature"] = Math.Round(surveydata.Tempreture, 2);
                    dr["loadReading_A"] = Math.Round(surveydata.loadReading_A, 4);
                    dr["loadReading_B"] = Math.Round(surveydata.loadReading_B, 4);
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            dt.Columns.Add("Temperature");
            dt.Columns.Add("Reading_A");
            dt.Columns.Add("Reading_B");
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            var sqlhelper = CSqlServerHelper.GetInstance();
            var sid = sqlhelper.SelectFirst("select max(ID) as sid  from " + TableName);
            int id = sid == DBNull.Value ? 0 : Convert.ToInt32(sid);
            foreach (PointSurveyData pd in datas)
            {
                foreach (var sd in pd.Datas)
                {
                    var surveydata = sd as Survey_Slant_FixedSurveyData;
                    id++;
                    DataRow dr = dt.NewRow();
                    dr["ID"] = id;
                    dr["Survey_point_Number"] = pd.SurveyPoint;
                    dr["Observation_Date"] = surveydata.SurveyDate;
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString();
                    dr["Temperature"] = Math.Round(surveydata.Survey_RorT, 4);
                    dr["Reading_A"] = Math.Round(surveydata.Reading_A, 4);
                    dr["Reading_B"] = Math.Round(surveydata.Reading_B, 4);
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }

    }

    /// <summary>
    /// 下游水位
    /// </summary>
    public class Fiducial_Environment_DW_level : BaseInstrument
    {
        public Fiducial_Environment_DW_level()
        {
            base.InsType = InstrumentType.Fiducial_Environment_DW_level;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return base.ShakeString(param,data,expand);
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 精密水准
    /// </summary>
    public class Fiducial_Geometry_Level : BaseInstrument
    {
        public Fiducial_Geometry_Level()
        {
            base.InsType = InstrumentType.Fiducial_Geometry_Level;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            //return base.DifBlock(param,data,expand);
            return 0;        
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            //return base.ShakeString(param,data,expand);
            return 0;   
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }
}
