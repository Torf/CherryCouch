using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CherryCouch.Common.Config
{
    public class ConfigNode
    {
        public const string NodeName = "Key";
        public const string AttributeName = "name";

        private string name;
        private XmlNode node;
        private object value;

        protected ConfigNode()
        {
        }

        public ConfigNode(XmlNode node, object instance)
        {
            if (node == null) throw new ArgumentNullException("node");

            Load(node, instance);
        }

        public ConfigNode(string name, object value)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");
            this.name = name;
            this.value = value;
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool IsSynchronised
        {
            get;
            protected set;
        }

        public static string GetNodeName(XmlNode node)
        {
            if (node.Attributes == null || node.Attributes[AttributeName] == null)
                throw new Exception(string.Format("Attribute {0} not found", AttributeName));

            return node.Attributes[AttributeName].Value;
        }

        public virtual object GetValue(Type type)
        {
            if (node != null)
            {
                value = new XmlSerializer(type).Deserialize(new StringReader(node.InnerXml));
                node = null;
                IsSynchronised = true;
            }

            return value;
        }

        public virtual void SetValue(object value)
        {
            this.value = value;
            IsSynchronised = false;
        }

        internal virtual void Save(XmlWriter writer, object instance)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            writer.WriteStartElement(NodeName);
            writer.WriteAttributeString(AttributeName, Name);

            if (value != null)
                new XmlSerializer(value.GetType()).Serialize(writer, value);
            IsSynchronised = true;

            writer.WriteEndElement();
        }

        internal virtual void Load(XmlNode node, object instance)
        {
            name = GetNodeName(node);
            this.node = node;
        }
    }
}