using Newtonsoft.Json;

namespace CherryCouch.Common.Protocol.Search
{
    public class TorrentResult : ISearchResult
    {
        [JsonProperty("torrent_id")]
        public string Id { get; set; }

        [JsonProperty("release_name")]
        public string Name { get; set; }

        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }

        [JsonProperty("details_url")]
        public string AddressDetail { get; set; }

        [JsonProperty("download_url")]
        public string AddressDownload { get; set; }

        [JsonProperty("seeders")]
        public int Seeders { get; set; }

        [JsonProperty("leechers")]
        public int Leechers { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("freeleech")]
        public bool Freeleech { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
