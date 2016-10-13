using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Reflection;
using System.IO;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Globalization;
using System.Data.SqlClient;


namespace LoadDataCalc
{
    /// <summary>
    /// 渗压计
    /// </summary>
    public class Fiducial_Leakage_PressureProcess : ProcessData
    {
        /// <summary> 从excel文件中读取数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override void ReadData(string path, out  List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            DataInfo index = new DataInfo() { TableName=this.SurveyTableName,DateIndex=0,TimeIndex=1,ZoRIndex=2,
            RorTIndex=3,RemarkIndex=6};
            base.LoadData(path, index, out datas, out errors);
                   
        }
        /// <summary>把测量数据写入到数据库
        /// </summary>
        /// <returns></returns>
        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            return base.WriteSurveyToDB(datas, SurveyTableName);
        }
        /// <summary>把计算后的结果数据写入数据库
        /// </summary>
        /// <returns></returns>
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            return base.WriteResultToDB(datas, ResultTableName);
        }

        public  Fiducial_Leakage_PressureProcess()
        {
            base.InsType = InstrumentType.Fiducial_Leakage_Pressure;
            base.ErrorLimitRT = 10;
            base.ErrorLimitZR = 10;
            base.SurveyTableName = "Survey_Leakage_Pressure";
            base.ResultTableName = "Result_Leakage_Pressure";
        }
    }

    /// <summary>
    /// 单点位移计
    /// </summary>
    public class Fiducial_Single_DisplacementProcess : ProcessData
    {
        
        public Fiducial_Single_DisplacementProcess()
        {
            base.InsType = InstrumentType.Fiducial_Single_Displacement;
            base.ErrorLimitRT = 10;
            base.ErrorLimitZR = 10;
            base.SurveyTableName = "Survey_Single_Displacement";
            base.ResultTableName = "Result_Single_Displacement";

        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            DataInfo index = new DataInfo()
            {
                TableName = this.SurveyTableName,
                DateIndex = 0,
                TimeIndex = 1,
                ZoRIndex = 2,
                RorTIndex = -1,
                RemarkIndex = 4
            };
            base.LoadData(path, index, out datas, out errors);
        }
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            throw new NotImplementedException();
        }
        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            throw new NotImplementedException();
        }
    
    }

    /// <summary>
    /// 测缝计
    /// </summary>
    public class Fiducial_Measure_ApertureProcess : ProcessData
    {
        public Fiducial_Measure_ApertureProcess()
        {
            base.InsType = InstrumentType.Fiducial_Measure_Aperture;
            base.ErrorLimitRT = 10;
            base.ErrorLimitZR = 10;
            base.SurveyTableName = "Survey_Measure_Aperture";
            base.ResultTableName = "Result_Measure_Aperture";
        }

        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            DataInfo index = new DataInfo()
            {
                TableName = this.SurveyTableName,
                DateIndex = 0,
                TimeIndex = 1,
                ZoRIndex = 2,
                RorTIndex = 3,
                RemarkIndex = 6
            };
            base.LoadData(path, index, out datas, out errors);
        }
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            return base.WriteResultToDB(datas, ResultTableName);
        }
        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            return base.WriteResultToDB(datas, SurveyTableName);
        }
    }

    public class Fiducial_ApertureProcess : ProcessData
    {
        public Fiducial_ApertureProcess()
        {
            base.InsType = InstrumentType.Fiducial_Aperture;
            base.ErrorLimitRT = 10;
            base.ErrorLimitZR = 10;
            base.SurveyTableName = "Survey_Aperture";

        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            DataInfo index = new DataInfo()
            {
                TableName = this.SurveyTableName,
                DateIndex = 0,
                TimeIndex = 1,
                ZoRIndex = 2,
                RorTIndex = 3,
                RemarkIndex = 6
            };
            base.LoadData(path, index, out datas, out errors);
        }
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            throw new NotImplementedException();
        }
        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            throw new NotImplementedException();
        }
 
    }
    public class Fiducial_Steel_BarProcess : ProcessData
    {
        public Fiducial_Steel_BarProcess()
        {
            base.InsType = InstrumentType.Fiducial_Steel_Bar;
            base.ErrorLimitRT = 10;
            base.ErrorLimitZR = 10;
            base.SurveyTableName = "Survey_Steel_Bar";

        }
        public override void ReadData(string path, out List<PointSurveyData> datas, out List<ErrorMsg> errors)
        {
            DataInfo index = new DataInfo()
            {
                TableName = this.SurveyTableName,
                DateIndex = 0,
                TimeIndex = 1,
                ZoRIndex = 2,
                RorTIndex = 3,
                RemarkIndex = 5
            };
            base.LoadData(path, index, out datas, out errors);
        }
        public override int WriteResultToDB(List<PointSurveyData> datas)
        {
            throw new NotImplementedException();
        }
        public override int WriteSurveyToDB(List<PointSurveyData> datas)
        {
            throw new NotImplementedException();
        }
    }
}
