
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
using App.UI.Elements;
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
        private IContext _mainContext;
        private AppUI _ui;
        private CancellationTokenSource _searchRequestCancellationTokenSource;
        private CancellationTokenSource _modelLoadCancellationTokenSource;

        public ModelLoadState()
        {
            _mainContext = MainContext.Instance;
            _ui = _mainContext.Get<AppUI>();
        }

        public void Enter()
        {
            _ui.HideAllPopups();
            ShowModelSearchScreen();
        }

        public void Exit()
        {
            _searchRequestCancellationTokenSource?.Cancel(false);
            _searchRequestCancellationTokenSource?.Dispose();
            _searchRequestCancellationTokenSource = null;

            _modelLoadCancellationTokenSource?.Cancel(false);
            _modelLoadCancellationTokenSource?.Dispose();
            _modelLoadCancellationTokenSource = null;
        }

        private void ShowModelSearchScreen() 
        {
            ModelsSearchScreenView modelSearchPopupView = _ui.GetScreen<ModelsSearchScreenView>();
            modelSearchPopupView.OnSearchClicked += OnSearchClicked;
            
            _ui.HideAllPopups();
            _ui.ShowScreen(modelSearchPopupView);
        }

        private void ShowModelsListScreen(SearchResultsEntity searchResultEntity)
        {
            ModelsListScreenView modelsListScreenView = _ui.GetScreen<ModelsListScreenView>();
            modelsListScreenView.OnItemClicked += OnItemClicked;
            modelsListScreenView.OnPreviousPageClicked += OnPreviousSearchPage;
            modelsListScreenView.OnNextPageClicked += OnNextSearchPage;

            modelsListScreenView.BackButton.OnClick += () =>
            {
                ShowModelSearchScreen();
            };

            _ui.HideAllPopups();
            _ui.ShowScreen(modelsListScreenView);
            modelsListScreenView.SetData(searchResultEntity);
        }

        private void ShowErrorPopup(string errorText)
        {
            ErrorPopupView errorPopupView = _ui.GetPopup<ErrorPopupView>();
            errorPopupView.ErrorText = errorText;
            errorPopupView.OnOkClicked += ShowModelSearchScreen;

            _ui.HideAllPopups();
            _ui.ShowPopup(errorPopupView);
        }

        private void ShowProgressPopup(ProgressReport<ProgressInfo> progressReport)
        {
            ProgressPopupView progressPopupView = _ui.GetPopup<ProgressPopupView>();

            progressPopupView.SetProgress(
                progressReport.ProgressValue,
                progressReport.ProgressInfo.Message);

            _ui.ShowPopup(progressPopupView);
        }

        private void HideProgressPopup(Action onHidden = null)
        {
            UIConfig uiConfig = _mainContext.Get<AppConfig>().UIConfig;
            Coroutines.InvokeAfter(uiConfig.DownloadProgressCloseDelay, () =>
            {
                _ui.HidePopup<ProgressPopupView>();
                onHidden?.Invoke();
            });
        }

        private void LoadModel(ModelEntity modelEntity)
        {
            UIConfig uiConfig = MainContext.Instance.Get<AppConfig>().UIConfig;
            SketchfabModelDownloader downloader = _mainContext.Get<SketchfabModelDownloader>();
            IProgressReporter<ProgressInfo> progressReporter = new ProgressReporter<ProgressInfo>(0f, 0.75f, ShowProgressPopup);
            
            _modelLoadCancellationTokenSource?.Cancel(false);
            _modelLoadCancellationTokenSource?.Dispose();
            _modelLoadCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _modelLoadCancellationTokenSource.Token;

            Task<Result<byte[]>> downloadingTask = Task.Factory.StartNew(
                () => downloader.DownloadGlbAsync(modelEntity.UId, progressReporter, cancellationToken),
                cancellationToken,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext()).Unwrap();

            downloadingTask.ContinueWith(downloadTask =>
            {
                Result<byte[]> result = downloadTask.Result;

                if (result.IsSuccess)
                {
                    progressReporter.Report(new ProgressInfo
                    {
                        ProgressNormalizedValue = 1f,
                        Message = uiConfig.Texts["Processing"]
                    });

                    byte[] data = result.Entity;

                    Task<byte[]> unzippingTask = Task.Factory.StartNew(
                        () => AppHelpers.UnZipAsync(data),
                        cancellationToken,
                        TaskCreationOptions.None,
                        TaskScheduler.FromCurrentSynchronizationContext()).Unwrap();

                    unzippingTask.ContinueWith(unzipTask => 
                    {
                        progressReporter = new ProgressReporter<ProgressInfo>(0.85f, 1f, ShowProgressPopup);

                        byte[] unzippedData = unzippingTask.Result; 
                        if (unzippedData != null)
                            data = unzippedData;

                        ARViewer arModelViewer = _mainContext.Get<ARViewer>();

                        progressReporter.Report(new ProgressInfo
                        {
                            ProgressNormalizedValue = 0f,
                            Message = uiConfig.Texts["ModelLoading"]
                        });

                        Task<ModelObject> modelLoadingTask = Task.Factory.StartNew(
                            () => LoadGlbModelAsync(data),
                            cancellationToken,
                            TaskCreationOptions.None,
                            TaskScheduler.FromCurrentSynchronizationContext()).Unwrap();

                        modelLoadingTask.ContinueWith(modelLoadTask => 
                        {
                            ModelObject model = modelLoadTask.Result;

                            if (model != null)
                            {
                                progressReporter.Report(new ProgressInfo
                                {
                                    ProgressNormalizedValue = 1f,
                                    Message = uiConfig.Texts["ModelLoadingCompleted"]
                                });

                                AppDataStorage appDataStorage = _mainContext.Get<AppDataStorage>();
                                appDataStorage.Data.LoadedModelObject = model;


                                HideProgressPopup(() => {
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
                                arModelViewer.ResetViewer();
                            }

                            HideProgressPopup();

                        }, TaskScheduler.FromCurrentSynchronizationContext());

                        unzipTask.Dispose();

                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                {
                    ShowErrorPopup(result.Message);
                    HideProgressPopup();
                }

                downloadingTask.Dispose();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task<ModelObject> LoadGlbModelAsync(byte[] glbModelData)
        {
            IGlbModelLoader glbModelLoader = new GLTFastGlbModelLoader();
            ModelObject model = await glbModelLoader.LoadModelAsync(glbModelData);

            return model;
        }

        private void ShowARViewerScreen()
        {
            ARViewerScreenView arViewerScreenView = _ui.GetScreen<ARViewerScreenView>();
            arViewerScreenView.SearchButtonVisible = false;
            _ui.ShowScreen(arViewerScreenView);
        }

        private SearchDTO GetModelSearchDTO()
        {
            ModelsSearchScreenView modelSearchScreenView = _ui.GetScreen<ModelsSearchScreenView>();

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

        private async void OnSearchClicked()
        {
            AppDataStorage appDataStorage = _mainContext.Get<AppDataStorage>();
            SketchfabAPIFactory sketchfabAPIFactory = _mainContext.Get<SketchfabAPIFactory>();
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
                ShowModelsListScreen(searchResult.Entity);
            }
            else
            {
                OnModelsSearchRequestFailed(searchResult);
            }
        }

        private async void OnPreviousSearchPage(string url)
        {
            _searchRequestCancellationTokenSource?.Cancel(false);
            _searchRequestCancellationTokenSource?.Dispose();
            _searchRequestCancellationTokenSource = new CancellationTokenSource();

            using (HTTPRequest searchRequest = new HTTPRequest(url, HTTPMethod.Get))
            {
                CancellationToken token = _searchRequestCancellationTokenSource.Token;
                HTTPResponse respons = await searchRequest.SendAsync(token);
                Result<SearchResultsEntity> searchResult = HTTPResponseProcessor.GetResult<SearchResultsEntity>(respons);

                if (searchResult.IsSuccess) 
                {
                    ModelsListScreenView modelListScreenView = _ui.GetScreen<ModelsListScreenView>();
                    modelListScreenView.SetData(searchResult.Entity);
                }
                else
                {
                    OnModelsSearchRequestFailed(searchResult);
                }
            }
        }

        private async void OnNextSearchPage(string url)
        {
            _searchRequestCancellationTokenSource?.Cancel(false);
            _searchRequestCancellationTokenSource?.Dispose();
            _searchRequestCancellationTokenSource = new CancellationTokenSource();

            using (HTTPRequest searchRequest = new HTTPRequest(url, HTTPMethod.Get))
            {
                CancellationToken token = _searchRequestCancellationTokenSource.Token;
                HTTPResponse respons = await searchRequest.SendAsync(token);
                Result<SearchResultsEntity> searchResult = HTTPResponseProcessor.GetResult<SearchResultsEntity>(respons);

                if (searchResult.IsSuccess)
                {
                    ModelsListScreenView modelListScreenView = _ui.GetScreen<ModelsListScreenView>();
                    modelListScreenView.SetData(searchResult.Entity);
                }
                else
                {
                    OnModelsSearchRequestFailed(searchResult);
                }
            }
        }

        private void OnItemClicked(ModelEntity model)
        {
            ARViewer arViewer = _mainContext.Get<ARViewer>();
            
            arViewer.SetEnvironmentScanning();
            AppConfig appConfig = _mainContext.Get<AppConfig>();
            AppDataStorage appDataStorage = _mainContext.Get<AppDataStorage>();
            InfoPopupView infoPopup = _ui.GetPopup<InfoPopupView>();
            infoPopup.SetText(appConfig.UIConfig.Texts["Message - ScanEnvironment"]);
            _ui.ShowPopup(infoPopup);

            appDataStorage.Data.SelectedModel = model;

            ShowARViewerScreen();
            LoadModel(model);
        }

        private void OnModelsSearchRequestFailed(Result<SearchResultsEntity> result)
        {
            if (result.StatusCode == 0)
            {
                return;
            }

            KeyedConfig uiTexts = _mainContext.Get<AppConfig>().UIConfig.Texts;

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
