
namespace RestHTTP
{
    public class APIClientBase
    {
        protected RESTClient RESTClient { get; private set; }

        public APIClientBase() { }

        public APIClientBase(RESTClient restClient)
        {
            SetRESTClient(restClient);
        }

        public void SetRESTClient(RESTClient restClient)
        {
            RESTClient = restClient;
        }
    }
}
