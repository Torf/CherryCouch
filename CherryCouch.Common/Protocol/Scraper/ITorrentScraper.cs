using System.Collections.Generic;
using System.Xml;
using CherryCouch.Common.Protocol.Search;

namespace CherryCouch.Common.Protocol.Scraper
{
    public interface ITorrentScraper
    {
        void Load(XmlDocument document);

        List<TorrentResult> Execute(string mainNodeXPath, IEnumerable<ScrapingRule> rules);

        TorrentResult ExecuteSingle(TorrentResult result, IEnumerable<ScrapingRule> rules);
    }
}
