using System;
using CherryCouch.Common.Protocol.Api;
using CherryCouch.Common.Protocol.Request;
using CherryCouch.Core;
using CherryCouch.Core.Handlers;
using CherryCouch.Requests;
using CherryCouch.Requests.Authentifiers;
using CherryCouch.Requests.Parsers;
using Nancy;
using Newtonsoft.Json;
using NLog;

namespace CherryCouch.Modules
{
    public class SearchModule : NancyModule
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IRequestParser parser = new AuthentifiedRequestParser(new BasicRequestAuthentifier(Global.Instance.User, Global.Instance.Passkey));

        public SearchModule()
        {
            Get["/search/movie"] = _ =>
            {
                object response = null;

                try
                {
                    Logger.Info("Parsing request...");
                    ISearchMovieRequest request = parser.ParseRequest<SearchMovieRequest>(Request);

                    Logger.Info("Parsed request : {0} - {1}", request.Authorization.Username, request.ImdbId ?? request.Terms);

                    var handler = new SearchHandler();

                    var result = handler.SearchMovie(request);
                    Logger.Info("Served with {0} results.", result.ResultsCount);

                    return JsonConvert.SerializeObject(result);
                }
                catch (Exception exception)
                {
                    response = new ErrorResult(exception.Message);
                    Logger.Info("Bugged.");
                }

                return Response.AsJson(response);
            };
        }
    }
}
