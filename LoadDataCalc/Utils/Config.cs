using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;
using NPOI.SS.UserModel;
using System.Globalization;

namespace LoadDataCalc
{
    public static class Config
    {
        #region//config.xml中保存的配置参数
        /// <summary> 根目录
        /// </summary>
        public static string DataRoot;
        /// <summary>项目名
        /// </summary>
        public static string ProjectName = "DefaultProject";
        /// <summary>     //数据库字符串
        /// </summary>
        public static string DataBase = "";
        /// <summary>
        /// 项目编码//构造数据处理类
        /// </summary>
        public static string ProCode = "";
        /// <summary>
        /// 是否是模数//考证表中录入的参数基准录入的是模数还是频率
        /// 主要针对振弦式仪器
        /// </summary>
        public static bool IsMoshu = false;
        /// <summary>是否自动化,自动化数据库多一个字段
        /// </summary>
        public static bool IsAuto = true;
        /// <summary> 当前项目用到的仪器类型
        /// </summary>
        public static List<InsConfig> Instruments = new List<InsConfig>();
        #endregion

        #region //全局静态变量
        /// <summary>误差限制默认为20
        /// </summary>
        public static double LimitZ = 20;
        public static double LimitT = 20;
        /// <summary>是否覆盖导入
        /// </summary>
        public static bool IsCovery = false;
        /// <summary> 是否覆盖导入全部
        /// </summary>
        public static bool IsCoveryAll = false;
        /// <summary> 指定起始时间
        /// </summary>
        public static DateTime StartTime = new DateTime();
        /// <summary>
        /// 最小温度//默认为0，每次进入程序从数据库表中查一遍
        /// </summary>
        public static double MinTemperature = 0;
        /// <summary>
        /// 最大温度，默认为70，每次进入程序从数据库表中查一遍
        /// </summary>
        public static double MaxTemperature = 70;

        /// <summary>
        /// 0值写空还是写0，0--0，1--null
        /// </summary>
        public static int  ZeroNull = 0;

        /// <summary>用户名
        /// </summary>
        public static string UserName = "sa";
        /// <summary> 用户密码
        /// </summary>
        public static string password = "sa";

        /// <summary> 多点位移计算配置类的缓存
        /// </summary>
        public static MultiDisplacementCalc MultiDisplacementCalcs = new MultiDisplacementCalc();
        /// <summary>各仪器类型表名缓存
        /// </summary>
        public static InsTableCollection InsCollection = new InsTableCollection();
        #endregion

        //程序集目录
        private static string Assemblydir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //0 为叶方毅数据库1为程翔数据库
        private static int DataBaseType = 0;

        /// <summary>
        /// 扩展数据库，流域监测系统写数据
        /// </summary>
        public static string DatabaseExpand = @"Data Source =127.0.0.1\MSSQLSERVER08;Initial Catalog = WHDT;User ID = sa;Password = 12345678;";
#if TEST
        public static long Tick1 = 0;
        public static long Tick2 = 0;
#endif


        /// <summary>从默认路径加载配置文件
        /// </summary>
        public static bool LoadConfig(bool IsReadDefault = false)
        {
            string filename = "\\config\\Config.xml";//针对项目的配置文件，只包含项目涉及到的仪器种类
            if (IsReadDefault) filename = "\\config\\Config_Default.xml";//default文件 包含所有种类仪器
            string path = Assemblydir + filename;
            if (!File.Exists(path)) return false;
            XmlDocument xml = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create(path, settings);
                xml.Load(reader);
                var root = xml.DocumentElement;
                ProjectName = root.Attributes["ProjectName"].Value;
                var dataroot = root.SelectSingleNode("DataRoot");
                DataRoot = dataroot.InnerText;
                var database = root.SelectSingleNode("DataBase");
                IsAuto = int.Parse(database.Attributes["IsAuto"].Value) == 0 ? false : true;
                //DataBaseType = int.Parse(database.Attributes["DataBaseType"].Value);
                DataBase = database.InnerText;
                var DataProCode = root.SelectSingleNode("ProCode");
                IsMoshu = int.Parse(DataProCode.Attributes["IsMoshu"].Value) == 0 ? false : true;
                ProCode = DataProCode.InnerText;
                var Instrumentents = root.SelectSingleNode("Instruments");
                var list = Instrumentents.ChildNodes;
                foreach (var node in list)
                {
                    var ent = node as XmlElement;
                    InsConfig ins = new InsConfig();
                    ins.InsName = ent.GetAttribute("Name");
                    var keylist = ent.ChildNodes;
                    foreach (var nd in keylist)
                    {
                        var keyent = nd as XmlElement;
                        ins.KeyWord.Add(keyent.InnerText);
                    }
                    Instruments.Add(ins);

                }
                //if (DataBaseType == 0)
                //{ 
                loadIns();
                //}
                //else if (DataBaseType == 1)
                //{
                //    loadNewIns();
                //}
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }
        //导入仪器类型对应的考证表和数据表
        private static void loadIns()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = dir + "\\config\\InsConfig.xml";
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            var root = xml.DocumentElement;
            var inslist = root.ChildNodes;
            List<InsTableInfo> list = new List<InsTableInfo>();
            foreach (var node in inslist)
            {
                var ent = node as XmlElement;
                InsTableInfo info = new InsTableInfo();
                info.Instrument_Name = ent.GetAttribute("Instrument_Name");
                info.Measure_Table = ent.GetAttribute("Measure_Table");
                info.Monitor_Name = ent.GetAttribute("Monitor_Name");
                info.Result_Table = ent.GetAttribute("Result_Table");
                info.Fiducial_Table = ent.GetAttribute("Fiducial_Table");
                list.Add(info);
            }
            InsCollection.InsTables = list;
            Dictionary<string, InstrumentType> InsDic = new Dictionary<string, InstrumentType>();
            foreach (int myCode in Enum.GetValues(typeof(InstrumentType)))
            {
                var instype = (InstrumentType)myCode;
                string insname = instype.GetDescription();
                InsDic.Add(insname, (InstrumentType)myCode);
            }
            InsCollection.InstrumentDic = InsDic;
        }
        private static void loadNewIns()
        {
            CSqlServerHelper.Connectionstr = Config.DataBase;
            var sqlhelper = CSqlServerHelper.GetInstance();
            string sql = @"SELECT * FROM Table_PointTableName";
            var table = sqlhelper.SelectData(sql);
            List<InsTableInfo> list = new List<InsTableInfo>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                InsTableInfo info = new InsTableInfo();
                info.Instrument_Name = table.Rows[i]["Point_Name"].ToString();
                info.Measure_Table = table.Rows[i]["Result_Number"].ToString();
                info.Fiducial_Table = table.Rows[i]["Info_Number"].ToString();
                list.Add(info);
            }
            InsCollection.InsTables = list;
            Dictionary<string, InstrumentType> InsDic = new Dictionary<string, InstrumentType>();
            foreach (int myCode in Enum.GetValues(typeof(InstrumentType)))
            {
                var instype = (InstrumentType)myCode;
                string insname = instype.GetDescription();
                InsDic.Add(insname, (InstrumentType)myCode);
            }
            InsCollection.InstrumentDic = InsDic;

        }

        /// <summary>加载多点位移计计算excel
        /// </summary>
        public static void LoadMultiDisplacementCalcs()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config\\MultiDisplacement.xls";
            if (!File.Exists(path)) return;
            MultiDisplacementCalcs.loadexcel(path);
        }

        /// <summary>
        /// 保存,只有修改路径时有调用
        /// </summary>
        public static void WriteConfig()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            XmlDocument xml = new XmlDocument();
            XmlDeclaration xmlDeclaration = xml.CreateXmlDeclaration("1.0", "utf-8", "yes");
            var root = xml.CreateElement("ProConfig");
            root.SetAttribute("ProjectName", ProjectName);
            xml.AppendChild(xmlDeclaration);
            xml.AppendChild(root);
            var element = xml.CreateElement("DataRoot");
            element.InnerText = DataRoot;
            root.AppendChild(element);
            element = xml.CreateElement("DataBase");
            element.SetAttribute("IsAuto", (IsAuto ? 1 : 0).ToString());
            element.InnerText = DataBase;
            root.AppendChild(element);

            element = xml.CreateElement("ProCode");
            element.SetAttribute("IsMoshu", (IsMoshu ? 1 : 0).ToString());
            element.InnerText = ProCode;
            root.AppendChild(element);

            element = xml.CreateElement("Instruments");
            root.AppendChild(element);
            if (Instruments.Count == 0)
            {//初始化给个默认值
                var ent = xml.CreateElement("Ins");
                var attr = xml.CreateAttribute("Name");
                attr.Value = "渗压计";
                ent.Attributes.Append(attr);
                var entkeyword = xml.CreateElement("KeyWord");
                entkeyword.InnerText = "渗压计";
                ent.AppendChild(entkeyword);
                element.AppendChild(ent);
            }
            else
            {
                foreach (var ins in Instruments)
                {
                    var ent = xml.CreateElement("Ins");
                    var attr = xml.CreateAttribute("Name");
                    attr.Value = ins.InsName;
                    ent.Attributes.Append(attr);
                    foreach (var key in ins.KeyWord)
                    {
                        var entkeyword = xml.CreateElement("KeyWord");
                        entkeyword.InnerText = key;
                        ent.AppendChild(entkeyword);
                    }
                    element.AppendChild(ent);

                }
            }
            root.AppendChild(element);
            string path = dir + "\\config\\Config.xml";
            xml.Save(path);
        }
        /// <summary>写一个默认模板
        /// </summary>
        public static void WriteDifaultConfig()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = dir + "\\config\\Config_Default.xml";
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
            element.SetAttribute("IsAuto", (IsAuto ? 1 : 0).ToString());
            element.InnerText = "Data Source = 10.6.179.44,1433;Network Library = DBMSSOCN;Initial Catalog = MWDatabase;User ID = sa;Password = sa;";
            element = xml.CreateElement("ProCode");
            element.SetAttribute("IsMoshu", (IsMoshu ? 1 : 0).ToString());
            element.InnerText = ProCode;
            root.AppendChild(element);
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
                ent.AppendChild(entkeyword);
                element.AppendChild(ent);
            }
            root.AppendChild(element);
            xml.Save(path);
        }

        /// <summary>写文件列表缓存
        /// </summary>
        /// <param name="files"></param>
        public static void WriteFilesList(Dictionary<string, List<string>> files)
        {
            XmlDocument xml = new XmlDocument();
            XmlDeclaration xmlDeclaration = xml.CreateXmlDeclaration("1.0", "utf-8", "yes");
            var root = xml.CreateElement("FileList");
            xml.AppendChild(xmlDeclaration);
            xml.AppendChild(root);
            foreach (var d in files)
            {
                var element = xml.CreateElement("Ins");
                var attr = xml.CreateAttribute("Name");
                attr.Value = d.Key;
                element.Attributes.Append(attr);
                foreach (var f in d.Value)
                {
                    var entkeyword = xml.CreateElement("File");
                    entkeyword.InnerText = f;
                    element.AppendChild(entkeyword);
                }
                root.AppendChild(element);
            }
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = dir + "\\config\\FileList.xml";
            xml.Save(path);
        }
        /// <summary> 读取文件列表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<string>> ReadFileList()
        {
            Dictionary<string, List<string>> files = new Dictionary<string, List<string>>();
            string filename = "\\config\\FileList.xml";
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = dir + filename;
            if (!File.Exists(path)) return null;
            try
            {
                XmlDocument xml = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(path, settings);
                xml.Load(reader);
                var root = xml.DocumentElement;
                var nodes = root.ChildNodes;
                foreach (XmlNode node in nodes)
                {
                    var ent = node as XmlElement;
                    string insname = ent.GetAttribute("Name");
                    List<string> fs = new List<string>();
                    foreach (XmlNode nd in node.ChildNodes)
                    {
                        fs.Add((nd as XmlElement).InnerText);
                    }
                    files.Add(insname, fs);
                }
                reader.Close();
                return files;
            }
            catch
            {
                return null;
            }

        }
        /// <summary> 获取多点位移计从浅到深排序的点，默认不在此列中得点为从深到浅排序
        /// </summary>
        /// <returns></returns>
        public static List<string> GetMultiDisplacementOrder()
        {
            List<string> list = new List<string>();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config\\MultiDisplacement.xls";
            var workbook = WorkbookFactory.Create(path);
            string sheetname = Config.ProCode + "_Order";
            var psheet = workbook.GetSheet(sheetname);
            if (psheet == null) return list;
            int count = psheet.LastRowNum + 1;
            for (int i = 0; i < count; i++)
            {
                IRow row = psheet.GetRow(i);
                list.Add(row.GetCell(0).StringCellValue.ToUpper().Trim());
            }
            return list;
        }
        /// <summary> 获取锚索测力计每根弦的初始值
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<double>> GetAnchor_Cable_R0()
        {
            Dictionary<string, List<double>> Dic = new Dictionary<string, List<double>>();
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config\\Anchor_Cable_R0.xls";
            var workbook = WorkbookFactory.Create(path);
            string sheetname = Config.ProCode;
            var psheet = workbook.GetSheet(sheetname);
            if (psheet == null) return Dic;
            int count = psheet.LastRowNum + 1;
            for (int i = 0; i < count; i++)
            {
                IRow row = psheet.GetRow(i);
                string name = row.GetCell(0).StringCellValue;
                List<double> list = new List<double>();
                for (int j = 1; j < row.Cells.Count; j++)
                {
                    list.Add(row.GetCell(j).NumericCellValue);
                }
                Dic.Add(name, list);
            }
            return Dic;
        }
    }
    public class InsConfig
    {
        public string InsName;
        public List<string> KeyWord = new List<string>();
    }
    /// <summary> 仪器名，考证表，测值表，成果表参照类
    /// </summary>
    public class InsTableInfo
    {
        public string Instrument_Name;
        public string Fiducial_Table;
        public string Measure_Table;
        public string Monitor_Name;
        public string Result_Table;
    }
   /// <summary>仪器类型集合类
   /// </summary>
    public class InsTableCollection
    {
       
        /// <summary>所有仪器名和枚举类型词典
        /// </summary>
        public  Dictionary<string, InstrumentType> InstrumentDic = new Dictionary<string, InstrumentType>();
        /// <summary> 表名信息
        /// </summary>
        public List<InsTableInfo> InsTables = new List<InsTableInfo>();
        public InsTableInfo this[string Instrument_Name]
        {
            get { return InsTables.Where(ins => ins.Instrument_Name == Instrument_Name).FirstOrDefault(); }
        }
        public InsTableInfo GetFromFiducial(string Fiducial_Table)
        {
            return InsTables.Where(ins => ins.Fiducial_Table == Fiducial_Table).First();
        }
        public InsTableInfo GetFromMeasure(string Measure_Table)
        {
            return InsTables.Where(ins => ins.Measure_Table == Measure_Table).First();
        }
        public InsTableInfo GetFromResult(string Result_Table)
        {
            return InsTables.Where(ins => ins.Result_Table == Result_Table).First();
        }
    }
    public static class DataUtils
    {
        public static  bool CheckDateTime(DateTime dt)
        {
            var maxDt = DateTime.Now;
            var minDt = new DateTime(1990,1,1,0,0,0);
            return (dt.CompareTo(maxDt) < 0 && dt.CompareTo(minDt) > 0);
        }
        public static bool CheckContainStr(string ChkStr,params string[] keys)
        {
            if (keys == null) return false;
            if (keys.Length ==0) return false;
            foreach (string key in keys)
            {
                if (ChkStr.Contains(key))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool CheckStrIgnoreCN(string a,string b)
        {
            return CultureInfo.GetCultureInfo("zh-cn").CompareInfo.Compare(a, b, CompareOptions.IgnoreWidth|CompareOptions.IgnoreCase) == 0;
        }
       
    
    }
    
}
