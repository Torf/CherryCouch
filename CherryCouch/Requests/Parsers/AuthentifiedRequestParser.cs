using System;
using CherryCouch.Exceptions;
using CherryCouch.Requests.Authentifiers;
using Nancy;

namespace CherryCouch.Requests.Parsers
{
    public class AuthentifiedRequestParser : IRequestParser
    {
        private readonly IRequestAuthentifier authentifier;

        public AuthentifiedRequestParser(IRequestAuthentifier authentifier)
        {
            if(authentifier == null)
                throw new ArgumentNullException("authentifier");

            this.authentifier = authentifier;
        }

        public T ParseRequest<T>(Request request) where T : IRequest<T>, new()
        {
            var parsedRequest = new T().Parse(authentifier, request.Query);

            if(parsedRequest.Authorization == null || parsedRequest.Authorization.IsAuthorized == false)
                throw new ForbiddenRequestException("invalid user or passkey");

            return parsedRequest;
        }
    }
}
