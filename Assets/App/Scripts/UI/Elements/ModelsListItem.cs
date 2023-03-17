
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SketchfabAPI.Entities;
using App.Infrastructure.UI;

namespace App.UI.Elements
{
    public class ModelsListItem: View
    {
        [SerializeField] private TextMeshProUGUI _titleLabel;
        [SerializeField] private Button _loadButton;

        private ModelEntity _data;

        public event Action<ModelEntity> OnLoadClicked;
        
        protected override void OnVisible()
        {
            _loadButton.onClick.AddListener(() =>
            {
                OnLoadClicked?.Invoke(_data);
            });       
        }

        protected override void OnInvisible()
        {
            _loadButton.onClick.RemoveAllListeners();
            OnLoadClicked = null;
        }

        public void SetData(ModelEntity itemData)
        {
            _data = itemData;
            _titleLabel.text = itemData.Name;
        }
    }
}