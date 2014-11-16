namespace CherryCouch.Common.Protocol.Request
{
    public interface IRequestAuthorization
    {
        string Username { get; }
        string Passkey { get; }
        bool IsAuthorized { get; }
    }
}
