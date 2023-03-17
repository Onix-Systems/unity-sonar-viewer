
using System;
using UnityEngine;

namespace Utils.KeyedConfiguration
{
    [Serializable]
    public class ConfigItem
    {
        [field: SerializeField] public string Key { get; private set; }
        [field: SerializeField, TextArea(4, 8)] public string Value { get; private set; }
    }
}
