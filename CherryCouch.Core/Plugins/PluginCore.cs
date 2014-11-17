using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CherryCouch.Common.Plugins;
using CherryCouch.Common.Plugins.Browsers;
using CherryCouch.Common.Plugins.Context;
using CherryCouch.Common.Plugins.Scrapers;

namespace CherryCouch.Core.Plugins
{
    public class PluginCore<TB, TS> : IPluginCore<TB, TS> where TB : IBrowser where TS : IScraper
    {
        public IPluginContext CurrentContext { get; private set; }

        public TB Browser { get; private set; }

        public TS Scraper { get; private set; }

        public PluginCore(IPluginContext context, TB browserInstance, TS scraperInstance)
        {
            CurrentContext = context;
            Browser = browserInstance;
            Scraper = scraperInstance;
        }
    }
}
