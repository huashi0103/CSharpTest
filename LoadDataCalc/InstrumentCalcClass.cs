/*
 * 各种仪器的实现类
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public class Fiducial_Earth_Measure : BaseInstrument
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

    /// <summary>
    /// 正倒垂线
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
                param.TemperatureRead * (param.RorT - param.ZeroR))) *param.MpaToKpa;

            data.ResultReading = result * 102.0408 ;//最终结果水头ResultReading  Kpa
            data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);//温度
            data.LoadReading = result;//渗透压力LoadReading
            return result;
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            double result = 0;
            if (data.Survey_ZorR > 5000)
            {
                result = (param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT)) * param.MpaToKpa;
            }
            else
            {
                result =(param.Gorf * (Math.Pow(data.Survey_ZorR,2)/1000  - param.ZorR) +
                    param.Korb * (data.Survey_RorT- param.RorT))*param.MpaToKpa;
            }
            data.ResultReading = result * 102.0408;//这里要乘以系数每种仪器不一样
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
            ParamData pd = base.GetParam(Survey_point_Number, this.InsType.ToString());
            if (pd != null)
            {
                string sql = @"select Instrument_Span from {0} where Survey_point_Number='{1}'";
                sql = string.Format(sql, this.InsType.ToString(), Survey_point_Number);
                var SqlHelper = CSqlServerHelper.GetInstance();
                var dt = SqlHelper.SelectData(sql);
                var result = SqlHelper.SelectFirst(sql);
                if (result != null)
                {
                    pd.MpaToKpa = result.ToString().ToLower().Contains("mpa") ? 1 : 0.001;
                }
            }
            return pd;
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
        /// <summary>从数据库读取参数
        /// </summary>
        /// <param name="Survey_point_Number">测点名称</param>
        /// <param name="表名"></param>
        /// <returns></returns>
        public override ParamData GetParam(string Survey_point_Number, string tablename)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Instru_Expansion_b,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance 
                                from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                ParamData pd = new ParamData();
                pd.SurveyPoint = Survey_point_Number;
                if (dt.Rows[0][1] == null || dt.Rows[0][3] == null) return null;//G和Z必须有
                pd.Gorf = Convert.ToDouble(dt.Rows[0][1]);
                pd.ZorR = Convert.ToDouble(dt.Rows[0][3]);
                if (dt.Rows[0][2] == null)
                {
                    pd.Korb = 0;
                }
                else
                {
                    pd.Korb = Convert.ToDouble(dt.Rows[0][2]);
                    pd.RorT = Convert.ToDouble(dt.Rows[0][4]);
                }
                string instype = dt.Rows[0][0].ToString();
                if (instype == "振弦式")
                {
                    pd.InsCalcType = CalcType.ShakeString;
                }
                else if (instype == "差阻式")
                {
                    pd.InsCalcType = CalcType.DifBlock;
                    pd.TemperatureRead = Convert.ToDouble(dt.Rows[0][5]);
                    pd.ZeroR = Convert.ToDouble(dt.Rows[0][6]);
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
        /// <summary>从数据库读取参数
        /// </summary>
        /// <param name="Survey_point_Number">测点名称</param>
        /// <param name="表名"></param>
        /// <returns></returns>
        public override ParamData GetParam(string Survey_point_Number, string tablename)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Instru_Expansion_b,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance 
                                from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number);
            var SqlHelper = CSqlServerHelper.GetInstance();
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                ParamData pd = new ParamData();
                pd.SurveyPoint = Survey_point_Number;
                if (dt.Rows[0][1] == null || dt.Rows[0][3] == null) return null;//G和Z必须有
                pd.Gorf = Convert.ToDouble(dt.Rows[0][1]);
                pd.ZorR = Convert.ToDouble(dt.Rows[0][3]);
                if (dt.Rows[0][2] == null)
                {
                    pd.Korb = 0;
                }
                else
                {
                    pd.Korb = Convert.ToDouble(dt.Rows[0][2]);
                    pd.RorT = Convert.ToDouble(dt.Rows[0][4]);
                }
                string instype = dt.Rows[0][0].ToString();
                if (instype == "振弦式")
                {
                    pd.InsCalcType = CalcType.ShakeString;
                }
                else if (instype == "差阻式")
                {
                    pd.InsCalcType = CalcType.DifBlock;
                    pd.TemperatureRead = Convert.ToDouble(dt.Rows[0][5]);
                    pd.ZeroR = Convert.ToDouble(dt.Rows[0][6]);
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
    public class Fiducial_Steel_Bar : BaseInstrument
    {
        public Fiducial_Steel_Bar()
        {
            base.InsType = InstrumentType.Fiducial_Steel_Bar;
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
    /// 土压力计
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
            return base.GetParam(Survey_point_Number, this.InsType.ToString());
        }
    }

    /// <summary>
    /// 锚索测力计
    /// </summary>
    public class Fiducial_Anchor_Cable : BaseInstrument
    {
        public Fiducial_Anchor_Cable()
        {
            base.InsType = InstrumentType.Fiducial_Anchor_Cable;
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
            double result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
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
}
