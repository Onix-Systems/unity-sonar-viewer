
using UnityEngine;
using App.Infrastructure.UI;
using App.UI.Elements;
using App.Infrastructure.Contexts;
using App.Services;

namespace App.UI.Screens
{
    public class AboutScreenView : ScreenView
    {
        [SerializeField] BackButton _backButton;
        [SerializeField] LinkButton _contactUs;

        public BackButton BackButton => _backButton;

        protected override void OnVisible()
        {
            AppConfig appConfig = MainContext.Instance.Get<AppConfig>();
            _contactUs.SetLink(appConfig.ContactUsUrl);
        }
    }
}
