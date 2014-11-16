using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using CherryCouch.Common.Extensions;

namespace CherryCouch.Common.Config
{
    public class Config
    {
        private readonly string filePath;
        private readonly object associatedInstance;
        private readonly List<ConfigNode> nodes = new List<ConfigNode>();

        public Config(string filePath, object associatedInstance)
        {
            this.filePath = filePath;
            this.associatedInstance = associatedInstance;
        }

        public string FilePath
        {
            get { return filePath; }
        }

        public List<ConfigNode> Nodes
        {
            get { return nodes; }
        }

        /// <summary>
        /// Register all decorated field of the instance.
        /// </summary>
        public void RegisterAttributes()
        {
            var type = associatedInstance.GetType();

            foreach (var field in type.GetFields())
            {
                var attr = field.GetCustomAttribute<ConfigurableAttribute>();

                if (attr == null)
                    continue;

                if (Exists(attr.Name))
                    throw new Exception(string.Format("Node with name {0} already used, a node name must be unique", attr.Name));

                nodes.Add(new ConfigVariable(attr, field));
            }

            foreach (var property in type.GetProperties())
            {
                var attr = property.GetCustomAttribute<ConfigurableAttribute>();

                if (attr == null)
                    continue;

                if (Exists(attr.Name))
                    throw new Exception(string.Format("Node with name {0} already used, a node name must be unique", attr.Name));

                nodes.Add(new ConfigVariable(attr, property));
            }
        }

        public void Load()
        {
            if (!File.Exists(FilePath))
            {
                // Create directories if they don't exist
                var dir = Path.GetDirectoryName(FilePath);
                if (dir != null && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                Save(); // create the config file
            }

            var document = new XmlDocument();
            document.Load(FilePath);

            var navigator = document.CreateNavigator();
            var listedNames = new List<string>();
            foreach (XPathNavigator iterator in navigator.Select("//" + ConfigNode.NodeName + "[@" + ConfigNode.AttributeName + "]"))
            {
                if (!iterator.IsNode)
                    continue;

                var xmlNode = ((IHasXmlNode)iterator).GetNode();
                var name = ConfigNode.GetNodeName(xmlNode);
                var node = GetNode(name);

                if (listedNames.Contains(name))
                    throw new Exception(string.Format("Node with name {0} already used, a node name must be unique", name));

                // if the noad already exist we set the value
                if (node != null)
                {
                    node.Load(xmlNode, associatedInstance);
                }
                else
                {
                    node = new ConfigNode(xmlNode, associatedInstance);
                    nodes.Add(node);
                }

                listedNames.Add(name);
            }
        }

        public void Save()
        {
            var writer = XmlWriter.Create(FilePath, new XmlWriterSettings
            {
                Indent = true,
                IndentChars = " ",
                Encoding = Encoding.UTF8
            });

            writer.WriteStartDocument();
            writer.WriteStartElement("Configuration");

            foreach (var node in nodes)
            {
                node.Save(writer, associatedInstance);
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Close();
        }

        public bool Exists(string nodeName)
        {
            return nodes.Any(entry => entry.Name == nodeName);
        }

        public ConfigNode GetNode(string name)
        {
            var currentNodes = nodes.Where(entry => entry.Name == name).ToArray();

            if (currentNodes.Length > 1)
                throw new Exception(string.Format("Found {0} nodes with name {1}, a node name must be unique", currentNodes.Length, name));

            return currentNodes.SingleOrDefault();
        }

        public T Get<T>(string key)
        {
            var node = GetNode(key);

            if (node == null)
                throw new KeyNotFoundException(string.Format("Node {0} not found", key));

            return (T)node.GetValue(typeof(T));
        }

        public T Get<T>(string key, T defaultValue)
        {
            var node = GetNode(key);

            if (node == null)
            {
                nodes.Add(new ConfigNode(key, defaultValue));

                return defaultValue;
            }

            return (T)node.GetValue(typeof(T));
        }

        public void Set<T>(string key, T value)
        {
            var node = GetNode(key);

            if (node == null)
                throw new KeyNotFoundException(string.Format("Node {0} not found", key));

            node.SetValue(value);
        }

        /// <summary>
        /// Loads the config file and fill associated fields of the class.
        /// </summary>
        /// <param name="instance">instance of the class</param>
        /// <param name="filepath">config file path</param>
        public static Config LoadFile(object instance, string filepath)
        {
            var config = new Config(filepath, instance);
            config.RegisterAttributes();
            config.Load();

            return config;
        }
    }
}