using System.Collections.Generic;
using CherryCouch.Common.Protocol.Api;
using CherryCouch.Common.Protocol.Request;
using CherryCouch.Common.Protocol.Search;
using CherryCouch.Core.Providers;

namespace CherryCouch.Core.Handlers
{
    public class SearchHandler
    {
        public SearchHandler()
        {
        }

        public ListResult<ISearchResult> SearchMovie(ISearchMovieRequest request)
        {
            var results = new List<ISearchResult>();

            foreach (var torrentProvider in ProvidersManager.GetTorrentProviders())
            {
                results.AddRange(torrentProvider.Search(request.ImdbId ?? request.Terms));
            }

            return new ListResult<ISearchResult>(results);
        }
    }
}
