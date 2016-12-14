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

namespace LoadDataCalc
{
    /// <summary>仪器基类
    /// </summary>
    public abstract class BaseInstrument
   {
        /// <summary>
        /// 计算方法类型枚举和函数委托枚举字典
        /// </summary>
        public  Dictionary<CalcType, Func<ParamData, SurveyData,string,double>> FuncDic = new Dictionary<CalcType, Func<ParamData, SurveyData,string, double>>();
        /// <summary>特殊计算方法的缓存
        /// </summary>
        public List<CalcExpand> CalcExpandList = new List<CalcExpand>();

        /// <summary>仪器类型
        /// </summary>
        public InstrumentType InsType = InstrumentType.BaseInstrument;


        public  BaseInstrument()
        {
            FuncDic.Add(CalcType.DifBlock, DifBlockExpand);
            FuncDic.Add(CalcType.ShakeString, ShakeStringExpand);
            FuncDic.Add(CalcType.AutoDefine, AutoDefined);
            FuncDic.Add(CalcType.CalcExpand1, Expand1);
            FuncDic.Add(CalcType.CalcExpand2, Expand2);
            FuncDic.Add(CalcType.CalcExpand3, Expand3);
        }

        public double DifBlockExpand(ParamData param, SurveyData data, string expression)
        {
            return DifBlock(param, data);
        }
        public double ShakeStringExpand(ParamData param, SurveyData data, string expression)
        {
            return ShakeString(param, data);
        }

        /// <summary>单点差阻式默认计算方法 
        /// </summary>
        /// <param name="param">数据</param>
        /// <param name="expand">扩展数据</param>
        /// <returns></returns>
        public virtual double DifBlock(ParamData param,SurveyData data,params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(TemperatureRead*(Survey_RorT-ZeroR)-RorT)
            double result = 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) +
                    param.Korb * (param.TemperatureRead * (data.Survey_RorT - param.ZeroR) - param.TemperatureRead * (param.RorT - param.ZeroR));
            }
            data.ResultReading = result;//这里要乘以系数
            data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);
            data.LoadReading = result;
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
            double result = 0;
            double Tcorrect = (data.Survey_RorT != 0) ? param.Korb * (data.Survey_RorT - param.RorT) : 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR>4000)//模数
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
            data.ResultReading = result;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;

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
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Tempera_Revise_K,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename,Survey_point_Number);
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
        public virtual int WriteSurveyToDB(List<PointSurveyData> datas)
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
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            if(Config.IsAuto) dt.Columns.Add("RecordMethod");
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
                    dr["Temperature"] = (float)surveydata.Survey_RorT;
                    dr["Frequency"] = (float)surveydata.Survey_ZorR;
                    if (Encoding.Default.GetBytes(surveydata.Remark).Length > 60)
                    {
                        surveydata.Remark = "";
                    }
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
        /// <summary> 写成果表
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public virtual int WriteResultToDB(List<PointSurveyData> datas)
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
                    dr["Temperature"] = Math.Round(surveydata.Tempreture,2);
                    dr["loadReading"] = Math.Round(surveydata.LoadReading,4);
                    dr["ResultReading"] = Math.Round(surveydata.ResultReading,4);
                    dr["Remark"] = surveydata.Remark;
                    dr["UpdateTime"] = DateTime.Now;
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
        /// <summary> 扩展方法1 对应CalcExpand1,子类有特殊计算方法才继承
        /// </summary>
        /// <param name="param"></param>
        /// <param name="data"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual double Expand1(ParamData param, SurveyData data, string expression)
        {
            return 0;
        }
        /// <summary> 扩展方法2 对应CalcExpand2
        /// </summary>
        /// <param name="param"></param>
        /// <param name="data"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual double Expand2(ParamData param, SurveyData data, string expression)
        {
            return 0;
        }

        /// <summary> 扩展方法3 对应CalcExpand3
        /// </summary>
        /// <param name="param"></param>
        /// <param name="data"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual double Expand3(ParamData param, SurveyData data, string expression)
        {
            return 0;
        }

        protected double ConvetToData(object obj)
        {
            if(obj!=null&&obj!=DBNull.Value)
            {
                return Convert.ToDouble(obj);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 根据G 判断是否是模数 Math.Abs(G * 100000) > 1为模数
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        protected bool CheckIsMoshu(double G)
        {
            return (Math.Abs(G * 10000) > 1);
        }
        /// <summary>
        /// 根据G 判断系数Math.Abs(G * 100) > 1 为kpa  返回0.001 
        /// 用于渗压计
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        protected double CheckIsMpa(double G)
        {
            if(Math.Abs(G * 100) < 1)//MPA
            {
                return 1;
            }
            else//KPA
            {
                return 0.001;
            }
        }
    }


    /// <summary> 参数类
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
        /// <summary>
        /// 自定义的计算公式,默认为空
        /// </summary>
        public string Expression = null;

        public InstrumentType InsType = InstrumentType.BaseInstrument;
        /// <summary>
        /// KPA转MPA 转换系数
        /// 数据为MPA时为1  数据为KPA时为0.001
        /// </summary>
        public double KpaToMpa = 1;
        /// <summary>
        /// 渗压计系数
        /// </summary>
        public double WaterHead_Coeffi_C = 0.102040816;
        /// <summary>
        /// 钢板计钢板模数
        /// </summary>
        public double Elastic_Modulus_E = 1;
        /// <summary>
        /// 常数项b
        /// </summary>
        public double Constant_b;
        /// <summary>
        /// 是否特殊标识
        /// </summary>
        public bool Special = false;
        /// <summary>
        /// 多点仪器的第二列
        /// </summary>
        public string Ins_Serial;
        /// <summary>
        /// 是否有无应力计//应变计和应变计组
        /// </summary>
        public bool IsHasNonStress = true;

        /// <summary>
        /// 是否式电位器式//计算方式为和振弦式的模数一样
        /// </summary>
        public bool IsPotentiometer = false;

        /// <summary>
        /// 是否是模数
        /// </summary>
        public bool IsMoshu = false;
        /// <summary>
        /// 钢筋计直径
        /// </summary>
        public double Steel_Diameter_L;

        /// <summary>
        /// 混凝土膨胀系数//应变计无应力计专用
        /// </summary>
        public double Concrete_Expansion_ac;

        public string NonStressNumber;
        /// <summary>
        /// 多点位移计的参数
        /// </summary>
        public Dictionary<string, ParamData> MParamData = new Dictionary<string, ParamData>();

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
    /// 锚索测力计参数
    /// </summary>
    public class Anchor_CableParam : ParamData
    {
        /// <summary>
        /// 锚索测力计计划荷载
        /// </summary>
        public double Plan_Loading;
        /// <summary>
        /// 锚索测力计最大荷载
        /// </summary>
        public double MAX_Loading;
        /// <summary>
        /// 锚索测力计锁定荷载
        /// </summary>
        public double Lock_Value;
        /// <summary>
        /// 弦数
        /// </summary>
        public int Sum;
    }
    /// <summary>
    /// 固定测斜仪参数
    /// </summary>
    public class Survey_Slant_FixedParam : ParamData
    {
        public double A_US;
        public double A_DS;
        public double A_BS;
        public double A_C0;
        public double A_C1;
        public double A_C2;
        public double A_C3;
        public double A_F0;
        public double A_F1;
        public double B_US;
        public double B_DS;
        public double B_BS;
        public double B_C0;
        public double B_C1;
        public double B_C2;
        public double B_C3;
        public double B_F0;
        public double AB_F1;
        public double T_US;
        public double T_DS;
        public double T_BS;
    }
    
    /// <summary>
    /// 测点数据
    /// </summary>
    public class PointSurveyData
    {

        public PointSurveyData(InstrumentType ins)
        {
            m_InsType = ins;
        }
        /// <summary>
        /// 数据对应的仪器类型
        /// </summary>
        public InstrumentType InsType
        {
            get { return m_InsType; }
        }
        private InstrumentType m_InsType;
        /// <summary>
        /// 测点名
        /// </summary>
        public string SurveyPoint;
        /// <summary>
        /// 当前测点的测量数据
        /// </summary>
        public List<SurveyData> Datas = new List<SurveyData>();

        /// <summary>是否计算成功
        /// </summary>
        public bool IsCalc = false;
        /// <summary> 测点所在的文件
        /// </summary>
        public string ExcelPath = "";
    }
    /// <summary>
    /// 一组测量值
    /// </summary>
    public class SurveyData
    {
        /// <summary>ZorR 对应的测值//该值为直接读取的值，可能为频率或者模数
        /// </summary>
        public double Survey_ZorR;
        /// <summary>RorT对应的测值
        /// </summary>
        public double Survey_RorT;
        /// <summary>观测时间
        /// </summary>
        public DateTime SurveyDate = new DateTime();

        /// <summary>计算出来的温度
        /// </summary>
        public double Tempreture;

        /// <summary>第一个计算结果,多点位移计的成果值
        /// </summary>
        public double LoadReading;
        /// <summary> 最终计算结果
        /// </summary>
        public double ResultReading;
        /// <summary>是否计算，校准参数不参与计算//备用 ，暂时没用到
        /// </summary>
        public bool IsCalc = true;
        /// <summary>备注信息
        /// </summary>
        public string Remark = "";

        /// <summary> 深度，多点位移计
        /// </summary>
        public double Drill_Depth;

        /// <summary>多点仪器的数据,词典中的测点数据用来存测值，其他字段不存值
        /// </summary>
        public Dictionary<string, SurveyData> MultiDatas = new Dictionary<string, SurveyData>();
        /// <summary>
        /// 模数，当直接读取的值为模数时，与Survey_ZorR相同，否则为平方后/1000的值
        /// </summary>
        public double Survey_ZorRMoshu;

        /// <summary> 用于存储应变计和应变计组对应的无应力计的数据
        /// </summary>
        public SurveyData NonStressSurveyData=null;

        /// <summary>
        /// 锚索测力计锁定值
        /// </summary>
        public double AfterLock;
        /// <summary>
        /// 锚索测力计计划锁定值
        /// </summary>
        public double PlanLock;
        /// <summary>
        /// 锚索测力计最大锁定值
        /// </summary>
        public double ResultLock;

        /// <summary>应变计组的参数//向家坝改正用的
        /// </summary>
        public double StrainGroup_x;
        public double StrainGroup_y;
        public double StrainGroup_z;
        public double StrainGroup_xz;

        /// <summary>excel中的计算结果，对比用
        /// </summary>
        public double ExcelResult;

        /// <summary>
        /// 锚索测力计的平均值
        /// </summary>
        public double Average;
        //计算出的平均值
        public double clacAverage;
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
    /// 固定测斜仪测值
    /// </summary>
    public class Survey_Slant_FixedSurveyData : SurveyData
    {
        public double Reading_A;
        public double Reading_B;
        public double loadReading_A;
        public double loadReading_B;
        public double This_A;
        public double This_B;
        public double Sum_A;
        public double Sum_B;
    }

    /// <summary>
    /// 大地测量数据，直接读取结果
    /// </summary>
    public class Earth_MeasureData : SurveyData
    {
        public double X;
        public double Y;
        public double H;
        public double DeltX;
        public double DeltY;
        public double DeltH;
        public double SumX;
        public double SumY;
        public double SumH;
    }

    public enum CalcType
    {
        /// <summary>
        /// 差阻
        /// </summary>
        DifBlock=0,
        /// <summary>
        /// 振弦
        /// </summary>
        ShakeString=1,
        /// <summary>
        /// 自定义
        /// </summary>
        AutoDefine=2,
        /// <summary> 扩展方法1
        /// </summary>
        CalcExpand1=3,
        /// <summary> 扩展方法2
        /// </summary>
        CalcExpand2=4,
        /// <summary> 扩展方法3
        /// </summary>
        CalcExpand3=5
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


    public class Anchor_CableCalc
    {
        /// <summary>
        /// 掉弦索引
        /// </summary>
        public Dictionary<int, List<int>> DicCach = new Dictionary<int, List<int>>();
        /// <summary>
        /// 新的基准
        /// </summary>
        public double Average;
        /// <summary>
        /// 算出来的转换系数
        /// </summary>
        public double Coefficient;
    }
}
