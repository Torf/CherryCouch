namespace CherryCouch.Common.Plugins
{
    public interface IGlobal
    {
        string ReverseProxy { get; set; }

        string Ip { get; set; }

        short Port { get; set; }
    }
}