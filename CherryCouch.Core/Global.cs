using CherryCouch.Common.Config;

namespace CherryCouch.Core
{
    public class Global
    {
        #region Singleton

        private static Global instance;
        public static Global Instance { get { return instance ?? (instance = new Global()); } }

        #endregion

        [Configurable("reverse_proxy")]
        public string ReverseProxy = "http://127.0.0.1:9112";

        [Configurable("ip")] 
        public string Ip = "127.0.0.1";

        [Configurable("port")]
        public short Port = 9112;

        [Configurable("user")]
        public string User = "alice";

        [Configurable("passkey")]
        public string Passkey = "6fds4gs";

        private Global()
        {
            const string configFilepath = "./config/global.xml";

            Config.LoadFile(this, configFilepath);
        }
    }
}
