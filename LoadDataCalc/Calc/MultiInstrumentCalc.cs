using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LoadDataCalc
{

    /// <summary>
    /// 多点位移计
    /// </summary>
    public class Fiducial_Multi_Displacement : BaseInstrument
    {
        public Fiducial_Multi_Displacement()
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

            //倒序排列下//读数据时从深到浅。
            //数据库中是从浅到深，有基准基准写0，没有从1开始写
            Dictionary<string, SurveyData> Dic = new Dictionary<string, SurveyData>();
            if (Mdata.MultiDatas.Count == 0) return result;
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
            for (int i = 1; i < 7; i++)
            {
                dt.Columns.Add("Frequency"+i.ToString());
            }
            dt.Columns.Add("Remark");
            dt.Columns.Add("UpdateTime");
            if(Config.IsAuto)dt.Columns.Add("RecordMethod");
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
                    dr["Temperature"] = Math.Round(surveydata.Survey_RorT,2);
                    int index = 1;
                    foreach (var dic in surveydata.MultiDatas)
                    {
                        dr["Frequency" + index.ToString()] = Math.Round(dic.Value.Survey_ZorR, 4);
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
            for (int i = 0; i < 7; i++)
            {
                dt.Columns.Add("loadReading"+i.ToString());
            }
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
                    //dr["loadReading0"] = Math.Round(surveydata.LoadReading,4);
                    int index = surveydata.MultiDatas.Keys.First().EndsWith("A") ? 0 : 1;
                    foreach (var dic in surveydata.MultiDatas)
                    {
                        dr["loadReading" + index.ToString()] = Math.Round(dic.Value.LoadReading,4);
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

    }

    /// <summary>
    /// 应变计组
    /// </summary>
    public class Fiducial_Strain_Group : BaseInstrument
    {
        public Fiducial_Strain_Group()
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
            if(Config.IsAuto)dt.Columns.Add("RecordMethod");
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
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
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
                    if (Config.IsAuto) dr["RecordMethod"] = "人工";
                    dt.Rows.Add(dr);
                }
            }
            return sqlhelper.BulkCopy(dt) ? dt.Rows.Count : 0;
        }
    }

    /// <summary>
    /// 多点锚杆应力计
    /// </summary>
    public class Fiducial_Multi_Anchor_Pole : BaseInstrument
    {
        public Fiducial_Multi_Anchor_Pole()
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
