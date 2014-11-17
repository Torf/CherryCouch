using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CherryCouch.Common.Plugins.Browsers.Html;
using CherryCouch.Common.Plugins.Context;
using CherryCouch.Common.Plugins.Scrapers.Torrent;
using Newtonsoft.Json.Linq;

namespace CherryCouch.Providers.Ftdb
{
    public class FtdbLoginHandshake
    {
        public string UrlStartHandshake { get; set; }
        public string UrlEndHandshake { get; set; }

        private string[] challenges;
        private string hash;
        private string decodedChallenge;

        private readonly IPluginCore<IHtmlBrowser, IHtmlTorrentScraper> core;

        public FtdbLoginHandshake(IPluginCore<IHtmlBrowser, IHtmlTorrentScraper> core)
        {
            this.core = core;
        }

        public bool Execute(string username, string password)
        {
            if(String.IsNullOrEmpty(UrlStartHandshake) || String.IsNullOrEmpty(UrlEndHandshake))
                throw new InvalidOperationException("need to define UrlStartHandshake and UrlEndHandshake before execution.");

            StartHandshake();

            return EndHandshake(username, password);
        }

        private void StartHandshake()
        {
            string handshake = core.Browser.GetRaw(UrlStartHandshake);

            // Read Json and get info
            var handshakeObj = JObject.Parse(handshake);
            this.challenges = handshakeObj["challenge"].Values<string>().ToArray();
            this.hash = handshakeObj["hash"].Value<string>();

            // Decode info
            this.decodedChallenge = DecodeChallenges();
        }

        private bool EndHandshake(string username, string password)
        {
            // Build post data
            var data = new NameValueCollection();
            data["username"] = username;
            data["password"] = password;
            data["secure_login"] = this.decodedChallenge;
            data["hash"] = this.hash;

            // send request & get response
            var response = core.Browser.PostRaw(UrlEndHandshake, data);

            if (String.IsNullOrWhiteSpace(response)) // Invalid response, something gone wrong.
                return false;

            // convert string to json object & get success value
            var responseObj = JObject.Parse(response);
            var isSuccess = responseObj.GetValue("success").Value<bool>();

            if(!isSuccess)
                core.CurrentContext.Log("Can't log to ftdb provider : {0}", response);

            return isSuccess;
        }

        #region Challenges decoding

        /// <summary>
        /// Decodes the challenge data
        /// </summary>
        private string DecodeChallenges()
        {
            string secureLogin = "";

            foreach (var challenge in this.challenges)
            {
                secureLogin += DecodeChallenge(challenge);
            }

            return  secureLogin;
        }
        
        /// <summary>
        /// Adapted from https://github.com/sarakha63/CouchPotatoServer
        /// </summary>
        private string DecodeChallenge(string challengeEntry)
        {
            challengeEntry = DecodePrintableCharacters(challengeEntry);

            var regexGetArgs = new Regex("\'([^\']+)\',([0-9]+),([0-9]+),\'([^\']+)\'");
            var regexIsEncoded = new Regex("decodeURIComponent");
            var regexUnquote = new Regex("\'");

            if (challengeEntry == "a")
                return "05f";

            if (!regexIsEncoded.Match(challengeEntry).Success)
                return regexUnquote.Replace(challengeEntry, "");

            var args = regexGetArgs.Match(challengeEntry);
            var decoded = DecodeString(args.Groups[1].Value, args.Groups[2].Value, args.Groups[3].Value, args.Groups[4].Value.Split('|'));

            decoded = DecodePrintableCharacters(decoded);

            return decoded;
        }

        /// <summary>
        /// Adapted from https://github.com/sarakha63/CouchPotatoServer
        /// </summary>
        private string DecodeString(string p, string sa, string sc, string[] k)
        {
            int a = Convert.ToInt32(sa);
            int c = Convert.ToInt32(sc);

            while (c != 0)
            {
                c--;

                if (c >= 0 && c < k.Length)
                {
                    int e = Convert.ToInt32(ComputeE(c, a));

                    var regex = new Regex("%" + e.ToString("x").ToLower());
                    p = regex.Replace(p, k[c]);
                }
            }

            return p;
        }

        /// <summary>
        /// Decodes a URI string, but lets unpritable characters encoded.
        /// </summary>
        private string DecodePrintableCharacters(string s)
        {
            string result = "";
            int i = 0;

            while (i < s.Length)
            {
                if (s[i] == '%')
                {
                    int hexValue = Int32.Parse(s.Substring(i + 1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                    if (hexValue <= 126) // if it's a printable character (alphanumeric or basic special char)
                    {
                        result += (char)hexValue;
                        i += 3;
                        continue;
                    }
                }

                result += s[i];
                i++;
            }

            return result;
        }

        /// <summary>
        /// Adapted from https://github.com/sarakha63/CouchPotatoServer
        /// </summary>
        private string ComputeE(int c, int a)
        {
            string f = c < a ? "" : ComputeE(c / a, a);

            return f + (c % a + 161);
        }

        #endregion

    }
}
