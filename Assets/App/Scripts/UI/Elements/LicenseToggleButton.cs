
using UnityEngine;
using UnityEngine.UI;
using App.Infrastructure.UI;
using TMPro;

namespace App.UI.Elements
{
    public class LicenseToggleButton : View
    {
        public enum LicenseType
        {
            Any,
            CC_0,
        }

        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private Button _button;
        [SerializeField] private LicenseType _initialLicense = LicenseType.Any;

        public LicenseType SelectedLicenseType { get; private set; }

        public void SetLicenseType(LicenseType licenseType)
        {
            _label.text = licenseType.ToString();
            SelectedLicenseType = licenseType;
        }

        protected override void OnAwake()
        {
            SetLicenseType(_initialLicense);
        }

        protected override void OnVisible()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        protected override void OnInvisible()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            SetLicenseType(_initialLicense);
        }
        #endif

        private void Toggle()
        {
            switch(SelectedLicenseType)
            {
                case LicenseType.Any:
                    SetLicenseType(LicenseType.CC_0);
                    break;

                case LicenseType.CC_0:
                    SetLicenseType(LicenseType.Any);
                    break;
            }
        }

        private void OnButtonClicked()
        {
            Toggle();
        }
    }
}
