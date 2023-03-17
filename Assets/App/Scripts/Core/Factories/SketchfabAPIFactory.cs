
using App.Infrastructure.Contexts;
using App.Services.AppDataStoring;
using RestHTTP;
using SketchfabAPI;

namespace App.Core.Factories
{
    public class SketchfabAPIFactory : APIFactory<SketchfabAPIClientBase>
    {
        private SketchfabAPISettings _settings;

        public SketchfabAPIFactory(RESTClient restClient, SketchfabAPISettings settings) : base(restClient)
        {
            _settings = settings;
        }

        protected override void OnAPIClientCreate<TApiClient>(TApiClient apiClient)
        {
            AppDataStorage appDataStorage = MainContext.Instance.Get<AppDataStorage>();
            AppData appData = appDataStorage.Data;

            apiClient.SetAPISettings(_settings);
            
            if (appData.SketchfabAccessToken != null)
            {
                string tokenType = appData.SketchfabAccessToken.TokenType;
                string accessToken = appData.SketchfabAccessToken.AccessToken;
                apiClient.SetAccessToken(tokenType, accessToken);
            }
        }
    }
}
