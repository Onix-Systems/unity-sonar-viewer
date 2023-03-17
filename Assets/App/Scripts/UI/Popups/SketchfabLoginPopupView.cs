
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SketchfabAPI.DTOs;
using App.Infrastructure.Contexts;
using App.Services;
using App.Infrastructure.UI;
using App.Services.AppDataStoring;

namespace App.UI.Popups
{
    public class SketchfabLoginPopupView : PopupView
    {
        [SerializeField] private TMP_InputField _emailInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private Button _loginButton;

        public string Email => _emailInputField.text;
        public string Password => _passwordInputField.text;

        public event Action<CredentialsDTO> OnLoginClick;

        protected override void OnVisible()
        {
            if(!TrySetMockCredentials())
            {
                SetCredentialsFromAppData();
            }

            _loginButton.onClick.AddListener(() =>
            {
                CredentialsDTO credentialsDTO = new CredentialsDTO() 
                { 
                    Email = Email.Trim(),
                    Password = Password
                };

                OnLoginClick?.Invoke(credentialsDTO);
            });
        }

        protected override void OnInvisible()
        {
            _loginButton.onClick.RemoveAllListeners();
            OnLoginClick = null;
        }

        private bool TrySetMockCredentials()
        {
            IContext mainContext = MainContext.Instance;
            AppConfig config = mainContext.Get<AppConfig>();
            CredentialsDTO mockCredentials = config.Defaults.SketchafabMockCredentials;

            if (!string.IsNullOrEmpty(mockCredentials.Email) &&
                !string.IsNullOrEmpty(mockCredentials.Password))
            {
                _emailInputField.text = mockCredentials.Email;
                _passwordInputField.text = mockCredentials.Password;
                return true;
            }

            return false;
        }

        private void SetCredentialsFromAppData()
        {
            IContext mainContext = MainContext.Instance;
            AppDataStorage dataStorage = mainContext.Get<AppDataStorage>();
            CredentialsDTO lastAuthCredentials = dataStorage.Data.LastAuthCredentials;

            if (lastAuthCredentials != null)
            {
                _emailInputField.text = lastAuthCredentials.Email;
                _passwordInputField.text = lastAuthCredentials.Password;
            }
        }
    }
}