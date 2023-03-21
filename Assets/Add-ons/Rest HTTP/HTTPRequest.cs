
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace RestHTTP
{
    public class HTTPRequest: IDisposable
    {
        private float ProgressCompletedValue = 1f;
        private int ProgressCheckIntervalMS = 25;

        public string Url => _url;

        private HTTPMethod _httpMethod;
        private TimeSpan _timeout = TimeSpan.Zero;
        private Headers _headers;
        private Dictionary<string, string> _parameters;
        private Dictionary<string, string> _fields;
        private string _url;
        private UnityWebRequest _unityWebRequest;


        public event Action<RequestProgressData> OnDownloadProgress;

        public HTTPRequest(string url, HTTPMethod method)
        {
            _headers = new Headers();
            _parameters = new Dictionary<string, string>();
            _fields = new Dictionary<string, string>();
            _url = url;

            _httpMethod = method;
        }

        public void SetTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
        }

        public void SetHeaders(Headers headers)
        {
            if (headers == null)
            {
                _headers.Clear();
                return;
            }

            _headers = headers;
        }

        public void SetParameters(object parameters)
        {
            if (parameters == null)
            {
                _parameters.Clear();
                return;
            }

            _parameters = ObjectToDictionary(parameters);
        }

        public void SetPayload(object payload)
        {
            if (payload == null)
            {
                _fields.Clear();
                return;
            }

            _fields = ObjectToDictionary(payload);
        }

        public void Abort()
        {
            if (_unityWebRequest != null && !_unityWebRequest.isDone)
            {
                _unityWebRequest.Abort();
            }
        }

        public void Dispose()
        {
            OnDownloadProgress = null;
            _unityWebRequest?.Dispose();
            _unityWebRequest = null;
        }

        public async Task<HTTPResponse> SendAsync(CancellationToken token = default)
        {
            _unityWebRequest = CreateUnityWebRequest();
            DownloadHandler downloadHandler = new DownloadHandlerBuffer();

            _unityWebRequest.downloadHandler = downloadHandler;
            UnityWebRequestAsyncOperation webAsyncOperation = _unityWebRequest.SendWebRequest();
            bool aborted = false;

            while (true)
            {
                if (webAsyncOperation.isDone)
                {
                    break;
                }

                if (token.IsCancellationRequested)
                {
                    Abort();
                    aborted = true;
                }

                OnDownloadProgress?.Invoke(new RequestProgressData()
                {
                    Progress = _unityWebRequest.downloadProgress
                });

                await Task.Delay(ProgressCheckIntervalMS);
            }

            OnDownloadProgress?.Invoke(new RequestProgressData()
            {
                Progress = ProgressCompletedValue
            });

            HTTPResponse response = new HTTPResponse(
                _unityWebRequest.responseCode,
                downloadHandler.data,
                _unityWebRequest.error,
                _unityWebRequest.result == UnityWebRequest.Result.Success, 
                aborted);

            return response;
        }

        private UnityWebRequest CreateUnityWebRequest()
        {
            UnityWebRequest unityWebRequest = new UnityWebRequest();
            unityWebRequest.timeout = Mathf.RoundToInt((float)_timeout.TotalSeconds);
            unityWebRequest.disposeCertificateHandlerOnDispose = true;
            unityWebRequest.disposeDownloadHandlerOnDispose = true;
            unityWebRequest.disposeUploadHandlerOnDispose = true;

            SetupMethodType(unityWebRequest);
            SetupRequestHeaders(unityWebRequest);
            SetupRequestUri(unityWebRequest);
            SetupRequestFields(unityWebRequest);

            return unityWebRequest;
        }

        private void SetupRequestHeaders(UnityWebRequest unityWebRequest)
        {
            foreach (KeyValuePair<string, string> header in _headers)
            {
                unityWebRequest.SetRequestHeader(header.Key, header.Value);
            }
        }

        private void SetupRequestUri(UnityWebRequest unityWebRequest)
        {
            string url = Url;
            int paramsCount = _parameters.Count;

            if (paramsCount < 1)
            {
                if (!url.EndsWith("/") && !url.Contains("?"))
                {
                    url += "/";
                }

                unityWebRequest.uri = new Uri(url);
                return;
            }

            url += "?";

            var keys = _parameters.Keys;
            int iParam = 0;

            foreach (string key in keys)
            {
                string value = _parameters[key];
                url += $"{key}={value}";

                if (++iParam < paramsCount)
                {
                    url += "&";
                }
            }

            unityWebRequest.uri = new Uri(url);
        }
        
        private void SetupMethodType(UnityWebRequest unityWebRequest)
        {
            switch (_httpMethod)
            {
                case HTTPMethod.Post:
                    unityWebRequest.method = UnityWebRequest.kHttpVerbPOST;
                    break;

                case HTTPMethod.Get:
                    unityWebRequest.method = UnityWebRequest.kHttpVerbGET;
                    break;

                case HTTPMethod.Put:
                    unityWebRequest.method = UnityWebRequest.kHttpVerbPUT;
                    break;

                case HTTPMethod.Delete:
                    unityWebRequest.method = UnityWebRequest.kHttpVerbDELETE;
                    break;
            }
        }

        private void SetupRequestFields(UnityWebRequest unityWebRequest)
        {
            if (_fields.Count < 1)
            {
                return;
            }

            WWWForm form = new WWWForm();
            var keys = _fields.Keys;
            
            foreach (string key in keys)
            {
                string value = _fields[key];
                form.AddField(key, value, Encoding.UTF8);
            }

            byte[] data = form.data;
            unityWebRequest.uploadHandler = new UploadHandlerRaw(data);
        }

        private Dictionary<string, string> ObjectToDictionary(object obj)
        {
            Dictionary<string, string> fields;

            try
            {
                string json = JsonConvert.SerializeObject(obj);
                fields = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch (Exception exp)
            {
                fields = new Dictionary<string, string>();
                Debug.LogException(exp);
            }

            return fields;
        }
    }
}