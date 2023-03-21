
using System;
using System.Threading.Tasks;
using System.Threading;
using RestHTTP;
using SketchfabAPI;
using SketchfabAPI.Entities;
using Utils.KeyedConfiguration;
using Utils.ProgressReporting;
using Utils.UnityCoroutineHelpers;
using App.UI;
using App.Helpers;
using App.Services;
using App.UI.Screens;
using App.UI.Popups;
using App.Core.Factories;
using App.Infrastructure.Contexts;
using App.Services.ModelARViewing;
using App.Services.ModelARViewing.ModelLoading;
using App.Services.AppDataStoring;
using App.Infrastructure.CommonInterfaces;
using App.Services.AppFSM;

namespace App.Core.States
{
    public class ModelLoadState : IAppState, IExitable
    {
        private CancellationTokenSource _pageLoadingCancellationTokenSource;
        private CancellationTokenSource _modelLoadCancellationTokenSource;

        public void Enter()
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();
            AppData appData = mainContext.Get<AppDataStorage>().Data;

            ui.HideAllPopups();
            ShowModelsListScreen(appData.LastSearchResults);
        }

        public void Exit()
        {
            _pageLoadingCancellationTokenSource?.Cancel(false);
            _pageLoadingCancellationTokenSource?.Dispose();
            _pageLoadingCancellationTokenSource = null;

            _modelLoadCancellationTokenSource?.Cancel(false);
            _modelLoadCancellationTokenSource?.Dispose();
            _modelLoadCancellationTokenSource = null;
        }

        private void ShowModelsListScreen(SearchResultsEntity searchResultEntity)
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            ModelsListScreenView modelsListScreenView = ui.GetScreen<ModelsListScreenView>();
            modelsListScreenView.OnItemClicked += OnItemClicked;
            modelsListScreenView.OnPreviousPageClicked += OnPreviousSearchPage;
            modelsListScreenView.OnNextPageClicked += OnNextSearchPage;

            modelsListScreenView.BackButton.OnClick += () =>
            {
                AppStateNavigator.GoTo<ModelSearchState>();
            };

            ui.HideAllPopups();
            ui.ShowScreen(modelsListScreenView);
            modelsListScreenView.SetData(searchResultEntity);
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

        private void ShowProgressPopup(ProgressReport<ProgressInfo> progressReport)
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            ProgressPopupView progressPopupView = ui.GetPopup<ProgressPopupView>();

            progressPopupView.SetProgress(
                progressReport.ProgressValue,
                progressReport.ProgressInfo.Message);

            ui.ShowPopup(progressPopupView);
        }

        private void HideProgressPopup(Action onHidden = null)
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            UIConfig uiConfig = mainContext.Get<AppConfig>().UIConfig;
            Coroutines.InvokeAfter(uiConfig.DownloadProgressCloseDelay, () =>
            {
                ui.HidePopup<ProgressPopupView>();
                onHidden?.Invoke();
            });
        }

        private async void LoadModelAsync(ModelEntity modelEntity)
        {
            IContext mainContext = MainContext.Instance;
            UIConfig uiConfig = mainContext.Get<AppConfig>().UIConfig;
            AppDataStorage appDataStorage = mainContext.Get<AppDataStorage>();
            ARViewer arViewer = mainContext.Get<ARViewer>();
            IProgressReporter<ProgressInfo> progressReporter = new ProgressReporter<ProgressInfo>(0f, 0.75f, ShowProgressPopup);

            _modelLoadCancellationTokenSource?.Cancel(false);
            _modelLoadCancellationTokenSource?.Dispose();
            _modelLoadCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _modelLoadCancellationTokenSource.Token;

            Result<byte[]> result = await DownloadGlbAsync(modelEntity.UId, progressReporter, cancellationToken);

            if (result.IsSuccess)
            {
                progressReporter = new ProgressReporter<ProgressInfo>(0.75f, 1f, ShowProgressPopup);
                progressReporter.Report(new ProgressInfo()
                {
                    ProgressNormalizedValue = 0f,
                    Message = uiConfig.Texts["Processing"],
                    Success = true
                });

                byte[] modelData = await ProcessDownloadedGlbDataAsync(result.Entity, cancellationToken);

                progressReporter.Report(new ProgressInfo()
                {
                    ProgressNormalizedValue = 0.5f,
                    Message = uiConfig.Texts["ModelLoading"],
                    Success = true
                });

                Model modelObject = await LoadGlbModelAsync(modelData, cancellationToken);

                if (modelObject != null)
                {
                    progressReporter.Report(new ProgressInfo
                    {
                        ProgressNormalizedValue = 1f,
                        Message = uiConfig.Texts["ModelLoadingCompleted"]
                    });

                    appDataStorage.Data.LoadedModelObject = modelObject;

                    HideProgressPopup(() =>
                    {
                        AppStateNavigator.GoTo<ModelViewingState>();
                    });
                }
                else
                {
                    progressReporter.Report(new ProgressInfo
                    {
                        ProgressNormalizedValue = 1f,
                        Message = uiConfig.Texts["ModelLoadingError"]
                    });

                    ShowErrorPopup(uiConfig.Texts["ModelLoadingError"]);
                    arViewer.ResetViewer();
                }

                HideProgressPopup();
            }
            else
            {
                ShowErrorPopup(result.Message);
                HideProgressPopup();
            }
        }

        private async Task<Result<byte[]>> DownloadGlbAsync(string modelId, IProgressReporter<ProgressInfo> progressReporter, CancellationToken cancellationToken)
        {
            IContext mainContext = MainContext.Instance;
            Result<byte[]> result = new Result<byte[]>();
            UIConfig _uiConfig = mainContext.Get<AppConfig>().UIConfig;

            SketchfabAPIFactory sketchfabAPIFactory = mainContext.Get<SketchfabAPIFactory>();
            DownloadAPI downloadAPI = sketchfabAPIFactory.CreateAPI<DownloadAPI>();
            Result<ArchivesEntity> archivesResult = await downloadAPI.GetArchiveLinksAsync(modelId);

            ArchivesEntity archives = archivesResult.Entity;
            bool isSucceed = archives != null;

            if (isSucceed)
            {
                ProgressInfo progressInfo = new ProgressInfo();
                progressInfo.ProgressNormalizedValue = 0f;
                progressInfo.Message = _uiConfig.Texts["StartDownloading"];
                progressReporter.Report(progressInfo);

                result = await downloadAPI.DownloadArchiveAsync(archives.Glb.Url,
                    (RequestProgressData requestProgressData) =>
                    {
                        progressInfo.ProgressNormalizedValue = requestProgressData.Progress;
                        float progressPercent = progressInfo.ProgressNormalizedValue * 100f;
                        progressInfo.Message = string.Format(_uiConfig.Texts["Downloading"], progressPercent);
                        progressReporter.Report(progressInfo);
                    },
                    cancellationToken);
            }
            else
            {
                result.Message = archivesResult.Message;
                result.StatusCode = archivesResult.StatusCode;
                result.IsSuccess = false;
            }

            return result;
        }

        private async Task<byte[]> ProcessDownloadedGlbDataAsync(byte[] downloadedData, CancellationToken cancellationToken)
        {
            IContext mainContext = MainContext.Instance;

            UIConfig uiConfig = mainContext.Get<AppConfig>().UIConfig;
            byte[] modelData = null;
            byte[] unzippedData = await AppHelpers.UnZipAsync(downloadedData, cancellationToken);

            if (unzippedData != null)
            {
                modelData = unzippedData;
            }
            else
            {
                modelData = downloadedData;
            }

            return modelData;
        }

        private async Task<Model> LoadGlbModelAsync(byte[] glbModelData, CancellationToken cancellationToken)
        {
            IGlbModelLoader glbModelLoader = new GLTFastGlbModelLoader();
            Model model = await glbModelLoader.LoadModelAsync(glbModelData, cancellationToken);

            return model;
        }

        private void ShowARViewerScreen()
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            ARViewerScreenView arViewerScreenView = ui.GetScreen<ARViewerScreenView>();
            arViewerScreenView.SearchButtonVisible = false;
            ui.ShowScreen(arViewerScreenView);
        }

        private async void OnPreviousSearchPage(string url)
        {
            _pageLoadingCancellationTokenSource?.Cancel(false);
            _pageLoadingCancellationTokenSource?.Dispose();
            _pageLoadingCancellationTokenSource = new CancellationTokenSource();

            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            using (HTTPRequest searchRequest = new HTTPRequest(url, HTTPMethod.Get))
            {
                CancellationToken token = _pageLoadingCancellationTokenSource.Token;
                HTTPResponse respons = await searchRequest.SendAsync(token);
                Result<SearchResultsEntity> searchResult = HTTPResponseProcessor.GetResult<SearchResultsEntity>(respons);

                if (searchResult.IsSuccess)
                {
                    ModelsListScreenView modelListScreenView = ui.GetScreen<ModelsListScreenView>();
                    modelListScreenView.SetData(searchResult.Entity);
                }
                else
                {
                    OnPageLoadRequestFailed(searchResult);
                }

                _pageLoadingCancellationTokenSource?.Dispose();
                _pageLoadingCancellationTokenSource = null;
            }
        }

        private async void OnNextSearchPage(string url)
        {
            _pageLoadingCancellationTokenSource?.Cancel(false);
            _pageLoadingCancellationTokenSource?.Dispose();
            _pageLoadingCancellationTokenSource = new CancellationTokenSource();

            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            using (HTTPRequest searchRequest = new HTTPRequest(url, HTTPMethod.Get))
            {
                CancellationToken token = _pageLoadingCancellationTokenSource.Token;
                HTTPResponse respons = await searchRequest.SendAsync(token);
                Result<SearchResultsEntity> searchResult = HTTPResponseProcessor.GetResult<SearchResultsEntity>(respons);

                if (searchResult.IsSuccess)
                {
                    ModelsListScreenView modelListScreenView = ui.GetScreen<ModelsListScreenView>();
                    modelListScreenView.SetData(searchResult.Entity);
                }
                else
                {
                    OnPageLoadRequestFailed(searchResult);
                }

                _pageLoadingCancellationTokenSource?.Dispose();
                _pageLoadingCancellationTokenSource = null;
            }
        }

        private void OnItemClicked(ModelEntity model)
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();
            ARViewer arViewer = mainContext.Get<ARViewer>();

            arViewer.SetEnvironmentScanning();
            AppConfig appConfig = mainContext.Get<AppConfig>();
            AppDataStorage appDataStorage = mainContext.Get<AppDataStorage>();
            InfoPopupView infoPopup = ui.GetPopup<InfoPopupView>();
            infoPopup.SetText(appConfig.UIConfig.Texts["Message - ScanEnvironment"]);
            ui.ShowPopup(infoPopup);

            appDataStorage.Data.SelectedModel = model;

            ShowARViewerScreen();
            LoadModelAsync(model);
        }

        private void OnPageLoadRequestFailed(Result<SearchResultsEntity> result)
        {
            if (result.StatusCode == 0 || result.Aborted)
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
