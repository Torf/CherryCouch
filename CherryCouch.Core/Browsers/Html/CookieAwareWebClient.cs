using System;
using System.Net;
using System.Text.RegularExpressions;
using NLog;

namespace CherryCouch.Core.Browsers.Html
{
    public class CookieAwareWebClient : WebClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CookieContainer CookieContainer { get; set; }
        public Uri Uri { get; set; }

        public CookieAwareWebClient()
            : this(new CookieContainer())
        {
        }

        public CookieAwareWebClient(CookieContainer cookies)
        {
            this.CookieContainer = cookies;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = this.CookieContainer;
            }
            HttpWebRequest httpRequest = (HttpWebRequest)request;
            httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            String setCookieHeader = response.Headers[HttpResponseHeader.SetCookie];

            if (!String.IsNullOrWhiteSpace(setCookieHeader))
            {
                //create cookie w/ key, value & domain
                var keyValueRegex = new Regex(@" ?([a-zA-Z0-9_]+)=([a-zA-Z0-9_]+);.*domain=([a-zA-Z0-9_\.-]+)");

                var cookieInfos = keyValueRegex.Match(setCookieHeader);
                var cookie = new Cookie(cookieInfos.Groups[1].Value, cookieInfos.Groups[2].Value, "/", cookieInfos.Groups[3].Value);
                this.CookieContainer.Add(cookie);
            }
            return response;
        }
    }
}
