using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BG.EmuladorCnb.App.App_Code
{
    public class Configuration
    {
        private XmlDocument xmlDoc;
        private string fullFileName;
        private string rootTagName;

        public Configuration(string fullFileName)
        {
            this.fullFileName = fullFileName;
            this.rootTagName = nameof(Configuration);
            this.xmlDoc = new XmlDocument();
            this.loadXmlDocument();
        }

        private void loadXmlDocument()
        {
            try
            {
                this.xmlDoc.Load(this.fullFileName);
            }
            catch (Exception ex)
            {
            }
        }

        public string RootTagName
        {
            get => this.rootTagName;
            set => this.rootTagName = value;
        }

        public string getString(string sectionTagName, string fieldTagName, string defaultValue)
        {
            try
            {
                return ((XmlElement)this.xmlDoc.GetElementsByTagName(sectionTagName)[0]).GetElementsByTagName(fieldTagName)[0].InnerXml;
            }
            catch (Exception ex)
            {
                return defaultValue;
            }
        }

        public int getInteger(string sectionTagName, string fieldTagName, int defaultValue)
        {
            try
            {
                return int.Parse(((XmlElement)this.xmlDoc.GetElementsByTagName(sectionTagName)[0]).GetElementsByTagName(fieldTagName)[0].InnerXml);
            }
            catch (Exception ex)
            {
                return defaultValue;
            }
        }

        public bool getBoolean(string sectionTagName, string fieldTagName, bool defaultValue)
        {
            try
            {
                return bool.Parse(((XmlElement)this.xmlDoc.GetElementsByTagName(sectionTagName)[0]).GetElementsByTagName(fieldTagName)[0].InnerXml);
            }
            catch (Exception ex)
            {
                return defaultValue;
            }
        }
    }
}
