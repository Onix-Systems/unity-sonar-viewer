
using UnityEngine;
using App.Infrastructure.UI;
using App.UI.Elements;
using SketchfabAPI.Entities;
using TMPro;
using UnityEngine.UI;

namespace App.UI.Screens
{
    public class ModelInfoScreenView : ScreenView
    {
        [SerializeField] BackButton _backButton;
        [SerializeField] private TextMeshProUGUI _modelNameLabel;
        [SerializeField] private Button _modelLinkButton;
        [SerializeField] private TextMeshProUGUI _modelLinkLabel;
        [SerializeField] private TextMeshProUGUI _authorUsernameLabel;
        [SerializeField] private Button _authorLinkButton;
        [SerializeField] private TextMeshProUGUI _authorProfileLinkLabel;
        [SerializeField] private TextMeshProUGUI _licenseLabel;
        
        public BackButton BackButton => _backButton;

        private string _modelLink = string.Empty;
        private string _authorProfileLink = string.Empty;

        protected override void OnVisible()
        {
            _modelLinkButton.onClick.AddListener(OnModelLinkClicked);
            _authorLinkButton.onClick.AddListener(OnAuthorProfileLinkClicked);
        }

        protected override void OnInvisible()
        {
            _modelLinkButton.onClick.RemoveListener(OnModelLinkClicked);
            _authorLinkButton.onClick.RemoveListener(OnAuthorProfileLinkClicked);
        }

        public void SetData(ModelEntity modelEntity)
        {
            _modelLink = modelEntity.ViewerUrl;
            _authorProfileLink = modelEntity.User?.ProfileUrl;
            _modelNameLabel.text = modelEntity.Name;
            _modelLinkLabel.text = _modelLink;
            _authorUsernameLabel.text = modelEntity.User?.Username;
            _authorProfileLinkLabel.text = _authorProfileLink;
            _licenseLabel.text = modelEntity.License?.Label;
        }

        private void OnModelLinkClicked()
        {
            Application.OpenURL(_modelLink);
        }

        private void OnAuthorProfileLinkClicked()
        {
            Application.OpenURL(_authorProfileLink);
        }
    }
}
