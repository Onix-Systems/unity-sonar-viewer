
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using App.Infrastructure.UI;

namespace App.UI.Popups
{
    public class ErrorPopupView : PopupView
    {
        [SerializeField] private TextMeshProUGUI _errorLabel;
        [SerializeField] private Button _okButton;

        public string ErrorText
        {
            get => _errorLabel.text;
            set => _errorLabel.text = value;
        }

        public event Action OnOkClicked;

        protected override void OnVisible()
        {
            _okButton.onClick.AddListener(() =>
            {
                OnOkClicked?.Invoke();
            });
        }

        protected override void OnInvisible()
        {
            _okButton.onClick.RemoveAllListeners();
            OnOkClicked = null;
        }
    }
}
