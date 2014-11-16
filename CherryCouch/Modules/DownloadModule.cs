using System;
using System.IO;
using CherryCouch.Common.Protocol.Api;
using CherryCouch.Core.Handlers;
using CherryCouch.Requests;
using CherryCouch.Requests.Parsers;
using Nancy;
using NLog;

namespace CherryCouch.Modules
{
    public class DownloadModule : NancyModule
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRequestParser parser = new BasicRequestParser();

        public DownloadModule()
        {
            Get["/download/{provider}"] = param =>
            {
                object response = null;

                try
                {
                    Logger.Info("Parsing download request...");
                    var request = parser.ParseRequest<DownloadRequest>(Request);
                    request.ProviderName = param.provider;

                    Logger.Info("Parsed download request : {0} - {1}", request.Authorization.Username, request.ProviderName);

                    var handler = new DownloadHandler();

                    var result = handler.DownloadTorrent(request);
                    Logger.Info("Served with file {0}.", result);

                    return Response.AsFile(result)
                                    .AsAttachment(Path.GetFileName(result), "application/x-bittorrent");
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
