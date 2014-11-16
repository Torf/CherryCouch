using Nancy;

namespace CherryCouch.Requests.Parsers
{
    public interface IRequestParser
    {
        T ParseRequest<T>(Request request) where T : IRequest<T>, new();
    }
}