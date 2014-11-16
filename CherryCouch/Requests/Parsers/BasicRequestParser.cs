using Nancy;

namespace CherryCouch.Requests.Parsers
{
    public class BasicRequestParser : IRequestParser
    {
        public BasicRequestParser()
        {
        }

        public T ParseRequest<T>(Request request) where T : IRequest<T>, new()
        {
            return new T().Parse(null, request.Query);
        }
    }
}
