using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using CherryCouch.Common.Plugins.Providers;
using CherryCouch.Common.Utils;

namespace CherryCouch.Common.Plugins.Scrapers
{
    [Serializable]
    public class ScrapingRule : IScrapingRule
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
        /// <param name="currentProvider">Current provider</param>
        /// <param name="node">XmlNode to examine</param>
        /// <returns>value retrieved</returns>
        public object GetValue(IProvider currentProvider, XmlNode node)
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
                        var type = currentProvider.GetType();

                        var method = type.GetMethod(ConverterParameter);
                        if (method == null)
                            throw new InvalidOperationException(String.Format("incorrect customMethod converter parameter (method {0} not found in {1}).", ConverterParameter, type.FullName));

                        value = method.Invoke(currentProvider, new[] {value});

                        break;
                }
            }

            return value;
        }
    }
}
