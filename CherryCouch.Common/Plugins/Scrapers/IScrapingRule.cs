using System.Xml;
using CherryCouch.Common.Plugins.Providers;

namespace CherryCouch.Common.Plugins.Scrapers
{
    public interface IScrapingRule
    {
        /// <summary>
        /// Associated property of result
        /// </summary>
        string AssociatedProperty { get; set; }

        /// <summary>
        /// XPath to the property value
        /// </summary>
        string XPath { get; set; }

        /// <summary>
        /// Scraping Converter, can be null.
        /// </summary>
        ScrapingConverterEnum? ConverterType { get; set; }

        /// <summary>
        /// Scraping Converter Parameter, can be null.
        /// </summary>
        string ConverterParameter { get; set; }

        bool ShouldSerializeConverterType();
        bool ShouldSerializeConverterParameter();

        /// <summary>
        /// Retrieves the value from the node
        /// </summary>
        /// <param name="currentProvider">current provider</param>
        /// <param name="node">XmlNode to examine</param>
        /// <returns>value retrieved</returns>
        object GetValue(IProvider currentProvider, XmlNode node);
    }
}