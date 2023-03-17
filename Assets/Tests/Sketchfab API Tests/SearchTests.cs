
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RestHTTP;
using SketchfabAPI;
using SketchfabAPI.Entities;

namespace Tests.SketchafbAPI
{
    public class SearchTests
    {
        private AccessTokenEntity _accessTokenEntity;

        [Test]
        public async Task SearchByQuery()
        {
            string searchQuery = "Table";

            Result<AccessTokenEntity> accessTokenEntityResult = await Auth();
            AccessTokenEntity accessTokenEntity = accessTokenEntityResult.Entity;

            Assert.IsNotNull(accessTokenEntity, accessTokenEntityResult.Message);

            RESTClient restClient = new RESTClient(TestConfig.NetworkSettings);
            SearchAPI searchAPI = new SearchAPI();
            searchAPI.SetRESTClient(restClient);
            searchAPI.SetAPISettings(TestConfig.SketchfabAPISettings);
            searchAPI.SetAccessToken(accessTokenEntity.TokenType, accessTokenEntity.AccessToken);

            SearchDTO searchDTO = new SearchDTO()
            {
                Query = searchQuery,
                Type = SearchAPI.ModelsSearchType
            };

            Result<SearchResultsEntity> searchResult = await searchAPI.Search(searchDTO, CancellationToken.None);

            Assert.IsTrue(searchResult.IsSuccess, searchResult.Message);
            Assert.NotNull(searchResult.Entity, searchResult.Message);
        }

        private async Task<Result<AccessTokenEntity>> Auth()
        {
            RESTClient restClient = new RESTClient(TestConfig.NetworkSettings);
            AuthAPI authAPI = new AuthAPI();
            authAPI.SetRESTClient(restClient);
            authAPI.SetAPISettings(TestConfig.SketchfabAPISettings);

            Result<AccessTokenEntity> authTokenResult = await authAPI.LoginAsync(TestConfig.LoginCredentials);
            return authTokenResult;
        }
    }
}