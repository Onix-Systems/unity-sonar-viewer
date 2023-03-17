
using System;
using System.Collections;
using UnityEngine;
using Utils.UnityCoroutineHelpers;
using App.Services;
using App.Infrastructure.Contexts;
using App.UI;
using App.Services.ModelARViewing;
using App.Services.AppDataStoring;
using App.Infrastructure.CommonInterfaces;
using App.Services.Input;
using App.UI.Screens;
using App.Helpers;
using App.Services.AppFSM;
using App.Services.Input.GestureDetectors.Tap;
using App.Services.ModelARViewing.States;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace App.Core.States
{
    public class ModelViewingState : IAppState, IExitable
    {
        private Coroutine _arPlanesWaitCo;

        public void Enter()
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();
            ARViewer arViewer = mainContext.Get<ARViewer>();
            InputService inputService = mainContext.Get<InputService>();

            ShowARViewerScreen();

            WaitForARPlanes(() =>
            {
                SetModelPositioning();
                inputService.TapDetector.OnTap += OnTap;
            });

            arViewer.OnTargetARPlaneRemoved += OnTargetARPlaneRemoved;
        }

        public void Exit()
        {
            IContext mainContext = MainContext.Instance;
            InputService inputService = mainContext.Get<InputService>();
            ARViewer arViewer = mainContext.Get<ARViewer>();
            AppUI ui = mainContext.Get<AppUI>();

            if (_arPlanesWaitCo != null)
            {
                Coroutiner.StopCoroutine(_arPlanesWaitCo);
                _arPlanesWaitCo = null;
            }

            ARViewerScreenView arViewerScreenView = ui.GetScreen<ARViewerScreenView>();
            arViewerScreenView.SearchButtonVisible = false;
            inputService.TapDetector.OnTap -= OnTap;
            arViewer.OnTargetARPlaneRemoved -= OnTargetARPlaneRemoved;
        }


        private void ShowARViewerScreen()
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();
            ARViewerScreenView arViewerScreenView = ui.GetScreen<ARViewerScreenView>();
            ARViewer arViewer = mainContext.Get<ARViewer>();

            arViewerScreenView.OnMenuClicked += OnMenuClicked;
            arViewerScreenView.SearchButtonVisible = true;

            ui.ShowScreen(arViewerScreenView);

            if (arViewer.State is EnvironmentScanning)
            {
                ShowEnvironmentScanMessage();
            }

            if (arViewer.State is ModelPositioningWithScreenCenter)
            {
                ShowPositionateModelMessage();
            }

            if (arViewer.State is ModelTransformEditing)
            {
                ShowEditModelTransformMessage();
            }
        }

        private void ShowMenuScreen()
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            MenuScreenView menuScreenView = ui.GetScreen<MenuScreenView>();

            menuScreenView.BackButton.OnClick += () =>
            {
                ShowARViewerScreen();
            };

            menuScreenView.OnModelInfoClicked += () =>
            {
                ShowModelInfoScreen();
            };

            menuScreenView.OnModelSearchClicked += () =>
            {
                mainContext.Get<ARViewer>().ResetViewer();
                AppStateNavigator.GoTo<ModelLoadState>();
            };

            menuScreenView.OnAboutClicked += () =>
            {
                ShowAboutScreen();
            };

            ui.HideAllPopups();
            ui.ShowScreen(menuScreenView);
        }

        private void ShowModelInfoScreen()
        {
            IContext mainContext = MainContext.Instance;
            AppData appData = mainContext.Get<AppDataStorage>().Data;
            AppUI ui = mainContext.Get<AppUI>();

            ModelInfoScreenView modelInfoScreenView = ui.GetScreen<ModelInfoScreenView>();

            modelInfoScreenView.BackButton.OnClick += () =>
            {
                ShowMenuScreen();
            };

            ui.HideAllPopups();
            ui.ShowScreen(modelInfoScreenView);
            modelInfoScreenView.SetData(appData.SelectedModel);
        }

        private void ShowAboutScreen()
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();

            AboutScreenView aboutScreen = ui.GetScreen<AboutScreenView>();

            aboutScreen.BackButton.OnClick += () =>
            {
                ShowMenuScreen();
            };

            ui.HideAllPopups();
            ui.ShowScreen(aboutScreen);
        }

        private void ShowEnvironmentScanMessage()
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();
            UIConfig uiConfig = mainContext.Get<AppConfig>().UIConfig;

            if (!ui.IsCurrentScreen<ARViewerScreenView>())
            {
                return;
            }

            InfoPopupView infoPopup = ui.GetPopup<InfoPopupView>();
            infoPopup.SetText(uiConfig.Texts["Message - ScanEnvironment"]);
            ui.ShowPopup(infoPopup);
        }

        private void ShowPositionateModelMessage()
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();
            UIConfig uiConfig = mainContext.Get<AppConfig>().UIConfig;

            if (!ui.IsCurrentScreen<ARViewerScreenView>())
            {
                return;
            }

            InfoPopupView infoPopup = ui.GetPopup<InfoPopupView>();
            infoPopup.SetText(uiConfig.Texts["Message - PositionateModel"]);
            ui.ShowPopup(infoPopup);
        }

        private void ShowEditModelTransformMessage()
        {
            IContext mainContext = MainContext.Instance;
            AppUI ui = mainContext.Get<AppUI>();
            UIConfig uiConfig = mainContext.Get<AppConfig>().UIConfig;

            if (!ui.IsCurrentScreen<ARViewerScreenView>())
            {
                return;
            }

            InfoPopupView infoPopup = ui.GetPopup<InfoPopupView>();
            infoPopup.SetText(uiConfig.Texts["Message - EditModelTransform"]);
            ui.ShowPopup(infoPopup);
        }

        private void WaitForARPlanes(Action onDetected)
        {
            Coroutiner.StopCoroutine(_arPlanesWaitCo);

            _arPlanesWaitCo = Coroutiner.StartCoroutine(WaitForARPlanesDetectedCo(() =>
            {
                onDetected?.Invoke();
                _arPlanesWaitCo = null;
            }));
        }

        private void SetEnvironmentScanning()
        {
            IContext mainContext = MainContext.Instance;
            InputService inputService = mainContext.Get<InputService>();
            ARViewer arViewer = mainContext.Get<ARViewer>();
            
            ShowEnvironmentScanMessage();
            arViewer.SetEnvironmentScanning();

            WaitForARPlanes(() =>
            {
                SetModelPositioning();
                inputService.TapDetector.OnTap += OnTap;
            });
        }

        private void SetModelPositioning()
        {
            IContext mainContext = MainContext.Instance;
            AppDataStorage dataStorage = mainContext.Get<AppDataStorage>();
            ARViewer arViewer = mainContext.Get<ARViewer>();

            ShowPositionateModelMessage();
            arViewer.SetModelPositioning(dataStorage.Data.LoadedModelObject);
        }

        private void SetModelEditTransform()
        {
            IContext mainContext = MainContext.Instance;
            AppDataStorage dataStorage = mainContext.Get<AppDataStorage>();
            ARViewer arViewer = mainContext.Get<ARViewer>();

            ShowEditModelTransformMessage();
            arViewer.SetModelTransfromEdit(dataStorage.Data.LoadedModelObject);
        }

        private void OnTap(TapEventArgs tapEventArgs)
        {
            IContext mainContext = MainContext.Instance;
            InputService inputService = mainContext.Get<InputService>();
            
            inputService.TapDetector.OnTap -= OnTap;
            SetModelEditTransform();
        }

        private void OnMenuClicked()
        {
            ShowMenuScreen();
        }

        private void OnTargetARPlaneRemoved()
        {
            SetEnvironmentScanning();
        }

        private IEnumerator WaitForARPlanesDetectedCo(Action onDetected)
        {
            #if UNITY_EDITOR
            yield return null;
            onDetected?.Invoke();
            #else
            IContext mainContext = MainContext.Instance;
            ARViewer arViewer = mainContext.Get<ARViewer>();

            yield return new WaitUntil(() => arViewer.ARPlanesDetected);
            onDetected?.Invoke();
            #endif
        }
    }
}