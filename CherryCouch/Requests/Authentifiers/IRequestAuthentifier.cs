using CherryCouch.Common.Protocol.Request;
using Nancy;

namespace CherryCouch.Requests.Authentifiers
{
    public interface IRequestAuthentifier
    {
        IRequestAuthorization ParseQuery(DynamicDictionary query);
    }
}