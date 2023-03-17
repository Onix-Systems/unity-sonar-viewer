
using RestHTTP;
using SketchfabAPI;
using SketchfabAPI.DTOs;
using SketchfabAPI.Entities;
using Utils.KeyedConfiguration;
using App.Helpers;
using App.Infrastructure.Contexts;
using App.Services;
using App.UI.Popups;
using App.Core.Factories;
using App.Services.AppDataStoring;
using App.Services.AppFSM;

namespace App.Core.States
{
    public class AuthState : IAppState
    {
        public void Enter()
        {
            ShowLoginPopup();
        }

        private void ShowLoginPopup()
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            SketchfabLoginPopupView loginPopup = ui.GetPopup<SketchfabLoginPopupView>();
            loginPopup.OnLoginClick += LoginClickCallback;

            ui.HideAllPopups();
            ui.ShowPopup(loginPopup);
        }

        private void ShowErrorPopup(string errorText)
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            ErrorPopupView errorPopupView = ui.GetPopup<ErrorPopupView>();
            errorPopupView.ErrorText = errorText;
            errorPopupView.OnOkClicked += ShowLoginPopup;

            ui.HideAllPopups();
            ui.ShowPopup(errorPopupView);
        }

        private async void LoginClickCallback(CredentialsDTO credentials)
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            SketchfabLoginPopupView sketchfabLoginPopupView = ui.GetPopup<SketchfabLoginPopupView>();
            sketchfabLoginPopupView.OnLoginClick -= LoginClickCallback;

            SketchfabAPIFactory sketchfabAPIFactory = mainContext.Get<SketchfabAPIFactory>();
            AuthAPI authAPI = sketchfabAPIFactory.CreateAPI<AuthAPI>();

            Result<AccessTokenEntity> response = await authAPI.LoginAsync(credentials);

            if (response.IsSuccess)
            {
                OnLoginSucceed(response.Entity, credentials);
            }
            else
            {
                OnLoginFailed(response);
            }
        }

        private void OnLoginSucceed(AccessTokenEntity accessTokenEntity, CredentialsDTO credentials)
        {
            IContext mainContext = MainContext.Instance;

            AppDataStorage appDataStorage = mainContext.Get<AppDataStorage>();
            AppData data = appDataStorage.Data;

            data.SketchfabAccessToken = accessTokenEntity;
            data.LastAuthCredentials = credentials;
            
            AppStateNavigator.GoTo<ModelLoadState>();
        }

        private void OnLoginFailed(Result<AccessTokenEntity> response)
        {
            IContext mainContext = MainContext.Instance;
            KeyedConfig uiTexts = mainContext.Get<AppConfig>().UIConfig.Texts;

            if (uiTexts.TryGetItem(response.StatusCode.ToString(), out string text))
            {
                ShowErrorPopup(text);
            }
            else if (!string.IsNullOrEmpty(response.Message))
            {
                ShowErrorPopup(response.Message);
            }
            else
            {
                ShowErrorPopup(uiTexts.GetItem("Unknown Error"));
            }
        }
    }
}