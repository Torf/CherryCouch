using System;
using System.Collections.Generic;
using CherryCouch.Common.Protocol.Request;
using CherryCouch.Requests.Authentifiers;
using Nancy;

namespace CherryCouch.Requests
{
    public class DownloadRequest : IRequest<DownloadRequest>, IDownloadRequest
    {
        public IRequestAuthorization Authorization { get; private set; }
        public Dictionary<string, string> Parameters { get; private set; }
        public string ProviderName { get; set; }

        public DownloadRequest()
        {
            Parameters = new Dictionary<string, string>();
        }

        public DownloadRequest Parse(IRequestAuthentifier authentifier, DynamicDictionary query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            Authorization = (authentifier != null) ? authentifier.ParseQuery(query) : new RequestAuthorization(null, null, true);
            
            foreach (string key in query.Keys)
            {
                Parameters.Add(key, query[key].ToString());
            }
            
            return this;
        }
    }
}