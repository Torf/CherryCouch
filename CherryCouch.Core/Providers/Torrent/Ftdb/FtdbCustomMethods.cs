using System;
using System.Text.RegularExpressions;

namespace CherryCouch.Core.Providers.Torrent.Ftdb
{
    public class FtdbCustomMethods
    {
        public static string ParseDownloadUrl(string value)
        {
            var regex = new Regex(@"&id=(\d+)&.*hash=([a-zA-Z0-9]+)");
            var match = regex.Match(value);
            string id = match.Groups[1].Value;
            string hash = match.Groups[2].Value;

            return String.Format("{0}/download/ftdb?id={1}&hash={2}", Global.Instance.ReverseProxy, id, hash);
        }
    }
}
