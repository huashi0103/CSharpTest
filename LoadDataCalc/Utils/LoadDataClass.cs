/*
 * 读取文件、读数据、写数据
 * controller
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Diagnostics;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;

namespace LoadDataCalc
{
    /// <summary>
    /// 读取计算表格和配置文件
    /// </summary>
    public  sealed class LoadDataClass
    {
       //唯一实例
        private static   LoadDataClass loadData = null;
        //数据库实例
        private CSqlServerHelper sqlhelper = null;
        //程序集目录
        private string Assemblydir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //状态委托
        public Action<string> StatusAction;
        //数据锁
        private object DataLock = new object();
        /// <summary>写入数据的其实行ID，默认-1;每次写数据的时候重新赋值
        /// </summary>
        public int ID = -1;

        private int FileIndex = 0;
        public Action<int> FileIndexAction;

        private LoadDataClass()
        {
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static LoadDataClass GetInstance()
        {
            if (loadData == null)
            {
                loadData = new LoadDataClass();
            }
            return loadData;
        }
        /// <summary>初始化,返回进行到第几步
        /// 0-写默认模板 1-读取配置文件，2-初始化数据库 3-全部完成
        /// </summary>
        /// <returns></returns>
        public   string Init()
        {
            string res = "";
            try
            {
                Config.WriteDifaultConfig();//写一个默认模板
                res = "读取项目配置文件";
                //读取本项目配置文件
                if (!Config.LoadConfig()) return res;
                res = "初始化数据库";
                CSqlServerHelper.Connectionstr = Config.DataBase;
                sqlhelper = CSqlServerHelper.GetInstance();
                if (sqlhelper.Check())
                {
                    res = "OK";
                    LoadTempereture();
                }
            }
            catch
            { 
            }
            return res;

        }
        /// <summary>加载温度量程
        /// </summary>
        public  void LoadTempereture()
        {
            CSqlServerHelper helper = CSqlServerHelper.GetInstance();
            string sql = @"select * from InstrumentTable where Instrument_Name='温度计'";
            var data = helper.SelectData(sql);
            if (data.Rows.Count > 0)
            {
                Config.MinTemperature = data.Rows[0]["Min_Threshold"] == null ? 0 : Convert.ToDouble(data.Rows[0]["Min_Threshold"]);
                Config.MaxTemperature = data.Rows[0]["Max_Threshold"] == null ? 70 : Convert.ToDouble(data.Rows[0]["Max_Threshold"]);
            }
        }
   
        private bool CheckFile(string filename,params string[] insname)
        {
            foreach(string ins in insname)
            {
                var result = Config.Instruments.Where(x => x.InsName == ins).FirstOrDefault();
                if (result == null) continue;
                string[] multikey = result.KeyWord.ToArray();
                if (DataUtils.CheckContainStr(filename, multikey)) return true;
            }
            return false;
        }

        /// <summary>获取包含关键词的文件，关键词为空，返回所有文件
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>文件</returns>
        public List<string> GetFiles(string insname)
        {
            string[] keyword = Config.Instruments.Where(x => x.InsName == insname).FirstOrDefault().KeyWord.ToArray();

            List<string> files = new List<string>();
            getDir(Config.DataRoot, files);
            if (keyword == null)
            {
                return files;
            }
            else
            {
                List<string> refiles = new List<string>();
                foreach (var file in files)
                {
                    string filename = Path.GetFileName(file);
                    if (DataUtils.CheckContainStr(filename, keyword))
                    {
                        if (insname == "应变计")
                        {
                            if (!CheckFile(filename, "应变计组")) refiles.Add(file);
                        }
                        else if (insname == "无应力计")
                        {
                            if (!CheckFile(filename, "应变计组", "应变计")) refiles.Add(file);
                        }
                        else if (insname == "测斜仪")
                        {
                            if (!CheckFile(filename,"固定测斜仪")) refiles.Add(file);
                        }
                        else if (insname == "单点位移计")
                        {
                            if (!CheckFile(filename, "多点位移计", "引张线式水平位移计")) refiles.Add(file);
                        }
                        else
                        {
                            refiles.Add(file);
                        }
                    }
                    
                }
                return refiles;
            }
 
        }
        
        /// <summary> //读取计算的数据缓存
        /// </summary>
        public List<PointSurveyData> SurveyDataCach = new List<PointSurveyData>();
        public Dictionary<string, List<ErrorMsg>> ErrorMsgCach = new Dictionary<string, List<ErrorMsg>>();

        /// <summary>数据缓存,缓存应变计对应的无应力计数据
        /// </summary>
        public List<PointSurveyData> SurveyDataCachExpand = new List<PointSurveyData>();
     
        private BaseInstrument inscalc = null;

        /// <summary> 清空缓存数据
        /// </summary>
        public void ClearCach()
        {
            SurveyDataCach.Clear();
            ErrorMsgCach.Clear();
            SurveyDataCachExpand.Clear();
            ID = -1;
        }
        /// <summary>
        /// 检查文件是否被占用
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public bool CheckFiles(List<string> files, out string errfile)
        {
            errfile = null;
            foreach (string file in files)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch
                {
                    errfile = file;
                    return false;
                }
                finally
                {
                    if (fs != null)fs.Close();
                }
            }
            return true;

 
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="instype"></param>
        public void ReadData(InstrumentType instype,List<string>files)
        {
            //临时文件每次读取清空
            if (File.Exists(ErrorMsg.temp)) File.Delete(ErrorMsg.temp);
            if (File.Exists(ErrorMsg.tempsheeterror)) File.Delete(ErrorMsg.tempsheeterror);
           
            ProcessData process = ProcessFactory.CreateInstance(instype);
            if (process == null) return;
            process.StatusAction = this.StatusAction;
            process.ErrorLimitZR = Config.LimitZ;
            process.ErrorLimitZR = Config.LimitT;
            process.LoadPointCach(instype);
            FileIndex = 0;
            //files.AsParallel().ForAll((file) => {
            foreach (string file in files){
                List<PointSurveyData> datas = new List<PointSurveyData>();
                List<ErrorMsg> msgs = new List<ErrorMsg>(); 
                lock (DataLock)
                {
                    process.ReadData(file, out datas, out msgs);
                    SurveyDataCach.AddRange(datas);
                    if (ErrorMsgCach.Keys.Contains(file))
                    {
                        ErrorMsgCach[file].AddRange(msgs);
                    }
                    else
                    {
                        ErrorMsgCach.Add(file, msgs);
                    }
                }
                FileIndex++;
                FileIndexAction(FileIndex);
            }
            //});

            //应变计和应变计组同时也读一遍无应力计的数据
            if (instype==InstrumentType.Fiducial_Strain_Gauge||instype==InstrumentType.Fiducial_Strain_Group)
            {
                SurveyDataCachExpand = process.ExpandDataCach;
            }
            if (StatusAction != null) StatusAction("读取完成,正在写入日志文件");
            ErrorMsg.Log(ErrorMsgCach);
            ErrorMsg.LogSheetErr(process.ErrorSheetName);
            ClearData();
#if TEST
            using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\test.csv",false,Encoding.UTF8))
            {
                foreach (var dic in process.PointCach)
                {
                    if (dic.Value.Count == 0) sw.WriteLine("," + dic.Key);
                    foreach (var p in dic.Value)
                    {
                        sw.WriteLine(p+","+dic.Key);
                    }

                }
            }
#endif
        }
        //去掉重复的点数据
        private void ClearData()
        {
            for (int j=0;j<SurveyDataCach.Count;j++)
            {
                var array = SurveyDataCach.FindAll((pt) => pt.SurveyPoint == SurveyDataCach[j].SurveyPoint).ToList();
                if (array.Count > 1)
                {
                    int index = 0;
                    for (int i = 1; i < array.Count; i++)
                    {
                        if (array[i].Datas.Count > array[i - 1].Datas.Count)
                        {
                            index = i;
                        }
                    }
                    for (int i = 0; i < array.Count; i++)
                    {
                        if (i == index) continue;
                        SurveyDataCach.Remove(array[i]);
                    }
                }
            }
        }

        /// <summary>
        ///  //清理NPOI生成tmp文件
        /// </summary>
        public void ClearDirTmp()
        {
            var files = Directory.GetFiles(Assemblydir,"*.tmp");
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
        /// <summary>
        /// 计算
        /// </summary>
        public void Calc(InstrumentType instype, string expression=null)
        {
            inscalc = CalcFactoryClass.CreateInstCalc(instype);
            inscalc.CalcExpandList = CalcExpand.LoadList(instype);
            List<string> Errors = new List<string>();
            //应变计和应变计组
            switch (instype)
            {
                case InstrumentType.Fiducial_Strain_Gauge:
                case InstrumentType.Fiducial_Strain_Group:
                        CalcStrain(instype, expression);
                        break;
                case InstrumentType.Fiducial_Multi_Displacement:
                        CalcMultiDislacment(instype, expression);
                        break;
                case  InstrumentType.Fiducial_Anchor_Cable:
                        CalcAnchor_Cable(instype, expression);
                        break;
                case InstrumentType.Fiducial_Water_Level_Hole://水位孔数据//需要上一次的测值
                        foreach (var pd in SurveyDataCach)
                        {
                            ParamData param = inscalc.GetParam(pd.SurveyPoint, instype.ToString());
                            if (param == null)
                            {
                                Errors.Add(String.Format("PARAM:{0},{1}", pd.SurveyPoint, "找不到测点参数"));//保存读取参数的错误
                                pd.IsCalc = false;
                                continue;
                            }
                            if (expression != null) param.InsCalcType = CalcType.AutoDefine;
                            double lastWaterLevel = -99999;
                            foreach (SurveyData sd in pd.Datas)
                            {
                                if (lastWaterLevel == -99999)
                                {
                                    lastWaterLevel = GetLasWaterLevelHole(pd, sd);
                                }
                                inscalc.ShakeString(param, sd, lastWaterLevel);
                                lastWaterLevel = sd.LoadReading;
                            }
                            pd.IsCalc = true;
                        }
                        break;
                default:
                        foreach (var pd in SurveyDataCach)
                        {
                            ParamData param = inscalc.GetParam(pd.SurveyPoint, instype.ToString());
                            if (param == null)
                            {
                                Errors.Add(String.Format("PARAM:{0},{1}", pd.SurveyPoint,"找不到测点参数"));//保存读取参数的错误
                                pd.IsCalc = false;
                                continue;
                            }
                            if (expression != null)param.InsCalcType = CalcType.AutoDefine;
                            foreach (SurveyData sd in pd.Datas)
                            {
                                inscalc.FuncDic[param.InsCalcType](param, sd,expression);
                            }
                            pd.IsCalc = true;
                        }
                        break;
            }
            ErrorMsg.Log(Errors);
        }

        #region//多点仪器需要特殊处理
        //应变计特殊处理
        private void CalcStrain(InstrumentType instype, string expression = null)
        {    //应变计和应变计组
            if ((instype != InstrumentType.Fiducial_Strain_Gauge && instype != InstrumentType.Fiducial_Strain_Group)) return;
            inscalc = CalcFactoryClass.CreateInstCalc(instype);
            inscalc.CalcExpandList = CalcExpand.LoadList(instype);
            List<string> Errors = new List<string>();
            //先计算无应力计
            var fn = CalcFactoryClass.CreateInstCalc(InstrumentType.Fiducial_Nonstress) as Fiducial_Nonstress;
            foreach (var pd in SurveyDataCachExpand)
            {
                ParamData nonpd = fn.GetParam(pd.SurveyPoint, "Fiducial_Nonstress");
                foreach (var sd in pd.Datas)
                {
                    fn.FuncDic[nonpd.InsCalcType](nonpd, sd, null);
                }
            }
            foreach (var pd in SurveyDataCach)
            {
                ParamData param = inscalc.GetParam(pd.SurveyPoint, instype.ToString());
                PointSurveyData NonStressPoint = null;
                if (param.IsHasNonStress)
                {
                    // 查下本次数据中有没有找到对应的无应力计数据
                    NonStressPoint = SurveyDataCachExpand.Where(p => p.SurveyPoint.ToUpper().Trim() == param.NonStressNumber.ToUpper()).FirstOrDefault();
                    if (NonStressPoint == null || NonStressPoint.Datas.Count == 0)
                    {
                        Errors.Add(String.Format("PARAM:{0},{1},{2}", pd.SurveyPoint,param.NonStressNumber ,"本次数据中找不到对应的无应力计数据"));
                    }
                }
                if (param == null)
                {
                    Errors.Add(String.Format("PARAM:{0},{1}", pd.SurveyPoint, "找不到测点参数"));//保存读取参数的错误
                    pd.IsCalc = false;
                    continue;
                }
                if (expression != null) param.InsCalcType = CalcType.AutoDefine;
                foreach (SurveyData sd in pd.Datas)
                {

                    //应变计和应变计组额外计算无应力计的数据
                    if (param.IsHasNonStress)
                    {
                        var nondata = GetNonStressSurveyData(NonStressPoint, sd,param.NonStressNumber);
                        if (nondata != null)
                        {
                            sd.NonStressSurveyData = nondata;
                            if (instype == InstrumentType.Fiducial_Strain_Group)
                            {//应变机组中的所有应变计都以无应力计为基准
                                foreach (var dic in sd.MultiDatas)
                                {
                                    dic.Value.NonStressSurveyData = nondata;
                                }
                            }
                        }
                        else
                        {
                            Errors.Add(String.Format("PARAM:{0},{1},{2}", pd.SurveyPoint, sd.SurveyDate.Date.ToString(), "找不到对应的无应力计数据"));
                        }
                    }
                    inscalc.FuncDic[param.InsCalcType](param, sd, expression);
                }
                pd.IsCalc = true;
            }
            ErrorMsg.Log(Errors);
        }
        //获取应变计和应变机组对应的无应力计数据
        SurveyData GetNonStressSurveyData(PointSurveyData pd, SurveyData sd,string NonNumber)
        {
            if (pd != null&&pd.Datas.Count>0)
            {
                SurveyData DNsd = null;
                foreach (var nsd in pd.Datas)
                {
                    if (nsd.SurveyDate.Date.CompareTo(sd.SurveyDate.Date) > 0) break;
                    if (nsd.Survey_ZorR != 0)
                    {
                        DNsd = nsd;
                    }
                }
                if (DNsd != null) return DNsd;
            }
            var sqlhelper = CSqlServerHelper.GetInstance();
            //当前数据中找不到数据，在数据库里边找,查7天以内最近的数据//这个不好使
            //string sql = Config.IsAuto ? "select * from Result_Nonstress where Survey_point_Number='{0}' and abs(datediff(d,Observation_Date,'{1}'))<7  and RecordMethod='人工' Order by abs(datediff(dd,Observation_Date,'{2}'))" :
            //    "select * from Result_Nonstress where Survey_point_Number='{0}' and abs(datediff(d,Observation_Date,'{1}'))<7 Order by abs(datediff(dd,Observation_Date,'{2}'))";
            //查最后一次的数据
            string sql = Config.IsAuto ? "select top 1 * from Result_Nonstress where Survey_point_Number='{0}' and Observation_Date<='{1}' and RecordMethod='人工' Order by Observation_Date desc" :
                "select  top 1 * from Result_Nonstress where Survey_point_Number='{0}' and Observation_Date<='{1}' Order by  Observation_Date desc";

            var result = sqlhelper.SelectData(String.Format(sql, NonNumber, sd.SurveyDate.Date.ToString(), sd.SurveyDate.Date.ToString()));
            if (result.Rows.Count > 0)
            {
                SurveyData DNsd = new SurveyData();
                DNsd.LoadReading = Convert.ToDouble(result.Rows[0]["loadReading"]);
                return DNsd;
            }
            return null;

        }
        //计算锚索测力计
        private void CalcAnchor_Cable(InstrumentType instype, string expression = null)
        {
            inscalc = CalcFactoryClass.CreateInstCalc(instype);
            inscalc.CalcExpandList = CalcExpand.LoadList(instype);
            
            List<string> Errors = new List<string>();
            foreach (var pd in SurveyDataCach)
            {
                ((Fiducial_Anchor_Cable)inscalc).coefficient_K = 1;//换一个测点重置改正系数
                ParamData param = inscalc.GetParam(pd.SurveyPoint, instype.ToString());
                if (param == null)
                {
                    Errors.Add(String.Format("PARAM:{0},{1}", pd.SurveyPoint, "找不到测点参数"));//保存读取参数的错误
                    pd.IsCalc = false;
                    continue;
                }
                if (expression != null) param.InsCalcType = CalcType.AutoDefine;
                foreach (SurveyData sd in pd.Datas)
                {
                    ((Fiducial_Anchor_Cable)inscalc).GetC(param, sd, pd);//计算之前计算系数
                    inscalc.FuncDic[param.InsCalcType](param, sd, expression);
                }
                pd.IsCalc = true;
            }
            ErrorMsg.Log(Errors);
        }
        //计算多点位移计
        private void CalcMultiDislacment(InstrumentType instype, string expression = null)
        {
            inscalc = CalcFactoryClass.CreateInstCalc(instype);
            inscalc.CalcExpandList = CalcExpand.LoadList(instype);
            List<string> Errors = new List<string>();
            //多点位移计，先导入计算表格
            if (instype == InstrumentType.Fiducial_Multi_Displacement) Config.LoadMultiDisplacementCalcs();
            foreach (var pd in SurveyDataCach)
            {
                ParamData param = inscalc.GetParam(pd.SurveyPoint, instype.ToString());
                ((Fiducial_Multi_Displacement)inscalc).Laststandard = 0;
                if (param == null)
                {
                    Errors.Add(String.Format("PARAM:{0},{1}", pd.SurveyPoint, "找不到测点参数"));//保存读取参数的错误
                    pd.IsCalc = false;
                    continue;
                }
                if (expression != null) param.InsCalcType = CalcType.AutoDefine;
                foreach (SurveyData sd in pd.Datas)
                {
         
                    string standard = null;
                    foreach(var dic in sd.MultiDatas)
                    {
                        if (dic.Key.EndsWith("A"))
                        {
                            standard=dic.Key;
                        }
                    }
                    //从本次读取的数据中查询基准不为0的值
                    if (standard != null && sd.MultiDatas[standard].Survey_ZorR == 0)
                    {
                        foreach (var ssd in pd.Datas)
                        {
                            if (ssd.SurveyDate.CompareTo(sd.SurveyDate) >= 0) break;
                            if (ssd.MultiDatas[standard].Survey_ZorR != 0)
                            {
                                ((Fiducial_Multi_Displacement)inscalc).Laststandard = ssd.MultiDatas[standard].LoadReading;
                            }
                        }

                    }
                    inscalc.FuncDic[param.InsCalcType](param, sd, expression);
                }
                pd.IsCalc = true;
            }
            ErrorMsg.Log(Errors);
        }

        #endregion

        /// <summary> 检查数据缓存的第一个点的第一条数据是否比数据库中的最后一条语句大，
        /// 防止多次写入，每次写入之前都做判断
        /// </summary>
        /// <param name="ins">仪器类型</param>
        /// <param name="tableName">测值表还是成果表</param>
        /// <returns></returns>
        public bool CheckSurveyDate(InstrumentType ins,string tableName)
        {
            var firstPd = SurveyDataCach.FirstOrDefault(p => p.Datas.Count > 0);
            if (firstPd==null||firstPd.InsType != ins) return false;
            if (Config.IsCovery) return true;//覆盖导入不检查了

            var sqlhelper = CSqlServerHelper.GetInstance();
            DateTime maxDatetime = new DateTime();
            string sql = Config.IsAuto ? "select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number and RecordMethod='人工'" :
                                 "select max(Observation_Date) from {0} where Survey_point_Number=@Survey_point_Number";
            sql = String.Format(sql, tableName);
            var dtresult = sqlhelper.SelectFirst(sql, new SqlParameter("@Survey_point_Number", firstPd.SurveyPoint));
            bool flag = (dtresult != DBNull.Value);
            if (flag) maxDatetime = (DateTime)dtresult;
            if (flag)
            {
                if (firstPd.Datas[0].SurveyDate.Date.CompareTo(maxDatetime.Date) <= 0)
                {
                    return false;
                }
            }
            var sid = sqlhelper.SelectFirst("select max(ID) as sid  from  " + tableName);
            ID = sid == DBNull.Value ? 0 : Convert.ToInt32(sid);
            return true;
        }
        /// <summary>写测值表
        /// </summary>
        /// <returns></returns>
        public bool WirteToSurvey()
        {
            if (inscalc == null) return false;
            int icount=0;
            foreach(var pd in SurveyDataCach)
            {
                icount += pd.Datas.Count;
            }
          
            #region //覆盖导入//导入前删除要覆盖的数据

            var sqlhelper = CSqlServerHelper.GetInstance();
            if (Config.IsCovery)
            {
                string tablename = Config.InsCollection[inscalc.InsType.GetDescription()].Measure_Table;
                string sql = Config.IsAuto?"delete from {0} where Survey_point_Number='{1}'  and Observation_Date>'{2}' and RecordMethod='人工' ":
                    "delete from {0} where Survey_point_Number='{1}'  and Observation_Date>'{2}'";
                if (Config.IsCoveryAll)//全部覆盖导入
                {
                    foreach (var pd in SurveyDataCach)
                    {
                        if (pd.Datas.Count == 0) continue;
                        DateTime dt = pd.Datas[0].SurveyDate;
                        sqlhelper.InsertDelUpdate(String.Format(sql, tablename, pd.SurveyPoint, dt.Date.ToString()));
                    }
                }
                else
                {
                    foreach (var pd in SurveyDataCach)
                    {
                        if (pd.Datas.Count == 0) continue;
                        sqlhelper.InsertDelUpdate(String.Format(sql, tablename, pd.SurveyPoint, Config.StartTime.Date.ToString()));
                    }
                }
            }
            #endregion
            int result = inscalc.WriteSurveyToDB(SurveyDataCach);
            ErrorMsg.Log(String.Format("写入{0}行测值",result));
           //应变计和应变计组,还需要写入无应力计的数据
            if ((inscalc.InsType == InstrumentType.Fiducial_Strain_Gauge || inscalc.InsType == InstrumentType.Fiducial_Strain_Group))
            {
                if (Config.IsCovery)
                {
                    string sql = Config.IsAuto ? "DELETE FROM Survey_Nonstress where Survey_point_Number='{0}' and Observation_Date>'{1}'and RecordMethod='人工'" :
                        "DELETE FROM Survey_Nonstress where Survey_point_Number='{0}' and Observation_Date>'{1}'";
                    if (Config.IsCoveryAll)//全部覆盖导入
                    {
                        foreach (var pd in SurveyDataCachExpand)
                        {
                            if (pd.Datas.Count == 0) continue;
                            DateTime dt = pd.Datas[0].SurveyDate;
                            sqlhelper.InsertDelUpdate(String.Format(sql, pd.SurveyPoint, dt.Date.ToString()));
                        }
                    }
                    else
                    {
                        foreach (var pd in SurveyDataCachExpand)
                        {
                            if (pd.Datas.Count == 0) continue;
                            sqlhelper.InsertDelUpdate(String.Format(sql, pd.SurveyPoint, Config.StartTime.Date.ToString()));
                        }
                    }
                }
                Fiducial_Nonstress fn = new Fiducial_Nonstress();
                fn.WriteSurveyToDB(SurveyDataCachExpand);
            }
            return result == icount;
        }
        /// <summary>
        /// 写成果表
        /// </summary>
        /// <returns></returns>
        public bool WirteToResult()
        {
            if (inscalc == null) return false;
            int icount = 0;
            foreach (var pd in SurveyDataCach)
            {
                icount += pd.Datas.Count;
            }

            #region //覆盖导入//导入前删除要覆盖的数据
            var sqlhelper = CSqlServerHelper.GetInstance();
            if (Config.IsCovery)
            {
                string tablename = Config.InsCollection[inscalc.InsType.GetDescription()].Result_Table;
                string sql = Config.IsAuto ? "delete from {0} where Survey_point_Number='{1}'  and Observation_Date>'{2}' and RecordMethod='人工'" :
                    "delete from {0} where Survey_point_Number='{1}'  and Observation_Date>'{2}'";
                if (Config.IsCoveryAll)//全部覆盖导入
                {
                    foreach (var pd in SurveyDataCach)
                    {
                        if (pd.Datas.Count == 0) continue;
                        DateTime dt = pd.Datas[0].SurveyDate;
                        sqlhelper.InsertDelUpdate(String.Format(sql, tablename, pd.SurveyPoint, dt.ToString()));
                    }
                }
                else
                {
                    foreach (var pd in SurveyDataCach)
                    {
                        if (pd.Datas.Count == 0) continue;
                        sqlhelper.InsertDelUpdate(String.Format(sql, tablename, pd.SurveyPoint, Config.StartTime.Date.ToString()));
                    }
                }
            }
            #endregion
            int  result = inscalc.WriteResultToDB(SurveyDataCach);
            ErrorMsg.Log(String.Format("写入{0}行成果值", result));
            //应变计和应变计组,还需要写入无应力计的数据
            if ((inscalc.InsType == InstrumentType.Fiducial_Strain_Gauge || inscalc.InsType == InstrumentType.Fiducial_Strain_Group))
            {
                if (Config.IsCovery)
                {
                    string sql = Config.IsAuto ? "DELETE FROM Result_Nonstress where Survey_point_Number='{0}' and Observation_Date>'{1}' and RecordMethod='人工'" :
                        "DELETE FROM Result_Nonstress where Survey_point_Number='{0}' and Observation_Date>'{1}' ";
                    if (Config.IsCoveryAll)//全部覆盖导入
                    {
                        foreach (var pd in SurveyDataCachExpand)
                        {
                            if (pd.Datas.Count == 0) continue;
                            DateTime dt = pd.Datas[0].SurveyDate;
                            sqlhelper.InsertDelUpdate(String.Format(sql, pd.SurveyPoint, dt.Date.ToString()));
                        }
                    }
                    else
                    {
                        foreach (var pd in SurveyDataCachExpand)
                        {
                            if (pd.Datas.Count == 0) continue;
                            sqlhelper.InsertDelUpdate(String.Format(sql, pd.SurveyPoint, Config.StartTime.Date.ToString()));
                        }
                    }
                }
                Fiducial_Nonstress fn = new Fiducial_Nonstress();
                fn.WriteResultToDB(SurveyDataCachExpand);
            }
            return result == icount;
        }
        /// <summary> 回滚数据
        /// </summary>
        /// <param name="ins">仪器类型</param>
        public void Rollback(InstrumentType ins)
        {
            if (ID < 0) return;
            var firstPd = SurveyDataCach.First(p => p.Datas.Count > 0);
            if (firstPd.InsType != ins) return;
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql = Config.IsAuto ? "Delete from {0} where ID in  (Select ID from {1} where ID>{2} and RecordMethod='人工')" :
               "Delete from {0} where  ID in (Select ID from {1} where ID>{2})";
            string table=Config.InsCollection[ins.GetDescription()].Measure_Table;
            sqlhelper.InsertDelUpdate(string.Format(sql, table, table, ID));
            table = Config.InsCollection[ins.GetDescription()].Result_Table;
            sqlhelper.InsertDelUpdate(string.Format(sql, table, table, ID));
            ID = -1;
        }
        public bool RollbackCheck(InstrumentType ins,out DateTime dt)
        {
          var sqlhelper = CSqlServerHelper.GetInstance();
          dt = new DateTime();
          if (ID < 0) return false;
          string table=Config.InsCollection[ins.GetDescription()].Measure_Table;
          string sql = Config.IsAuto ? "select UpdateTime from {0} where  ID>{1} and RecordMethod='人工'" :
                "select UpdateTime from {0} where ID>{1}";
            sql = String.Format(sql,table,ID);
            var result = sqlhelper.SelectFirst(sql);
            bool flag=(result != DBNull.Value||result==null);
            if(flag)dt = (DateTime)result;
            return flag;
        }
        
        /// <summary>
        /// 检查基准值
        /// </summary>
        /// <param name="instype"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public dynamic CheckStandard(InstrumentType instype, List<string> files)
        {
            ProcessData process = ProcessFactory.CreateInstance(instype);
            if (process == null) return null;
            process.StatusAction = this.StatusAction;
            process.ErrorLimitZR = Config.LimitZ;
            process.ErrorLimitZR = Config.LimitT;
            process.LoadPointCach(instype);
            FileIndex = 0;
            List<dynamic> res = new List<dynamic>();
            //files.AsParallel().ForAll((file) => {
            foreach (string file in files)
            {
                List<dynamic> fres = new List<dynamic>();
                lock (DataLock)
                {
                    res.AddRange(process.CheckStandard(file));
                }
                FileIndex++;
                FileIndexAction(FileIndex);
            }
            //});
            return res;
            ////应变计和应变计组同时也读一遍无应力计的数据
            //if (instype == InstrumentType.Fiducial_Strain_Gauge || instype == InstrumentType.Fiducial_Strain_Group)
            //{
            //    SurveyDataCachExpand = process.ExpandDataCach;
            //}
            //if (StatusAction != null) StatusAction("读取完成,正在写入日志文件");
        }

        public void getDir(string path, List<string> FileList)
        {
            getFiles(FileList, path, "*.xls");
            //getFiles(FileList, path, "*.xlsx");
            //FileList.AddRange(Directory.GetFiles(path,"*.xls"));
            //FileList.AddRange(Directory.GetFiles(path, "*.xlsx"));
            var dirs = Directory.GetDirectories(path);
            if (dirs.Length > 0)
            {
                foreach (var d in dirs)
                {
                    getDir(d, FileList);
                }
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// 写扩展数据库
        /// </summary>
        /// <returns></returns>
        public bool WriteDBExpand()
        {
            //换数据库
            CSqlServerHelper.Connectionstr = Config.DatabaseExpand;
            if (inscalc == null) return false;
            int icount = 0;
            foreach (var pd in SurveyDataCach)
            {
                icount += pd.Datas.Count;
            }

            #region //覆盖导入//导入前删除要覆盖的数据
            var sqlhelper = CSqlServerHelper.GetInstance();
            string insname = inscalc.InsType.GetDescription();
            string sql = @"SELECT Result_Number from Table_PointTableName where Point_Name='{0}'";
            string tablename = sqlhelper.SelectFirst(string.Format(sql, insname)).ToString();

            sql = "delete from {0} where Point_Number='{1}' ";
            foreach (var pd in SurveyDataCach)
            {
                if (pd.Datas.Count == 0) continue;
                DateTime dt = pd.Datas[0].SurveyDate;
                sqlhelper.InsertDelUpdate(String.Format(sql, tablename, pd.SurveyPoint, dt.Date.ToString()));
            }
            #endregion
            int result = inscalc.WriteDBExpand(SurveyDataCach);
            ErrorMsg.Log(String.Format("写入{0}行测值", result));
            //应变计和应变计组,还需要写入无应力计的数据
            if ((inscalc.InsType == InstrumentType.Fiducial_Strain_Gauge || inscalc.InsType == InstrumentType.Fiducial_Strain_Group))
            {
                sql = "DELETE FROM Table_Result_N where Point_Number='{0}'";
                foreach (var pd in SurveyDataCachExpand)
                {
                    if (pd.Datas.Count == 0) continue;
                    DateTime dt = pd.Datas[0].SurveyDate;
                    sqlhelper.InsertDelUpdate(String.Format(sql, pd.SurveyPoint, dt.Date.ToString()));
                }
                Fiducial_Nonstress fn = new Fiducial_Nonstress();
                fn.WriteDBExpand(SurveyDataCachExpand);
            }
            //换回去
            CSqlServerHelper.Connectionstr = Config.DataBase;
            return icount != 0;
            //return result == icount;
        }

        void getFiles(List<string> list, string path, string pattern)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            //var allfiles = di.GetFiles(pattern,SearchOption.TopDirectoryOnly);
            var allfiles = di.GetFiles();
            foreach (FileInfo fi in allfiles)
            {
                if ((fi.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden&&
                    (fi.FullName.EndsWith(".xls")||fi.FullName.EndsWith(".xlsx")||
                    fi.FullName.EndsWith(".xlsm")))
                {
                    list.Add(fi.FullName);
                }

            }
        }
        double GetLasWaterLevelHole(PointSurveyData pd, SurveyData sd)
        {
            if (pd == null) return 0;
            foreach (var wsd in pd.Datas)
            {
                if (wsd.SurveyDate.CompareTo(sd.SurveyDate) >= 0) break;
                if (wsd.SurveyDate.Date.CompareTo(sd.SurveyDate.Date) < 0
                    && wsd.SurveyDate.Date.CompareTo(sd.SurveyDate.Date.AddDays(-3)) > 0)
                {
                    return wsd.LoadReading;
                }
            }

            //查上一次的数据
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql = Config.IsAuto ? "select top 1 Water_Level from Result_Water_Level_Hole where Survey_point_Number='{0}' and Observation_Date<'{1}' and RecordMethod='人工' Order by Observation_Date desc" :
                "select  top 1 Water_Level from Result_Water_Level_Hole where Survey_point_Number='{0}' and Observation_Date<'{1}' Order by  Observation_Date desc";
            var result = sqlhelper.SelectFirst(String.Format(sql, pd.SurveyPoint, sd.SurveyDate.Date.ToString()));
            if (result != DBNull.Value)
            {
                return Convert.ToDouble(result);
            }
            return 0;

        }


        
    }

   /// <summary>处理数据类的工厂类
   /// </summary>
    public sealed class ProcessFactory
   {
       public static ProcessData CreateInstance(InstrumentType type)
       {
           Assembly ass = Assembly.GetExecutingAssembly();
           string completeName = String.Format("LoadDataCalc.{0}Process{1}", type.ToString(),Config.ProCode);
           var process = ass.CreateInstance(completeName) as ProcessData;
           return process; 
       }
   }

   /// <summary>仪器计算方法构造类
   /// </summary>
    public sealed class CalcFactoryClass
   {
       /// <summary>仪器工厂类
       /// </summary>
       /// <param name="type"></param>
       /// <returns></returns>
       public static BaseInstrument CreateInstCalc(InstrumentType type)
       {
           Assembly ass = Assembly.GetExecutingAssembly();
           string completeName = "LoadDataCalc."+ type.ToString()+Config.ProCode;
           var ins = ass.CreateInstance(completeName) as BaseInstrument;
           if (ins == null)
           {
               completeName = "LoadDataCalc." + type.ToString();
               ins = ass.CreateInstance(completeName) as BaseInstrument;
           }
           return ins;

       }
   }
    
   /// <summary>日志类
   /// </summary>
   public class ErrorMsg
   {
       public string PointNumber;
       public int ErrorRow;
       public string Exception = "";

       private static string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
       /// <summary>
       /// 临时日志文件
       /// </summary>
       public static string temp= dir + "\\log\\temp_error.log";
       /// <summary>
       /// 临时得sheet名统计文件
       /// </summary>
       public static string tempsheeterror = dir + "\\log\\sheet_error.log";
       /// <summary>
       /// 写文件，默认程序目录下新建当天文件，每天一个
       /// </summary>
       /// <param name="msg"></param>
       public static void Log(string msg)
       {
           string filepath = dir + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
           //总日志文件 每天一个
           using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
           {
               sw.WriteLine(msg);
           }
           //临时日志文件,每次刷新
           using (StreamWriter sw = new StreamWriter(temp, true, Encoding.UTF8))
           {
               sw.WriteLine(msg);
           }
       }
       public static void Log(Dictionary<string, List<ErrorMsg>> ErrorMsgCach)
       {
           string filepath = dir + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
           using (StreamWriter sw = new StreamWriter(filepath,true, Encoding.UTF8))
           {
               foreach (var dic in ErrorMsgCach)
               {
                   sw.WriteLine(String.Format("FILE:{0},{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), dic.Key));
                   foreach (var msg in dic.Value)
                   {
                       sw.WriteLine(msg.ToString());
                   }
               }
               
           }

           //临时日志文件,每次刷新
           using (StreamWriter sw = new StreamWriter(temp, true, Encoding.UTF8))
           {
               foreach (var dic in ErrorMsgCach)
               {
                   sw.WriteLine(String.Format("FILE:{0},{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), dic.Key));
                   foreach (var msg in dic.Value)
                   {
                       sw.WriteLine(msg.ToString());
                   }
               }
           }

       }
       public static void Log(List<string> ErrorMsgCach)
       {
           string filepath = dir + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
           using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
           {
               foreach (var msg in ErrorMsgCach)
               {
                     sw.WriteLine(msg);
               }

           }

           //临时日志文件,每次刷新
           using (StreamWriter sw = new StreamWriter(temp, true, Encoding.UTF8))
           {
               foreach (var dic in ErrorMsgCach)
               {
                   sw.WriteLine(dic);
               }
           }

       }
       /// <summary>
       /// 记下没有读取的sheetname名
       /// </summary>
       /// <param name="sheetnames"></param>
       public static void LogSheetErr(Dictionary<string, List<string>> sheetnames)
       {
           using (StreamWriter sw = new StreamWriter(tempsheeterror, true, Encoding.UTF8))
           {
               foreach (var dic in sheetnames)
               {
                   sw.WriteLine(String.Format("FILE:{0},{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), dic.Key));
                   foreach (string sheetname in dic.Value)
                   {
                       sw.WriteLine(sheetname);
                   }
               }
           }
       }
       /// <summary>
       /// 打开日志文件
       /// </summary>
       /// <param name="fileindex">1-打开临时，2-打开当天日志</param>
       public static void OpenLog(int fileindex)
       {
           switch (fileindex)
           {
               case 1:
                   if (File.Exists(temp))
                   {
                       Process.Start(new ProcessStartInfo("notepad", temp));
                   }
                   if (File.Exists(tempsheeterror))
                   {
                       Process.Start(new ProcessStartInfo("notepad", tempsheeterror));
                   }
                   break;
               case 2:
                   string filepath = dir + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                   Process.Start(new ProcessStartInfo("notepad", filepath));
                   break;
           }
       }

       public override string ToString()
       {
           return String.Format("DATA:{0},{1},{2}", this.PointNumber, this.ErrorRow,this.Exception);
       }

   }
  

}
