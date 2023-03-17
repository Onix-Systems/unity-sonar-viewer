
using App.Helpers;
using System;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace App.Services.Input.GestureDetectors.TowFingerSwipe
{
    public class TwoFingerSwipeDetector : IGestureDetector
    {
        public bool Detected { get; private set; }

        public event Action<TwoFingerSwipeEventArgs> OnSwipe;
        private float _swipeDelta;

        public TwoFingerSwipeDetector(InputConfig inputConfig)
        {
            _swipeDelta = AppHelpers.GetSmallestScreenSideSize() * inputConfig.RelativeSwipeDelta;
        }

        public void Tick()
        {
            Detected = false;

            if (UnityInput.touches.Length == 2)
            {
                Touch touch1 = UnityInput.touches[0];
                Touch touch2 = UnityInput.touches[1];

                if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
                {
                    if (IsSwipe(touch1, touch2, Time.deltaTime))
                    {
                        Detected = true;

                        TwoFingerSwipeEventArgs twoFingerSwipeEventArgs = new TwoFingerSwipeEventArgs()
                        {
                            Finger1Position = touch1.position,
                            Finger2Position = touch2.position,
                            SwipeHorizontalValue = (touch1.deltaPosition.x + touch2.deltaPosition.x) / 2f,
                            SwipeVerticalValue = (touch1.deltaPosition.y + touch2.deltaPosition.y) / 2f,
                        };

                        OnSwipe?.Invoke(twoFingerSwipeEventArgs);
                    }
                }
            }
        }

        private bool IsSwipe(Touch touch1, Touch touch2, float deltaTime)
        {
            float dot = Vector2.Dot(touch1.deltaPosition, touch2.deltaPosition);
            bool angleValid =  dot > 0;

            if (!angleValid)
            {
                return false;
            }

            bool swipe1Valid = touch1.position.magnitude >= (_swipeDelta * deltaTime);
            bool swipe2Valid = touch1.position.magnitude >= (_swipeDelta * deltaTime);

            return swipe1Valid && swipe2Valid;
        }
    }
}
