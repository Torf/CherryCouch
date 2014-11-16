using CherryCouch.Common.Protocol.Request;
using CherryCouch.Requests.Authentifiers;
using Nancy;

namespace CherryCouch.Requests
{
    public interface IRequest<out T>
    {
        T Parse(IRequestAuthentifier authentifier, DynamicDictionary query);

        IRequestAuthorization Authorization { get; }
    }
}
