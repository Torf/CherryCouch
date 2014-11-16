using System.Collections.Generic;

namespace CherryCouch.Core.Providers.Torrent
{
    public interface ITorrentProvider : IProvider
    {
        /// <summary>
        /// Downloads the .torrent file.
        /// </summary>
        /// <param name="parameters">parameters needed to download the .torrent</param>
        /// <returns>relative file path of the .torrent</returns>
        string DownloadTorrentFile(Dictionary<string, string> parameters);
    }
}