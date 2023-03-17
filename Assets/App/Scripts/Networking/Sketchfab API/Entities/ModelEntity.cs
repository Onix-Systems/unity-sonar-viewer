
using Newtonsoft.Json;

namespace SketchfabAPI.Entities
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ModelEntity
    {
        [JsonProperty("uid")]
        public string UId { get; set; }
        [JsonProperty("animationCount")]
        public int AnimationCount { get; set; }
        [JsonProperty("viewerUrl")]
        public string ViewerUrl { get; set; }
        [JsonProperty("publishedAt")]
        public string PublishedAt { get; set; }
        [JsonProperty("likeCount")]
        public int LikeCount { get; set; }
        [JsonProperty("commentCount")]
        public int CommentCount { get; set; }
        [JsonProperty("user")]
        public UserEntity User { get; set; }
        [JsonProperty("isDownloadable")]
        public bool IsDownloadable { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("viewCount")]
        public int ViewCount { get; set; }
        [JsonProperty("thumbnails")]
        public ThumbnailEntity Thumbnails { get; set; }
        [JsonProperty("license")]
        public LicenseEntity License { get; set; }
        [JsonProperty("isPublished")]
        public bool IsPublished { get; set; }
        [JsonProperty("archives")]
        public ArchivesInfoEntity Archives { get; set; }
        [JsonProperty("embedUrl")]
        public string EmbedUrl { get; set; }
    }
}
