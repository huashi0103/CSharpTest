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
            return base.GetParam(Survey_point_Number, this.InsType.GetDescription());
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
            return base.GetParam(Survey_point_Number, this.InsType.GetDescription());
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
            return base.GetParam(Survey_point_Number, this.InsType.GetDescription());
        }
    }
}
