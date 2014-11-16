using System.Collections.Generic;

namespace CherryCouch.Common.Protocol.Request
{
    public interface IDownloadRequest
    {
        IRequestAuthorization Authorization { get; }

        Dictionary<string, string> Parameters { get; }

        string ProviderName { get; }
    }
}