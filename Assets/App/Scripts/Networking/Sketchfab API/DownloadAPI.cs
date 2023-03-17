
using System;
using System.Threading;
using System.Threading.Tasks;
using RestHTTP;

namespace SketchfabAPI
{
    public class DownloadAPI : SketchfabAPIClientBase
    {
        public const string DownlodEndpoint = Constants.BaseURL + "/v3/models/{0}/download";

        public async Task<Result<ArchivesEntity>> GetArchiveLinksAsync(
            string modelId, 
            CancellationToken cancellationToken = default)
        {
            Headers headers = new Headers()
            {
                { "authorization", AccessToken },
            };

            string url = string.Format(DownlodEndpoint, modelId);

            Result<ArchivesEntity> response = await RESTClient.GetAsync<ArchivesEntity>(
                   url: url, 
                   headers: headers, 
                   cancellationToken: cancellationToken);

            return response;
        }

        public async Task<Result<byte[]>> DownloadArchiveAsync(
            string url, 
            Action<RequestProgressData> onProgress, 
            CancellationToken cancellationToken)
        {
            Result<byte[]> response = await RESTClient.GetAsync<byte[]>(
                url: url, 
                onProgress: onProgress, 
                timeout: RESTClient.NoTimeout, 
                cancellationToken: cancellationToken);

            return response;
        }
    }
}