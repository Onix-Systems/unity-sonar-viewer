
using App.Infrastructure.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.UI.Elements
{
    [RequireComponent(typeof(Button))]
    public class LinkButton : View
    {
        [SerializeField] private string _link = string.Empty;

        private Button _button;
        private TextMeshProUGUI _linkLabel;

        private string _targetLink;

        protected override void OnAwake()
        {
            _button = GetComponent<Button>();
            _linkLabel = GetComponentInChildren<TextMeshProUGUI>();

            SetLink(_link);
        }

        protected override void OnVisible()
        {
            _button.onClick.AddListener(() =>
            {
                Application.OpenURL(_targetLink);
            }); 
        }

        protected override void OnInvisible()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void SetLabel(string label)
        {
            _linkLabel.text = label;
        }

        public void SetLink(string link)
        {
            _link = link;

            if (!string.IsNullOrEmpty(_link))
            {
                _targetLink = _link;
            }
            else
            {
                _targetLink = _linkLabel.text;
            }
        }
    }
}
