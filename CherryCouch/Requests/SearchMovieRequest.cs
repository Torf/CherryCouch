using System;
using CherryCouch.Common.Protocol.Request;
using CherryCouch.Exceptions;
using CherryCouch.Requests.Authentifiers;
using Nancy;

namespace CherryCouch.Requests
{
    public class SearchMovieRequest : IRequest<SearchMovieRequest>, ISearchMovieRequest
    {
        private IRequestAuthorization authorization;
        private string imdbid;
        private string terms;

        public SearchMovieRequest()
        {
        }
        
        public string ImdbId
        {
            get { return imdbid; }
        }

        public string Terms
        {
            get { return terms; }
        }

        public IRequestAuthorization Authorization
        {
            get { return authorization; }
        }

        public SearchMovieRequest Parse(IRequestAuthentifier authentifier, DynamicDictionary query)
        {
            if(query == null)
                throw new ArgumentNullException("query");

            authorization =  (authentifier != null) ? authentifier.ParseQuery(query) : new RequestAuthorization(null, null, true);
            
            if (!query.ContainsKey("imdbid"))
            {
                if (!query.ContainsKey("search"))
                    throw new MissingParameterRequestException("imdbid or search");

                terms = query["search"];
            }
            else
            {
                imdbid = query["imdbid"];
            }

            return this;
        }
    }
}
