
using System.Threading.Tasks;
using NUnit.Framework;
using RestHTTP;
using SketchfabAPI.Entities;
using SketchfabAPI;
using System.Threading;

namespace Tests.SketchafbAPI
{
    public class DownloadModelTests
    {
        private AccessTokenEntity _accessTokenEntity;

        [Test]
        public async Task RetrieveLinks()
        {
            Result<AccessTokenEntity> accessTokenEntityResult = await Auth();
            AccessTokenEntity accessTokenEntity = accessTokenEntityResult.Entity;

            Assert.IsNotNull(accessTokenEntity, accessTokenEntityResult.Message);

            Result<ArchivesEntity> archives = await GetArchiveLinksAsync(accessTokenEntity);

            Assert.IsNotNull(archives.Entity, archives.Message);
        }

        [Test]
        public async Task DownloadGlbModel()
        {
            Result<AccessTokenEntity> accessTokenEntityResult = await Auth();
            AccessTokenEntity accessTokenEntity = accessTokenEntityResult.Entity;

            Assert.IsNotNull(accessTokenEntity, accessTokenEntityResult.Message);

            Result<ArchivesEntity> archivesResult = await GetArchiveLinksAsync(accessTokenEntity);

            Assert.IsNotNull(archivesResult, archivesResult.Message);

            RESTClient restClient = new RESTClient(TestConfig.NetworkSettings);
            DownloadAPI downloadAPI = new DownloadAPI();
            downloadAPI.SetRESTClient(restClient);
            downloadAPI.SetAPISettings(TestConfig.SketchfabAPISettings);

            Result<byte[]> result = await downloadAPI.DownloadArchiveAsync(archivesResult.Entity.Glb.Url, null, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess, result.Message);
            Assert.IsNotNull(result.Entity, result.Message);
        }

        private async Task<Result<ArchivesEntity>> GetArchiveLinksAsync(AccessTokenEntity accessTokenEntity)
        {
            RESTClient restClient = new RESTClient(TestConfig.NetworkSettings);
            DownloadAPI downloadAPI = new DownloadAPI();
            downloadAPI.SetRESTClient(restClient);
            downloadAPI.SetAccessToken(accessTokenEntity.TokenType, accessTokenEntity.AccessToken);
            downloadAPI.SetAPISettings(TestConfig.SketchfabAPISettings);

            Result<ArchivesEntity> archives = await downloadAPI.GetArchiveLinksAsync(TestConfig.DownloadModelId);

            return archives;
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