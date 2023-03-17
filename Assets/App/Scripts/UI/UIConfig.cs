
using System;
using UnityEngine;
using Utils.KeyedConfiguration;

namespace App.UI
{
    [Serializable]
    public class UIConfig
    {
        [field: SerializeField] public float PreviewScreenVisibleTime { get; private set; } = 3f;
        [field: SerializeField] public float DownloadProgressCloseDelay { get; private set; } = 3f;
        [field: SerializeField] public KeyedConfig Texts { get; private set; }
    }
}