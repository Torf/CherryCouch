using CherryCouch.Common.Protocol.Request;
using CherryCouch.Exceptions;
using Nancy;

namespace CherryCouch.Requests.Authentifiers
{
    public class BasicRequestAuthentifier : IRequestAuthentifier
    {
        private string authorizedUser;
        private string authorizedPasskey;

        public BasicRequestAuthentifier(string user, string passkey)
        {
            authorizedUser = user;
            authorizedPasskey = passkey;
        }

        public IRequestAuthorization ParseQuery(DynamicDictionary query)
        {
            if (!query.ContainsKey("user"))
                throw new MissingParameterRequestException("user");

            var user = query["user"];

            if (!query.ContainsKey("passkey"))
                throw new MissingParameterRequestException("passkey");

            var passkey = query["passkey"];

            return new RequestAuthorization(user, passkey, this.IsAuthorized(user, passkey));
        }

        private bool IsAuthorized(string username, string passkey)
        {
            return (username == authorizedUser && passkey == authorizedPasskey);
        }
    }
}
