
using Newtonsoft.Json;

namespace SketchfabAPI
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ArchiveEntity
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("expires")]
        public int Expires { get; set; }
    }
}
