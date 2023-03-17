
using Newtonsoft.Json;

namespace SketchfabAPI.Entities
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class SearchResultsEntity
    {
        [JsonProperty("cursors")]
        public CursorEntity Cursor { get; set; }
        [JsonProperty("next")]
        public string Next { get; set; }
        [JsonProperty("previous")]
        public string Previous { get; set; }
        [JsonProperty("results")]
        public ModelEntity[] Results { get; set; }
    }
}
