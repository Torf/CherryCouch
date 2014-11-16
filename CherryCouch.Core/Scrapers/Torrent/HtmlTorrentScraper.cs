using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using CherryCouch.Common.Protocol.Scraper;
using CherryCouch.Common.Protocol.Search;

namespace CherryCouch.Core.Scrapers.Torrent
{
    public class HtmlTorrentScraper : ITorrentScraper
    {
        private XmlDocument currentDocument;

        public void Load(XmlDocument document)
        {
            currentDocument = document;
        }

        public List<TorrentResult> Execute(string mainNodeXPath, IEnumerable<ScrapingRule> rules)
        {
            if(currentDocument == null)
                throw new InvalidOperationException("need to load document before execution");

            var results = new List<TorrentResult>();

            // if there isn't any rules, stop here.
            if (rules == null || !rules.Any())
                return results;

            var torrentNodes = currentDocument.SelectNodes(mainNodeXPath);

            if (torrentNodes != null && torrentNodes.Count > 0)
            {
                foreach (XmlNode torrentNode in torrentNodes)
                {
                    var result = new TorrentResult();
                    var resultType = result.GetType();
                    foreach (var rule in rules)
                    {
                        var resultProperty = resultType.GetProperty(rule.AssociatedProperty);
                        if (resultProperty != null)
                        {
                            var value = rule.GetValue(torrentNode);
                            resultProperty.SetValue(result, value, null);
                        }
                    }

                    results.Add(result);
                }
            }

            return results;
        }

        /// <summary>
        /// Execute only once the rules, in order to fill one search result. Mainly dedicated to detail page scraping.
        /// </summary>
        public TorrentResult ExecuteSingle(TorrentResult result, IEnumerable<ScrapingRule> rules)
        {
            if (currentDocument == null)
                throw new InvalidOperationException("need to load document before execution");

            var resultType = result.GetType();

            // if there isn't any rules, stop here.
            if (rules == null || !rules.Any())
                return result;

            foreach (var rule in rules)
            {
                var resultProperty = resultType.GetProperty(rule.AssociatedProperty);
                if (resultProperty != null)
                {
                    var value = rule.GetValue(currentDocument);
                    resultProperty.SetValue(result, value, null);
                }
            }

            return result;
        }
    }
}
