using CherryCouch.Common.Protocol.Request;
using CherryCouch.Core.Providers;
using CherryCouch.Core.Providers.Torrent;

namespace CherryCouch.Core.Handlers
{
    public class DownloadHandler
    {
        public DownloadHandler()
        {
            
        }

        public string DownloadTorrent(IDownloadRequest request)
        {
            ITorrentProvider provider = ProvidersManager.GetTorrentProvider(request.ProviderName);

            return provider.DownloadTorrentFile(request.Parameters);

            //todo: /download/ftdb?id={torrent_id}&hash={hash mis via download_url}
            // il suffit de reprendre le provider, et d'aller DL le .torrent (il faut rajouter &get=1 seulement)
            // puis renvoyer le .torrent

            //return null;
        }
    }
}
