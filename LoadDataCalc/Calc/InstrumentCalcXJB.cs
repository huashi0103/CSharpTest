/*
 * 向家坝数据计算类，对应ProCode=XJB
 * 考证表中的参数默认频率都转换为模数
 * 只用实现向家坝涉及的仪器就可以了
 * 测值表中测的是什么 就写什么
 * 暂未使用
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LoadDataCalc
{

    /// <summary>渗压计
    /// </summary>
    public class Fiducial_Leakage_PressureXJB : Fiducial_Leakage_Pressure
    {
        public Fiducial_Leakage_PressureXJB()
            : base()
        {
            base.InsType = InstrumentType.Fiducial_Leakage_Pressure;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            //温度改正用减
            double result = (param.Gorf * (data.Survey_ZorR - param.ZorR) -GetTCorrect(param,data)) * param.KpaToMpa;

            data.LoadReading = result;//渗透压力LoadReading

            if (data.LoadReading > 0)
            {
                data.ResultReading = 0;
            }
            else
            {
                data.ResultReading = -1 * result * param.WaterHead_Coeffi_C;//这里要乘以系数每种仪器不一样
            }
            data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);//温度
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            data.SW = data.ResultReading + param.Elevation;
            return result;
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            //温度改正是减的
            double result = 0;
            //录入的是频率或者模数
            if (data.Survey_ZorR > 3000)//用Z值判断是频率还是模数
            {
                result = (param.Gorf * (data.Survey_ZorR - param.ZorR) - param.Korb * (data.Survey_RorT - param.RorT));
            }
            else
            {

                result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000.0 - param.ZorR) -
                    param.Korb * (data.Survey_RorT - param.RorT);
            }

            // if (Math.Abs(param.Gorf * 100) > 1) param.KpaToMpa = 0.001;
            result += param.Constant_b;
            result = result * param.KpaToMpa;
            data.LoadReading = result;
            if (data.LoadReading > 0)
            {
                data.ResultReading = 0;//正的 直接为零
            }
            else
            {
                data.ResultReading = -1*result * param.WaterHead_Coeffi_C;//负的变成正的
            }
            //data.ExcelResult = data.ExcelResult * 0.001;//把excel中的结果乘以0.001，方便对数据
            data.Tempreture = data.Survey_RorT;

            data.SW = data.ResultReading + param.Elevation;
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

                return pd;
            }
            catch
            {
                return null;
            }
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
                    dr["Temperature"] = Math.Round(surveydata.Tempreture, 2);
                    dr["loadReading"] = Math.Round(surveydata.LoadReading*0.001, 4);//把第一个结果除以1000
                    dr["ResultReading"] = Math.Round(surveydata.ResultReading, 4);
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
    /// <summary>单点位移计
    /// </summary>
    public class Fiducial_Single_DisplacementXJB : Fiducial_Single_Displacement
    {
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
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            if (param.SurveyPoint.StartsWith("ID-"))//特殊的计算方式
            {
                data.LoadReading = calcOnePointExpand(param, data);
                data.Tempreture = data.Survey_RorT;
                return data.LoadReading;
            }

            double result = 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 5000)
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
                }
                else
                {
                    result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000 - param.ZorR) +
                        param.Korb * (data.Survey_RorT - param.RorT);
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
 
    }

    /// <summary> 多点位移计
    /// </summary>
    public class Fiducial_Multi_DisplacementXJB : Fiducial_Multi_Displacement
    {
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
                                 param.Korb * (param.TemperatureRead * (Mdata.Survey_RorT - param.ZeroR) - param.TemperatureRead * (param.RorT - param.ZeroR)) - firstvalue;
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
            foreach (var dic in Mdatas)
            {
                if (dic.Key.EndsWith("A"))
                {
                    return dic.Key;
                }
            }
            return null;
        }
        //正常的计算方式
        private double calcOnePoint(ParamData pd, SurveyData sd, MultiDisplacementCalc calc)
        {
            if (Math.Abs(sd.Survey_ZorR) < 1)
            {
                //第一种计算方式或者不是基准值直接返回0
                if (calc.MCalcType == 1 || calc.IsStanderd != 1) return 0;
                string surveypoint = pd.SurveyPoint;
                string loadindex = "loadReading0";
                if (calc.R0IsZero == 0)//从浅到深
                {
                    loadindex = "loadReading" + pd.MParamData.Count.ToString();
                }
                return getLastStandValue(surveypoint, loadindex);

            }
            double result = 0;
            double tcorrect = GetTCorrect(pd, sd);
            if (calc.IsMoshu == 1)//是否是模数
            {
                result = pd.Gorf * (sd.Survey_ZorR - pd.ZorR) + tcorrect;
                
            }
            else
            {
                result = pd.Gorf * (Math.Pow(sd.Survey_ZorR, 2) / 1000 - pd.ZorR) + tcorrect;
            }
            sd.Survey_ZorRMoshu = sd.Survey_ZorR;
            return result;
        }
        //特殊的计算方式/对应Mcalctype=2
        private double calcOnePointExpand(ParamData pd, SurveyData sd, MultiDisplacementCalc calc = null)
        {
            if (Math.Abs(sd.Survey_ZorR) < 1 && calc.IsStanderd == 1)
            {
                string loadindex = "loadReading0";
                if (calc.R0IsZero == 0)
                {
                    loadindex = "loadReading" + pd.MParamData.Count.ToString();
                }
                return getLastStandValue(pd.SurveyPoint, loadindex);
            }
            return ((int)(pd.Gorf * (sd.Survey_ZorR - pd.ZorR) * 100 + 0.5)) / 100.0 + GetTCorrect(pd, sd);
        }
        //没有基准的直接遍历计算
        private void calcOneGroupExpand(ParamData Mparam, SurveyData Mdata, MultiDisplacementCalc[] Mcalc,
            Func<ParamData, SurveyData, MultiDisplacementCalc, double> clacAction)
        {
            foreach (var dic in Mdata.MultiDatas)
            {
                SurveyData data = Mdata.MultiDatas[dic.Key];
                ParamData pd = Mparam.MParamData[dic.Key];
                var tcalc = Mcalc.FirstOrDefault(c => c.Ins_serial == dic.Key);
                data.LoadReading = clacAction(pd, data, tcalc);
                data.Survey_ZorRMoshu = data.Survey_ZorR;
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
                        result = calcOnePoint(pd, sd, calc);//计算基准值

                        foreach (var dic in Mdata.MultiDatas)
                        {
                            SurveyData data = Mdata.MultiDatas[dic.Key];
                            pd = Mparam.MParamData[dic.Key];
                            if (dic.Key != calc.Ins_serial)
                            {
                                var tcalc = Mcalc.FirstOrDefault(c => c.Ins_serial == dic.Key);
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
                                data.LoadReading = (calc.R0IsZero == 0) ? result : 0;
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
                        foreach (var dic in Mdata.MultiDatas)
                        {
                            sd = Mdata.MultiDatas[dic.Key];
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
                                if (Math.Abs(sd.Survey_ZorR) < 1) sd.LoadReading = 0;//基准值挂了
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
            //if (Mcalc[0].R0IsZero != 0)//倒序排列下//默认是从深到浅。当孔口为0的时候为从浅到深
            //{
            //    Dictionary<string, SurveyData> Dic = new Dictionary<string, SurveyData>();
            //    List<string> li = new List<string>(Mdata.MultiDatas.Keys);
            //    for (int i = li.Count - 1; i >= 0; i--)
            //    {
            //        Dic.Add(li[i], Mdata.MultiDatas[li[i]]);
            //    }
            //    Mdata.MultiDatas = Dic;
            //}

        }
        public override double ShakeString(ParamData Mparam, SurveyData Mdata, params double[] expand)
        {
            double result = 0;
            var Mcalc = Config.MultiDisplacementCalcs.GetBySurveypoint(Mparam.SurveyPoint);
            if (Mcalc.Length > 0)
            {
                calcOneGroup(Mparam, Mdata, Mcalc);
            }
            else
            {
                calcOneGroupNull(Mparam, Mdata);
            }
            Mdata.Tempreture = Mdata.Survey_RorT;
            //倒序排列下//默认是从深到浅
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
            double result = 0;
            string skey = GetStanderd(Mdata.MultiDatas);//获取基准值
            if (skey != null)
            {
                SurveyData sd = Mdata.MultiDatas[skey];
                ParamData pd = Mparam.MParamData[skey];
                if (Math.Abs(sd.Survey_ZorR) < 1)
                {
                    var list = new List<string>(Mdata.MultiDatas.Keys);
                    string loadindex = "loadReading" + (list.IndexOf(skey) + 1).ToString();
                    result = getLastStandValue(Mparam.SurveyPoint, loadindex);
                }
                else
                {
                    result = base.ShakeString(pd, sd);
                }
                foreach (var di in Mdata.MultiDatas)
                {
                    if (di.Key == skey) continue;
                    sd = di.Value;
                    pd = Mparam.MParamData[di.Key];
                    result = base.ShakeString(pd, sd);
                    sd.LoadReading = result - sd.LoadReading;
                    sd.Survey_ZorRMoshu = sd.Survey_ZorR;
                }
            }
            else
            {
                foreach (var di in Mdata.MultiDatas)
                {
                    SurveyData sd = di.Value;
                    ParamData pd = Mparam.MParamData[di.Key];
                    base.ShakeString(pd, sd);
                   sd.Survey_ZorRMoshu=sd.Survey_ZorR;

                }
            }
            Mdata.LoadReading = 0;

        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
    }

    /// <summary> 应变计组
    /// </summary>
    public class Fiducial_Strain_GroupXJB : Fiducial_Strain_Group
    {

        /// <summary>
        /// 5向应变计组平衡计算
        /// </summary>
        /// <param name="data"></param>
        public override void CalcGroup(SurveyData data)
        {
            List<string> seriallist = new List<string>(data.MultiDatas.Keys);
            //(1+3)-(2+4)
            //1'=1-delt/4    2'=1+delt/4   3'=1+delt/4    4'=4+delt/4  除以2或者4
            double tempd = 4.0;
            double DeltZ = (data.MultiDatas[seriallist[1]].LoadReading + data.MultiDatas[seriallist[3]].LoadReading) -
                (data.MultiDatas[seriallist[2]].LoadReading + data.MultiDatas[seriallist[4]].LoadReading);
            double c1 = data.MultiDatas[seriallist[1]].LoadReading - DeltZ / tempd;
            double c2z = data.MultiDatas[seriallist[2]].LoadReading + DeltZ / tempd;
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
            Fiducial_Strain_GaugeXJB FStrain_Gauge = new Fiducial_Strain_GaugeXJB();
            //计算应变计
            foreach (var sd in data.MultiDatas)
            {
                FStrain_Gauge.FuncDic[param.MParamData[sd.Key.ToUpper().Trim()].InsCalcType](param.MParamData[sd.Key.ToUpper().Trim()], sd.Value, null);
            }
            if (data.MultiDatas.Keys.Count == 5)
            {
                CalcGroup(data);
            }
            return 1;

        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            Fiducial_Strain_GaugeXJB FStrain_Gauge = new Fiducial_Strain_GaugeXJB();
            foreach (var sd in data.MultiDatas)
            {
                FStrain_Gauge.FuncDic[param.MParamData[sd.Key.ToUpper().Trim()].InsCalcType](param.MParamData[sd.Key.ToUpper().Trim()], sd.Value, null);
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
      
    }

    /// <summary>应变计
    /// </summary>
    public class Fiducial_Strain_GaugeXJB : Fiducial_Strain_Gauge
    {
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            double result = 0;
            double tcorrect = data.Survey_RorT <= 0 ? 0 :
               (param.Korb - param.Concrete_Expansion_ac) * (param.TemperatureRead * (data.Survey_RorT - param.ZeroR) - param.TemperatureRead * (param.RorT - param.ZeroR));
 
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) + tcorrect;
                if (param.ZeroR > 1 && Math.Abs(data.Survey_RorT) > 1) data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);
            }

            data.ResultReading = result;
            data.LoadReading = result;


            if (data.NonStressSurveyData != null && Math.Abs(data.Survey_ZorR) > 1)
            {
                data.LoadReading = data.LoadReading - data.NonStressSurveyData.LoadReading;
            }

            return result;
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(Survey_RorT-RorT)
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            double result = 0;
            double Tcorrect = (data.Survey_RorT > Config.MinTemperature && data.Survey_RorT < Config.MaxTemperature) ?
                            (param.Korb - param.Concrete_Expansion_ac) * (data.Survey_RorT - param.RorT) : 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 5000)
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + Tcorrect;
                }
                else
                {
                    result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000 - param.ZorR) + Tcorrect;
                }
            }
            data.ResultReading = result;
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            if (data.NonStressSurveyData != null && Math.Abs(data.Survey_ZorR) > 1)
            {
                data.LoadReading = data.LoadReading - data.NonStressSurveyData.LoadReading;
            }
            return result;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }

    }

    /// <summary> 无应力计
    /// </summary>
    public class Fiducial_NonstressXJB : Fiducial_Nonstress
    {
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            double result = 0;
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            double tcorrect = data.Survey_RorT <= 0 ? 0 :
                    (param.Korb - param.Concrete_Expansion_ac) * (param.TemperatureRead * (data.Survey_RorT - param.ZeroR) - param.TemperatureRead * (param.RorT - param.ZeroR));
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) + tcorrect;
                if (param.ZeroR > 1 && Math.Abs(data.Survey_RorT) > 1) data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);
            }
            data.ResultReading = result;
            data.LoadReading = result;
            return result;
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            double result = 0;
            double Tcorrect = (data.Survey_RorT > Config.MinTemperature && data.Survey_RorT < Config.MaxTemperature) ?
                (param.Korb - param.Concrete_Expansion_ac) * (data.Survey_RorT - param.RorT) : 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 5000)
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + Tcorrect;
                }
                else
                {
                    result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000 - param.ZorR) + Tcorrect;
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
    }

    /// <summary> 钢筋计
    /// </summary>
    public class Fiducial_Steel_BarXJB : Fiducial_Steel_Bar
    {
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
                //if (data.Survey_ZorR > 2000)//模数
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + Tcorrect;
                }
                //else//频率
                //{
                //    //录入的是频率或者模数
                //    if (Config.IsMoshu)
                //    {

                //        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000.0 - param.ZorR) + Tcorrect;
                //    }
                //    else
                //    {
                //        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) - param.ZorR * param.ZorR) + Tcorrect;
                //    }
                //}
            }
            result += param.Constant_b;
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            //*1000/面积
            data.ResultReading = result * 1000 / (Math.PI * Math.Pow(param.Steel_Diameter_L / 2.0, 2));
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            return result;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }

    }

    /// <summary>锚杆应力计
    /// </summary>
    public class Fiducial_Anchor_PoleXJB : Fiducial_Anchor_Pole
    {
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
                  result = param.Gorf * (data.Survey_ZorR - param.ZorR) + Tcorrect;
            }
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            data.ResultReading = result * 1000 / (Math.PI * Math.Pow(param.Steel_Diameter_L / 2.0, 2));
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            return result;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }

    }

    /// <summary> 压应力计
    /// </summary>
    public class Fiducial_Press_StressXJB : Fiducial_Press_Stress
    {
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {

            double result = 0;
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                //加了一个负号
                result = -param.Gorf * (data.Survey_ZorR - param.ZorR) +GetTCorrect(param,data);
            }
            data.ResultReading = result;//这里要乘以系数
            data.Tempreture = data.Survey_RorT > 0 ? param.TemperatureRead * (data.Survey_RorT - param.ZeroR) : 0;
            data.LoadReading = result;
            data.Survey_ZorRMoshu = data.Survey_ZorR;
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

    }

    /// <summary>土压力计
    /// </summary>
    public class Fiducial_Soil_StresXJB : Fiducial_Soil_Stres
    {
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
    }

    /// <summary>锚索测力计
    /// </summary>
    public class Fiducial_Anchor_CableXJB : Fiducial_Anchor_Cable
    {
        public Fiducial_Anchor_CableXJB()
            : base()
        {
            //DicR0 = Config.GetAnchor_Cable_R0();
        }
  
        //private Dictionary<string, List<double>> DicR0 = new Dictionary<string, List<double>>();
        //掉弦索引缓存
        private List<string> ListCach = new List<string>();
        private string[] sbStrlist =new string[]{ "Reading_Red", "Reading_Black", "Reading_Yellow", "Reading_Blue", "Reading_Ash", "Reading_Purple" };

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
        SurveyData GetLastData(DateTime end, string key,PointSurveyData pd,List<string>IgnoreKey=null)
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
                        if (d.Key != key&& d.Value.Survey_ZorR == 0)
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
        private void GetCOne(ParamData param,SurveyData sd, string key, PointSurveyData pd)
        {
            Anchor_CableParam p = param as Anchor_CableParam;
            var sqlhelper = CSqlServerHelper.GetInstance();
            SurveyData lastSD = GetLastData(sd.SurveyDate, key, pd);
            string ignorestr = "";
            for (int i = 0; i < p.Sum; i++)
            {
                if (key!=i.ToString())
                {
                    ignorestr += " and " + sbStrlist[i] + ">0";
                }
            }

            if (lastSD == null)
            {
                lastSD = new SurveyData();
                string sql = "select top 1 * from Survey_Anchor_Cable where Survey_point_Number='{0}' and {1}>0 {2} order by Observation_Date desc";
                string anchorName = sbStrlist[Convert.ToInt16(key)];
                var dt = sqlhelper.SelectData(string.Format(sql, p.SurveyPoint, anchorName,ignorestr));
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
                    moshu=moshu*moshu/1000.0;
                }
                sum += moshu;
                if (d.Key == key) badZ = moshu;
            }
            coefficient_K= ((sum - badZ) / (p.Sum - 1)) / (sum / p.Sum);
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
                if (dt.Rows.Count < 1) return  ;
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
        public override double GetC(ParamData param, SurveyData data,PointSurveyData pd)
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
                if (dic.Value.Survey_ZorR >= 3000)
                {
                    value += dic.Value.Survey_ZorR;

                }
                else
                {
                    value += Math.Pow(dic.Value.Survey_ZorR, 2) / 1000.0;
                }
                dic.Value.Survey_ZorRMoshu = dic.Value.Survey_ZorR;
            }
            if (param.Sum - count > param.Sum / 2) return result;//掉弦的数量超过半数，不再计算
            value = value / count;//直接算平均值
            value = value / coefficient_K;//把改正系数带入计算
            
            data.clacAverage = value;
            result = param.Gorf * (value - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
            data.Survey_ZorRMoshu = value;
            data.LoadReading = result;
            data.AfterLock = ((param.Lock_Value - result) / param.Lock_Value) * 100;
            data.PlanLock = ((param.Plan_Loading - result) / param.Plan_Loading) * 100;
            data.ResultLock = ((param.MAX_Loading - result) / param.MAX_Loading) * 100;

            data.ResultReading = result;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            return result;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
    }

    /// <summary> 钢板计
    /// </summary>
    public class Fiducial_Armor_plateXJB : Fiducial_Armor_plate
    {
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            //Gorf*(Survey_ZorR-ZorR)+Korb*(TemperatureRead*(Survey_RorT-ZeroR)-RorT)
            double result = param.Gorf * (data.Survey_ZorR - param.ZorR) +GetTCorrect(param,data);

            data.ResultReading = result * param.Elastic_Modulus_E;//这里要乘以系数
            data.Tempreture = param.TemperatureRead * (data.Survey_RorT - param.ZeroR);
            data.LoadReading = result * param.Elastic_Modulus_E;
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            return result;
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            double result = 0;
            if (param.SurveyPoint.StartsWith("PS011"))//特殊处理
            {
                result = param.Gorf * param.Korb * (data.Survey_ZorR - param.ZorR);
            }
            else
            {
                result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
            }
            data.Survey_ZorRMoshu = data.Survey_ZorR;
            data.ResultReading = result * param.Elastic_Modulus_E;//这里要乘以系数每种仪器不一样
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result * param.Elastic_Modulus_E;

            return result;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }

    }

    /// <summary>
    /// 温度计
    /// </summary>
    public class Fiducial_TemperatureXJB : Fiducial_Temperature
    {
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
    }

    public class Fiducial_Water_Level_HoleXJB : Fiducial_Water_Level_Hole
    {
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
    }
    /// <summary>
    /// 引张线
    /// </summary>
    public class Fiducial_Flex_DisplacementXJB : Fiducial_Flex_Displacement
    {
        public Fiducial_Flex_DisplacementXJB()
        {
            base.InsType = InstrumentType.Fiducial_Flex_Displacement;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return 1;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
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
                return pd;
            }
            catch
            {
                return null;
            }
        }
        public override DataTable WriteSurveyToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Result_X");
            dt.Columns.Add("UpdateTime");
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
                    dr["Result_X"] = surveydata.Survey_ZorR;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
            //return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
        public override DataTable WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Result_X");
            dt.Columns.Add("UpdateTime");
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
                    dr["Result_X"] = surveydata.Survey_ZorR;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
            //return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }


    }


    public class Fiducial_Inverst_VerticalXJB : Fiducial_Inverst_Vertical
    {
        public Fiducial_Inverst_VerticalXJB()
        {
            base.InsType = InstrumentType.Fiducial_Inverst_Vertical;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return 1;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
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
                return pd;
            }
            catch
            {
                return null;
            }
        }
        public override DataTable WriteSurveyToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Result_X");
            dt.Columns.Add("Result_Y");
            dt.Columns.Add("UpdateTime");
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
                    dr["Result_X"] = surveydata.Survey_ZorR;
                    dr["Result_Y"] = surveydata.Survey_RorT;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
            //return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
        public override DataTable WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Result_X");
            dt.Columns.Add("Result_Y");
            dt.Columns.Add("UpdateTime");
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
                    dr["Result_X"] = surveydata.Survey_ZorR;
                    dr["Result_Y"] = surveydata.Survey_RorT;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
            //return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }

    }
    public class Fiducial_Earth_MeasureXJB : Fiducial_Earth_Measure
    {
        public Fiducial_Earth_MeasureXJB()
        {
            base.InsType = InstrumentType.Fiducial_Earth_Measure;
        }
        public override double DifBlock(ParamData param,SurveyData data, params double[] expand)
        {
            return base.DifBlock(param,data,expand);
        }
        public override double ShakeString(ParamData param,SurveyData data, params double[] expand)
        {
            return 1;
        }
        public override double AutoDefined(ParamData param,SurveyData data, string expression)
        {
            return base.AutoDefined(param,data,expression);
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
                return pd;
            }
            catch
            {
                return null;
            }
        }
        public override DataTable WriteSurveyToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Result_X");
            dt.Columns.Add("Result_Y");
            dt.Columns.Add("UpdateTime");
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
                    dr["Result_X"] = surveydata.Survey_ZorR;
                    dr["Result_Y"] = surveydata.Survey_RorT;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
            //return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
        public override DataTable WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Result_X");
            dt.Columns.Add("Result_Y");
            dt.Columns.Add("UpdateTime");
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
                    dr["Result_X"] = surveydata.Survey_ZorR;
                    dr["Result_Y"] = surveydata.Survey_RorT;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
            //return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }

    }
    public class Fiducial_Statics_LevelXJB : Fiducial_Earth_Measure
    {
        public Fiducial_Statics_LevelXJB()
        {
            base.InsType = InstrumentType.Fiducial_Statics_Level;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return 1;
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
                return pd;
            }
            catch
            {
                return null;
            }
        }
        public override DataTable WriteSurveyToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Result_X");
            dt.Columns.Add("UpdateTime");
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
                    dr["Result_X"] = surveydata.Survey_ZorR;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
            //return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
        public override DataTable WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Result_X");
            dt.Columns.Add("UpdateTime");
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
                    dr["Result_X"] = surveydata.Survey_ZorR;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
            //return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }

    }

    public class Fiducial_Laser_CollimationXJB : Fiducial_Laser_Collimation
    {
        public Fiducial_Laser_CollimationXJB()
        {
            base.InsType = InstrumentType.Fiducial_Laser_Collimation;
        }
        public override double DifBlock(ParamData param, SurveyData data, params double[] expand)
        {
            return base.DifBlock(param, data, expand);
        }
        public override double ShakeString(ParamData param, SurveyData data, params double[] expand)
        {
            return 1;
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
                return pd;
            }
            catch
            {
                return null;
            }
        }
        public override DataTable WriteSurveyToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Result_Y");
            dt.Columns.Add("Result_Z");
            dt.Columns.Add("UpdateTime");
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
                    dr["Result_Y"] = surveydata.Survey_ZorR;
                    dr["Result_Z"] = surveydata.Survey_RorT;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
            //return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
        public override DataTable WriteResultToDB(List<PointSurveyData> datas)
        {
            DataTable dt = new DataTable();
            string TableName = Config.InsCollection[InsType.GetDescription()].Measure_Table;
            dt.TableName = TableName;
            dt.Columns.Add("ID");
            dt.Columns.Add("Survey_point_Number");
            dt.Columns.Add("Observation_Date");
            dt.Columns.Add("Result_X");
            dt.Columns.Add("UpdateTime");
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
                    dr["Result_X"] = surveydata.Survey_ZorR;
                    dr["UpdateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
            //return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
    }

}
