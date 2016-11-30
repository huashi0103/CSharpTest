/*
 * 苗尾数据计算类,对应ProCode=MW
 * 考证表中的参数表格中是什么就录的什么，
 * 暂未使用
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LoadDataCalc
{
    /// <summary>
    /// 大地测量
    /// </summary>
    public class Fiducial_Earth_MeasureMW : Fiducial_Earth_Measure
    {
        public Fiducial_Earth_MeasureMW()
        {
            base.InsType = InstrumentType.Fiducial_Earth_Measure;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 引张线
    /// </summary>
    public class Fiducial_Flex_LineMW : Fiducial_Flex_Line
    {
        public Fiducial_Flex_LineMW()
        {
            base.InsType = InstrumentType.Fiducial_Flex_Line;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 测斜仪
    /// </summary>
    public class Fiducial_Survey_SlantMW : Fiducial_Survey_Slant
    {
        public Fiducial_Survey_SlantMW()
        {
            base.InsType = InstrumentType.Fiducial_Survey_Slant;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 单点位移计
    /// </summary>
    public class Fiducial_Single_DisplacementMW : Fiducial_Single_Displacement
    {
        public Fiducial_Single_DisplacementMW()
        {
            base.InsType = InstrumentType.Fiducial_Single_Displacement;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
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
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            if (Config.ProCode == "XJB" && param.SurveyPoint.StartsWith("ID-"))//特殊的计算方式
            {
                data.LoadReading = calcOnePointExpand(param, data);
                data.Tempreture = data.Survey_RorT;
                return data.LoadReading;
            }

            double result = 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 5000 || param.IsPotentiometer)
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
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
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
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString();
                    dr["Temperature"] = Math.Round(surveydata.Tempreture, 2);
                    dr["loadReading"] = Math.Round(surveydata.LoadReading, 4);
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
    /// 基岩变形计
    /// </summary>
    public class Fiducial_Basic_Rock_DistortionMW : Fiducial_Basic_Rock_Distortion
    {
        public Fiducial_Basic_Rock_DistortionMW()
        {
            base.InsType = InstrumentType.Fiducial_Basic_Rock_Distortion;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 电磁沉降环
    /// </summary>
    public class Fiducial_Elect_Settlement_GaugeMW : Fiducial_Elect_Settlement_Gauge
    {
        public Fiducial_Elect_Settlement_GaugeMW()
        {
            base.InsType = InstrumentType.Fiducial_Elect_Settlement_Gauge;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 水管式沉降仪
    /// </summary>
    public class Fiducial_Hose_Settlement_GaugeMW : Fiducial_Hose_Settlement_Gauge
    {
        public Fiducial_Hose_Settlement_GaugeMW()
        {
            base.InsType = InstrumentType.Fiducial_Hose_Settlement_Gauge;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 引张线式水平位移计
    /// </summary>
    public class Fiducial_Flex_DisplacementMW : Fiducial_Flex_Displacement
    {
        public Fiducial_Flex_DisplacementMW()
        {
            base.InsType = InstrumentType.Fiducial_Flex_Displacement;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 土体位移计
    /// </summary>
    public class Fiducial_Soil_DisplacementMW : Fiducial_Soil_Displacement
    {
        public Fiducial_Soil_DisplacementMW()
        {
            base.InsType = InstrumentType.Fiducial_Soil_Displacement;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 测缝计
    /// </summary>
    public class Fiducial_Measure_ApertureMW : Fiducial_Measure_Aperture
    {
        public Fiducial_Measure_ApertureMW()
        {
            base.InsType = InstrumentType.Fiducial_Measure_Aperture;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
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
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
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
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString();
                    dr["Temperature"] = Convert.ToDecimal(surveydata.Tempreture);
                    dr["loadReading"] = Convert.ToDecimal(surveydata.LoadReading);
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
    /// 渗压计
    /// </summary>
    public class Fiducial_Leakage_PressureMW : Fiducial_Leakage_Pressure
    {
        public Fiducial_Leakage_PressureMW()
            : base()
        {
            base.InsType = InstrumentType.Fiducial_Leakage_Pressure;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {

            double result = (param.Gorf * (data.Survey_ZorR - param.ZorR) +
                param.Korb * (param.TemperatureRead * (data.Survey_RorT - param.ZeroR) -
                param.TemperatureRead * (param.RorT - param.ZeroR))) * param.KpaToMpa;

            data.ResultReading = result * 102.0408;//最终结果水头ResultReading  Kpa
            data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);//温度
            data.LoadReading = result;//渗透压力LoadReading
            return result;
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
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
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
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
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }

        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
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
                 //苗尾
                sql = @"select Add_Temperature,Conversion_C,Minus_Type,Kpa_or_Mpa, Freq_or_Modul from Instrument_PointCompute where Instrument_Name='渗压计' and Survey_point_Number='{0}'";
                dt = SqlHelper.SelectData(string.Format(sql, Survey_point_Number));
                if (dt.Rows.Count > 0)
                {
                    int isAddT = Convert.ToInt16(dt.Rows[0]["Add_Temperature"]);
                    pd.Korb = (isAddT == 1) ? 0 : pd.Korb;//1--不改，2-改
                    pd.Gorf = Convert.ToInt16(dt.Rows[0]["Minus_Type"]) == 2 ? pd.Gorf * -1 : pd.Gorf;
                    pd.Constant_b = Convert.ToDouble(dt.Rows[0]["Conversion_C"]);
                    pd.KpaToMpa = Convert.ToInt16(dt.Rows[0]["Kpa_or_Mpa"]) == 2 ? 0.001 : 1; //
                    pd.IsMoshu = Convert.ToInt16(dt.Rows[0]["Freq_or_Modul"]) == 2 ? true : false;
                    pd.Special = true;
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
    public class Fiducial_MeasureStress_HoseMW : Fiducial_MeasureStress_Hose
    {
        public Fiducial_MeasureStress_HoseMW()
        {
            base.InsType = InstrumentType.Fiducial_MeasureStress_Hose;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 量水堰
    /// </summary>
    public class Fiducial_MeasureWater_WeirMW : Fiducial_MeasureWater_Weir
    {
        public Fiducial_MeasureWater_WeirMW()
        {
            base.InsType = InstrumentType.Fiducial_MeasureWater_Weir;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 应变计
    /// </summary>
    public class Fiducial_Strain_GaugeMW : Fiducial_Strain_Gauge
    {
        public Fiducial_Strain_GaugeMW()
        {
            base.InsType = InstrumentType.Fiducial_Strain_Gauge;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
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
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            double result = 0;
            double Tcorrect = (data.Survey_RorT != 0) ? (param.Korb - param.Concrete_Expansion_ac) * (data.Survey_RorT - param.RorT) : 0;
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
            if (param.IsHasNonStress && data.NonStressSurveyData != null)
            {
                data.LoadReading = data.LoadReading - data.NonStressSurveyData.LoadReading;
            }
            return result;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        /// <summary>从数据库读取参数
        /// </summary>
        /// <param name="Survey_point_Number">测点名称</param>
        /// <param name="表名"></param>
        /// <returns></returns>
        public override ParamData GetParam(string Survey_point_Number, string tablename)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Instru_Expansion_b,Concrete_Expansion_ac,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance,Nonstress_Number from {0} where Survey_point_Number='{1}'";
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
                pd.Concrete_Expansion_ac = ConvetToData(dt.Rows[0]["Concrete_Expansion_ac"]);
                pd.NonStressNumber = dt.Rows[0]["Nonstress_Number"].ToString();
                pd.IsHasNonStress = !String.IsNullOrEmpty(pd.NonStressNumber.Trim());
                if (dt.Rows[0]["Instru_Expansion_b"] == null)
                {
                    pd.Korb = 0;
                }
                else
                {
                    pd.Korb = ConvetToData(dt.Rows[0]["Instru_Expansion_b"]);
                    pd.RorT = ConvetToData(dt.Rows[0]["Benchmark_Resist"]);
                }
                string instype = dt.Rows[0]["Instrument_Type"].ToString();
                pd.TemperatureRead = ConvetToData(dt.Rows[0]["Temperature_Read"]);
                pd.ZeroR = ConvetToData(dt.Rows[0]["Zero_Resistance"]);
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
            double result = 0;
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
    public class Fiducial_NonstressMW : Fiducial_Nonstress
    {
        public Fiducial_NonstressMW()
        {
            base.InsType = InstrumentType.Fiducial_Nonstress;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
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
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
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
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
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
                pd.Gorf = ConvetToData(dt.Rows[0]["Calculate_Coeffi_G"]);
                pd.ZorR = ConvetToData(dt.Rows[0]["Benchmark_Resist_Ratio"]);
                pd.Concrete_Expansion_ac = ConvetToData(dt.Rows[0]["Concrete_Expansion_ac"]);
                if (dt.Rows[0]["Instru_Expansion_b"] == null)
                {
                    pd.Korb = 0;
                }
                else
                {
                    pd.Korb = ConvetToData(dt.Rows[0]["Instru_Expansion_b"]);
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
    /// 钢筋计
    /// </summary>
    public class Fiducial_Steel_BarMW : Fiducial_Steel_Bar
    {
        public Fiducial_Steel_BarMW()
        {
            base.InsType = InstrumentType.Fiducial_Steel_Bar;

        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
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
            data.ResultReading = result * param.KpaToMpa;
            return result;
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
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
            data.ResultReading = result * param.KpaToMpa;

            return result;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename)
        {
            ParamData pd = base.GetParam(Survey_point_Number, tablename);
            if (Config.ProCode == "MW")
            {
                string sql = @"select Add_Temperature,Conversion_C,Minus_Type from Instrument_PointCompute where Instrument_Name='钢筋计' and Survey_point_Number='{0}'";
                var sqlhelper = CSqlServerHelper.GetInstance();
                var dt = sqlhelper.SelectData(string.Format(sql, Survey_point_Number));
                if (dt.Rows.Count > 0)
                {
                    int isAddT = Convert.ToInt16(dt.Rows[0]["Add_Temperature"]);
                    pd.Korb = (isAddT == 1) ? 0 : pd.Korb;//1--不改，2-改
                    pd.Gorf = Convert.ToInt16(dt.Rows[0]["Minus_Type"]) == 2 ? pd.Gorf * -1 : pd.Gorf;
                    pd.Constant_b = Convert.ToDouble(dt.Rows[0]["Conversion_C"]);
                }
                pd.KpaToMpa = 3.14 * 0.014 * 0.014 * 1000;
            }
            return pd;
        }
    }

    /// <summary>锚杆应力计
    /// </summary>
    public class Fiducial_Anchor_PoleMW : Fiducial_Anchor_Pole
    {
        public Fiducial_Anchor_PoleMW()
        {
            base.InsType = InstrumentType.Fiducial_Anchor_Pole;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
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
            return base.WriteResultToDB(datas);
        }


    }

    /// <summary>
    /// 压应力计
    /// </summary>
    public class Fiducial_Press_StressMW : Fiducial_Press_Stress
    {
        public Fiducial_Press_StressMW()
        {
            base.InsType = InstrumentType.Fiducial_Press_Stress;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
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
    public class Fiducial_Soil_StresMW : Fiducial_Soil_Stres
    {
        public Fiducial_Soil_StresMW()
        {
            base.InsType = InstrumentType.Fiducial_Soil_Stres;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
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
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    dr["RecordMethod"] = "人工";
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
            result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) + param.Korb;
            //data.Survey_ZorRMoshu = Math.Pow(data.Survey_ZorR, 2) / 1000;
            data.ResultReading = result;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            return result;
        }
    }

    /// <summary> 锚索测力计
    /// </summary>
    public class Fiducial_Anchor_CableMW : Fiducial_Anchor_Cable
    {
        public Fiducial_Anchor_CableMW()
        {
            base.InsType = InstrumentType.Fiducial_Anchor_Cable;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        /// <summary>
        /// 计算掉弦的系数
        /// </summary>
        /// <param name="SurveyPoint"></param>
        /// <returns></returns>
        public double GetC(ParamData param, SurveyData data)
        {
            return 1;
        }
        public override double ShakeString(ParamData paramd, SurveyData data, params double[] expand)
        {
            var param = paramd as Anchor_CableParam;
            double result = 0;
            int count = 0;
            double value = 0;
             
            foreach (var dic in data.MultiDatas)
            {
                if (dic.Value.Survey_ZorR <= 0) continue;
                count++;
                value += dic.Value.Survey_ZorR;

            }
            
            if (count == data.MultiDatas.Keys.Count)
            {
                value = value / count;
            }
            else
            {//掉弦
                value = (value / count) * GetC(param, data);
            }
            double ZorR = paramd.IsMoshu ? paramd.ZorR :Math.Pow(param.ZorR, 2) ;
            data.Survey_ZorR = value;
            value = paramd.IsMoshu ? value : Math.Pow(value, 2);

            result = param.Gorf * (value - ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
            result += param.Constant_b;
            //data.Survey_ZorRMoshu = value;
            data.LoadReading = result;
            data.AfterLock = ((param.Lock_Value - result) / param.Lock_Value) * 100;
            data.PlanLock = ((param.Plan_Loading - result) / param.Plan_Loading) * 100;
            data.ResultLock = param.MAX_Loading == 0 ? 0 : ((param.MAX_Loading - result) / param.MAX_Loading) * 100;

            data.ResultReading = result;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;

            return result;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Tempera_Revise_K,Benchmark_Resist_Ratio, Benchmark_Resist,Temperature_Read,Zero_Resistance,Plan_Loading,MAX_Loading,Lock_Value from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                Anchor_CableParam pd = new Anchor_CableParam();
                pd.SurveyPoint = Survey_point_Number;
                if (dt.Rows[0]["Calculate_Coeffi_G"] == null || dt.Rows[0]["Benchmark_Resist_Ratio"] == null) return null;//G和Z必须有
                pd.Gorf = ConvetToData(dt.Rows[0]["Calculate_Coeffi_G"]);
                pd.ZorR = ConvetToData(dt.Rows[0]["Benchmark_Resist_Ratio"]);
                pd.Plan_Loading = ConvetToData(dt.Rows[0]["Plan_Loading"]);
                pd.MAX_Loading = ConvetToData(dt.Rows[0]["MAX_Loading"]);//可以为0
                pd.Lock_Value = ConvetToData(dt.Rows[0]["Lock_Value"]);
                if (pd.Lock_Value == 0 || pd.Plan_Loading == 0) return null;//这两个值必须有
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
                    pd.IsMoshu = Math.Abs(pd.Gorf * 100) >= 1;
                }
                
                if (Config.ProCode == "MW")
                {
                    sql = @"select Add_Temperature,Conversion_C,Minus_Type from Instrument_PointCompute where Instrument_Name='锚索测力计' and Survey_point_Number='{0}'";
                    dt = SqlHelper.SelectData(string.Format(sql, Survey_point_Number));
                    if (dt.Rows.Count > 0)
                    {
                        int isAddT = Convert.ToInt16(dt.Rows[0]["Add_Temperature"]);
                        pd.Korb = (isAddT == 1) ? 0 : pd.Korb;//1--不改，2-改
                        pd.Gorf = Convert.ToInt16(dt.Rows[0]["Minus_Type"]) == 2 ? pd.Gorf * -1 : pd.Gorf;
                        pd.Constant_b = Convert.ToDouble(dt.Rows[0]["Conversion_C"]);
                    }
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
    /// 钢板计
    /// </summary>
    public class Fiducial_Armor_plateMW : Fiducial_Armor_plate
    {
        public Fiducial_Armor_plateMW()
        {
            base.InsType = InstrumentType.Fiducial_Armor_plate;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(TemperatureRead*(Survey_RorT-ZeroR)-RorT)
            double result = param.Gorf * (data.Survey_ZorR - param.ZorR) +
                param.Korb * (param.TemperatureRead * (data.Survey_RorT - param.ZeroR) - param.TemperatureRead * (param.RorT - param.ZeroR));
            data.ResultReading = result * param.Elastic_Modulus_E;//这里要乘以系数
            data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);
            data.LoadReading = result * param.Elastic_Modulus_E;
            return result;
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            double result = 0;
            if (Config.IsMoshu)
            {
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
                //data.Survey_ZorRMoshu = data.Survey_ZorR;
            }
            else
            {
                result = param.Gorf * (data.Survey_ZorR * data.Survey_ZorR - param.ZorR * param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
                //data.Survey_ZorRMoshu = data.Survey_ZorR * data.Survey_ZorR/1000.0;
            }


            data.ResultReading = result * param.Elastic_Modulus_E;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result * param.Elastic_Modulus_E;
            return result;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            ParamData pd = new ParamData();
            pd = base.GetParam(Survey_point_Number, this.InsType.ToString());
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql = String.Format("select Elastic_Modulus_E from Fiducial_Armor_plate where Survey_point_Number='{0}'",
                Survey_point_Number);
            var result = sqlhelper.SelectFirst(sql);
            if (result != DBNull.Value) pd.Elastic_Modulus_E = Convert.ToDouble(result);

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
                    dr["Observation_Time"] = surveydata.SurveyDate.TimeOfDay.ToString();
                    dr["Temperature"] = Convert.ToDecimal(surveydata.Tempreture);
                    dr["loadReading"] = Convert.ToDecimal(surveydata.LoadReading);
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
    /// 温度计
    /// </summary>
    public class Fiducial_TemperatureMW : Fiducial_Temperature
    {
        public Fiducial_TemperatureMW()
        {
            base.InsType = InstrumentType.Fiducial_Temperature;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
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
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
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
            dt.Columns.Add("Temperature");
            dt.Columns.Add("loadReading");
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
    /// 裂缝计
    /// </summary>
    public class Fiducial_ApertureMW : Fiducial_Aperture
    {
        public Fiducial_ApertureMW()
        {
            base.InsType = InstrumentType.Fiducial_Aperture;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }

    }

    /// <summary>
    /// 固定测斜仪
    /// </summary>
    public class Fiducial_Survey_Slant_FixedMW : Fiducial_Survey_Slant_Fixed
    {
        public Fiducial_Survey_Slant_FixedMW()
        {
            base.InsType = InstrumentType.Fiducial_Survey_Slant_Fixed;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData sdata, params double[] expand)
        {
            var data = sdata as Survey_Slant_FixedSurveyData;
            var fparam = param as Survey_Slant_FixedParam;
            double result = fparam.A_C0 * (fparam.A_US * (data.Reading_A - fparam.A_DS) - fparam.A_US * (fparam.A_BS - fparam.A_DS));
            data.loadReading_A = data.ResultReading = result;
            return result;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
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
    /// 多点位移计
    /// </summary>
    public class Fiducial_Multi_DisplacementMW : Fiducial_Multi_Displacement
    {
        public Fiducial_Multi_DisplacementMW()
        {
            base.InsType = InstrumentType.Fiducial_Multi_Displacement;
        }
        public override double DifBlock(ParamData Mparam, SurveyData Mdata, params double[] expand)
        {
            double result = 0;
            double firstvalue = 0;
            foreach (var dic in Mdata.MultiDatas)
            {
                SurveyData data = dic.Value;
                var param = Mparam.MParamData[dic.Key];
                if (Math.Abs(data.Survey_ZorR) < 1)
                {
                    result = 0;
                }
                else
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) +
                                 param.Korb * (param.TemperatureRead * (Mdata.Survey_RorT - param.ZeroR) - param.TemperatureRead * (param.RorT - param.ZeroR))-firstvalue;
                    firstvalue = firstvalue == 0 ? result : firstvalue;
                }
                data.ResultReading = result;//这里要乘以系数
                data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);
                data.LoadReading = result;
            }
            return result;
        }
       //获取基准值的serial编码
        private string GetStanderd(Dictionary<string, SurveyData> Mdatas)
        {
            foreach(var dic in Mdatas)
            {
                if (dic.Key.EndsWith("A"))
                {
                    return dic.Key;
                }
            }
            return null;
        }
        //正常的计算方式
        private double calcOnePoint(ParamData pd, SurveyData sd,MultiDisplacementCalc calc)
        {
            if (Math.Abs(sd.Survey_ZorR) < 1)
            {
                //第一种计算方式或者不是基准值直接返回0
                if (calc.MCalcType == 1 || calc.IsStanderd != 1) return 0;
                string surveypoint = pd.SurveyPoint;
                string loadindex="loadReading0";
                if (calc.R0IsZero == 0)//从浅到深
                {
                    loadindex = "loadReading" + pd.MParamData.Count.ToString();
                }
                return getLastStandValue(surveypoint, loadindex);

            }
            double result = 0;
            if (calc.IsMoshu == 1)//测值是否是模数
            {
                result = pd.Gorf * (sd.Survey_ZorR - pd.ZorR) + pd.Korb * (sd.Survey_RorT - pd.RorT);
            }
            else
            {
                if (Config.IsMoshu)//考证表是否是模数
                {
                    result = pd.Gorf * (Math.Pow(sd.Survey_ZorR, 2) / 1000 - pd.ZorR) + pd.Korb * (sd.Survey_RorT - pd.RorT);
                }
                else
                {
                    result = pd.Gorf * (Math.Pow(sd.Survey_ZorR, 2)  - pd.ZorR*pd.ZorR) + pd.Korb * (sd.Survey_RorT - pd.RorT);
                }

            }
            result += pd.Constant_b;
            return result;
        }
        //特殊的计算方式/对应Mcalctype=2
        private double calcOnePointExpand(ParamData pd, SurveyData sd,MultiDisplacementCalc calc=null)
        {
            if (Math.Abs(sd.Survey_ZorR) < 1&& calc.IsStanderd==1)
            {
                string loadindex = "loadReading0";
                if (calc.R0IsZero == 0)
                {
                    loadindex = "loadReading" + pd.MParamData.Count.ToString();
                }
                return getLastStandValue(pd.SurveyPoint, loadindex);
            }
            return ((int)(pd.Gorf * (sd.Survey_ZorR - pd.ZorR) * 100 + 0.5)) / 100.0 + pd.Korb * (sd.Survey_RorT - pd.RorT)+pd.Constant_b;
        }
        //没有基准的直接遍历计算
        private void calcOneGroupExpand(ParamData Mparam, SurveyData Mdata, MultiDisplacementCalc[] Mcalc,
            Func<ParamData,SurveyData,MultiDisplacementCalc,double> clacAction)
        {
           foreach (var dic in Mdata.MultiDatas)
            {
                SurveyData data = Mdata.MultiDatas[dic.Key];
                data.Survey_RorT = Mdata.Survey_RorT;
                ParamData pd = Mparam.MParamData[dic.Key];
                var tcalc = Mcalc.FirstOrDefault(c => c.Ins_serial == dic.Key);
                data.LoadReading = clacAction(pd, data, tcalc);
            }
        }
        //获取基准列的最后一次有效值
       
        private double getLastStandValue(string surveyPoint,string loadindex)
        {
            string sql = @"select {0} from Result_Multi_Displacement where Survey_point_Number='{1}' and
                        Observation_Date=(select max(Observation_Date) from  Result_Multi_Displacement where 
                        Survey_point_Number='{2}' and abs({3})>0)";
            sql = String.Format(sql, loadindex, surveyPoint, surveyPoint, loadindex);
            var sqlhelper = CSqlServerHelper.GetInstance();
            var res = sqlhelper.SelectFirst(sql);
            double result = 0;
            if (res != DBNull.Value)
            {
                result = ConvetToData(res);
            }
            return result;
            
        }
        //计算配置文件中存在的点的一组值
        private void calcOneGroup(ParamData Mparam, SurveyData Mdata, MultiDisplacementCalc[] Mcalc)
        {
            double result = 0;
            //有基准先算基准
            MultiDisplacementCalc calc = Mcalc.FirstOrDefault(c => c.IsStanderd == 1);
            switch (Mcalc[0].MCalcType)
            {
                case 0://正常算法
                    if (calc != null)
                    {
                        SurveyData sd = Mdata.MultiDatas[calc.Ins_serial];
                        ParamData pd = Mparam.MParamData[calc.Ins_serial];
                        sd.Survey_RorT = Mdata.Survey_RorT;
                        result = calcOnePoint(pd, sd, calc);//计算基准值
                        sd.LoadReading = result;
                        foreach (var dic in Mdata.MultiDatas)
                        {
                            SurveyData data = Mdata.MultiDatas[dic.Key];
                            pd = Mparam.MParamData[dic.Key];
                            if (dic.Key != calc.Ins_serial)
                            {
                                var tcalc = Mcalc.FirstOrDefault(c => c.Ins_serial.ToUpper().Trim() == dic.Key.ToUpper().Trim());
                                data.LoadReading = calcOnePoint(pd, data, tcalc);
                                if (Math.Abs(data.Survey_ZorR) > 1)//有测值才计算
                                {
                                    data.LoadReading = (tcalc.IsBySubtract == 1) ? data.LoadReading - result : result - data.LoadReading;
                                }
                            }
                            //else
                            //{
                            //    data.LoadReading = (calc.R0IsZero == 0) ? result : 0;
                            //}
                        }
                        Mdata.LoadReading = (calc.R0IsZero == 0) ? 0 : result;
                        if (Math.Abs(Mdata.MultiDatas[calc.Ins_serial].Survey_ZorR) < 1)//做基准的点挂了
                        {
                            Mdata.LoadReading = 0;
                        }
                    }
                    else
                    {//
                        calcOneGroupExpand(Mparam, Mdata, Mcalc, calcOnePoint);
                    }
                    break;
                case 1://没有基准，每个点都自己算自己
                    calcOneGroupExpand(Mparam, Mdata, Mcalc, calcOnePoint);
                    break;
                case 2://特殊算法  基准值-（g*(r1-r0)*100+0.5)/100,全部当模数算
                  
                    if (calc != null)
                    {
                        SurveyData sd = Mdata.MultiDatas[calc.Ins_serial];
                        ParamData pd = Mparam.MParamData[calc.Ins_serial];
                        result = calcOnePointExpand(pd, sd, calc);
                        sd.LoadReading = result;
                        foreach (var dic in Mdata.MultiDatas)
                        {
                            sd = Mdata.MultiDatas[dic.Key];
                            sd.Survey_RorT = Mdata.Survey_RorT;
                            pd = Mparam.MParamData[dic.Key];
                            if (dic.Key != calc.Ins_serial)//不是基准
                            {
                                if (Math.Abs(sd.Survey_ZorR) < 1)
                                {
                                    sd.LoadReading = 0;
                                }
                                else
                                {
                                    sd.LoadReading = calcOnePointExpand(pd, sd);
                                    sd.LoadReading = (calc.IsBySubtract == 1) ? sd.LoadReading - result : result - sd.LoadReading;
                                }
          
                            }
                            //else
                            //{
                            //    sd.LoadReading = (calc.R0IsZero == 0) ? result : 0;
                            //    if (Math.Abs(sd.Survey_ZorR) < 1) sd.LoadReading = 0;
                            //}
                        }
                        Mdata.LoadReading = (calc.R0IsZero == 0) ? 0 : result;
                        if (Math.Abs(Mdata.MultiDatas[calc.Ins_serial].Survey_ZorR) < 1)//做基准的点挂了
                        {
                            Mdata.LoadReading = 0;
                        }
                    }
                    else
                    {
                        calcOneGroupExpand(Mparam, Mdata, Mcalc, calcOnePointExpand);
                    }
                        
                    break;
            }

            
        }
        public override double ShakeString(ParamData Mparam, SurveyData Mdata, params double[] expand)
        {
            double result = 0;
            var Mcalc = Config.MultiDisplacementCalcs.GetBySurveypoint(Mparam.SurveyPoint.Trim());
            if (Mcalc.Length > 0)
            {
                calcOneGroup(Mparam, Mdata, Mcalc);
            }
            else
            {
                calcOneGroupNull(Mparam, Mdata);
            }

            //倒序排列下//默认是从深到浅。当孔口为0的时候为从浅到深
            Dictionary<string, SurveyData> Dic = new Dictionary<string, SurveyData>();
            List<string> li = new List<string>(Mdata.MultiDatas.Keys);
            string serial = null;
            foreach (string number in li)
            {
                if (number.EndsWith("A")) serial = number;            
            }
            li.Remove(serial);
            li.Add(serial);
            for (int i = li.Count - 1; i >= 0; i--)
            {
                Dic.Add(li[i], Mdata.MultiDatas[li[i]]);
                
            }
            Mdata.MultiDatas = Dic;
 
            return result;
        }
        //计算配置文件中没有的点的一组值
        private void calcOneGroupNull(ParamData Mparam, SurveyData Mdata)
        {
            double result=0;
            string skey = GetStanderd(Mdata.MultiDatas);//获取基准值
            if (skey != null)
            {
                SurveyData sd = Mdata.MultiDatas[skey];
                ParamData pd = Mparam.MParamData[skey];
                if (Math.Abs(sd.Survey_ZorR) < 1)
                {
                    var list = new List<string>(Mdata.MultiDatas.Keys);
                    string loadindex = "loadReading"+(list.IndexOf(skey)+1).ToString();
                    result = getLastStandValue(Mparam.SurveyPoint, loadindex);
                }
                else
                {
                    result= base.ShakeString(pd, sd);
                }
                foreach (var di in Mdata.MultiDatas)
                {
                    if (di.Key == skey) continue;
                    sd = di.Value;
                    pd = Mparam.MParamData[di.Key];
                    result=base.ShakeString(pd, sd);
                    sd.LoadReading = result - sd.LoadReading;
                }
            }
            else
            {
                foreach (var di in Mdata.MultiDatas)
                {
                    SurveyData sd = di.Value;
                    ParamData pd = Mparam.MParamData[di.Key];
                    base.ShakeString(pd, sd);

                }
            }
            Mdata.LoadReading = 0;
 
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            #region
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Tempera_Revise_K,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance,Instrument_Serial
                                from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            ParamData Mpd = new ParamData();
            Mpd.SurveyPoint = Survey_point_Number;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    ParamData pd = new ParamData();
                    pd.SurveyPoint = Survey_point_Number;
                    string insSerial = dt.Rows[i][7].ToString();
                    pd.Ins_Serial = insSerial;
                    if (dt.Rows[i][1] == null || dt.Rows[i][3] == null) return null;//G和Z必须有
                    pd.Gorf = ConvetToData(dt.Rows[i][1]);
                    pd.ZorR = ConvetToData(dt.Rows[i][3]);
                    if (dt.Rows[i][2] == null)
                    {
                        pd.Korb = 0;
                    }
                    else
                    {
                        pd.Korb = ConvetToData(dt.Rows[i][2]);
                        pd.RorT = ConvetToData(dt.Rows[i][4]);
                    }
                    string instype = dt.Rows[i][0].ToString();
                    pd.TemperatureRead = ConvetToData(dt.Rows[i][5]);
                    pd.ZeroR = ConvetToData(dt.Rows[i][6]);
                    if (instype.Contains("差阻") || (pd.TemperatureRead != 1 && pd.ZeroR > 0))//默认是振弦
                    {
                        pd.InsCalcType = CalcType.DifBlock;
                    }
                    Mpd.MParamData.Add(insSerial, pd);
                }
                catch {}
            }
            #endregion

            #region //读扩展表
            if (Config.ProCode == "MW")
            {
                sql = @"select Instrument_Serial,Add_Temperature,Conversion_C,Minus_Type from Instrument_PointCompute where Instrument_Name='多点位移计' and Survey_point_Number='{0}'";
                dt = SqlHelper.SelectData(string.Format(sql, Survey_point_Number));
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string seril = dt.Rows[i]["Instrument_Serial"].ToString();
                        int isAddT = Convert.ToInt16(dt.Rows[i]["Add_Temperature"]);
                        Mpd.MParamData[seril].Korb = (isAddT == 1) ? 0 : Mpd.MParamData[seril].Korb;//1--不改，2-改
                        Mpd.MParamData[seril].Gorf = Convert.ToInt16(dt.Rows[i]["Minus_Type"]) == 2 ? Mpd.MParamData[seril].Gorf * -1 : Mpd.MParamData[seril].Gorf;
                        Mpd.MParamData[seril].Constant_b = Convert.ToDouble(dt.Rows[i]["Conversion_C"]);
                        //Mpd.MParamData[seril].Special_Code = Convert.ToInt16(dt.Rows[0]["Special_Case"]); //1 2都是模数,
                    }

                }
            }
            #endregion
            return Mpd;
            
        }
    }

    /// <summary>
    /// 应变计组
    /// </summary>
    public class Fiducial_Strain_GroupMW : Fiducial_Strain_Group
    {
        public Fiducial_Strain_GroupMW()
        {
            base.InsType = InstrumentType.Fiducial_Strain_Group;
        }

        /// <summary>
        /// 5向应变计组平衡计算
        /// </summary>
        /// <param name="data"></param>
        public void CalcGroup(SurveyData data)
        {
            List<string> seriallist = new List<string>(data.MultiDatas.Keys);
            //(1+3)-(2+4)
            //1'=1-delt/4    2'=1+delt/4   3'=1+delt/4    4'=4+delt/4  除以2或者4
            double tempd = 4.0;
            double DeltZ = (data.MultiDatas[seriallist[1]].LoadReading + data.MultiDatas[seriallist[3]].LoadReading) -
                (data.MultiDatas[seriallist[2]].LoadReading + data.MultiDatas[seriallist[4]].LoadReading);
            double c1 = data.MultiDatas[seriallist[1]].LoadReading - DeltZ / tempd;
            double c2z= data.MultiDatas[seriallist[2]].LoadReading + DeltZ / tempd;
            double c3 = data.MultiDatas[seriallist[3]].LoadReading - DeltZ / tempd;
            double c4x = data.MultiDatas[seriallist[4]].LoadReading + DeltZ / tempd;
            double c5y = data.MultiDatas[seriallist[4]].LoadReading;
            data.StrainGroup_x = 0.021 * (c4x / (1 + 0.167) + (c2z + c4x + c5y) * 0.167 / (1 + 0.167) * (1 - 2 * 0.167));
            data.StrainGroup_y = 0.021 * (c5y / (1 + 0.167) + (c2z + c4x + c5y) * 0.167 / (1 + 0.167) * (1 - 2 * 0.167));
            data.StrainGroup_z = 0.021 * (c2z / (1 + 0.167) + (c2z + c4x + c5y) * 0.167 / (1 + 0.167) * (1 - 2 * 0.167));
            data.StrainGroup_xz = 0.009 * (c3 - c1);
                
        }

        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            Fiducial_Strain_Gauge FStrain_Gauge = new Fiducial_Strain_Gauge();
            //计算应变计
            foreach (var sd in data.MultiDatas)
            {
                FStrain_Gauge.DifBlock(param.MParamData[sd.Key], sd.Value);
            }
            if (data.MultiDatas.Keys.Count == 5)
            {
                CalcGroup(data);
            }
            return 1;

        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            Fiducial_Strain_Gauge FStrain_Gauge = new Fiducial_Strain_Gauge();
            foreach (var sd in data.MultiDatas)
            {
                var pa = param.MParamData[sd.Key.ToUpper().Trim()];
                switch (pa.InsCalcType)
                {
                    case CalcType.DifBlock:
                        FStrain_Gauge.DifBlock(pa, sd.Value);
                        break;
                    case CalcType.ShakeString:
                        FStrain_Gauge.ShakeString(pa, sd.Value);
                        break;
                    case CalcType.CalcExpand1:
                        FStrain_Gauge.Expand1(pa, sd.Value,null);
                        break;
                }
            }
            if (data.MultiDatas.Keys.Count == 5)
            {
                CalcGroup(data);
            }
            return 1;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Instru_Expansion_b,Concrete_Expansion_ac,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance,Instrument_Serial,Nonstress_Number from {0} where Survey_point_Number='{1}'";

            sql = string.Format(sql, tablename, Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            ParamData Mpd = new ParamData();
            Mpd.SurveyPoint = Survey_point_Number;
            Mpd.NonStressNumber = dt.Rows[0]["Nonstress_Number"].ToString();
            if(String.IsNullOrEmpty(Mpd.NonStressNumber))Mpd.IsHasNonStress=false;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    ParamData pd = new ParamData();
                    pd.SurveyPoint = Survey_point_Number;
                    string insSerial = dt.Rows[i]["Instrument_Serial"].ToString().ToUpper().Trim();
                    pd.Ins_Serial = insSerial;
                    if (dt.Rows[i]["Calculate_Coeffi_G"] == null || dt.Rows[i]["Benchmark_Resist_Ratio"] == null) return null;//G和Z必须有
                    pd.Gorf = ConvetToData(dt.Rows[i]["Calculate_Coeffi_G"]);
                    pd.ZorR = ConvetToData(dt.Rows[i]["Benchmark_Resist_Ratio"]);

                    pd.Concrete_Expansion_ac = ConvetToData(dt.Rows[i]["Concrete_Expansion_ac"]);
                    if (dt.Rows[i]["Instru_Expansion_b"] == null)
                    {
                        pd.Korb = 0;
                    }
                    else
                    {
                        pd.Korb = ConvetToData(dt.Rows[i]["Instru_Expansion_b"]);
                        pd.RorT = ConvetToData(dt.Rows[i]["Benchmark_Resist"]);
                    }
                    string instype = dt.Rows[i]["Instrument_Type"].ToString();
                    pd.TemperatureRead = ConvetToData(dt.Rows[i]["Temperature_Read"]);
                    pd.ZeroR = ConvetToData(dt.Rows[i]["Zero_Resistance"]);
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
        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Observation_Time");
            for (int i = 1; i < 7; i++)
            {
                dt.Columns.Add("Frequency"+i.ToString());
                dt.Columns.Add("Temperature" + i.ToString());
            }
            dt.Columns.Add("Non_Frequency");
            dt.Columns.Add("Non_Temperature");
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
                    int index = 1;
                    foreach (var dic in surveydata.MultiDatas)
                    {
                        dr["Frequency" + index.ToString()] = Math.Round(dic.Value.Survey_ZorR,4);
                        dr["Temperature" + index.ToString()] = Math.Round(dic.Value.Survey_RorT,4);
                        index++;
                    }
                    if (surveydata.NonStressSurveyData != null)
                    {
                        dr["Non_Frequency" + index.ToString()] = Math.Round(surveydata.NonStressSurveyData.Survey_ZorR, 4);
                        dr["Non_Temperature" + index.ToString()] = Math.Round(surveydata.NonStressSurveyData.Survey_RorT, 4);
                    }
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            //return dt.Rows.Count;
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
            for (int i = 1; i < 7; i++)
            {
                dt.Columns.Add("loadReading"+i.ToString());
                dt.Columns.Add("Temperature" + i.ToString());
            }
            dt.Columns.Add("Non_Temperature");
           
            dt.Columns.Add("Non_loadReading");
            dt.Columns.Add("Ex");
            dt.Columns.Add("Ey");
            dt.Columns.Add("Ez");
            dt.Columns.Add("σx");
            dt.Columns.Add("σy");
            dt.Columns.Add("σz");
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
                    int index = 1;
                    foreach (var dic in surveydata.MultiDatas)
                    {
                        dr["loadReading" + index.ToString()] = Math.Round(Convert.ToDecimal(dic.Value.LoadReading), 2);
                        dr["Temperature" + index.ToString()] = Math.Round((decimal)dic.Value.Tempreture, 2);
                        index++;
                    }
                    if (surveydata.NonStressSurveyData != null)
                    {
                        dr["Non_loadReading" + index.ToString()] = Math.Round((decimal)surveydata.NonStressSurveyData.LoadReading, 2);
                        dr["Non_Temperature" + index.ToString()] = Math.Round((decimal)surveydata.NonStressSurveyData.Survey_RorT, 2);
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
    /// 多点锚杆应力计
    /// </summary>
    public class Fiducial_Multi_Anchor_PoleMW : Fiducial_Multi_Anchor_Pole
    {
        public Fiducial_Multi_Anchor_PoleMW()
        {
            base.InsType = InstrumentType.Fiducial_Multi_Anchor_Pole;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return base.ShakeString(param, data, expand);
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
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

}
