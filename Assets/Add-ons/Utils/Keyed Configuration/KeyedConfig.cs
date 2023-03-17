
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.KeyedConfiguration
{
    [Serializable]
    public class KeyedConfig : ISerializationCallbackReceiver
    {
        [SerializeField] private ConfigItem[] _items;

        private Dictionary<string, ConfigItem> _itemsRegistry = new Dictionary<string, ConfigItem>();

        public string this[string key]
        {
            get
            {
                if (_itemsRegistry.TryGetValue(key, out ConfigItem configItem))
                {
                    return configItem.Value;
                }

                Debug.LogError($"Item with key \"{key}\" is not registered");

                return default;
            }
        }
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() => SetupItems();

        public bool TryGetItem(string key, out string text)
        {
            if (_itemsRegistry.TryGetValue(key, out ConfigItem configItem))
            {
                text = configItem.Value;
                return true;
            }

            text = string.Empty;
            return false;
        }

        public string GetItem(string key)
        {
            if (_itemsRegistry.TryGetValue(key, out ConfigItem configItem))
            {
                return configItem.Value;
            }

            Debug.LogWarning($"Key \"{key}\" not found");

            return string.Empty;
        }

        public bool Contains(string key) => _itemsRegistry.ContainsKey(key);

        private void SetupItems()
        {
            if (_items == null)
            {
                return;
            }

            int count = _items.Length;
            _itemsRegistry = new Dictionary<string, ConfigItem>(count);

            for (int i = 0; i < count; ++i)
            {
                ConfigItem configItem = _items[i];
                if (!_itemsRegistry.TryAdd(_items[i].Key, configItem))
                {
                    Debug.LogError($"Multiple items with key \"{configItem.Key}\" were found");
                }
            }
        }
    }
}
