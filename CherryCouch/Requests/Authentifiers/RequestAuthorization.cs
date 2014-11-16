using CherryCouch.Common.Protocol.Request;

namespace CherryCouch.Requests.Authentifiers
{
    public class RequestAuthorization : IRequestAuthorization
    {
        private readonly string username;
        private readonly string passkey;
        private readonly bool isAuthorized;

        public RequestAuthorization(string username, string passkey, bool isAuthorized)
        {
            this.username = username;
            this.passkey = passkey;
            this.isAuthorized = isAuthorized;
        }

        public string Username { get { return username; } }
        public string Passkey { get { return passkey; } }
        public bool IsAuthorized { get { return isAuthorized; } }
    }
}