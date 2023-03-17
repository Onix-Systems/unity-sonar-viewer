
using App.Helpers;
using App.Infrastructure.CommonInterfaces;
using App.Infrastructure.Contexts;
using App.Services.Input.GestureDetectors.OneFingerSwipe;
using App.Services.Input.GestureDetectors.Pinch;
using App.Services.Input.GestureDetectors.Tap;
using App.Services.Input.GestureDetectors.TowFingerSwipe;

namespace App.Services.Input
{
    public class InputService : IInitializable, ITickable
    {
        public TapDetector TapDetector { get; private set; }
        public OneFingerSwipeDetector OneFingerSwipeDetector { get; private set; }
        public TwoFingerSwipeDetector TwoFingerSwipeDetector { get; private set; }
        public PinchDetector PinchDetector { get; private set; }

        public void Initialize()
        {
            InputConfig inputConfig = MainContext.Instance.Get<AppConfig>().InputConfig;

            TapDetector = new TapDetector(inputConfig);
            OneFingerSwipeDetector = new OneFingerSwipeDetector(inputConfig);
            TwoFingerSwipeDetector = new TwoFingerSwipeDetector(inputConfig);
            PinchDetector = new PinchDetector(inputConfig);
        }

        public void Tick()
        {
            if (AppHelpers.TouchOverUI(UnityEngine.Input.touches))
            {
                return;
            }

            TapDetector.Tick();
            OneFingerSwipeDetector.Tick();
            TwoFingerSwipeDetector.Tick();
            PinchDetector.Tick();
        }
    }
}
