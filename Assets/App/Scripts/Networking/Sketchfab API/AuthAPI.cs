
using System;
using System.Threading;
using System.Threading.Tasks;
using RestHTTP;
using SketchfabAPI.DTOs;
using SketchfabAPI.Entities;

namespace SketchfabAPI
{
    public class AuthAPI : SketchfabAPIClientBase
    {
        public const string AccessTokenEndpoint = Constants.BaseURL + "/oauth2/token";

        public async Task<Result<AccessTokenEntity>> LoginAsync(CredentialsDTO credentials, CancellationToken cancellationToken = default)
        {
            Headers headers = GetAuthHeaders();
            
            Result<AccessTokenEntity> accessTokenEntity = 
                await RESTClient.PostAsync<AccessTokenEntity>(
                    url: AccessTokenEndpoint, 
                    headers: headers, 
                    payload: credentials, 
                    cancellationToken: cancellationToken);

            return accessTokenEntity;
        }

        private string GetBasicCredentialsHeader()
        {
            string base64Creds = GetBase64Credentials();
            return $"Basic {base64Creds}";
        }

        private string GetBase64Credentials()
        {
            string id = APISettings.Client.Id;
            string secret = APISettings.Client.Secret;

            string clientCreds = $"{id}:{secret}";
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(clientCreds);
            string base64 = Convert.ToBase64String(bytes);

            return base64;
        }

        private Headers GetAuthHeaders()
        {
            string basicCreds = GetBasicCredentialsHeader();
            
            Headers headers = new Headers()
            {
                {"Authorization", basicCreds},
                {"Content-Type", "application/x-www-form-urlencoded"}
            };

            return headers;
        }
    }
}
