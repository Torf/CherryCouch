using Newtonsoft.Json;

namespace CherryCouch.Common.Protocol.Api
{
    public class ErrorResult
    {
        /// <summary>
        /// Error message
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; private set; }

        public ErrorResult(string error)
        {
            this.Error = error;
        }
    }
}
