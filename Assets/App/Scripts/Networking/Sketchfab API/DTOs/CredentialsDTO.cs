
using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SketchfabAPI.DTOs
{
    [Serializable]
    public class CredentialsDTO
    {
        [JsonProperty("username")]
        [field: SerializeField] public string Email { get; set; }
        [JsonProperty("password")]
        [field: SerializeField] public string Password { get; set; }
        [JsonProperty("grant_type")]
        [field: SerializeField] public string GrantType { get; set; } = "password";
    }
}