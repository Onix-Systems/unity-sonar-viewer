
using Newtonsoft.Json;

namespace SketchfabAPI
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ArchivesEntity
    {
        [JsonProperty("source")]
        public ArchiveEntity Source { get; set; }
        [JsonProperty("gltf")]
        public ArchiveEntity Gltf { get; set; }
        [JsonProperty("glb")]
        public ArchiveEntity Glb { get; set; }
        [JsonProperty("usdz")]
        public ArchiveEntity Usdz { get; set; }
    }
}
