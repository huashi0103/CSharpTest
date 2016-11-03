/*
 * 读取文件、读数据、写数据
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
    public class LoadDataClass
    {
       //唯一实例
        private static  LoadDataClass loadData = null;
        //数据库实例
        private CSqlServerHelper sqlhelper = null;
        //程序集目录
        private string Assemblydir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //状态委托
        public Action<string> StatusAction;
        //数据锁
        private object DataLock = new object();

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
        public string Init()
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
                }
            }
            catch
            { 
            }
            return res;

        }

        private bool CheckFile(string filename,params string[] insname)
        {
            foreach(string ins in insname)
            {
                string[] multikey = Config.Instruments.Where(x => x.InsName == ins).FirstOrDefault().KeyWord.ToArray();
                if (DataUtils.CheckContainStr(filename, multikey)) return true;
            }
            return false;
      
        }

        /// <summary>
        /// 获取包含关键词的文件，关键词为空，返回所有文件
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
                        else if (insname == "锚杆应力计")
                        {
                            if (!CheckFile(filename, "多点锚杆应力计")) refiles.Add(file);
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
        
        /// <summary>
        ///         //数据缓存
        /// </summary>
        public List<PointSurveyData> SurveyDataCach = new List<PointSurveyData>();
        public Dictionary<string, List<ErrorMsg>> ErrorMsgCach = new Dictionary<string, List<ErrorMsg>>();

        /// <summary>
        ///数据缓存,缓存应变计对应的无应力计数据
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
            }
            //});

            //应变计和应变计组同时也读一遍无应力计的数据
            if (instype==InstrumentType.Fiducial_Strain_Gauge||instype==InstrumentType.Fiducial_Strain_Group)
            {
                SurveyDataCachExpand = process.NonStressDataCach;
            }
            if (StatusAction != null) StatusAction("读取完成,正在写入日志文件");
            ErrorMsg.Log(ErrorMsgCach);
            ErrorMsg.LogSheetErr(process.ErrorSheetName);
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

        //获取应变计和应变机组对应的无应力计数据
        private SurveyData GetNonStressSurveyData(PointSurveyData pd, SurveyData sd)
        {
            if (pd == null) return null;
            foreach (var nsd in pd.Datas)
            {
                if (nsd.SurveyDate.Date == sd.SurveyDate.Date)
                {
                    return nsd;
                }
            }
            return null;
            
        }

        /// <summary>
        /// 计算
        /// </summary>
        public void Calc(InstrumentType instype, string expression=null)
        {
            inscalc = CalcFactoryClass.CreateInstCalc(instype);
            List<string> Errors = new List<string>();
            foreach (var pd in SurveyDataCach)
            {
                ParamData param = inscalc.GetParam(pd.SurveyPoint, instype.ToString());
                PointSurveyData NonStressPoint = new PointSurveyData();
                if (param.IsHasNonStress && (instype == InstrumentType.Fiducial_Strain_Gauge || instype == InstrumentType.Fiducial_Strain_Group))
                {
                   NonStressPoint= SurveyDataCachExpand.Where(p => p.SurveyPoint == param.NonStressNumber).FirstOrDefault();
                   if (NonStressPoint == null || NonStressPoint.Datas.Count < 1)
                   {
                       Errors.Add(String.Format("PARAM:{0},{1}", pd.SurveyPoint, "找不到对应的无应力计数据"));
                   }
                }
                if (param == null)
                {
                    Errors.Add(String.Format("PARAM:{0}", pd.SurveyPoint));//保存读取参数的错误
                    pd.IsCalc = false;
                    continue;
                }
                if (expression != null)param.InsCalcType = CalcType.AutoDefine;
                foreach (SurveyData sd in pd.Datas)
                {
                    sd.Survey_ZorRMoshu = sd.Survey_ZorR;
                    if (param.IsHasNonStress && (instype == InstrumentType.Fiducial_Strain_Gauge || instype == InstrumentType.Fiducial_Strain_Group))
                    {
                        if (NonStressPoint == null || NonStressPoint.Datas.Count < 1)
                        {
                            continue;
                        }
                        var nondata = GetNonStressSurveyData(NonStressPoint, sd);
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

                            //计算无应力计
                            Fiducial_Nonstress fn = new Fiducial_Nonstress();
                            ParamData nonpd = fn.GetParam(param.NonStressNumber, "Fiducial_Nonstress");
                            fn.FuncDic[nonpd.InsCalcType](nonpd, sd.NonStressSurveyData, null);
                        }
                        else
                        {
                            Errors.Add(String.Format("PARAM:{0},{1},{2}", pd.SurveyPoint,sd.SurveyDate.Date.ToString(), "找不到对应的无应力计数据"));
                        }
                    }

                    inscalc.FuncDic[param.InsCalcType](param, sd,expression);
                }
                pd.IsCalc = true;
            }
            ErrorMsg.Log(Errors);
        }
        /// <summary>写数据
        /// </summary>
        /// <returns></returns>
        public void Wirte(InstrumentType instype)
        {
            if (inscalc == null) return;
            int result = inscalc.WriteSurveyToDB(SurveyDataCach);
            ErrorMsg.Log(String.Format("写入{0}行测值",result));
            result = inscalc.WriteResultToDB(SurveyDataCach);
            ErrorMsg.Log(String.Format("写入{0}行成果值", result));
            
        }
        void getFiles(List<string> list, string path, string pattern)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            var allfiles = di.GetFiles(pattern);
            foreach (FileInfo fi in allfiles)
            {
                if ((fi.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    list.Add(fi.FullName);
                }

            }
        }
        public void getDir(string path, List<string> FileList)
        {
            getFiles(FileList, path, "*.xls");
            getFiles(FileList, path, "*.xlsx");
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
    }

    /// <summary> 读写数据基类，项目涉及的仪器类型都要继承实现该类
    /// </summary>
  
   /// <summary>处理数据类的工厂类
   /// </summary>
   public class ProcessFactory
   {
       public static ProcessData CreateInstance(InstrumentType type)
       {
           Assembly ass = Assembly.GetExecutingAssembly();
           string completeName = String.Format("LoadDataCalc.{0}Process{1}", type.ToString(),Config.ProCode);
           var process = ass.CreateInstance(completeName) as ProcessData;
           return process; 
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
       public static string temp= dir + "\\log\\temp_error.log";
       public static string tempsheeterror = dir + "\\log\\sheet_error.log";
       /// <summary>
       /// 写文件，默认程序目录下新建当天文件，每天一个
       /// </summary>
       /// <param name="msg"></param>
       public  static void Log(string msg)
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
    /// <summary>单点仪器 数据在excel中的索引列
    /// 多点数据列记录的为起始列
    /// </summary>
   public class DataInfo:ICloneable
   {
       public string TableName = "";
       public int DateIndex = 0;
       public int TimeIndex = 1;
       public int ZoRIndex = 2;
       public int RorTIndex = 3;
       public int RemarkIndex = 6;
       /// <summary>应变机组中的无应力计
       /// </summary>
       public int NZoRIndex = 2;
       public int NRorTIndex = 3;
       public int Sum = 4;
       /// <summary>
       ///多点位移计起始列,应变计组的起始列
       /// </summary>
       public int Findex = -1;
       /// <summary>
       /// 应变计组的电阻比总列数，与数据库中的向数做对比,
       /// 确定是否包含无应力计的数据
       /// </summary>
       public int FCount = 0;

        public object  Clone()
        {
            return this.MemberwiseClone();
        }
}


}
