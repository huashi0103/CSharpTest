using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public override double ShakeString(ParamData Mparam, SurveyData Mdata, params double[] expand)
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
                    if (data.Survey_ZorR > 4000)
                    {
                        result = param.Gorf * (data.Survey_ZorR - param.ZorR) + param.Korb * (Mdata.Survey_RorT - param.RorT)-firstvalue;
                        firstvalue = firstvalue == 0 ? result : firstvalue;

                    }
                    else
                    {
                        result = param.Gorf * (Math.Pow(data.Survey_ZorR, 2) / 1000 - param.ZorR) +
                            param.Korb * (Mdata.Survey_RorT - param.RorT)-firstvalue;
                        firstvalue = firstvalue == 0 ? result : firstvalue;
                    }
                }
                data.ResultReading = result ;//这里要乘以系数每种仪器不一样
                data.Tempreture = data.Survey_RorT;
                data.LoadReading = result;
            }
            return result;
        }
        public override double AutoDefined(ParamData param, SurveyData data, string expression)
        {
            return base.AutoDefined(param, data, expression);
        }
        public override ParamData GetParam(string Survey_point_Number, string tablename = null)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Tempera_Revise_K,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance,Instrument_Serial
                                from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number);
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
                catch {}
            }
            return Mpd;
            
        }
        private  ParamData GetParamExpand(string Survey_point_Number,string Instrument_Serial, string tablename)
        {
            string sql = @"select Instrument_Type,Calculate_Coeffi_G,Tempera_Revise_K,Benchmark_Resist_Ratio,Benchmark_Resist,Temperature_Read,Zero_Resistance,Instrument_Serial
                                from {0} where Survey_point_Number='{1}'";
            sql = string.Format(sql, tablename, Survey_point_Number,Instrument_Serial);
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
                pd.TemperatureRead = Convert.ToDouble(dt.Rows[0][5]);
                pd.ZeroR = Convert.ToDouble(dt.Rows[0][6]);
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
    /// 应变计组
    /// </summary>
    public class Fiducial_Strain_Group : BaseInstrument
    {
        public Fiducial_Strain_Group()
        {
            base.InsType = InstrumentType.Fiducial_Strain_Group;
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
    }
}
