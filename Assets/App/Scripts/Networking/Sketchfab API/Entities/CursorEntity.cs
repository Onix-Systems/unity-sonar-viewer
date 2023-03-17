
using Newtonsoft.Json;

namespace SketchfabAPI.Entities
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CursorEntity
    {
        [JsonProperty("next")]
        public string Next { get; set; }
        [JsonProperty("previous")]
        public string Previous { get; set; }
    }
}
