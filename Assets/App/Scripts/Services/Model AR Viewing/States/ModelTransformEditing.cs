
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using App.Infrastructure.CommonInterfaces;
using App.Infrastructure.Contexts;
using App.Infrastructure.StateMachine;
using App.Services.Input;
using App.Services.Input.GestureDetectors.OneFingerSwipe;
using App.Services.Input.GestureDetectors.TowFingerSwipe;
using App.Services.Input.GestureDetectors.Pinch;

namespace App.Services.ModelARViewing.States
{
    public class ModelTransformEditing : IARViewerState, IState, IExitable
    {
        public void Enter()
        {
            IContext mainContext = MainContext.Instance;
            InputService inputService = mainContext.Get<InputService>();
            ModelController modelController = mainContext.Get<ModelController>();
            ARViewer arViewer = mainContext.Get<ARViewer>();

            inputService.PinchDetector.OnPinch += OnPinch;
            inputService.OneFingerSwipeDetector.OnSwipe += OnOneFingerSwipe;
            inputService.TwoFingerSwipeDetector.OnSwipe += OnTwoFingerSwipe;
            modelController.AttachModelToARPlane(arViewer.TargetARPlane);
        }

        public void Exit()
        {
            IContext mainContext = MainContext.Instance;
            InputService inputService = mainContext.Get<InputService>();
            
            inputService.PinchDetector.OnPinch -= OnPinch;
            inputService.OneFingerSwipeDetector.OnSwipe -= OnOneFingerSwipe;
            inputService.TwoFingerSwipeDetector.OnSwipe -= OnTwoFingerSwipe;
        }

        private void UpdateModelPosition(Vector2 touchPosition, SelectedObject selectedObject)
        {
            IContext mainContext = MainContext.Instance;
            ModelController modelController = mainContext.Get<ModelController>();
            ARRaycastManager arRaycastManager = mainContext.Get<ARRaycastManager>();
            ARViewer arViewer = mainContext.Get<ARViewer>();

            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            Vector2 objectTouchPosition = touchPosition + selectedObject.TouchOffsetToObjectPivot;

            if (arRaycastManager.Raycast(objectTouchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                int hitsCount = hits.Count;

                for (int i = 0; i < hitsCount; ++i)
                {
                    ARRaycastHit hit = hits[i];
                    ARPlane arPlane = hit.trackable as ARPlane;

                    if (arPlane != arViewer.TargetARPlane)
                    {
                        continue;
                    }

                    Pose targetPose = hit.pose;
                    modelController.DetachModelFromARPlane();
                    Vector3 objectPosition = targetPose.position;

                    modelController.SetRootPosition(objectPosition, true, () =>
                    {
                        modelController.AttachModelToARPlane(arViewer.TargetARPlane);
                    });

                    break;
                }
            }
        }

        private void UpdateModelRotation(float swipeValue)
        {
            IContext mainContext = MainContext.Instance;
            ModelController modelController = mainContext.Get<ModelController>();
            ARViewerConfig arViewerConfig = mainContext.Get<AppConfig>().ARViewerConfig;

            float angle = swipeValue * arViewerConfig.ModelRotationSwipeFactor;
            Quaternion rotationOffset = Quaternion.Euler(0, angle, 0);
            Transform modelGOTransform = modelController.Model.Rig.transform;
            Quaternion modelLocalRotation = modelGOTransform.localRotation;
            modelController.SetModelRotation(modelLocalRotation * rotationOffset, true);
        }

        private void OnOneFingerSwipe(OneFingerSwipeEventArgs e)
        {
            IContext mainContext = MainContext.Instance;
            ModelController modelController = mainContext.Get<ModelController>();

            SelectedObject selectedObject = e.SelectedObject;
            bool isObjectSelected = (selectedObject != null) && (selectedObject.GameObject == modelController.Model.Rig);

            if (isObjectSelected)
            {
                UpdateModelPosition(e.FingerPosition, e.SelectedObject);
            }
        }

        private void OnTwoFingerSwipe(TwoFingerSwipeEventArgs e)
        {
            float hValue = e.SwipeHorizontalValue;
            float vValue = e.SwipeVerticalValue;
            bool isHorizontal = Mathf.Abs(hValue) > Mathf.Abs(vValue);

            if (isHorizontal)
            {
                UpdateModelRotation(hValue);
            }
        }

        private void OnPinch(PinchEventArgs e)
        {
            IContext mainContext = MainContext.Instance;
            ModelController modelController = mainContext.Get<ModelController>();
            ARViewerConfig arViewerConfig = mainContext.Get<AppConfig>().ARViewerConfig;

            float scale = modelController.RootScale + (e.PinchValue * arViewerConfig.ModelScalePinchFactor);
            modelController.SetRootScale(scale, true);
        }
    }
}