
using System;
using UnityEngine;
using UnityEngine.UI;
using App.Infrastructure.UI;
using App.UI.Elements;

namespace App.UI.Screens
{
    public class MenuScreenView : ScreenView
    {
        [SerializeField] private BackButton _backButton;
        [SerializeField] private Button _modelInfoButton;
        [SerializeField] private Button _searchModelButton;
        [SerializeField] private Button _aboutButton;

        public BackButton BackButton => _backButton;

        public event Action OnModelInfoClicked;
        public event Action OnModelSearchClicked;
        public event Action OnAboutClicked;

        protected override void OnVisible()
        {
            _modelInfoButton.onClick.AddListener(OnModelInfoButtonClicked);
            _searchModelButton.onClick.AddListener(OnModelSearchButtonClicked);
            _aboutButton.onClick.AddListener(OnAboutButtonClicekd);
        }

        protected override void OnInvisible()
        {
            _modelInfoButton.onClick.RemoveAllListeners();
            _searchModelButton.onClick.RemoveAllListeners();
            _aboutButton.onClick.RemoveAllListeners();

            OnModelInfoClicked = null;
            OnModelSearchClicked = null;
            OnAboutClicked = null;
        }

        private void OnModelInfoButtonClicked()
        {
            OnModelInfoClicked?.Invoke();
        }

        private void OnModelSearchButtonClicked()
        {
            OnModelSearchClicked?.Invoke();
        }

        private void OnAboutButtonClicekd()
        {
            OnAboutClicked?.Invoke();
        }
    }
}
