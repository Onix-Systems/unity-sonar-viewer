
using System;
using UnityEngine;

namespace SketchfabAPI
{
    [Serializable]
    public class ClientSettings
    {
        [field: SerializeField] public string Id { get; set; }
        [field: SerializeField] public string Secret { get; set; }
    }
}
