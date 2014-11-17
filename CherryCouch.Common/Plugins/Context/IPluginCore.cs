using CherryCouch.Common.Plugins.Browsers;
using CherryCouch.Common.Plugins.Scrapers;

namespace CherryCouch.Common.Plugins.Context
{
    public interface IPluginCore<out TB, out TS> where TB : IBrowser where TS : IScraper
    {
        /// <summary>
        /// Gets the current plugin context.
        /// </summary>
        IPluginContext CurrentContext { get; }

        /// <summary>
        /// Gets a browser instance.
        /// </summary>
        TB Browser { get; }

        /// <summary>
        /// Gets a scraper instance.
        /// </summary>
        TS Scraper { get; }
    }
}