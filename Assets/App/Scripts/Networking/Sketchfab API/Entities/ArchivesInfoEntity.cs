
using Newtonsoft.Json;

namespace SketchfabAPI.Entities
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ArchivesInfoEntity
    {
        [JsonProperty("source")]
        public ArchiveInfoEntity Source { get; set; }
        [JsonProperty("glb")]
        public ArchiveInfoEntity Glb { get; set; }
        [JsonProperty("usdz")]
        public ArchiveInfoEntity Usdz { get; set; }
        [JsonProperty("gltf")]
        public ArchiveInfoEntity Gltf { get; set; }
    }
}
