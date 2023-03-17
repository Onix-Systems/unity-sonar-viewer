
namespace RestHTTP
{
    public abstract class APIFactory<TAPIClientBase> where TAPIClientBase: APIClientBase
    {
        private RESTClient _restClient;

        public APIFactory(RESTClient restClient) 
        {
            _restClient = restClient;
        }

        public TApiClient CreateAPI<TApiClient>() where TApiClient: TAPIClientBase, new()
        {
            TApiClient api = new TApiClient();
            api.SetRESTClient(_restClient);

            OnAPIClientCreate(api);

            return api;
        }

        protected virtual void OnAPIClientCreate<TApiClient>(TApiClient apiClient) where TApiClient : TAPIClientBase { }
    }
}
