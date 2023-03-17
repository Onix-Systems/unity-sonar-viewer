
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestHTTP
{
    public class RESTClient
    {
        public static readonly TimeSpan NoTimeout = TimeSpan.Zero;

        private NetworkSettings _networkSettings;

        public RESTClient(NetworkSettings networkSettings)
        {
            _networkSettings = networkSettings;
        }

        public async Task<Result> PostAsync(
            string url,
            Headers headers = null,
            object parameters = null,
            object payload = null,
            Action<RequestProgressData> onProgress = null,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            HTTPRequest httpRequest = new HTTPRequest(url, HTTPMethod.Post);

            httpRequest.SetHeaders(headers);
            httpRequest.SetParameters(parameters);
            httpRequest.SetPayload(payload);
            
            if (timeout.HasValue)
            {
                httpRequest.SetTimeout(timeout.Value);
            }
            else
            {
                httpRequest.SetTimeout(TimeSpan.FromSeconds(_networkSettings.DefaultRequestTimeoutSeconds));
            }

            httpRequest.OnDownloadProgress += onProgress;

            Result result = await SendAsync(httpRequest, cancellationToken);
            return result;
        }

        public async Task<Result<TResult>> PostAsync<TResult>(
            string url,
            Headers headers = null,
            object parameters = null,
            object payload = null,
            Action<RequestProgressData> onProgress = null,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default) where TResult : class
        {
            HTTPRequest httpRequest = new HTTPRequest(url, HTTPMethod.Post);

            httpRequest.SetHeaders(headers);
            httpRequest.SetParameters(parameters);
            httpRequest.SetPayload(payload);

            if (timeout.HasValue)
            {
                httpRequest.SetTimeout(timeout.Value);
            }
            else
            {
                httpRequest.SetTimeout(TimeSpan.FromSeconds(_networkSettings.DefaultRequestTimeoutSeconds));
            }

            httpRequest.OnDownloadProgress += onProgress;

            Result<TResult> result = await SendAsync<TResult>(httpRequest, cancellationToken);
            return result;
        }

        public async Task<Result> GetAsync(
            string url,
            Headers headers = null,
            object parameters = null,
            object payload = null,
            Action<RequestProgressData> onProgress = null,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            HTTPRequest httpRequest = new HTTPRequest(url, HTTPMethod.Get);

            httpRequest.SetHeaders(headers);
            httpRequest.SetParameters(parameters);
            httpRequest.SetPayload(payload);

            if (timeout.HasValue)
            {
                httpRequest.SetTimeout(timeout.Value);
            }
            else
            {
                httpRequest.SetTimeout(TimeSpan.FromSeconds(_networkSettings.DefaultRequestTimeoutSeconds));
            }

            httpRequest.OnDownloadProgress += onProgress;

            Result result = await SendAsync(httpRequest, cancellationToken);
            return result;
        }

        public async Task<Result<TResult>> GetAsync<TResult>(
            string url,
            Headers headers = null,
            object parameters = null,
            object payload = null,
            Action<RequestProgressData> onProgress = null,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default) where TResult : class
        {
            HTTPRequest httpRequest = new HTTPRequest(url, HTTPMethod.Get);

            httpRequest.SetHeaders(headers);
            httpRequest.SetParameters(parameters);
            httpRequest.SetPayload(payload);

            if (timeout.HasValue)
            {
                httpRequest.SetTimeout(timeout.Value);
            }
            else
            {
                httpRequest.SetTimeout(TimeSpan.FromSeconds(_networkSettings.DefaultRequestTimeoutSeconds));
            }

            httpRequest.OnDownloadProgress += onProgress;

            Result<TResult> result = await SendAsync<TResult>(httpRequest, cancellationToken);
            return result;
        }

        public async Task<Result> PutAsync(
            string url,
            Headers headers = null,
            object parameters = null,
            object payload = null,
            Action<RequestProgressData> onProgress = null,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            HTTPRequest httpRequest = new HTTPRequest(url, HTTPMethod.Put);

            httpRequest.SetHeaders(headers);
            httpRequest.SetParameters(parameters);
            httpRequest.SetPayload(payload);

            if (timeout.HasValue)
            {
                httpRequest.SetTimeout(timeout.Value);
            }
            else
            {
                httpRequest.SetTimeout(TimeSpan.FromSeconds(_networkSettings.DefaultRequestTimeoutSeconds));
            }

            httpRequest.OnDownloadProgress += onProgress;

            Result result = await SendAsync(httpRequest, cancellationToken);
            return result;
        }

        public async Task<Result<TResult>> PutAsync<TResult>(
            string url,
            Headers headers = null,
            object parameters = null,
            object payload = null,
            Action<RequestProgressData> onProgress = null,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default) where TResult : class
        {
            HTTPRequest httpRequest = new HTTPRequest(url, HTTPMethod.Put);

            httpRequest.SetHeaders(headers);
            httpRequest.SetParameters(parameters);
            httpRequest.SetPayload(payload);

            if (timeout.HasValue)
            {
                httpRequest.SetTimeout(timeout.Value);
            }
            else
            {
                httpRequest.SetTimeout(TimeSpan.FromSeconds(_networkSettings.DefaultRequestTimeoutSeconds));
            }

            httpRequest.OnDownloadProgress += onProgress;

            Result<TResult> result = await SendAsync<TResult>(httpRequest, cancellationToken);
            return result;
        }

        public async Task<Result> DeleteAsync(
            string url,
            Headers headers = null,
            object parameters = null,
            object payload = null,
            Action<RequestProgressData> onProgress = null,
            TimeSpan? timeout = null,
            CancellationToken token = default)
        {
            HTTPRequest httpRequest = new HTTPRequest(url, HTTPMethod.Delete);

            httpRequest.SetHeaders(headers);
            httpRequest.SetParameters(parameters);
            httpRequest.SetPayload(payload);

            if (timeout.HasValue)
            {
                httpRequest.SetTimeout(timeout.Value);
            }
            else
            {
                httpRequest.SetTimeout(TimeSpan.FromSeconds(_networkSettings.DefaultRequestTimeoutSeconds));
            }

            httpRequest.OnDownloadProgress += onProgress;

            Result result = await SendAsync(httpRequest, token);
            return result;
        }

        public async Task<Result<TResult>> DeleteAsync<TResult>(
            string url,
            Headers headers = null,
            object parameters = null,
            object payload = null,
            Action<RequestProgressData> onProgress = null,
            TimeSpan? timeout = null,
            CancellationToken token = default) where TResult : class
        {
            HTTPRequest httpRequest = new HTTPRequest(url, HTTPMethod.Delete);

            httpRequest.SetHeaders(headers);
            httpRequest.SetParameters(parameters);
            httpRequest.SetPayload(payload);

            if (timeout.HasValue)
            {
                httpRequest.SetTimeout(timeout.Value);
            }
            else
            {
                httpRequest.SetTimeout(TimeSpan.FromSeconds(_networkSettings.DefaultRequestTimeoutSeconds));
            }

            httpRequest.OnDownloadProgress += onProgress;

            Result<TResult> result = await SendAsync<TResult>(httpRequest, token);
            return result;
        }

        private async Task<Result> SendAsync(HTTPRequest httpRequest, CancellationToken token)
        {
            Result result = null;
            try
            {
                HTTPResponse httpResponse = await httpRequest.SendAsync(token);
                result = HTTPResponseProcessor.GetResult(httpResponse);
            }
            catch (Exception exp)
            {
                result = new Result()
                {
                    IsSuccess = false,
                    Message = exp.Message
                };
            }
            finally
            {
                httpRequest?.Dispose();
            }

            return result;
        }

        private async Task<Result<TResult>> SendAsync<TResult>(HTTPRequest httpRequest, CancellationToken token) where TResult : class
        {
            Result<TResult> result = null;

            try
            {
                HTTPResponse httpResponse = await httpRequest.SendAsync(token);
                result = HTTPResponseProcessor.GetResult<TResult>(httpResponse);
            }
            catch (Exception exp)
            {
                result = new Result<TResult>()
                {
                    IsSuccess = false,
                    Message = exp.Message
                };
            }
            finally
            {
                httpRequest?.Dispose();
            }

            return result;
        }
    }
}