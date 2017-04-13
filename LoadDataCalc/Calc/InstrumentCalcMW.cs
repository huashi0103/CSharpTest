/*
 * 苗尾数据计算类,对应ProCode=MW
 * 考证表中的参数表格中是什么就录的什么，
 * 数据库的测值都写频率
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
            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            double result = 0;
            var pd = param as Survey_Slant_FixedParam;
            result = pd.A_C0 * (pd.A_US * (data.Survey_ZorR - pd.A_BS));
            data.ResultReading = 0;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            data.Survey_ZorRMoshu = data.Survey_ZorR;
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

        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {

            double result = 0;
            double tcorrect = param.Korb * (data.Survey_RorT - param.RorT);
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 5000 || param.IsPotentiometer)
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + tcorrect;
                    data.Survey_ZorRMoshu = param.IsPotentiometer ? data.Survey_ZorR : Math.Sqrt(data.Survey_ZorR * 1000);
                }
                else
                {
                    if (Config.IsMoshu)
                    {
                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000.0 - param.ZorR) + tcorrect;
                    }
                    else
                    {
                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) + tcorrect;
                    }
                    data.Survey_ZorRMoshu = data.Survey_ZorR;
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
            base.ShakeString(param, data, expand);
            data.Survey_ZorR = data.Survey_ZorR * 10;
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            return 1;
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
            base.ShakeString(param, data, expand);
            data.Survey_ZorR=data.Survey_ZorR * 10;
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            return 0;
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
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            double result = 0;
            double tcorrect=param.Korb * (data.Survey_RorT - param.RorT);
            if (CheckIsMoshu(param.Gorf))
            {
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) + tcorrect;
                data.Survey_ZorRMoshu = Math.Sqrt(data.Survey_ZorR * 1000);
            }
            else
            {
                //录入的是频率或者模数
                if (Config.IsMoshu)
                {
                    result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000.0 - param.ZorR) + tcorrect;
                }
                else
                {
                    result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) + tcorrect;
                }
                data.Survey_ZorRMoshu = data.Survey_ZorR;
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

            double result = (param.Gorf * (data.Survey_ZorR - param.ZorR) +GetTCorrect(param,data)) * param.KpaToMpa;
            data.Survey_ZorRMoshu = data.Survey_ZorR;

            data.LoadReading = result;//渗透压力LoadReading
            result = result < 0 ? 0 : result;
            data.ResultReading = result * 102.0408;//最终结果水头ResultReading  Kpa
            data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);//温度

            return result;
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            double result = 0;
            //录入的是频率或者模数
            double tcorrect = GetTCorrect(param, data);
            if (!param.Special)
            {
                if (Math.Abs(param.Gorf * 10000) > 1)
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + tcorrect;
                     data.Survey_ZorRMoshu = Math.Sqrt(data.Survey_ZorR*1000);
                }
                else
                {
                    if (Config.IsMoshu)
                    {
                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000.0 - param.ZorR) + tcorrect;
                    }
                    else
                    {
                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) + tcorrect;
                    }
                    data.Survey_ZorRMoshu = data.Survey_ZorR;
                }
                if (Math.Abs(param.Gorf * 100) > 1) param.KpaToMpa = 0.001;
            }
            else
            {
                if (param.IsMoshu)
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + tcorrect;
                    data.Survey_ZorRMoshu = Math.Sqrt(data.Survey_ZorR * 1000);
                }
                else
                {
                    result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) + tcorrect;
                    data.Survey_ZorRMoshu = data.Survey_ZorR;
                }
            }
            result += param.Constant_b;
            result = result * param.KpaToMpa;
            data.LoadReading = result;
            result = result < 0 ? 0 : result;
            data.ResultReading = result * param.WaterHead_Coeffi_C;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
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
            data.LoadReading = 1.86 * param.Gorf * Math.Pow(data.Survey_ZorR, 1.5) * 1000;
            data.ResultReading = data.LoadReading;
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            return data.ResultReading;
        }
        //矩形
        public override double Expand1(ParamData param, SurveyData data, string expression)
        {
            double Q1 = 50;//初值
            while (true)
            {
                double H = (param.RorT - data.Survey_ZorR) * param.Gorf + param.Constant_b;
                double deltg = param.ZorR * (H + param.Korb);
                double v = (Q1 / 1000.0) / deltg;
                double I = H + Math.Pow(v, 2) / (2 * 9.81);
                double F = 0.627 + 0.018 * I / param.Korb;
                double Q2 = F * 2 * Math.Sqrt(2 * 9.81) * param.ZorR * Math.Pow(I, 1.5) * 1000 / 3;
                if (Math.Abs(Q2 - Q1) < 0.01)
                {
                    Q1 = Q2;
                    break;
                }
                else
                {
                    Q1 = Q2;
                }
            }
            data.LoadReading = Q1;
            return Q1;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            string sql = @"select * from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                ParamData pd = new ParamData();
                pd.SurveyPoint = Survey_point_Number;
                pd.Gorf = ConvetToData(dt.Rows[0]["A1_C"]);//灵敏度
                pd.Korb = ConvetToData(dt.Rows[0]["A2_C"]);//高度
                pd.ZorR = ConvetToData(dt.Rows[0]["Weir_Size"]);//宽度
                pd.RorT = ConvetToData(dt.Rows[0]["Elevation"]);//初值
                pd.Constant_b = ConvetToData(dt.Rows[0]["Section_Name"]);//堰水上头初值

                string instype = dt.Rows[0]["Point_Type"].ToString();
                if (instype == "1")//三角
                {
                    pd.InsCalcType = CalcType.DifBlock;
                }
                else if (instype == "2")//梯形
                {
                    pd.InsCalcType = CalcType.ShakeString;
                }
                else if (instype == "3")//矩形
                {
                    pd.InsCalcType = CalcType.CalcExpand1;
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
                double tcorrect = data.Survey_RorT <= 0 ? 0 :
                     (param.Korb - param.Concrete_Expansion_ac) * (param.TemperatureRead * (data.Survey_RorT - param.ZeroR) - param.TemperatureRead * (param.RorT - param.ZeroR));
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) + tcorrect;
                if (param.ZeroR > 1 && Math.Abs(data.Survey_RorT) > 1) data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);
            }
            data.ResultReading = result;
            data.LoadReading = result;
            if (data.NonStressSurveyData != null)
            {
                data.LoadReading = data.LoadReading - data.NonStressSurveyData.LoadReading;
            }
            data.Survey_ZorRMoshu = data.Survey_ZorR;

            return result;
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            double result = 0;
            double Tcorrect = (data.Survey_RorT > Config.MinTemperature && data.Survey_RorT < Config.MaxTemperature) ?
                            (param.Korb - param.Concrete_Expansion_ac) * (data.Survey_RorT - param.RorT) : 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 5000)
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + Tcorrect;
                    data.Survey_ZorRMoshu = Math.Sqrt(data.Survey_ZorR * 1000);
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
                    data.Survey_ZorRMoshu = data.Survey_ZorR;
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
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            return result;
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
                double tcorrect = data.Survey_RorT <= 0 ? 0 :
                  (param.Korb - param.Concrete_Expansion_ac) * (param.TemperatureRead * (data.Survey_RorT - param.ZeroR) - param.TemperatureRead * (param.RorT - param.ZeroR));
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) + tcorrect;
            }
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            data.ResultReading = result;
            data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);
            data.LoadReading = result;
            return result;
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            double result = 0;
            double Tcorrect = (data.Survey_RorT > Config.MinTemperature && data.Survey_RorT < Config.MaxTemperature) ?
                            (param.Korb - param.Concrete_Expansion_ac) * (data.Survey_RorT - param.RorT) : 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 5000)
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + Tcorrect;
                    data.Survey_ZorRMoshu = Math.Sqrt(data.Survey_ZorR * 1000);
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
                    data.Survey_ZorRMoshu = data.Survey_ZorR;
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
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) +GetTCorrect(param,data);
            }
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            result += param.Constant_b;
            data.Tempreture = data.Survey_RorT > 0 ? param.TemperatureRead * (data.Survey_RorT - param.ZeroR) : 0;
            data.LoadReading = result;
            data.ResultReading = (param.Steel_Diameter_L != 0) ? result * 1000 / (Math.PI * Math.Pow(param.Steel_Diameter_L / 2.0, 2)) : result;
            return result;
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            double result = 0;
            double Tcorrect = GetTCorrect(param,data);
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 3000)//模数
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + Tcorrect;
                    data.Survey_ZorRMoshu = Math.Sqrt(data.Survey_ZorR*1000);
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
                    data.Survey_ZorRMoshu = data.Survey_ZorR;
                }
            }
            result += param.Constant_b;
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            data.ResultReading = (param.Steel_Diameter_L != 0) ? result * 1000 / (Math.PI * Math.Pow(param.Steel_Diameter_L / 2.0, 2)) : result;

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

            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            double result = 0;
            double Tcorrect = GetTCorrect(param, data);
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (CheckIsMoshu(param.Gorf,1000))//模数
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + Tcorrect;
                    data.Survey_ZorRMoshu = Math.Sqrt(data.Survey_ZorR*1000);
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
                    data.Survey_ZorRMoshu =data.Survey_ZorR;
                }
                result += param.Constant_b;
            }

            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            data.ResultReading = (param.Steel_Diameter_L != 0) ?
                result * 1000 / (Math.PI * Math.Pow(param.Steel_Diameter_L / 2.0, 2)) : data.LoadReading;
            return result;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {

            var Mpd=base.GetParam(Survey_point_Number, this.InsType.ToString());

            #region //读扩展表
            if (Config.ProCode == "MW")
            {
                var SqlHelper = CSqlServerHelper.GetInstance();
                string sql = @"select Instrument_Serial,Add_Temperature,Conversion_C,Minus_Type from Instrument_PointCompute where Instrument_Name='锚杆应力计' and Survey_point_Number='{0}'";
                var dt = SqlHelper.SelectData(string.Format(sql, Survey_point_Number));
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string seril = dt.Rows[i]["Instrument_Serial"].ToString();
                        int isAddT = Convert.ToInt16(dt.Rows[i]["Add_Temperature"]);
                        Mpd.Korb = (isAddT == 1) ? 0 : Mpd.Korb;//1--不改，2-改
                        Mpd.Gorf = Convert.ToInt16(dt.Rows[i]["Minus_Type"]) == 2 ? Mpd.Gorf * -1 : Mpd.Gorf;
                        Mpd.Constant_b = Convert.ToDouble(dt.Rows[i]["Conversion_C"]);
                        //Mpd.MParamData[seril].Special_Code = Convert.ToInt16(dt.Rows[0]["Special_Case"]); //1 2都是模数,
                    }

                }
            }
            #endregion

            return Mpd;
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
            data.Survey_ZorRMoshu = data.Survey_ZorR;
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
        public Fiducial_Anchor_CableMW(): base()
        {
        }

        //private Dictionary<string, List<double>> DicR0 = new Dictionary<string, List<double>>();
        //掉弦索引缓存
        private List<string> ListCach = new List<string>();
        private string[] sbStrlist = new string[] { "Reading_Red", "Reading_Black", "Reading_Yellow", "Reading_Blue", "Reading_Ash", "Reading_Purple" };

        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }

        /// <summary>
        ///         //从当前数据中查询有不为0的数据
        /// </summary>
        /// <param name="end">截至时间</param>
        /// <param name="key">查询的key</param>
        /// <param name="pd">本次数据缓存</param>
        /// <param name="IgnoreKey">被忽略的点</param>
        /// <returns></returns>
        SurveyData GetLastData(DateTime end, string key, PointSurveyData pd, List<string> IgnoreKey = null)
        {
            if (pd.Datas.Count == 0) return null;
            SurveyData lastData = new SurveyData();
            bool flag = true;
            foreach (var sd in pd.Datas)
            {
                if (sd.SurveyDate.CompareTo(end) >= 0) break;
                bool Zflag = true;
                if (sd.MultiDatas[key].Survey_ZorR != 0)
                {
                    foreach (var d in sd.MultiDatas)
                    {
                        if (IgnoreKey != null && IgnoreKey.Contains(d.Key)) continue;
                        if (d.Key != key && d.Value.Survey_ZorR == 0)
                        {
                            Zflag = false;
                        }
                    }
                    if (Zflag)
                    {
                        flag = false;
                        lastData = sd;
                    }
                }
            }
            if (flag) return null;//没找到
            return lastData;

        }
        //掉一根弦
        private void GetCOne(ParamData param, SurveyData sd, string key, PointSurveyData pd)
        {
            Anchor_CableParam p = param as Anchor_CableParam;
            var sqlhelper = CSqlServerHelper.GetInstance();
            SurveyData lastSD = GetLastData(sd.SurveyDate, key, pd);
            string ignorestr = "";
            for (int i = 0; i < p.Sum; i++)
            {
                if (key != i.ToString())
                {
                    ignorestr += " and " + sbStrlist[i] + ">0";
                }
            }

            if (lastSD == null)
            {
                lastSD = new SurveyData();
                string sql = "select top 1 * from Survey_Anchor_Cable where Survey_point_Number='{0}' and {1}>0 {2} order by Observation_Date desc";
                string anchorName = sbStrlist[Convert.ToInt16(key)];
                var dt = sqlhelper.SelectData(string.Format(sql, p.SurveyPoint, anchorName, ignorestr));
                if (dt.Rows.Count < 1) return;
                sd.SurveyDate = (DateTime)dt.Rows[0]["Observation_Date"];
                for (int i = 0; i < p.Sum; i++)
                {
                    SurveyData s = new SurveyData();
                    s.Survey_ZorR = Convert.ToDouble(dt.Rows[0][sbStrlist[i]]);
                    lastSD.MultiDatas.Add(i.ToString(), s);
                }
            }

            double sum = 0;
            double badZ = 0;
            foreach (var d in lastSD.MultiDatas)
            {
                double moshu = d.Value.Survey_ZorR;
                if (d.Value.Survey_ZorR < 3000)
                {
                    moshu = moshu * moshu / 1000.0;
                }
                sum += moshu;
                if (d.Key == key) badZ = moshu;
            }
            coefficient_K = ((sum - badZ) / (p.Sum - 1)) / (sum / p.Sum);
        }
        //掉两根或者三根弦
        private void GetCTwo(ParamData param, SurveyData data, List<string> keys, PointSurveyData pd)
        {
            Anchor_CableParam p = param as Anchor_CableParam;
            //查询每次掉弦前一次的数据
            var sqlhelper = CSqlServerHelper.GetInstance();
            Dictionary<string, SurveyData> DataDIc = new Dictionary<string, SurveyData>();
            List<string> ignoreKey = new List<string>();//呗忽略的点
            string ignorestr = "";
            for (int i = 0; i < p.Sum; i++)
            {
                ignorestr += " and " + sbStrlist[i] + ">0";
            }
            var ls = GetLastData(data.SurveyDate, keys[0], pd, keys);
            SurveyData sd = new SurveyData();
            if (ls != null)
            {
                sd = ls;
            }
            else
            {
                string sql = "select top 1 * from Survey_Anchor_Cable where Survey_point_Number='{0}'  {1} order by Observation_Date desc";
                //string anchorName = sbStrlist[Convert.ToInt16(k)];
                var dt = sqlhelper.SelectData(string.Format(sql, p.SurveyPoint, ignorestr));
                if (dt.Rows.Count < 1) return;
                sd.SurveyDate = (DateTime)dt.Rows[0]["Observation_Date"];
                for (int i = 0; i < p.Sum; i++)
                {
                    SurveyData s = new SurveyData();
                    s.Survey_ZorR = Convert.ToDouble(dt.Rows[0][sbStrlist[i]]);
                    sd.MultiDatas.Add(i.ToString(), s);
                }
            }
            double K = 1;
            double sum = 0;
            double badZ = 0;
            foreach (var d in sd.MultiDatas)
            {
                double moshu = d.Value.Survey_ZorR;
                if (d.Value.Survey_ZorR < 3000)
                {
                    moshu = moshu * moshu / 1000.0;
                }
                sum += moshu;
                if (keys.Contains(d.Key)) badZ += moshu;
            }
            K = ((sum - badZ) / (p.Sum - keys.Count)) / (sum / p.Sum);

            //foreach (string k in keys)
            //{
            //    var ls = GetLastData(data.SurveyDate, k, pd, keys);
            //    if (ls != null)
            //    {
            //        DataDIc.Add(k, ls);//在本次数据中找到了数据，直接下一根弦
            //        continue;
            //    }
            //    SurveyData sd = new SurveyData();
            //    string sql = "select top 1 * from Survey_Anchor_Cable where Survey_point_Number='{0}'  {1} order by Observation_Date desc";
            //    //string anchorName = sbStrlist[Convert.ToInt16(k)];
            //    var dt = sqlhelper.SelectData(string.Format(sql, p.SurveyPoint, ignorestr));
            //    if (dt.Rows.Count < 1) continue;
            //    sd.SurveyDate = (DateTime)dt.Rows[0]["Observation_Date"];
            //    for (int i = 0; i < p.Sum; i++)
            //    {
            //        SurveyData s = new SurveyData();
            //        s.Survey_ZorR = Convert.ToDouble(dt.Rows[0][sbStrlist[i]]);
            //        sd.MultiDatas.Add(i.ToString(), s);
            //    }
            //    DataDIc.Add(k, sd);
            //}
            ////按时间排序
            //var dic = DataDIc.OrderBy(x => x.Value.SurveyDate).ToDictionary(x => x.Key, x => x.Value);
            //double K = 1;
            //int cn = 0;//有多根掉弦，可能要循环计算多次
            ////计算改正系数
            //foreach (var di in dic)
            //{
            //    cn++;
            //    double sum = 0;
            //    double badZ = 0;
            //    foreach (var d in di.Value.MultiDatas)
            //    {
            //        double moshu = d.Value.Survey_ZorR;
            //        if (d.Value.Survey_ZorR < 3000)
            //        {
            //            moshu = moshu * moshu / 1000.0;
            //        }
            //        sum += moshu;
            //        if (keys.Contains(d.Key)) badZ += moshu;
            //    }
            //    //sum = (sum / (p.Sum - cn + 1) / K) * (p.Sum - cn +1);//循环计算
            //    K = ((sum - badZ) / (p.Sum - keys.Count)) / (sum / p.Sum);
            //    break;
            //}
            //result = ((lastSum - dieZ) / (p.Sum - count)) / (lastSum / p.Sum);
            coefficient_K = K;
            ListCach = keys;
            return;
        }

        /// <summary>计算掉弦的系数
        /// </summary>
        /// <param name="SurveyPoint"></param>
        /// <returns></returns>
        public override double GetC(ParamData param, SurveyData data, PointSurveyData pd)
        {

            var dicCach = new Dictionary<int, List<string>>();
            int count = 0;//掉弦的弦数
            List<string> keys = new List<string>();//获取掉弦的索引
            foreach (var d in data.MultiDatas)
            {
                if (d.Value.Survey_ZorR == 0)
                {
                    keys.Add(d.Key);
                    count++;
                }
            }
            if (count == 0) return 1;
            Anchor_CableParam p = param as Anchor_CableParam;
            if (count > p.Sum / 2) return 1;

            if (keys.Count == ListCach.Count)//掉弦状况没变
            {
                bool flag = true;
                for (int i = 0; i < keys.Count; i++)
                {
                    if (keys[i] != ListCach[i])
                    {
                        flag = false;
                    }
                }
                if (flag) return coefficient_K;
            }

            if (keys.Count == 1)
            {
                GetCOne(param, data, keys[0], pd);
            }
            else
            {
                GetCTwo(param, data, keys, pd);
            }
            ListCach = keys;
            return coefficient_K;

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
                dic.Value.Survey_ZorRMoshu = paramd.IsMoshu ? Math.Sqrt(dic.Value.Survey_ZorR * 1000) : dic.Value.Survey_ZorR;

            }

            value = value / count;//直接算平均值
            value = value / coefficient_K;//把改正系数带入计算
            data.clacAverage = value;

            double ZorR = paramd.IsMoshu ? paramd.ZorR :Math.Pow(param.ZorR, 2) ;
            data.Survey_ZorR = value;
            value = paramd.IsMoshu ? value : Math.Pow(value, 2);
            result = param.Gorf * (value - ZorR) + param.Korb * (data.Survey_RorT - param.RorT);

            result += param.Constant_b;
            data.Survey_ZorRMoshu = paramd.IsMoshu ? Math.Sqrt(value*1000) :Math.Sqrt(value);
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
            double result = param.Gorf * (data.Survey_ZorR - param.ZorR) +GetTCorrect(param,data);
            data.ResultReading = result * param.Elastic_Modulus_E;//这里要乘以系数
            data.Tempreture = data.Survey_RorT > 0 ? param.TemperatureRead * (data.Survey_RorT - param.ZeroR) : 0;
            data.LoadReading = result * param.Elastic_Modulus_E;
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            return result;
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            double result = 0;
            if (Config.IsMoshu)
            {
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
                data.Survey_ZorRMoshu = Math.Sqrt(data.Survey_ZorR*1000);
            }
            else
            {
                result = param.Gorf * (data.Survey_ZorR * data.Survey_ZorR - param.ZorR * param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
                data.Survey_ZorRMoshu = data.Survey_ZorR;
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
            double result = 0;
            if (param.TemperatureRead != 0 && data.Survey_ZorR != 0)
            {
                result = param.TemperatureRead * (data.Survey_ZorR - param.ZeroR);
                result += param.Constant_b;
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

                sql = @"select Add_Temperature,Conversion_C,Minus_Type,Kpa_or_Mpa, Freq_or_Modul from Instrument_PointCompute where Instrument_Name='温度计' and Survey_point_Number='{0}'";
                dt = SqlHelper.SelectData(string.Format(sql, Survey_point_Number));
                if (dt.Rows.Count > 0)
                {
                    pd.Constant_b = Convert.ToDouble(dt.Rows[0]["Conversion_C"]);
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
            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            double result = 0;

            if (data.Survey_ZorR > 0)
            {
                result = Math.Round(param.Gorf * (data.Survey_ZorR - param.ZorR),3);
            }
            data.Survey_ZorRMoshu = data.Survey_ZorR;
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
            double result = fparam.A_C0 * (fparam.A_US * (data.Survey_ZorR - fparam.A_DS) - fparam.A_US * (fparam.A_BS - fparam.A_DS));
            data.LoadReading = result;
            return result;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {

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
        /// <summary> 写测值表
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public override DataTable WriteSurveyToDB(List<PointSurveyData> datas)
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
                    if (Config.ZeroNull == 0)
                    {
                        dr["Temperature"] = (float)surveydata.Survey_RorT;
                        dr["Reading_A"] = (float)surveydata.Survey_ZorR;
                    }
                    else
                    {
                        dr["Temperature"] = GetData(surveydata.Survey_RorT);
                        dr["Reading_A"] = GetData(surveydata.Survey_ZorR);
                    }
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return dt;
            //return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
        /// <summary> 写成果表
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public override DataTable WriteResultToDB(List<PointSurveyData> datas)
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
            dt.Columns.Add("This_A");
            dt.Columns.Add("This_B");
            dt.Columns.Add("Sum_A");
            dt.Columns.Add("Sum_B");
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
                    if (Config.ZeroNull == 0)
                    {
                        dr["Sum_A"] = Math.Round(surveydata.LoadReading, 4);
                    }
                    else
                    {
                        dr["Sum_A"] = GetData(surveydata.LoadReading, 4);
                    }
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return dt;
            //return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
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
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) +GetTCorrect(param,data)-firstvalue;
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
                sd.Survey_ZorRMoshu = Math.Sqrt(sd.Survey_ZorR*1000);
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
                sd.Survey_ZorRMoshu = sd.Survey_ZorR;

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
                            else
                            {
                                if (Math.Abs(sd.Survey_ZorR) < 1)
                                {
                                    sd.LoadReading = 0;//基准值挂了
                                    continue;
                                }
                                data.LoadReading = result;
                            }
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
                            else
                            {
                                sd.LoadReading = (calc.R0IsZero == 0) ? result : 0;
                                if (Math.Abs(sd.Survey_ZorR) < 1) sd.LoadReading = 0;
                            }
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
            Mdata.Tempreture = Mdata.Survey_RorT;
            //倒序排列下//默认是从深到浅。当孔口为0的时候为从浅到深
            Dictionary<string, SurveyData> Dic = new Dictionary<string, SurveyData>();
            List<string> li = new List<string>(Mdata.MultiDatas.Keys);
            string serial = null;
            foreach (string number in li)
            {
                if (number.EndsWith("A")) serial = number;            
            }
            if (serial != null)
            {
                li.Remove(serial);
                li.Add(serial);
            }
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

        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            Fiducial_Strain_GaugeMW FStrain_Gauge = new Fiducial_Strain_GaugeMW();
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
            Fiducial_Strain_GaugeMW FStrain_Gauge = new Fiducial_Strain_GaugeMW();
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
            Fiducial_Anchor_PoleMW Anchor_Pole = new Fiducial_Anchor_PoleMW();
            foreach (var sd in data.MultiDatas)
            {
                var pa = param.MParamData[sd.Key];
                switch (pa.InsCalcType)
                {
                    case CalcType.DifBlock:
                        Anchor_Pole.DifBlock(pa, sd.Value);
                        break;
                    case CalcType.ShakeString:
                        Anchor_Pole.ShakeString(pa, sd.Value);
                        break;
                    case CalcType.CalcExpand1:
                        Anchor_Pole.Expand1(pa, sd.Value, null);
                        break;
                }
            }
            return 1;
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
                    string insSerial = dt.Rows[i]["Instrument_Serial"].ToString();
                    pd.Ins_Serial = insSerial.ToUpper();
                    if (dt.Rows[i]["Calculate_Coeffi_G"] == null || dt.Rows[i]["Benchmark_Resist_Ratio"] == null) return null;//G和Z必须有
                    pd.Gorf = ConvetToData(dt.Rows[i]["Calculate_Coeffi_G"]);
                    pd.ZorR = ConvetToData(dt.Rows[i]["Benchmark_Resist_Ratio"]);
                    if (dt.Rows[i]["Tempera_Revise_K"] == null)
                    {
                        pd.Korb = 0;
                    }
                    else
                    {
                        pd.Korb = ConvetToData(dt.Rows[i]["Tempera_Revise_K"]);
                        pd.RorT = ConvetToData(dt.Rows[i]["Benchmark_Resist"]);
                    }
                    string instype = dt.Rows[i]["Instrument_Type"].ToString();
                    pd.TemperatureRead = ConvetToData(dt.Rows[i]["Temperature_Read"]);
                    pd.ZeroR = ConvetToData(dt.Rows[i]["Zero_Resistance"]);
                    if (instype.Contains("差阻") || (pd.TemperatureRead != 1 && pd.ZeroR > 0))//默认是振弦
                    {
                        pd.InsCalcType = CalcType.DifBlock;
                    }
                    Mpd.MParamData.Add(insSerial.ToUpper(), pd);
                }
                catch { }
            }
            #endregion
            #region //读扩展表
            sql = @"select Instrument_Serial,Add_Temperature,Conversion_C,Minus_Type from Instrument_PointCompute where Instrument_Name='多点锚杆应力计' and Survey_point_Number='{0}'";
            dt = SqlHelper.SelectData(string.Format(sql, Survey_point_Number));
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string seril = dt.Rows[i]["Instrument_Serial"].ToString().ToUpper();
                    int isAddT = Convert.ToInt16(dt.Rows[i]["Add_Temperature"]);
                    Mpd.MParamData[seril].Korb = (isAddT == 1) ? 0 : Mpd.MParamData[seril].Korb;//1--不改，2-改
                    Mpd.MParamData[seril].Gorf = Convert.ToInt16(dt.Rows[i]["Minus_Type"]) == 2 ? Mpd.MParamData[seril].Gorf * -1 : Mpd.MParamData[seril].Gorf;
                    Mpd.MParamData[seril].Constant_b = Convert.ToDouble(dt.Rows[i]["Conversion_C"]);
                    //Mpd.MParamData[seril].Special_Code = Convert.ToInt16(dt.Rows[0]["Special_Case"]); //1 2都是模数,
                }
            }
            
            #endregion
            return Mpd;
        }

    }

}
