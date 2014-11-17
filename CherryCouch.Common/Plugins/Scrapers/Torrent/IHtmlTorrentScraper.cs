using System.Collections.Generic;
using System.Xml;
using CherryCouch.Common.Plugins.Providers;
using CherryCouch.Common.Protocol.Search;

namespace CherryCouch.Common.Plugins.Scrapers.Torrent
{
    public interface IHtmlTorrentScraper : IScraper
    {
        void Load(IProvider provider, XmlDocument document);

        List<TorrentResult> Execute(string mainNodeXPath, IEnumerable<IScrapingRule> rules);

        TorrentResult ExecuteSingle(TorrentResult result, IEnumerable<IScrapingRule> rules);
    }
}
