
using System;
using UnityEngine;

namespace RestHTTP
{
    [Serializable]
    public class NetworkSettings
    {
        [field: SerializeField] public float DefaultRequestTimeoutSeconds { get; set; } = 30f;
    }
}
