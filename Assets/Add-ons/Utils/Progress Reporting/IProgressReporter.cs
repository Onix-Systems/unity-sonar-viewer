
namespace Utils.ProgressReporting
{
    public interface IProgressReporter<T>
    {
        void Report(T progressInfo);
    }
}
