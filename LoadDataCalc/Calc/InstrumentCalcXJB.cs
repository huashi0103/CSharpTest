﻿/*
 * 向家坝数据计算类，对应ProCode=XJB
 * 考证表中的参数默认频率都转换为模数
 * 只用实现向家坝涉及的仪器就可以了
 * 暂未使用
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoadDataCalc
{

    /// <summary>
    /// 渗压计
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
            // if (Math.Abs(param.Gorf * 100) > 1) param.KpaToMpa = 0.001;
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

                return pd;
            }
            catch
            {
                return null;
            }
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
                    //data.Survey_ZorRMoshu = data.Survey_ZorR;
                }
                else
                {
                    result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000 - param.ZorR) +
                        param.Korb * (data.Survey_RorT - param.RorT);
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
            if (calc.IsMoshu == 1)//是否是模数
            {
                result = pd.Gorf * (sd.Survey_ZorR - pd.ZorR) + pd.Korb * (sd.Survey_RorT - pd.RorT);
            }
            else
            {
                result = pd.Gorf * (Math.Pow(sd.Survey_ZorR, 2) / 1000 - pd.ZorR) +
                    pd.Korb * (sd.Survey_RorT - pd.RorT);
            }
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
            return ((int)(pd.Gorf * (sd.Survey_ZorR - pd.ZorR) * 100 + 0.5)) / 100.0 + pd.Korb * (sd.Survey_RorT - pd.RorT);
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
            }
        }
        //获取基准列的最后一次有效值
        private double getLastStandValue(string surveyPoint, string loadindex)
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
            if (Mcalc[0].R0IsZero != 0)//倒序排列下//默认是从深到浅。当孔口为0的时候为从浅到深
            {
                Dictionary<string, SurveyData> Dic = new Dictionary<string, SurveyData>();
                List<string> li = new List<string>(Mdata.MultiDatas.Keys);
                for (int i = li.Count - 1; i >= 0; i--)
                {
                    Dic.Add(li[i], Mdata.MultiDatas[li[i]]);
                }
                Mdata.MultiDatas = Dic;
            }

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
    }

    /// <summary> 应变计组
    /// </summary>
    public class Fiducial_Strain_GroupXJB : Fiducial_Strain_Group
    {

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
                FStrain_Gauge.ShakeString(param.MParamData[sd.Key], sd.Value);
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
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 5000)
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
                   // data.Survey_ZorRMoshu = data.Survey_ZorR;
                }
                else
                {
                    result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000 - param.ZorR) +
                        (param.Korb - param.Concrete_Expansion_ac) * (data.Survey_RorT - param.RorT);
                    //data.Survey_ZorRMoshu = Math.Pow(data.Survey_ZorR, 2) / 1000;
                }
            }
            data.ResultReading = result;
            data.Tempreture = data.Survey_RorT;
            data.LoadReading = result;
            if (data.NonStressSurveyData != null)
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
            if (Math.Abs(data.Survey_ZorR) > 1)
            {
                if (data.Survey_ZorR > 5000)
                {
                    result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
                    //data.Survey_ZorRMoshu = data.Survey_ZorR;
                }
                else
                {
                    result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000 - param.ZorR) +
                        (param.Korb - param.Concrete_Expansion_ac) * (data.Survey_RorT - param.RorT);
                    //data.Survey_ZorRMoshu = Math.Pow(data.Survey_ZorR, 2) / 1000;
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
            return base.ShakeString(param, data, expand);
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
            return base.ShakeString(param, data, expand);
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
                if (dic.Value.Survey_ZorR >= 4000)
                {
                    value += dic.Value.Survey_ZorR;
                    //dic.Value.Survey_ZorRMoshu = dic.Value.Survey_ZorR;
                }
                else
                {
                    //dic.Value.Survey_ZorRMoshu = Math.Pow(dic.Value.Survey_ZorR, 2) / 1000.0;
                    //value += dic.Value.Survey_ZorRMoshu;
                }
            }
            if (count == data.MultiDatas.Keys.Count)
            {
                value = value / count;
            }
            else
            {//掉弦
                value = (value / count) * GetC(param, data);
            }

            result = param.Gorf * (value - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
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
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
    }

    /// <summary>
    /// 钢板计
    /// </summary>
    public class Fiducial_Armor_plateXJB : Fiducial_Armor_plate
    {
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
            double result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (data.Survey_RorT - param.RorT);
            //data.Survey_ZorRMoshu = data.Survey_ZorR;
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

    /// <summary>
    /// 精密水准
    /// </summary>
    public class Fiducial_Geometry_LevelXJB : BaseInstrument
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


}