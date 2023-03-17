
using System.Threading.Tasks;
using NUnit.Framework;
using RestHTTP;
using SketchfabAPI;
using SketchfabAPI.Entities;

namespace Tests.SketchafbAPI
{
    public class AuthTests
    {
        [Test]
        public async Task Login()
        {
            RESTClient restClient = new RESTClient(TestConfig.NetworkSettings); 
            AuthAPI authAPI = new AuthAPI();
            authAPI.SetRESTClient(restClient);
            authAPI.SetAPISettings(TestConfig.SketchfabAPISettings);

            Result<AccessTokenEntity> authTokenResult = await authAPI.LoginAsync(TestConfig.LoginCredentials);

            Assert.IsTrue(authTokenResult.IsSuccess, authTokenResult.Message);
            Assert.IsNotNull(authTokenResult.Entity, authTokenResult.Message);
            Assert.IsFalse(string.IsNullOrEmpty(authTokenResult.Entity.AccessToken), authTokenResult.Message);
        }
    }
}