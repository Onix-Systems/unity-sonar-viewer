
using System;
using UnityEngine;
using SketchfabAPI;
using RestHTTP;

namespace App.Networking
{
    [Serializable]
    public class NetworkConfig
    {
        [field: SerializeField] public NetworkSettings NetworkSettings { get; private set; }
        [field: SerializeField] public SketchfabAPISettings SketchfabSettings { get; private set; }
    }
}
