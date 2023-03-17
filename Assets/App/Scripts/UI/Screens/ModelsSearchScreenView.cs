
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SketchfabAPI;
using App.Infrastructure.UI;
using App.Infrastructure.Contexts;
using App.Services;
using App.Core;
using App.Services.AppDataStoring;
using App.UI.Elements;

namespace App.UI.Screens
{
    public class ModelsSearchScreenView : ScreenView
    {
        [SerializeField] private TMP_InputField _searchInputField;
        [SerializeField] private TMP_InputField _vertexMaxCountInputField;
        [SerializeField] private TMP_InputField _textureMaxResolutionInputField;
        [SerializeField] private LicenseToggleButton _licenseToggleButton;
        [SerializeField] private Button _searchButton;
        
        public LicenseToggleButton LicenseToggle => _licenseToggleButton;
        public string SearchQuery => _searchInputField.text.Trim();
        public int VertexMaxCount => Convert.ToInt32(_vertexMaxCountInputField.text);
        public int TextureMaxResolution => Convert.ToInt32(_textureMaxResolutionInputField.text);

        public event Action OnSearchClicked;

        protected override void OnVisible()
        {
            SetDefaultSearchParameters();
            _searchButton.onClick.AddListener(() => { OnSearchClicked?.Invoke(); });
        }

        protected override void OnInvisible()
        {
            _searchButton.onClick.RemoveAllListeners();
            OnSearchClicked = null;
        }

        private void SetDefaultSearchParameters()
        {
            IContext mainContext = MainContext.Instance;
            DefaultsConfig defaults = mainContext.Get<AppConfig>().Defaults;
            AppData appData = mainContext.Get<AppDataStorage>().Data;

            SearchDTO lastSearchParameters = appData.LastSearchParameters;

            if (lastSearchParameters != null)
            {
                _vertexMaxCountInputField.text = lastSearchParameters.ArchivesMaxVertexCount?.ToString();
                _textureMaxResolutionInputField.text = lastSearchParameters.ArchivesTextureMaxResolution?.ToString();
                _searchInputField.text = lastSearchParameters.Query;
            }

            if (string.IsNullOrEmpty(_vertexMaxCountInputField.text))
            {
                _vertexMaxCountInputField.text = defaults.ModelSearchMaxVertices.ToString();
            }

            if (string.IsNullOrEmpty(_textureMaxResolutionInputField.text))
            {
                _textureMaxResolutionInputField.text = defaults.ModelSearchTextureMaxResolution.ToString();
            }

            if (string.IsNullOrEmpty(_searchInputField.text))
            {
                _searchInputField.text = defaults.ModelSearchQuery;
            }
        }
    }
}
