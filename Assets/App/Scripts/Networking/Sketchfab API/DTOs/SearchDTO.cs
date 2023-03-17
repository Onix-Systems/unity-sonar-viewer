
using UnityEngine;
using Newtonsoft.Json;

namespace SketchfabAPI
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class SearchDTO
    {
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("q")] public string Query { get; set; }
        [JsonProperty("user")]public string User { get; set; }
        [JsonProperty("categories")] public string[] Categories { get; set; } 
        [JsonProperty("date")] public int? Days { get; set; }
        [JsonProperty("downloadable")] public bool? Downloadable { get; set; }
        [JsonProperty("animated")] public bool? Animated { get; set; }
        [JsonProperty("staffpicked")] public bool? Staffpicked { get; set; }
        [JsonProperty("sound")] public bool? Sound { get; set; }
        [JsonProperty("min_face_count")] public int? MinFaceCount { get; set; }
        [JsonProperty("max_face_count")] public int? MaxFaceCount { get; set; }
        [JsonProperty("pbr_type")] public string PBRType { get; set; }
        [JsonProperty("rigged")] public bool? Rigged { get; set; }
        [JsonProperty("collection")] public string Collection { get; set; }
        [JsonProperty("sort_by")] public string SrotBy { get; set; }
        [JsonProperty("file_format")] public string FileFormat { get; set; }
        [JsonProperty("license")] public string License { get; set; }
        [JsonProperty("max_uv_layer_count")] public int? MaxUVLayerCount { get; set; }
        [JsonProperty("available_archive_type")] public string AvailableArchiveType { get; set; }
        [JsonProperty("archives_max_size")] public int? ArchivesMaxSize { get; set; }
        [JsonProperty("archives_max_face_count")] public int? ArchivesMaxFaceCount { get; set; }
        [JsonProperty("archives_max_vertex_count")] public int? ArchivesMaxVertexCount { get; set; }
        [JsonProperty("archives_max_texture_count")] public int? ArchivesMaxTextureCount { get; set; }
        [JsonProperty("archives_texture_max_resolution")] public int? ArchivesTextureMaxResolution { get; set; }
        [JsonProperty("archives_flavours")] public bool? ArchivesFlavours { get; set; }
    }
}
