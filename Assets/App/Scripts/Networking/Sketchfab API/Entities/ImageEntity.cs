
using Newtonsoft.Json;

namespace SketchfabAPI.Entities
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ImageEntity
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
    }
}
