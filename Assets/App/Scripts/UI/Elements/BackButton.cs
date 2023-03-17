
using App.Infrastructure.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace App.UI.Elements
{
    [RequireComponent(typeof(Button))]
    public class BackButton : View
    {
        private Button _button;

        public event Action OnClick;

        protected override void OnAwake()
        {
            _button = GetComponent<Button>();
        }

        protected override void OnVisible()
        {
            _button.onClick.AddListener(OnBackClicked);
        }

        protected override void OnInvisible()
        {
            _button.onClick.RemoveListener(OnBackClicked);
            OnClick = null;
        }

        #if UNITY_ANDROID
        protected override void OnUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                OnBackClicked();
            }
        }
        #endif

        private void OnBackClicked()
        {
            OnClick?.Invoke();
        }
    }
}
