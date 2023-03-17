
using App.Helpers;
using System;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace App.Services.Input.GestureDetectors.Tap
{
    public class TapDetector : IGestureDetector
    {
        public bool Detected { get; private set; }

        private bool _tapBegan;
        private float _swipeDelta;

        public event Action<TapEventArgs> OnTap;

        public TapDetector(InputConfig inputConfig)
        {
            _swipeDelta = AppHelpers.GetSmallestScreenSideSize() * inputConfig.RelativeSwipeDelta;
        }

        public void Tick()
        {
            Detected = false;

            if (UnityInput.touches.Length == 1)
            {
                Touch touch = UnityInput.touches[0];

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        _tapBegan = true;
                        break;

                    case TouchPhase.Ended:

                        if (_tapBegan)
                        {
                            Detected = true;

                            TapEventArgs tapEventArgs = new TapEventArgs()
                            {
                                TapPosition = touch.position
                            };

                            _tapBegan = false;
                            OnTap?.Invoke(tapEventArgs);
                        }

                        break;

                    case TouchPhase.Moved:
                        _tapBegan = !IsSwipe(touch, Time.deltaTime);
                        break;

                    case TouchPhase.Canceled:
                        _tapBegan = false;
                        break;
                }
            }
            else
            {
                _tapBegan = false;
            }
        }

        private bool IsSwipe(Touch touch, float deltaTime)
        {
            Vector2 deltaPosition = touch.deltaPosition;

            return deltaPosition.magnitude >= (_swipeDelta * deltaTime);
        }
    }
}
