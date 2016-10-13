﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;

namespace LoadDataCalc
{
    public  static class Config
    {
        /// <summary>
        /// 根目录
        /// </summary>
        public static string DataRoot
        {
            get { return dataRoot; }
            set
            {
                if (Directory.Exists(value))
                {
                    dataRoot = value;
                }
                else { throw new Exception("路径不存在"); }
            }
        }
        private  static string dataRoot = "";
        /// <summary> 当前项目用到的仪器类型
        /// </summary>
        public static  List<InsConfig> Instruments = new List<InsConfig>();
        /// <summary>项目名
        /// </summary>
        public static string ProjectName = "DefaultProject";
        /// <summary>
        /// 用户名
        /// </summary>
        public static  string UserName = "sa";
        /// <summary>
        /// 用户密码
        /// </summary>
        public static  string password = "sa";
        //数据库字符串
        public static  string DataBase = "";
        //程序集目录
        private  static string Assemblydir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


        /// <summary>从默认路径加载配置文件
        /// </summary>
        public  static bool LoadConfig(bool IsReadDefault = false)
        {
            string filename = "\\config\\Config.xml";//针对项目的配置文件，只包含项目涉及到的仪器种类
            if (IsReadDefault) filename = "\\config\\Config_Default.xml";//default文件 包含所有种类仪器
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = dir + filename;
            if (!File.Exists(path)) return false;
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(path);
                var root = xml.DocumentElement;
                ProjectName = root.Attributes["ProjectName"].Value;
                var dataroot = root.SelectSingleNode("DataRoot");
                DataRoot = dataroot.InnerText;
                var database = root.SelectSingleNode("DataBase");
                DataBase = database.InnerText;
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
                return true;
            }
            catch
            {
                return false;
            }
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
            element.InnerText = DataBase;
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
        public static  void WriteDifaultConfig()
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

        /// <summary>
        /// 写文件列表缓存
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
        /// <summary>
        /// 读取文件列表
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
                xml.Load(path);
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
                return files;
            }
            catch
            {
                return null;
            }
 
        }
    }
    public class InsConfig
    {
        public string InsName;
        public List<string> KeyWord = new List<string>();
    }
}
