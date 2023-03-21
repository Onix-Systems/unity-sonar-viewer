
using System.Threading;
using App.Core.Factories;
using App.Infrastructure.CommonInterfaces;
using App.Infrastructure.Contexts;
using App.Services;
using App.Services.AppDataStoring;
using App.Services.AppFSM;
using App.UI.Screens;
using App.UI.Popups;
using App.UI.Elements;
using RestHTTP;
using SketchfabAPI.Entities;
using SketchfabAPI;
using Utils.KeyedConfiguration;
using App.Helpers;

namespace App.Core.States
{
    public class ModelSearchState : IAppState, IExitable
    {
        private CancellationTokenSource _searchRequestCancellationTokenSource;

        public void Enter()
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            ui.HideAllPopups();
            ShowModelSearchScreen();
        }

        public void Exit()
        {
            _searchRequestCancellationTokenSource?.Cancel(false);
            _searchRequestCancellationTokenSource?.Dispose();
            _searchRequestCancellationTokenSource = null;
        }

        private void ShowModelSearchScreen()
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            ModelsSearchScreenView modelSearchPopupView = ui.GetScreen<ModelsSearchScreenView>();
            modelSearchPopupView.OnSearchClicked += OnSearchClicked;

            ui.HideAllPopups();
            ui.ShowScreen(modelSearchPopupView);
        }

        private void ShowErrorPopup(string errorText)
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            ErrorPopupView errorPopupView = ui.GetPopup<ErrorPopupView>();
            errorPopupView.ErrorText = errorText;
            errorPopupView.OnOkClicked += () =>
            {
                ui.HidePopup(errorPopupView);
            };

            ui.HideAllPopups();
            ui.ShowPopup(errorPopupView);
        }

        private async void OnSearchClicked()
        {
            IContext mainContext = MainContext.Instance;

            AppDataStorage appDataStorage = mainContext.Get<AppDataStorage>();
            SketchfabAPIFactory sketchfabAPIFactory = mainContext.Get<SketchfabAPIFactory>();
            SearchAPI searchAPI = sketchfabAPIFactory.CreateAPI<SearchAPI>();

            SearchDTO modelSearchDTO = GetModelSearchDTO();
            AppData appData = appDataStorage.Data;
            appData.LastSearchParameters = modelSearchDTO;

            _searchRequestCancellationTokenSource?.Cancel(false);
            _searchRequestCancellationTokenSource?.Dispose();
            _searchRequestCancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _searchRequestCancellationTokenSource.Token;

            Result<SearchResultsEntity> searchResult = await searchAPI.Search(modelSearchDTO, token);

            if (searchResult.IsSuccess)
            {
                appData.LastSearchResults = searchResult.Entity;
                AppStateNavigator.GoTo<ModelLoadState>();
            }
            else
            {
                OnModelsSearchRequestFailed(searchResult);
            }
        }

        private SearchDTO GetModelSearchDTO()
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            ModelsSearchScreenView modelSearchScreenView = ui.GetScreen<ModelsSearchScreenView>();

            SearchDTO modelSearchDTO = new SearchDTO()
            {
                Type = SearchAPI.ModelsSearchType,
                Downloadable = true,
                Query = modelSearchScreenView.SearchQuery,
                ArchivesMaxVertexCount = modelSearchScreenView.VertexMaxCount,
                ArchivesTextureMaxResolution = modelSearchScreenView.TextureMaxResolution,
            };

            LicenseToggleButton.LicenseType selectedLicenseType = modelSearchScreenView.LicenseToggle.SelectedLicenseType;

            if (selectedLicenseType == LicenseToggleButton.LicenseType.CC_0)
            {
                modelSearchDTO.License = SketchfabAPI.Constants.Licenses.CreativeCommmons_0;
            }

            return modelSearchDTO;
        }

        private void OnModelsSearchRequestFailed(Result<SearchResultsEntity> result)
        {
            if (result.StatusCode == 0)
            {
                return;
            }

            IContext mainContext = MainContext.Instance;
            KeyedConfig uiTexts = mainContext.Get<AppConfig>().UIConfig.Texts;

            if (uiTexts.TryGetItem(result.StatusCode.ToString(), out string text))
            {
                ShowErrorPopup(text);
            }
            else if (!string.IsNullOrEmpty(result.Message))
            {
                ShowErrorPopup(result.Message);
            }
            else
            {
                ShowErrorPopup("Unknown Error happened");
            }
        }
    }
}
