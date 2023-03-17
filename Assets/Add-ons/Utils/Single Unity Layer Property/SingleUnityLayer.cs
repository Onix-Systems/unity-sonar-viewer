
using System;
using UnityEngine;

namespace Utils.SingleUnityLayerProperty
{
    [Serializable]
    public class SingleUnityLayer
    {
        [field: SerializeField] private int _layerIndex = 0;

        public int LayerIndex => _layerIndex;
        public int LayerMask => _layerMask;

        private int _layerMask;

        public void Set(int layerIndex)
        {
            if (layerIndex > 0 && layerIndex < 32) 
            {
                _layerIndex = layerIndex;
                _layerMask = 1 << LayerIndex;
            }
        }
    }
}