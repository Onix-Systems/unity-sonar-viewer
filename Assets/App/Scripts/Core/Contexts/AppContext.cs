
using System;
using System.Collections.Generic;
using UnityEngine;
using RestHTTP;
using SketchfabAPI;
using Utils.UnityCoroutineHelpers;
using App.Services;
using App.Infrastructure.Contexts;
using App.Core.Factories;
using App.Services.Input;
using App.Services.SceneLoading;
using App.Services.AppDataStoring;
using App.Services.AppFSM;

namespace App.Core.Contexts
{
    public class AppContext : MainContext
    {
        protected override void SetupServices(out List<object> services)
        {
            services = new List<object>();

            AppConfig appConfig = LoadAppConfig();

            if (appConfig == null)
            {
                throw new Exception("Critical Error. AppConfig is not initialized.");
            }

            Application.targetFrameRate = appConfig.TargetFrameRate;
            Coroutiner coroutiner = gameObject.AddComponent<Coroutiner>();

            services.Add(appConfig);
            services.Add(coroutiner);
            services.Add(new SceneLoader());
            services.Add(new AppStateMachine());
            services.Add(new AppStateFactory());
            services.Add(new InputService());
            services.Add(new AppUI());
            services.Add(new AppDataStorage());

            SetupNetworking(services, appConfig);
        }

        private void SetupNetworking(List<object> services, AppConfig appConfig)
        {
            NetworkSettings networkSettings = appConfig.NetworkConfig.NetworkSettings;
            SketchfabAPISettings sketchfabAPISettings = appConfig.NetworkConfig.SketchfabSettings;

            RESTClient restClient = new RESTClient(networkSettings);
            services.Add(restClient);
            services.Add(new SketchfabAPIFactory(restClient, sketchfabAPISettings));
        }

        private AppConfig LoadAppConfig()
        {
            return Resources.Load<AppConfig>(Core.Constants.AppConfigResourcesFilePath);
        }
    }
}