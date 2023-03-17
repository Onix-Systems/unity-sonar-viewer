
using System;
using UnityEngine;

namespace App.Services.Input
{
    [Serializable]
    public class InputConfig
    {
        [field: SerializeField] public LayerMask SelectableObjectLayers { get; private set; }
        [Tooltip("Minimum finger movement distance per one seoncd that recognized as a swipe (Relative to smaller screen side)")]
        [field: SerializeField, Range(0.01f, 1f)] public float RelativeSwipeDelta { get; private set; } = 0.05f;
    }
}