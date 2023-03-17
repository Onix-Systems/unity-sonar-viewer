
using SketchfabAPI.DTOs;
using System;
using UnityEngine;

namespace App.Core
{
    [Serializable]
    public class DefaultsConfig
    {
        [field: SerializeField] public int ModelSearchMaxVertices { get; set; } = 500000;
        [field: SerializeField] public int ModelSearchTextureMaxResolution { get; set; } = 4096;
        [field: SerializeField] public string ModelSearchQuery { get; set; } = "Table";
        [field: SerializeField] public CredentialsDTO SketchafabMockCredentials { get; set; } = new CredentialsDTO();
    }
}
