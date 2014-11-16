using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using CherryCouch.Common.Utils;

namespace CherryCouch.Common.Protocol.Scraper
{
    [Serializable]
    public class ScrapingRule
    {
        /// <summary>
        /// Associated property of result
        /// </summary>
        public string AssociatedProperty { get; set; }

        /// <summary>
        /// XPath to the property value
        /// </summary>
        public string XPath { get; set; }

        /// <summary>
        /// Scraping Converter, can be null.
        /// </summary>
        public ScrapingConverterEnum? ConverterType { get; set; }
        public bool ShouldSerializeConverterType()
        {
            return ConverterType != null && ConverterType != ScrapingConverterEnum.None;
        }

        /// <summary>
        /// Scraping Converter Parameter, can be null.
        /// </summary>
        public string ConverterParameter { get; set; }
        public bool ShouldSerializeConverterParameter()
        {
            return !String.IsNullOrEmpty(ConverterParameter);
        }
        
        public ScrapingRule()
        {
        }

        public ScrapingRule(string associatedProperty, string xpath)
            : this()
        {
            AssociatedProperty = associatedProperty;
            XPath = xpath;
        }

        /// <summary>
        /// Retrieves the value from the node
        /// </summary>
        /// <param name="node">XmlNode to examine</param>
        /// <returns>value retrieved</returns>
        public object GetValue(XmlNode node)
        {
            if (XPath == null)
                throw new ApplicationException();

            var value = node.CreateNavigator().Evaluate(XPath);
            if (value == null)
                return null;

            // if the XPath selects a node instead of a value.
            if (value is XPathNodeIterator)
            {
                var elemList = (XPathNodeIterator) value;
                if (elemList.Count > 0)
                    value = elemList.Current.InnerXml;
            }
            
            // Value converters
            if (ConverterType != null && ConverterType != ScrapingConverterEnum.None)
            {
                switch (ConverterType)
                {
                    case ScrapingConverterEnum.RegexExtract:
                        var extractor = new Regex(ConverterParameter);
                        var match = extractor.Match(value.ToString());
                        if (match.Groups.Count == 2)
                            value = match.Groups[1].Value;
                        break;

                    case ScrapingConverterEnum.ConcatBefore:
                        value = String.Concat(ConverterParameter, value.ToString());
                        break;

                    case ScrapingConverterEnum.ConcatAfter:
                        value = String.Concat(value.ToString(), ConverterParameter);
                        break;

                    case ScrapingConverterEnum.ConvertUnitFileSize:
                        value = Converter.ConvertSize(value.ToString());
                        break;
                    
                    case ScrapingConverterEnum.ConvertDoubleToInt:
                        value = Convert.ToInt32(value);
                        break;
                    case ScrapingConverterEnum.CustomMethod:
                        var parameters = ConverterParameter.Split('.');
                        if(parameters.Length != 2)
                            throw new InvalidOperationException("incorrect customMethod converter parameter.");

                        var assembly = Assembly.GetCallingAssembly();

                        var type = assembly.GetTypes().FirstOrDefault(t => t.Name == parameters[0]);
                        if(type == null)
                            throw new InvalidOperationException(String.Format("incorrect customMethod converter parameter (class {0} not found).", parameters[0]));

                        var method = type.GetMethod(parameters[1]);
                        if (method == null)
                            throw new InvalidOperationException(String.Format("incorrect customMethod converter parameter (method {0} not found).", ConverterParameter));

                        value = method.Invoke(null, new[] {value});

                        break;
                }
            }

            return value;
        }
    }
}
