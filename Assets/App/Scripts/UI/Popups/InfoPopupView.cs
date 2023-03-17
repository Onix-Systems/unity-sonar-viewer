
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using App.Infrastructure.UI;

namespace App.UI
{
    public class InfoPopupView : PopupView
    {
        [SerializeField] private RectTransform _popupRoot;
        [SerializeField] private RectTransform _popupContent;
        [SerializeField] private TextMeshProUGUI _label;

        private bool _rebuildLayout = false;

        public void SetText(string text)
        {
            _label.text = text;
            _rebuildLayout = true;
        }

        protected override void OnLateUpdate()
        {
            if (_rebuildLayout)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_popupContent);
                LayoutRebuilder.ForceRebuildLayoutImmediate(_popupRoot);
                _rebuildLayout = false;
            }
        }
    }
}