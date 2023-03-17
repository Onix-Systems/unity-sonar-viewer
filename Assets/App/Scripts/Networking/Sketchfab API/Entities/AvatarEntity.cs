
using Newtonsoft.Json;

namespace SketchfabAPI.Entities
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class AvatarEntity
    {
        [JsonProperty("images")]
        public ImageEntity[] Images { get; set; }
        [JsonProperty("uid")]
        public string UId { get; set; }
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
