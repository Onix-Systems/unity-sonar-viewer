
using Newtonsoft.Json;

namespace SketchfabAPI.Entities
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ThumbnailEntity 
    {
        [JsonProperty("images")]
        public ImageEntity[] Images { get; set; }
    }
}
