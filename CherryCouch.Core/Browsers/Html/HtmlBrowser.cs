using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using Sgml;

namespace CherryCouch.Core.Browsers.Html
{
    public class HtmlBrowser
    {
        private readonly CookieAwareWebClient webClient;

        public HtmlBrowser()
        {
            webClient = new CookieAwareWebClient();   
        }

        /// <summary>
        /// Sends a GET request and convert response to XmlDocument.
        /// </summary>
        public XmlDocument Get(string url)
        {
            return this.ConvertFromHtml(this.GetRaw(url));
        }

        /// <summary>
        /// Sends a POST request and convert response to XmlDocument.
        /// </summary>
        public XmlDocument Post(string url, NameValueCollection data)
        {
            return this.ConvertFromHtml(this.PostRaw(url, data));
        }

        /// <summary>
        /// Sends a POST request at the given url with given data. 
        /// </summary>
        public string PostRaw(string url, NameValueCollection data)
        {
            // send request & get response
            var responseData = webClient.UploadValues(url, "POST", data);
            if (responseData == null || responseData.Length == 0)
                return null;

            // convert byte response to string
            var responseString = Encoding.UTF8.GetString(responseData);
            if (String.IsNullOrWhiteSpace(responseString))
                return null;

            return responseString;
        }

        /// <summary>
        /// Sends a GET request at the given url. 
        /// </summary>
        public string GetRaw(string url)
        {
            return webClient.DownloadString(url);
        }

        /// <summary>
        /// Downloads a file with the given url.
        /// </summary>
        public bool DownloadFile(string url, string filepath)
        {
            // Get clean filepath (create directories & delete file if already exists.
            var dir = Path.GetDirectoryName(filepath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            else if (File.Exists(filepath))
                File.Delete(filepath);

            // Download the file
            webClient.DownloadFile(url, filepath);

            // Return if file exists (as download is completed).
            return File.Exists(filepath);
        }

        /// <summary>
        /// Converts a string of HTML webpage in XmlDocument.
        /// </summary>
        private XmlDocument ConvertFromHtml(string html)
        {
            var reader = new StringReader(html);

            // setup SgmlReader
            var sgmlReader = new SgmlReader
            {
                DocType = "HTML",
                WhitespaceHandling = WhitespaceHandling.All,
                CaseFolding = CaseFolding.ToLower,
                InputStream = reader
            };

            // create document
            var doc = new XmlDocument { PreserveWhitespace = true, XmlResolver = null };
            doc.Load(sgmlReader);

            return doc;
        }
    }
}
