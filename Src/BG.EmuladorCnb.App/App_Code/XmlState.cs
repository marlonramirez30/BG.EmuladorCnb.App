using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BG.EmuladorCnb.App.App_Code
{
    internal class XmlState
    {
        private XmlDocument xmlDoc;
        private string fullName;

        public XmlState(string path, string fileName)
        {
            this.fullName = $"{path}\\{fileName}";
            this.load();
        }

        private void load()
        {
            try
            {
                this.xmlDoc = new XmlDocument();
                this.xmlDoc.Load(this.fullName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error initializing or reinitializing XmlState - {this.fullName} - {ex.Message}");
            }
        }

        public void reload() => this.load();

        public XmlNode getStateById(string id)
        {
            XmlNodeList elementsByTagName = this.xmlDoc.GetElementsByTagName("state");
            XmlNode stateById = (XmlNode)null;
            try
            {
                foreach (XmlNode xmlNode in elementsByTagName)
                {
                    if (xmlNode.Attributes[nameof(id)].Value.Equals(id))
                    {
                        stateById = xmlNode;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return stateById;
        }

        public string getStringAttribute(XmlNode node, string attrName, string defValue)
        {
            return !this.hasAttribute(node, attrName) ? defValue : node.Attributes[attrName].Value.Trim();
        }

        public char getCharAttribute(XmlNode node, string attrName, char defValue)
        {
            if (!this.hasAttribute(node, attrName))
                return defValue;
            return node.Attributes[attrName].Value.Trim().Length <= 0 ? ' ' : node.Attributes[attrName].Value.Trim()[0];
        }

        public float getFloatAttribute(XmlNode node, string attrName, float defValue)
        {
            if (!this.hasAttribute(node, attrName))
                return defValue;
            try
            {
                return float.Parse(node.Attributes[attrName].Value.Trim());
            }
            catch (Exception ex)
            {
                return defValue;
            }
        }

        public int getIntAttribute(XmlNode node, string attrName, int defValue)
        {
            if (!this.hasAttribute(node, attrName))
                return defValue;
            try
            {
                return int.Parse(node.Attributes[attrName].Value.Trim());
            }
            catch (Exception ex)
            {
                return defValue;
            }
        }

        public bool getBooleanAttribute(XmlNode node, string attrName, bool defValue)
        {
            try
            {
                return this.hasAttribute(node, attrName) ? bool.Parse(node.Attributes[attrName].Value.Trim()) : defValue;
            }
            catch (Exception ex)
            {
                return defValue;
            }
        }

        private bool hasAttribute(XmlNode node, string attributeName)
        {
            bool flag = false;
            int i = 0;
            while (i < node.Attributes.Count && !(flag = node.Attributes[i].Name.Equals(attributeName)))
                ++i;
            return flag;
        }
    }
}
