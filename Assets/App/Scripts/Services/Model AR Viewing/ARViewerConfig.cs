
using System;
using UnityEngine;
using Utils.SingleUnityLayerProperty;

namespace App.Services.ModelARViewing
{
    [Serializable]
    public class ARViewerConfig
    {
        [field: SerializeField] public float MinScanSeconds { get; private set; } = 3f;
        [Tooltip("Minimum AR plane size (square meters)")]
        [field: SerializeField] public float ARPlaneMinSquareSize { get; private set; } = 1f;
        [field: SerializeField] public bool ShowARPlanes { get; private set; } = false;
        [field: SerializeField] public GameObject ARPlanePrefab { get; private set; }
        [field: SerializeField] public SingleUnityLayer ModelLayer { get; private set; }
        [Tooltip("Model size box bounds (meters)")]
        [field: SerializeField] public float ModelSpawnSize { get; private set; } = 1f;
        [field: SerializeField] public float ModelMoveSmooth { get; private set; } = 8f;
        [field: SerializeField] public float ModelRotationSwipeFactor { get; private set; } = -0.2f;
        [field: SerializeField] public float ModelRotationSmooth { get; private set; } = 16f;
        [field: SerializeField] public float ModelScalePinchFactor { get; private set; } = 0.001f;
        [field: SerializeField] public float ModelScaleSmooth { get; private set; } = 16f;
        [field: SerializeField] public float ModelMinimumScale { get; private set; } = 0.01f;
        [field: SerializeField] public float ModelMaximumScale { get; private set; } = 5f;
    }
}