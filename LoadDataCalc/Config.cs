using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;

namespace LoadDataCalc
{
    public class Config
    {
        public string ProjectName;
        public string DataBase;
        public string DataRoot;
        public List<string> Instruments = new List<string>();


        /// <summary>从默认路径加载配置文件
        /// </summary>
        private bool LoadConfig(bool IsReadDefault = false)
        {
            string filename = "\\Config.xml";//针对项目的配置文件，只包含项目涉及到的仪器种类
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
                    this.Instruments.Add(ent.InnerText);
                }
                return true;
            }
            catch
            {
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
            root.SetAttribute("ProjectName", this.ProjectName);
            xml.AppendChild(xmlDeclaration);
            xml.AppendChild(root);
            var element = xml.CreateElement("DataRoot");
            element.InnerText = this.DataRoot;
            root.AppendChild(element);
            element = xml.CreateElement("DataBase");
            element.InnerText = this.DataBase;
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
                entkeyword.InnerXml = "渗压计";
                ent.AppendChild(entkeyword);
                element.AppendChild(ent);
            }
            else
            {
                foreach (string ins in Instruments)
                {
                    var ent = xml.CreateElement("Ins");
                    var attr = xml.CreateAttribute("Name");
                    attr.Value = ins;
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
            element.InnerText = "Data Source = 10.6.179.9,1433;Network Library = DBMSSOCN;Initial Catalog = MWDatabase;User ID = {0};Password = {1};";
            element = xml.CreateElement("Instruments");
            root.AppendChild(element);
            foreach (int myCode in Enum.GetValues(typeof(InstrumentType)))
            {
                var instype = (InstrumentType)myCode;
                string insname = instype.GetDescription();
                var ent = xml.CreateElement("Ins");
                ent.InnerText = insname;
                ent.SetAttribute("Name", insname);
                ent.SetAttribute("Col", "0");
                ent.SetAttribute("DateRowIndex", "0");
                ent.SetAttribute("TimeRowIndex", "1");
                ent.SetAttribute("ZorRRowIndex", "2");
                ent.SetAttribute("RorTRowIndex", "3");
                ent.SetAttribute("Remark", "4");
                var entkeyword = xml.CreateElement("KeyWord");
                entkeyword.InnerXml = insname;
                ent.AppendChild(ent);
                element.AppendChild(ent);
            }
            root.AppendChild(element);
            xml.Save(path);
        }

    }
    public class InsConfig
    {
        public string InsName;
        public List<string> KeyWord = new List<string>();
    }
}
