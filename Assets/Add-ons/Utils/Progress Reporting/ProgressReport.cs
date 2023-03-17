
namespace Utils.ProgressReporting
{
    public class ProgressReport<T> where T: ProgressInfo
    {
        public float ProgressValue { get; set; }
        public T ProgressInfo { get; set; }
    }
}
