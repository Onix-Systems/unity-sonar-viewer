
using SketchfabAPI.DTOs;
using System;
using UnityEngine;

namespace SketchfabAPI
{
    [Serializable]
    public class SketchfabAPISettings
    {
        [field: SerializeField] public ClientSettings Client { get; set; }
    }
}