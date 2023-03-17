
namespace RestHTTP
{
    public class Result<TEntity>
    {
        public TEntity Entity { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public long StatusCode { get; set; }
    }

    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public long StatusCode { get; set; }
    }
}