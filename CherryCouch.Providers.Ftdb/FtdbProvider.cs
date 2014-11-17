using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CherryCouch.Common.Config;
using CherryCouch.Common.Plugins;
using CherryCouch.Common.Plugins.Browsers.Html;
using CherryCouch.Common.Plugins.Context;
using CherryCouch.Common.Plugins.Providers.Torrent;
using CherryCouch.Common.Plugins.Scrapers;
using CherryCouch.Common.Plugins.Scrapers.Torrent;
using CherryCouch.Common.Protocol.Search;

namespace CherryCouch.Providers.Ftdb
{
    public class FtdbProvider : ITorrentProvider
    {
        public string Name { get { return "Ftdb"; } }

        #region Configuration

        [Configurable("username")]
        public string Username = "mylogin";

        [Configurable("password")]
        public string Password = "mypwd";

        [Configurable("link_root", "Web root address of the tracker, without leading slash.")]
        public string RootLink = "http://www.frenchtorrentdb.com";

        [Configurable("page_search", "Search page address, starting with a slash. {0} is replaced by search terms.")]
        public string SearchPage = "/?name={0}&search=Rechercher&exact=1&adv_cat%5Bm%5D%5B4%5D=136&adv_cat%5Bm%5D%5B5%5D=150&year=&year_end=&section=TORRENTS&group=films";

        [Configurable("page_download", "Download page address, starting with a slash. {0} is replaced by torrent id, {1} by torrent hash.")]
        public string DownloadPage = "/?section=DOWNLOAD&id={0}&hash={1}&get=1";

        [Configurable("page_login_start", "Search page address, starting with a slash. {0} is replaced by search terms.")]
        public string StartLoginPage = "/?section=LOGIN&challenge=1";

        [Configurable("page_login_end", "Search page address, starting with a slash. {0} is replaced by search terms.")]
        public string EndLoginPage = "/?section=LOGIN&ajax=1";

        [Configurable("xpath_torrent_nodes", "Absolute XPath to the node that contains torrent nodes.")]
        public string XPathTorrentNodes = "//*[@id='mod_torrents']/div/div[1]/div/div[2]/ul";

        [Configurable("xpath_rules", "rules")] 
        public List<ScrapingRule> ScrapingRules = new List<ScrapingRule>
        {
            new ScrapingRule("Id", "string(./li[contains(@class, 'torrents_download')]/a/@href)") { ConverterType = ScrapingConverterEnum.RegexExtract, ConverterParameter = "&id=([0-9]+)&uid" },
            new ScrapingRule("Name", "string(./li[contains(@class, 'torrents_name')]/a/@title)"),

            new ScrapingRule("AddressDetail", "string(./li[contains(@class, 'torrents_name')]/a/@href)") { ConverterType = ScrapingConverterEnum.ConcatBefore, ConverterParameter = "http://www.frenchtorrentdb.com" },
            new ScrapingRule("AddressDownload", "string(./li[contains(@class, 'torrents_download')]/a/@href)") { ConverterType = ScrapingConverterEnum.CustomMethod, ConverterParameter = "ParseDownloadUrl" },

            new ScrapingRule("Seeders", "number(./li[contains(@class, 'torrents_seeders')]/text())") { ConverterType = ScrapingConverterEnum.ConvertDoubleToInt },
            new ScrapingRule("Leechers", "number(./li[contains(@class, 'torrents_leechers')]/text())") { ConverterType = ScrapingConverterEnum.ConvertDoubleToInt },
            new ScrapingRule("Size", "string(./li[contains(@class, 'torrents_size')]/text())") { ConverterType = ScrapingConverterEnum.ConvertUnitFileSize },

            new ScrapingRule("Freeleech", "count(./li[contains(@class, 'torrents_name')]/a/img) > 0")
        };

        [Configurable("xpath_detailpage_rules", "rules")]
        public List<ScrapingRule> ScrapingDetailPageRules = new List<ScrapingRule>
        {
            new ScrapingRule("ImdbId", "string(//*[@id='mod_infos']/table/tr/td[1]/div[1]/div[2]/div/a/@href)") { ConverterType = ScrapingConverterEnum.RegexExtract, ConverterParameter = "imdb_id=(tt[0-9]+)"}
        };

        #endregion

        private readonly IPluginCore<IHtmlBrowser, IHtmlTorrentScraper> core; 

        public bool IsConnected { get; private set; }
        public bool IsWorking { get; private set; }

        public FtdbProvider(IPluginContext context)
        {
            IsConnected = false;
            IsWorking = false;

            core = context.GetPluginCore<IHtmlBrowser, IHtmlTorrentScraper>();
        }

        public bool Login()
        {
            if (IsConnected) return true;

            core.Browser.ClearSession();

            var handshake = new FtdbLoginHandshake(core)
            {
                UrlStartHandshake = RootLink + StartLoginPage,
                UrlEndHandshake = RootLink + EndLoginPage
            };

            IsConnected = handshake.Execute(Username, Password);

            return IsConnected;
        }

        public void Logout()
        {
            if (!IsConnected) return;

            IsConnected = false;
            core.Browser.ClearSession();
        }

        public List<ISearchResult> Search(string terms)
        {
            if(terms == null)
                throw new ArgumentNullException("terms");

            IsWorking = true;

            if (!IsConnected) // Lazy Login
                if(!Login())
                    throw new Exception("unknown error : can't log to Ftdb");
            
            // Browsing to tracker
            var html = core.Browser.Get(String.Format(RootLink + SearchPage, terms));

            // Scraping data
            core.Scraper.Load(this, html);
            var results = core.Scraper.Execute(XPathTorrentNodes, ScrapingRules);

            // if there is Detail Page rules, let's scrap
            if (ScrapingDetailPageRules.Any())
            {
                foreach (var result in results)
                {
                    if (!String.IsNullOrEmpty(result.AddressDetail))
                    {
                        html = core.Browser.Get(result.AddressDetail);
                        core.Scraper.Load(this, html);
                        core.Scraper.ExecuteSingle(result, ScrapingDetailPageRules);
                    }
                }
            }

            // temporary
            results.ForEach(r => r.Type = "movie");

            IsWorking = false;

            return results.ToList<ISearchResult>();
        }

        public string DownloadTorrentFile(Dictionary<string, string> parameters)
        {
            if(parameters == null)
                throw new ArgumentNullException("parameters");

            if (!parameters.ContainsKey("id"))
                throw new InvalidOperationException("Invalid download request : id parameter missing");

            if (!parameters.ContainsKey("hash"))
                throw new InvalidOperationException("Invalid download request : hash parameter missing");


            IsWorking = true;

            if (!IsConnected) // Lazy Login
                if (!Login())
                    throw new Exception("unknown error : can't log to Ftdb");

            // Create .torrent download link from parameters
            string torrentId = parameters["id"];
            string torrentHash = parameters["hash"];

            string downloadLink = String.Format(RootLink + DownloadPage, torrentId, torrentHash);

            const string filepathBase = @".\temp\ftdb\";

            // Create .torrent filepath.
            string filepath = Path.Combine(filepathBase, String.Format("ftdb_{0}.torrent", torrentId));

            // Get the file
            if (!core.Browser.DownloadFile(downloadLink, filepath))
            {
                IsWorking = false;
                throw new Exception("Can't download .torrent file, unknown error.");
            }

            IsWorking = false;

            return filepath;
        }

        #region Custom Method

        public string ParseDownloadUrl(string value)
        {
            var regex = new Regex(@"&id=(\d+)&.*hash=([a-zA-Z0-9]+)");
            var match = regex.Match(value);
            string id = match.Groups[1].Value;
            string hash = match.Groups[2].Value;

            return String.Format("{0}/download/ftdb?id={1}&hash={2}", core.CurrentContext.BaseAddress, id, hash);
        }

        #endregion
    }
}
