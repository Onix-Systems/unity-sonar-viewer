
using Newtonsoft.Json;

namespace SketchfabAPI
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class LicenseEntity
    {
        [JsonProperty("uid")]
        public string UId { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
