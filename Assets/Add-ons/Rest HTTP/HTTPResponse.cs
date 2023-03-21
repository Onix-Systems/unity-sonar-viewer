
namespace RestHTTP
{
    public  class HTTPResponse
    {
        public long StatusCode { get; private set; }
        public byte[] Data { get; private set; }
        public string Error { get; private set; }
        public bool IsSuccess { get; private set; }
        public bool RequestAborted { get; private set; }

        public HTTPResponse(long statusCode, byte[] data, string error, bool success, bool requestAborted = false)
        {
            StatusCode = statusCode;
            Data = data;
            Error = error;
            IsSuccess = success;
            RequestAborted = requestAborted;
        }
    }
}
