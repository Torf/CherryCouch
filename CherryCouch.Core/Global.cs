using CherryCouch.Common;
using CherryCouch.Common.Config;
using CherryCouch.Common.Plugins;

namespace CherryCouch.Core
{
    public class Global : IGlobal
    {
        #region Singleton

        private static Global instance;
        public static Global Instance { get { return instance ?? (instance = new Global()); } }

        #endregion

        [Configurable("reverse_proxy")]
        public string ReverseProxy { get; set; }

        [Configurable("ip")] 
        public string Ip { get; set; }

        [Configurable("port")]
        public short Port { get; set; }

        [Configurable("user")]
        public string User = "alice";

        [Configurable("passkey")]
        public string Passkey = "6fds4gs";

        private Global()
        {
            const string configFilepath = "./config/global.xml";

            // Default Values
            ReverseProxy = "http://127.0.0.1:9112";
            Ip = "127.0.0.1";
            Port = 9112;
            
            Config.LoadFile(this, configFilepath);
        }
    }
}
