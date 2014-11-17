using System.Collections.Specialized;
using System.Xml;

namespace CherryCouch.Common.Plugins.Browsers.Html
{
    public interface IHtmlBrowser : IBrowser
    {
        /// <summary>
        /// Sends a GET request and convert response to XmlDocument.
        /// </summary>
        XmlDocument Get(string url);

        /// <summary>
        /// Sends a POST request and convert response to XmlDocument.
        /// </summary>
        XmlDocument Post(string url, NameValueCollection data);

        /// <summary>
        /// Sends a POST request at the given url with given data. 
        /// </summary>
        string PostRaw(string url, NameValueCollection data);

        /// <summary>
        /// Sends a GET request at the given url. 
        /// </summary>
        string GetRaw(string url);

        /// <summary>
        /// Downloads a file with the given url.
        /// </summary>
        bool DownloadFile(string url, string filepath);
    }
}