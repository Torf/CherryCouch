using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CherryCouch.Common.Extensions;
using CherryCouch.Common.Plugins;
using CherryCouch.Common.Plugins.Browsers;
using CherryCouch.Common.Plugins.Context;
using CherryCouch.Common.Plugins.Scrapers;
using NLog;

namespace CherryCouch.Core.Plugins
{
    public class PluginContext : IPluginContext
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string BaseAddress
        {
            get { return Global.Instance.ReverseProxy; }
        }

        public void Log(string message, params object[] args)
        {
            Logger.Info(message, args);
        }

        public IPluginCore<TB, TS> GetPluginCore<TB, TS>() where TB : IBrowser where TS : IScraper
        {
            var browserType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.HasInterface(typeof (TB)));

            var scraperType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.HasInterface(typeof(TS)));

            if(browserType == null || scraperType == null)
                throw new NotImplementedException();

            var browserCtor = browserType.GetConstructor(new Type[0]);
            var browserInstance = (TB)browserCtor.Invoke(new object[0]);

            var scraperCtor = scraperType.GetConstructor(new Type[0]);
            var scraperInstance = (TS)scraperCtor.Invoke(new object[0]);

            return new PluginCore<TB, TS>(this, browserInstance, scraperInstance);
        }
    }
}
