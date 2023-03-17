

using App.Infrastructure.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.UI.Screens
{
    public class ARViewerScreenView : ScreenView
    {
        [field: SerializeField] public Button _menuButton;

        public bool SearchButtonVisible
        {
            get
            {
                return _menuButton.gameObject.activeSelf;
            }

            set
            {
                if (_menuButton.gameObject.activeSelf != value)
                {
                    _menuButton.gameObject.SetActive(value);
                }
            }
        }

        public event Action OnMenuClicked;

        protected override void OnVisible()
        {
            _menuButton.onClick.AddListener(() => 
            { 
                OnMenuClicked?.Invoke();    
            });
        }

        protected override void OnInvisible()
        {
            _menuButton.onClick.RemoveAllListeners();
            OnMenuClicked = null;
        }
    }
}
