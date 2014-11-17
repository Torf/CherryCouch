using CherryCouch.Common.Plugins.Browsers;
using CherryCouch.Common.Plugins.Context;
using CherryCouch.Common.Plugins.Scrapers;

namespace CherryCouch.Common.Plugins
{
    public interface IPluginContext
    {
        /// <summary>
        /// Gets the current base address of CherryCouch.
        /// </summary>
        string BaseAddress { get; }

        /// <summary>
        /// Logs info messages.
        /// </summary>
        void Log(string message, params object[] args);

        /// <summary>
        /// Gets the current plugin core instance (lazy loading).
        /// </summary>
        IPluginCore<TB, TS> GetPluginCore<TB, TS>() where TB : IBrowser where TS : IScraper;
    }
}