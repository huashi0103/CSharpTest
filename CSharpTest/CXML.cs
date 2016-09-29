using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CSharpTest
{
    public class CXML
    {
        public void readxml(string path)
        {
            XmlDocument doc=new XmlDocument();
            doc.Load(path);
            /*从指定的字符创加载xml文件 例如：
            xmlDoc.LoadXml("(<Book bookID='B001'><BookName>jeff</BookName><price>45.6</price></Book>)");
            */
            var root=doc.SelectSingleNode("CONFIG_LIST");
            Console.WriteLine(root.Attributes["Count"].Value);
            var list=root.ChildNodes;
            foreach(var node in list)
            {
               var ent=node as XmlElement;
               Console.WriteLine(ent.Attributes["Instrument_Name"].Value + ":" + ent.Attributes["Fiducial_Table"].Value);
            }
    
        }
        public void writexml(string path)
        {
            XmlDocument doc = new XmlDocument();
        }
    }
}
