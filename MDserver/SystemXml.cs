using System;
using System.IO;
using System.Xml;

namespace MDserver
{
    class SystemXml
    {
        public string filename;

        public SystemXml(string AFileName)
        {
            filename = AFileName;

            if (!File.Exists(AFileName))
            {
                var xmldoc = new XmlDocument();
                var xmldecl = xmldoc.CreateXmlDeclaration("1.0", "gb2312", null);
                xmldoc.AppendChild(xmldecl);

                var xmlelem = xmldoc.CreateElement("", "root", "");
                xmldoc.AppendChild(xmlelem);

                xmldoc.Save(AFileName);
            }
        }

        public XmlNode rootNode()
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);
            XmlNode root = xmldoc.SelectSingleNode("root");
            return root;
        }

        public XmlNode selectedNode(int index)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);

            XmlNode root = xmldoc.SelectSingleNode("root");
            XmlNode ch = root.ChildNodes.Item(index);

            xmldoc.Save(filename);
            return ch;
        }


        public Boolean updateNode(int index, string key, string value)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);

            XmlNode root = xmldoc.SelectSingleNode("root");
            XmlElement ch = (XmlElement)root.ChildNodes.Item(index);

            ch.SetAttribute(key,value);

            xmldoc.Save(filename);
            return true;
        }


        public Boolean addNode(string nodeName, string childKey, string childValue) {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);

            XmlElement n = xmldoc.CreateElement("pn");
            n.SetAttribute("name", nodeName);
            n.SetAttribute(childKey, childValue);

            XmlNode root = xmldoc.SelectSingleNode("root");
            root.AppendChild(n);

            xmldoc.Save(filename);
            return true;
        }

        public Boolean removeNode(string nodeName) {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);


            XmlNode root = xmldoc.SelectSingleNode("root");

            XmlNode selectedNdoe = root.SelectSingleNode(nodeName); ;
            root.ParentNode.RemoveChild(selectedNdoe);

            xmldoc.Save(filename);
            return true;
        }

        public Boolean removeNode(int index) {
            if (index < 0) {
                return false;
            }

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);


            XmlNode root = xmldoc.SelectSingleNode("root");

            XmlNode selectedNdoe = root.ChildNodes.Item(index);
            root.RemoveChild(selectedNdoe);

            xmldoc.Save(filename);

            return true;
        }


    }
}
