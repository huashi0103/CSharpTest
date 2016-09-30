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
using Microsoft.Office.Interop.Excel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Diagnostics;


namespace LoadDataCalc
{
    /// <summary>
    /// 读取计算表格和配置文件
    /// </summary>
    public class LoadDataClass
    {
        /// <summary>
        /// 根目录
        /// </summary>
        public string DataRoot
        {
            get { return dataRoot; }
            set { 
                if(Directory.Exists(value))
                {
                    dataRoot=value;
                }
                else { throw new Exception("路径不存在"); }
            }
        }
        private string dataRoot = "";
        /// <summary> 当前项目用到的仪器类型
        /// </summary>
        public List<InsConfig> Instruments = new List<InsConfig>();
        /// <summary>项目名
        /// </summary>
        public string ProjectName = "DefaultProject";
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName = "sa";
        /// <summary>
        /// 用户密码
        /// </summary>
        public string password = "sa";
        //数据库字符串
        private string DataBase = "";
       //唯一实例
        private static  LoadDataClass loadData = null;
        //数据库实例
        private CSqlServerHelper sqlhelper = null;
        //程序集目录
        private string Assemblydir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

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
        public int Init()
        {
            int res = 0;
            try
            {
                WriteDifaultConfig();//写一个默认模板
                res = 1;
                //读取本项目配置文件
                if (!LoadConfig()) return res;
                res = 2;
                CSqlServerHelper.Connectionstr = string.Format(DataBase, UserName, password);
                sqlhelper = CSqlServerHelper.GetInstance();
                if (sqlhelper.Check())
                {
                    res = 3;
                }
            }
            catch
            { 
            }
            return res;

        }

        /// <summary>
        /// 获取包含关键词的文件，关键词为空，返回所有文件
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>文件</returns>
        public List<string> GetFiles(string keyword)
        {
            List<string> files = new List<string>();
            getDir(dataRoot, files);
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
                    if (filename.Contains(keyword))
                    {
                        refiles.Add(file);
                    }
                }
                return refiles;
            }
 
        }
        /// <summary>从默认路径加载配置文件
        /// </summary>
        private bool LoadConfig(bool IsReadDefault=false)
        {
            string filename="\\Config.xml";//针对项目的配置文件，只包含项目涉及到的仪器种类
            if (IsReadDefault) filename = "\\Config_Default.xml";//default文件 包含所有种类仪器
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = dir + filename;
            if (!File.Exists(path)) return false;
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(path);
                var root = xml.DocumentElement;
                this.ProjectName = root.Attributes["ProjectName"].Value;
                var dataroot = root.SelectSingleNode("DataRoot");
                this.DataRoot = dataroot.InnerText;
                var database = root.SelectSingleNode("DataBase");
                this.DataBase = database.InnerText;
                var Instruments = root.SelectSingleNode("Instruments");
                var list = Instruments.ChildNodes;
                foreach (var node in list)
                {
                    var ent = node as XmlElement;
                    InsConfig ins = new InsConfig();
                    ins.InsName = ent.GetAttribute("Name");
                    var keylist = ent.ChildNodes;
                    foreach (var nd in keylist)
                    {
                        var keyent=nd as XmlElement;
                        ins.KeyWord.Add(keyent.InnerText);
                    }
                    this.Instruments.Add(ins);
                }
                return true;
            }
            catch {
                return false;
            }
        }
        /// <summary>
        /// 保存，暂时没有调用
        /// </summary>
        public void WriteConfig()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            XmlDocument xml = new XmlDocument();
            XmlDeclaration xmlDeclaration = xml.CreateXmlDeclaration("1.0", "utf-8", "yes");
            var root = xml.CreateElement("ProConfig");
            root.SetAttribute("ProjectName",this.ProjectName);
            xml.AppendChild(xmlDeclaration);
            xml.AppendChild(root);
            var element=xml.CreateElement("DataRoot");
            element.InnerText = this.dataRoot;
            root.AppendChild(element);
            element = xml.CreateElement("DataBase");
            element.InnerText = this.DataBase;
            root.AppendChild(element);
            element=xml.CreateElement("Instruments");
            root.AppendChild(element);
            if (Instruments.Count == 0)
            {//初始化给个默认值
                var ent = xml.CreateElement("Ins");

                var attr=xml.CreateAttribute("Name");
                attr.Value="渗压计";
                ent.Attributes.Append(attr);
               
                var entkeyword = xml.CreateElement("KeyWord");
                entkeyword.InnerXml="渗压计";
                ent.AppendChild(entkeyword);
                element.AppendChild(ent);
            }
            else
            {
                foreach (var ins in Instruments)
                {
                    var ent = xml.CreateElement("Ins");
                    var attr=xml.CreateAttribute("Name");
                    attr.Value=ins.InsName;
                    ent.Attributes.Append(attr);
                    element.AppendChild(ent);
                }
            }
            root.AppendChild(element);
            string path = dir + "\\Config.xml";
            xml.Save(path);
            
        }
        /// <summary>写一个默认模板
        /// </summary>
        public void WriteDifaultConfig()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = dir + "\\Config_Default.xml";
            if (File.Exists(path)) return;
            XmlDocument xml = new XmlDocument();
            XmlDeclaration xmlDeclaration = xml.CreateXmlDeclaration("1.0", "utf-8", "yes");
            var root = xml.CreateElement("ProConfig");
            root.SetAttribute("ProjectName", "DefaultProject");
            xml.AppendChild(xmlDeclaration);
            xml.AppendChild(root);
            var element = xml.CreateElement("DataRoot");
            element.InnerText = dir;
            root.AppendChild(element);
            element = xml.CreateElement("DataBase");
            element.InnerText = "Data Source = 10.6.179.44,1433;Network Library = DBMSSOCN;Initial Catalog = MWDatabase;User ID = {0};Password = {1};";
            element = xml.CreateElement("Instruments");
            root.AppendChild(element);
            foreach (int myCode in Enum.GetValues(typeof(InstrumentType)))
            {
                var instype = (InstrumentType)myCode;
                string insname = instype.GetDescription();
                var ent = xml.CreateElement("Ins");
                ent.InnerText = insname;
                ent.SetAttribute("Name", insname);
                var entkeyword = xml.CreateElement("KeyWord");
                entkeyword.InnerText = insname;
                ent.AppendChild(ent);
                element.AppendChild(ent);
            }
            root.AppendChild(element);
            xml.Save(path);
        }

        //数据缓存
        public List<PointSurveyData> SurveyDataCach = new List<PointSurveyData>();
        public List<ErrorMsg> ErrorMsgCach = new List<ErrorMsg>();
        /// <summary> 当前处理仪器类
        /// </summary>
        private ProcessData process = null;

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="instype"></param>
        public void ReadData(InstrumentType instype)
        {
            string temppath = Assemblydir + "\\temp_error.log";
            if (File.Exists(temppath)) File.Delete(temppath);//临时文件每次读取清空
            ProcessData process = ProcessFactory.CreateInstance(instype);
            if (process == null) return;
            List<string> files = GetFiles(instype.GetDescription());
            SurveyDataCach.Clear();
            ErrorMsgCach.Clear();
            foreach (string file in files)
            {
                List<PointSurveyData> datas = new List<PointSurveyData>();
                List<ErrorMsg> msgs = new List<ErrorMsg>();
                process.ReadData(file, out datas, out msgs);
                SurveyDataCach.AddRange(datas);
                ErrorMsgCach.AddRange(msgs);
            }
           
        }
        /// <summary>
        /// 计算
        /// </summary>
        public void Calc(InstrumentType instype, string expression=null)
        {
            BaseInstrument inscalc = CalcFactoryClass.CreateInstCalc(instype);
            foreach (var pd in SurveyDataCach)
            {

                ParamData param = inscalc.GetParam(pd.SurveyPoint, instype.ToString());
                if (param == null)
                {
                    ErrorMsg.Log(String.Format("PARAM:{0}",pd.SurveyPoint));//保存读取参数的错误
                    pd.IsCalc = false;
                    continue;
                }
                if (expression != null)
                {
                    param.InsCalcType = CalcType.AutoDefine;
                }

                switch (param.InsCalcType)
                {
                    case CalcType.DifBlock:
                        foreach (SurveyData sd in pd.Datas)
                        {
                            inscalc.DifBlock(param, sd);
                        }
                        break;
                    case CalcType.ShakeString:
                        foreach (SurveyData sd in pd.Datas)
                        {
                            inscalc.ShakeString(param, sd);
                        }
                        break;
                    case CalcType.AutoDefine:
                        foreach (SurveyData sd in pd.Datas)
                        {
                            inscalc.AutoDefined(param, sd, expression);
                        }
                        break;
                }
                pd.IsCalc = true;
            }
        }

        private void getDir(string path, List<string> FileList)
        {
            FileList.AddRange(Directory.GetFiles(path,"*.xls"));
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
   public  abstract class  ProcessData
    {
        /// <summary>仪器类型
        /// </summary>
        public InstrumentType InsType { get; set; }
        /// <summary> 从excel文件中读取数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract  void ReadData(string path, out List<PointSurveyData> datas,out List<ErrorMsg>errors);
        /// <summary>把测量数据写入到数据库
        /// </summary>
        /// <returns></returns>
        public abstract int WriteSurveyToDB(List<PointSurveyData> datas);
        /// <summary>把计算后的结果数据写入数据库
        /// </summary>
        /// <returns></returns>
        public abstract int WriteResultToDB(List<PointSurveyData> datas);

       /// <summary> 保存读取单个文件的错误信息
       /// </summary>
       /// <param name="filepath"></param>
       /// <param name="msgs"></param>
        public void Log(string filepath,List<ErrorMsg> msgs)
        {
            ErrorMsg.Log(String.Format("FILE:{0},{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),filepath));
            foreach (var msg in msgs)
            {
                ErrorMsg.Log(msg.ToString());
            }

        }

    }

   public class ProcessFactory
   {
       public static ProcessData CreateInstance(InstrumentType type)
       {
           switch (type)
           {
               case InstrumentType.Fiducial_Leakage_Pressure:
                   return new Fiducial_Leakage_PressureProcess();
               default:
                   return null;
           }
 
       }
   }


   /// <summary>日志类
   /// </summary>
   public class ErrorMsg
   {
       public string PointNumber;
       public int ErrorRow;
       private static string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
       private static string temp= dir + "\\temp_error.log";
       /// <summary>
       /// 写文件，默认程序目录下新建当天文件，每天一个
       /// </summary>
       /// <param name="msg"></param>
       public  static void Log(string msg)
       {
           string filepath = dir + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
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
       public static void OpenLog(int fileindex)
       {
           switch (fileindex)
           {
               case 1:
                   Process.Start(new ProcessStartInfo("notepad", temp));
                   break;
               case 2:
                   string filepath = dir + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                   Process.Start(new ProcessStartInfo("notepad", filepath));
                   break;
           }
       }

       public override string ToString()
       {
           return String.Format("DATA:{0},{1}", this.PointNumber, this.ErrorRow);
       }

   }



}
