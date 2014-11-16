using Newtonsoft.Json;

namespace CherryCouch.Common.Protocol.Search
{
    public interface ISearchResult
    {
        [JsonProperty("torrent_id")]
        string Id { get; }

        [JsonProperty("release_name")]
        string Name { get; }

        [JsonProperty("imdb_id")]
        string ImdbId { get; }

        [JsonProperty("details_url")]
        string AddressDetail { get; }

        [JsonProperty("download_url")]
        string AddressDownload { get; }

        [JsonProperty("seeders")]
        int Seeders { get; }

        [JsonProperty("leechers")]
        int Leechers { get; }

        [JsonProperty("size")]
        int Size { get; }

        [JsonProperty("freeleech")]
        bool Freeleech { get; }

        [JsonProperty("type")]
        string Type { get; }
        //[JsonConverter(typeof(StringEnumConverter))]
    }
}
