using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CherryCouch.Common.Config;
using CherryCouch.Common.Extensions;
using CherryCouch.Core.Providers.Torrent;
using NLog;

namespace CherryCouch.Core.Providers
{
    public static class ProvidersManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static List<ITorrentProvider> torrentProviders = null; 

        /// <summary>
        /// Initialize the Providers Manager, loading providers instance.
        /// </summary>
        public static void Initialize()
        {
            torrentProviders = LoadProviders<ITorrentProvider>();
        }

        public static ITorrentProvider[] GetTorrentProviders()
        {
            return (torrentProviders ?? new List<ITorrentProvider>()).ToArray();
        }

        public static ITorrentProvider GetTorrentProvider(string providerName)
        {
            return (torrentProviders ?? new List<ITorrentProvider>()).FirstOrDefault(p => GetProviderName(p.GetType()) == providerName);
        }

        /// <summary>
        /// Dispose the Providers Manager, disposing providers instance.
        /// </summary>
        public static void Dispose()
        {
            if (torrentProviders != null)
            {
                foreach (var torrentProvider in torrentProviders)
                {
                    if (torrentProvider.IsConnected)
                    {
                        torrentProvider.Logout();
                    }
                }
            }
            torrentProviders = null;
        }

        /// <summary>
        /// Loads instance of each providers that have the given interface.
        /// </summary>
        private static List<I> LoadProviders<I>() where I : IProvider
        {
            string interfaceName = typeof (I).Name.Substring(1);
            Logger.Info("Loading {0}s...", interfaceName);

            var result = new List<I>();
            var providerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.HasInterface(typeof(I)));

            foreach (var providerType in providerTypes)
            {
                var instance = LoadInstance<I>(providerType);

                if (instance != null)
                {
                    result.Add(instance);
                    Logger.Info("\tProvider \"{0}\" loaded with success.", providerType.Name);
                }
            }

            Logger.Info("{0} {1}s successfully loaded.", result.Count, interfaceName);

            return result;
        }

        /// <summary>
        /// Creates an instance of the provider and loads the associated configuration.
        /// </summary>
        private static I LoadInstance<I>(Type providerType) where I : IProvider
        {
            var ctor = providerType.GetConstructor(new Type[0]);
            var instance = ctor.Invoke(new object[0]);

            // Load configured properties
            var configFilename = GetProviderName(providerType);
            var configFilepath = String.Format("./config/providers/{0}.xml", configFilename);
            Config.LoadFile(instance, configFilepath);

            return (I) instance;
        }

        /// <summary>
        /// Puts the name in lower case and removes the "provider" suffix.
        /// </summary>
        private static string GetProviderName(Type providerType)
        {
            return providerType.Name.ToLowerInvariant().Replace("provider", "");
        }
    }
}
