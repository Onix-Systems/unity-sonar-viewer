
using App.Helpers;
using System;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace App.Services.Input.GestureDetectors.Pinch
{
    public class PinchDetector : IGestureDetector
    {
        public bool Detected { get; private set;}

        private float _pinchDistance;
        private bool _pinchBegan;
        private float _swipeDelta;

        public event Action<PinchEventArgs> OnPinch;

        public PinchDetector(InputConfig inputConfig)
        {
            _swipeDelta = AppHelpers.GetSmallestScreenSideSize() * inputConfig.RelativeSwipeDelta;
        }

        public void Tick()
        {
            Detected = false;

            if (UnityInput.touches.Length != 2)
            {
                _pinchBegan = false;
                return;
            }

            Touch touch1 = UnityInput.touches[0];
            Touch touch2 = UnityInput.touches[1];

            if (touch1.phase != TouchPhase.Moved || touch2.phase != TouchPhase.Moved)
            {
                _pinchBegan = false;
                return;
            }

            if (!_pinchBegan && IsPinch(touch1, touch2, Time.deltaTime))
            {
                _pinchBegan = true;
                _pinchDistance = GetTouchesDistance(touch1, touch2);
            }
            else if (_pinchBegan && IsPinch(touch1, touch2, Time.deltaTime))
            {
                Detected = true;
                float newPinchDistance = GetTouchesDistance(touch1, touch2);
                float pinchDelta = (newPinchDistance - _pinchDistance);
                _pinchDistance = newPinchDistance;

                PinchEventArgs twoFingerSwipeEventArgs = new PinchEventArgs()
                {
                    Finger1Position = touch1.position,
                    Finger2Position = touch2.position,
                    PinchValue = pinchDelta
                };

                OnPinch?.Invoke(twoFingerSwipeEventArgs);
            }
            else
            {
                _pinchBegan = false;
            }
        }

        private bool IsPinch(Touch touch1, Touch touch2, float deltaTime)
        {
            if (!IsSwipe(touch1, deltaTime) && !IsSwipe(touch2, deltaTime))
            {
                return false;
            }

            float dot = Vector2.Dot(touch1.deltaPosition, touch2.deltaPosition);

            return dot < 0;
        }

        private bool IsSwipe(Touch touch, float deltaTime)
        {
            Vector2 deltaPosition = touch.deltaPosition;

            return deltaPosition.magnitude >= (_swipeDelta * deltaTime);
        }

        private float GetTouchesDistance(Touch touch1, Touch touch2)
        {
            float distance = (touch2.position - touch1.position).magnitude;
            return distance;
        }
    }
}
