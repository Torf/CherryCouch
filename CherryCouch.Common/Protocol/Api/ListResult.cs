using System.Collections.Generic;
using Newtonsoft.Json;

namespace CherryCouch.Common.Protocol.Api
{
    /// <summary>
    /// Encapsulate a list in order to serialize it to json properly
    /// </summary>
    public class ListResult<T>
    {
        private readonly List<T> results;

        public ListResult(List<T> results)
        {
            this.results = results;
        }

        [JsonProperty("results")]
        public List<T> Results
        {
            get { return results; }
        }

        [JsonProperty("total_results")]
        public int ResultsCount
        {
            get { return results.Count; }
        }
    }
}
