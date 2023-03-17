
using System.Collections;
using UnityEngine;
using SketchfabAPI;
using App.Core;
using App.Infrastructure.CommonInterfaces;
using App.Infrastructure.Contexts;
using Utils.UnityCoroutineHelpers;

namespace App.Services.AppDataStoring
{
    public class AppDataStorage: IInitializable, IDisposable
    {
        private const float SaveIntervalSeconds = 2f;

        public AppData Data { get; set; } = new AppData();

        private Coroutine _saveingCo;

        public void Initialize()
        {
            AppConfig appConfig = MainContext.Instance.Get<AppConfig>();
            LoadAppData(appConfig.Defaults);

            _saveingCo = Coroutiner.StartCoroutine(SaveCo(SaveIntervalSeconds));
        }

        public void Dispose()
        {
            Coroutiner.StopCoroutine(_saveingCo);
        }

        private void LoadAppData(DefaultsConfig defaultsConfig)
        {
            if (PlayerPrefs.HasKey(nameof(AppData)))
            {
                Data = PlayerPrefs.GetString(nameof(AppData));
            }

            if (Data == null)
            {
                Data = new AppData()
                {
                    LastSearchParameters = new SearchDTO()
                    {
                        ArchivesMaxVertexCount = defaultsConfig.ModelSearchMaxVertices,
                        ArchivesTextureMaxResolution = defaultsConfig.ModelSearchTextureMaxResolution
                    }
                };
            }

            Data.IsDirty = false;
        }

        private IEnumerator SaveCo(float saveIntervalSeconds)
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(saveIntervalSeconds);

            while (true)
            {
                if (Data.IsDirty)
                {
                    PlayerPrefs.SetString(nameof(AppData), Data);
                    Data.IsDirty = false;
                }

                yield return waitForSeconds;
            }
        }
    }
}
