
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using App.Infrastructure.UI;

namespace App.UI.Popups
{
    public class ProgressPopupView : PopupView
    {
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TextMeshProUGUI _messageLabel;
        [SerializeField] private float _minProgressValue;
        [SerializeField] private float _maxProgressValue;

        protected override void OnAwake()
        {
            _progressSlider.minValue = _minProgressValue;
            _progressSlider.maxValue = _maxProgressValue;
        }

        public void SetProgress(float progressValue, string message)
        {
            _progressSlider.value = GetProgressValue(progressValue);
            _messageLabel.text = message;
        }

        private float GetProgressValue(float progressValue) 
        {
            float normalizedValue = Mathf.Clamp01(progressValue);
            float delta = _maxProgressValue - _minProgressValue;
            
            return  _minProgressValue + (delta * normalizedValue);
        }
    }
}
