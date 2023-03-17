
using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using App.Helpers;
using App.Infrastructure.Contexts;
using UnityInput = UnityEngine.Input;

namespace App.Services.Input.GestureDetectors.OneFingerSwipe
{
    public class OneFingerSwipeDetector : IGestureDetector
    {
        public bool Detected { get; private set; }

        private SelectedObject _selectedObject;
        private bool _swipeBegan;
        private float _swipeDelta;

        public event Action<OneFingerSwipeEventArgs> OnSwipe;
        
        public OneFingerSwipeDetector(InputConfig inputConfig)
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
                        {
                            IContext mainContet = MainContext.Instance;
                            Camera camera = mainContet.Get<ARSessionOrigin>().camera;
                            LayerMask selectableLayers = mainContet.Get<AppConfig>().InputConfig.SelectableObjectLayers;

                            AppHelpers.TrySelectGO(touch.position, out _selectedObject, camera, selectableLayers);
                            _swipeBegan = true;
                        }
                        break;

                    case TouchPhase.Moved:
                        {
                            if (_swipeBegan && IsSwipe(touch, Time.deltaTime))
                            {
                                Detected = true;

                                OneFingerSwipeEventArgs oneFingerSwipeEventArgs = new OneFingerSwipeEventArgs()
                                {
                                    FingerPosition = touch.position,
                                    SelectedObject = _selectedObject,
                                    SwipeHorizontalValue = touch.deltaPosition.x,
                                    SwipeVerticalValue = touch.deltaPosition.y,
                                };

                                OnSwipe?.Invoke(oneFingerSwipeEventArgs);
                            }
                        }
                        break;

                    case TouchPhase.Canceled:
                    case TouchPhase.Ended:
                        _swipeBegan = false;
                        break;
                }
            }
            else
            {
                _swipeBegan = false;
            }
        }

        private bool IsSwipe(Touch touch, float deltaTime)
        {
            Vector2 deltaPosition = touch.deltaPosition;

            return deltaPosition.magnitude >= (_swipeDelta * deltaTime);
        }
    }
}
