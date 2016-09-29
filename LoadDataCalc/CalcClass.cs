/*
 * 仪器基类、工厂类、枚举
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Data;
using System.Reflection;

namespace LoadDataCalc
{
    /// <summary>仪器构造类
    /// </summary>
    public class CalcFactoryClass
    {
        /// <summary>构造不同仪器对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static BaseInstrument CreateInstCalc(InstrumentType type)
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            string completeName ="LoadDataCalc."+type.ToString();
            var ins=ass.CreateInstance(completeName) as BaseInstrument;
            ins.SqlHelper = CSqlServerHelper.GetInstance();
            return ins; 

        }

    }
    /// <summary>仪器基类
    /// </summary>
    public  class BaseInstrument
   {
        public CSqlServerHelper SqlHelper = null;
        /// <summary>仪器类型
        /// </summary>
        public InstrumentType InsType = InstrumentType.BaseInstrument;
        /// <summary>单点差阻式默认计算方法 
        /// </summary>
        /// <param name="param">数据</param>
        /// <param name="expand">扩展数据</param>
        /// <returns></returns>
        public virtual double DifBlock(ParamData param,SurveyData data,params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(TemperatureRead*(Survey_RorT-ZeroR)-RorT)
            double result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (param.TemperatureRead * (data.Survey_RorT - param.ZeroR) - param.RorT);
            return result;
        }
        /// <summary>单点振弦式默认计算方法
        /// </summary>
        /// <param name="param">数据</param>
        /// <param name="expand">扩展数据</param>
        /// <returns></returns>
        public virtual double ShakeString(ParamData param,SurveyData data,params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            double result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
            return result;
        }
        /// <summary>自定义计算方法，表达式中的参数必须为ParamData类中的名字
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public virtual double AutoDefined(ParamData param,SurveyData data,string expression)
        {
            foreach (var pm in ParamData.ParamList)
            {
                expression.Replace(pm, param.GetValue(pm));
            }
            try
            {
                double result = double.Parse(new DataTable().Compute(expression, "").ToString());
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>从数据库读取参数
        /// </summary>
        /// <param name="Survey_point_Number">测点名称</param>
        /// <param name="表名"></param>
        /// <returns></returns>
        public virtual ParamData GetParam(string Survey_point_Number,string tablename)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Tempera_Revise_K,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance 
                                from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename,Survey_point_Number);
            var dt = SqlHelper.SelectData(sql);
            if (dt.Rows.Count < 1) return null;
            try
            {
                ParamData pd = new ParamData();
                pd.SurveyPoint = Survey_point_Number;
                if (dt.Rows[0][1] == null || dt.Rows[0][3]==null) return null;//G和Z必须有
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

    /// <summary> 单点参数类
    /// </summary>
    public class ParamData 
    {
        /// <summary>
        /// G值，灵敏度f
        /// </summary>
        public double Gorf;
        /// <summary>
        /// K值，温度系数
        /// </summary>
        public double Korb;
        /// <summary>
        /// 频率，模数基值
        /// </summary>
        public double ZorR;
        /// <summary>
        /// 温度基准/电阻基准？
        /// </summary>
        public double RorT;
        /// <summary>
        /// 温度读数
        /// </summary>
        public double TemperatureRead;
        /// <summary>
        /// 零度电阻
        /// </summary>
        public double ZeroR;
        /// <summary>
        /// 测点名
        /// </summary>
        public string SurveyPoint;
        /// <summary>
        /// 仪器类型振弦/差阻
        /// </summary>
        public CalcType InsCalcType = CalcType.ShakeString;

        public InstrumentType InsType = InstrumentType.BaseInstrument;

        public static string[] ParamList = new[] { "Gorf", "Korb", "ZorR", "RorT", "TemperatureRead", "ZeroR", "Survey_ZorR", "Survey_RorT" };

        /// <summary>根据变量名获取变量值
        /// </summary>
        /// <param name="ParamName"></param>
        /// <returns></returns>
        public string GetValue(string ParamName)
        {
            return this.GetType().GetField(ParamName).GetValue(this).ToString();
        }

    }
    /// <summary>
    /// 测点数据
    /// </summary>
    public class PointSurveyData
    {
        /// <summary>
        /// 测点名
        /// </summary>
        public string SurveyPoint;
        /// <summary>
        /// 当前测点的测量数据
        /// </summary>
        public List<SurveyData> Datas;
    }
    /// <summary>
    /// 一组测量值
    /// </summary>
    public class SurveyData
    {
        /// <summary>
        /// ZorR 对应的测值
        /// </summary>
        public double Survey_ZorR;
        /// <summary>
        /// RorT对应的测值
        /// </summary>
        public double Survey_RorT;
        /// <summary>
        /// 观测时间
        /// </summary>
        public DateTime SurveyDate = new DateTime();

        /// <summary>
        /// 第一个计算结果
        /// </summary>
        public double LoadReading;
        /// <summary>
        /// 最终计算结果
        /// </summary>
        public double ResultReading;
        /// <summary>
        /// 是否计算，校准参数不参与计算
        /// </summary>
        public bool IsCalc = true;


        public static string[] ParamList = new[] {"Survey_ZorR", "Survey_RorT" };

        /// <summary>根据变量名获取变量值
        /// </summary>
        /// <param name="ParamName"></param>
        /// <returns></returns>
        public string GetValue(string ParamName)
        {
            return this.GetType().GetField(ParamName).GetValue(this).ToString();
        }
    }

    /// <summary>
    /// 多点参数类
    /// </summary>
    public class MultiSurveyParamData : ParamData
    {
        
    }

    public enum CalcType
    {
        /// <summary>
        /// 差阻
        /// </summary>
        DifBlock,
        /// <summary>
        /// 振弦
        /// </summary>
        ShakeString,
        /// <summary>
        /// 
        /// </summary>
        unknown

    }
    /// <summary> 仪器类型枚举
    /// </summary>
    public enum InstrumentType
    {
        /// <summary>
        /// 未知仪器
        /// </summary>
        [Description("未知")]
        BaseInstrument,
        /// <summary>
        /// 钢丝位移计
        /// </summary>
        [Description("钢丝位移计")]
        Fiducial_SteelWire_Displacement,
        /// <summary>
        /// 双金属标
        /// </summary>
        [Description("双金属标")]
        Fiducial_Double_Metal_Target,
        /// <summary>
        /// 大地测量
        /// </summary>
        [Description("大地测量")]
        Fiducial_Earth_Measure,
        /// <summary>
        /// 正倒垂线
        /// </summary>
        [Description("正倒垂线")]
        Fiducial_Inverst_Vertical,
        /// <summary>
        /// 视准线
        /// </summary>
        [Description("视准线")]
        Fiducial_Watch_Directrix,
        /// <summary>
        /// 引张线
        /// </summary>
        [Description("引张线")]
        Fiducial_Flex_Line,
        /// <summary>
        /// 激光准直
        /// </summary>
        [Description("激光准直")]
        Fiducial_Laser_Collimation,
        /// <summary>
        /// 静力水准
        /// </summary>
        [Description("静力水准")]
        Fiducial_Statics_Level,
        /// <summary>
        /// 伸缩仪
        /// </summary>
        [Description("伸缩仪")]
        Fiducial_Extension,
        /// <summary>
        /// 测斜仪
        /// </summary>
        [Description("测斜仪")]
        Fiducial_Survey_Slant,
        /// <summary>
        /// 单点位移计
        /// </summary>
        [Description("单点位移计")]
        Fiducial_Single_Displacement,
        /// <summary>
        /// 多点位移计
        /// </summary>
        [Description("多点位移计")]
        Fiducial_Multi_Displacement,
        /// <summary>
        /// 滑动测微计
        /// </summary>
        [Description("滑动测微计")]
        Fiducial_Glide_Micrometer,
        /// <summary>
        /// 基岩变形计
        /// </summary>
        [Description("基岩变形计")]
        Fiducial_Basic_Rock_Distortion,
        /// <summary>
        /// 电磁沉降环
        /// </summary>
        [Description("电磁沉降环")]
        Fiducial_Elect_Settlement_Gauge,
        /// <summary>
        /// 水管式沉降仪
        /// </summary>
        [Description("水管式沉降仪")]
        Fiducial_Hose_Settlement_Gauge,
        /// <summary>
        /// 引张线式水平位移计
        /// </summary>
        [Description("引张线式水平位移计")]
        Fiducial_Flex_Displacement,
        /// <summary>
        /// 土体位移计
        /// </summary>
        [Description("土体位移计")]
        Fiducial_Soil_Displacement,
        /// <summary>
        /// 测缝计
        /// </summary>
        [Description("测缝计")]
        Fiducial_Measure_Aperture,
        /// <summary>
        /// 渗压计
        /// </summary>
        [Description("渗压计")]
        Fiducial_Leakage_Pressure,
        /// <summary>
        /// 测压管
        /// </summary>
        [Description("测压管")]
        Fiducial_MeasureStress_Hose,
        /// <summary>
        /// 量水堰
        /// </summary>
        [Description("量水堰")]
        Fiducial_MeasureWater_Weir,
        /// <summary>
        /// 水位计
        /// </summary>
        [Description("水位计")]
        Fiducial_Water_Level,
        /// <summary>
        /// 应变计
        /// </summary>
        [Description("应变计")]
        Fiducial_Strain_Gauge,
        /// <summary>
        /// 应变计组
        /// </summary>
        [Description("应变计组")]
        Fiducial_Strain_Group,
        /// <summary>
        /// 无应力计
        /// </summary>
        [Description("无应力计")]
        Fiducial_Nonstress,
        /// <summary>
        /// 钢筋计
        /// </summary>
        [Description("钢筋计")]
        Fiducial_Steel_Bar,
        /// <summary>
        /// 锚杆应力计
        /// </summary>
        [Description("锚杆应力计")]
        Fiducial_Anchor_Pole,
        /// <summary>
        /// 压应力计
        /// </summary>
        [Description("压应力计")]
        Fiducial_Press_Stress,
        /// <summary>
        /// 土压力计
        /// </summary>
        [Description("土压力计")]
        Fiducial_Soil_Stres,
        /// <summary>
        /// 锚索测力计
        /// </summary>
        [Description("锚索测力计")]
        Fiducial_Anchor_Cable,
        /// <summary>
        /// 钢板计
        /// </summary>
        [Description("钢板计")]
        Fiducial_Armor_plate,
        /// <summary>
        /// 温度计
        /// </summary>
        [Description("温度计")]
        Fiducial_Temperature,
        /// <summary>
        /// 裂缝计
        /// </summary>
        [Description("裂缝计")]
        Fiducial_Aperture,
        /// <summary>
        /// 竖直传高
        /// </summary>
        [Description("竖直传高")]
        Fiducial_Vertical_Height,
        /// <summary>
        /// 日平均气温
        /// </summary>
        [Description("日平均气温")]
        Fiducial_Environment_Temperature,
        /// <summary>
        /// 上游水位
        /// </summary>
        [Description("上游水位")]
        Fiducial_Environment_UW_level,
        /// <summary>
        /// 日降雨量
        /// </summary>
        [Description("日降雨量")]
        Fiducial_Environment_Rainfall,
        /// <summary>
        /// 姊妹杆
        /// </summary>
        [Description("姊妹杆")]
        Fiducial_Sisters_Pole,
        /// <summary>
        /// 位错计
        /// </summary>
        [Description("位错计")]
        Fiducial_Dislocation,
        /// <summary>
        /// 多点锚杆应力计
        /// </summary>
        [Description("多点锚杆应力计")]
        Fiducial_Multi_Anchor_Pole,
        /// <summary>
        /// 表面测缝计
        /// </summary>
        [Description("表面测缝计")]
        Fiducial_Surface_Measure_Aperture,
        /// <summary>
        /// 弦式沉降仪
        /// </summary>
        [Description("弦式沉降仪")]
        Fiducial_VST_Settlement_Gauge,
        /// <summary>
        /// 多向土压力计
        /// </summary>
        [Description("多向土压力计")]
        Fiducial_Multi_Soil_Stres,
        /// <summary>
        /// 双向测缝计
        /// </summary>
        [Description("双向测缝计")]
        Fiducial_TwoWays_Measure_Aperture,
        /// <summary>
        /// 横梁式沉降仪
        /// </summary>
        [Description("横梁式沉降仪")]
        Fiducial_CBT_Settlement_Gauge,
        /// <summary>
        /// 剪变形计
        /// </summary>
        [Description("剪变形计")]
        Fiducial_Shear_Deformation_Meter,
        /// <summary>
        /// 弦式沉降系统
        /// </summary>
        [Description("弦式沉降系统")]
        Fiducial_VST_Settlement_System,
        /// <summary>
        /// 土体位移计组
        /// </summary>
        [Description("土体位移计组")]
        Fiducial_Soil_Displacement_Group,
        /// <summary>
        /// 固定测斜仪
        /// </summary>
        [Description("固定测斜仪")]
        Fiducial_Survey_Slant_Fixed,
        /// <summary>
        /// 下游水位
        /// </summary>
        [Description("下游水位")]
        Fiducial_Environment_DW_level,
        /// <summary>
        /// 心墙填筑高程
        /// </summary>
        [Description("心墙填筑高程")]
        Fiducial_Environment_Elevation,
        /// <summary>
        /// 精密水准
        /// </summary>
        [Description("精密水准")]
        Fiducial_Geometry_Level,
    }
    /// <summary> 获取枚举的描述信息
    /// </summary>
    public static class Expand
    {
        /// <summary>
        /// 获取枚举描述特性值
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumerationValue">枚举值</param>
        /// <returns>枚举值的描述</returns>
        public static string GetDescription<TEnum>(this TEnum enumerationValue)
           where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue必须是一个枚举值", "enumerationValue");
            }

            //使用反射获取该枚举的成员信息
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //返回枚举值得描述信息
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //如果没有描述特性的值，返回该枚举值得字符串形式
            return enumerationValue.ToString();
        }
    }
         
}
