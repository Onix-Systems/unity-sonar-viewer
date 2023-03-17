
using Newtonsoft.Json;

namespace SketchfabAPI.Entities
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class UserEntity
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("profileUrl")]
        public string ProfileUrl { get; set; }
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("uid")]
        public string UId { get; set; }
        [JsonProperty("uri")]
        public string Uri { get; set; }
        [JsonProperty("avatar")]
        public AvatarEntity Avatar { get; set; }
    }
}
