
namespace App.Services.Input.GestureDetectors
{
    public interface IGestureDetector
    {
        bool Detected { get; }
        void Tick();
    }
}
