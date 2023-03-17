
using Newtonsoft.Json;

namespace SketchfabAPI.Entities
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ArchiveInfoEntity
    {
        [JsonProperty("faceCount")]
        public int FaceCount { get; set; }
        [JsonProperty("textureCount")]
        public int TextureCount { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("vertexCount")]
        public int VertexCount { get; set; }
        [JsonProperty("textureMaxResolution")]
        public int TextureMaxResolution { get; set; }
    }
}
