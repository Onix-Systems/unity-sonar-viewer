
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using App.Helpers;
using App.Infrastructure.CommonInterfaces;
using App.Infrastructure.Contexts;
using App.Services.Input;
using App.Services.Input.GestureDetectors.TowFingerSwipe;

namespace App.Services.ModelARViewing.States
{
    public class ModelPositioningWithScreenCenter : IARViewerState, ITickable, IExitable
    {
        public void Enter()
        {
            IContext mainContext = MainContext.Instance;
            InputService inputService = mainContext.Get<InputService>();
            ARViewer arViewer = mainContext.Get<ARViewer>();
            ModelController modelController = mainContext.Get<ModelController>();

            modelController.SetModel(arViewer.Model);
            modelController.DetachModelFromARPlane();
            modelController.SetModelVisible(true);

            inputService.TwoFingerSwipeDetector.OnSwipe += OnTwoFingerSwipe;
        }

        public void Exit()
        {
            IContext mainContext = MainContext.Instance;
            InputService inputService = mainContext.Get<InputService>();

            inputService.TwoFingerSwipeDetector.OnSwipe -= OnTwoFingerSwipe;
        }

        public void Tick()
        {
            DoPositioning();
        }

        private void DoPositioning()
        {
            IContext mainContext = MainContext.Instance;
            ARViewer arViewer = mainContext.Get<ARViewer>();
            ARRaycastManager arRaycastManager = mainContext.Get<ARRaycastManager>();

            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            Vector2 screenCenter = AppHelpers.ScreenCenter;

            if (arRaycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                int hitsCount = hits.Count;

                for (int i = 0; i < hitsCount; ++i)
                {
                    ARRaycastHit hit = hits[i];
                    ARPlane arPalne = hit.trackable as ARPlane;

                    if (arPalne != null && arPalne.isActiveAndEnabled)
                    {
                        Pose targetPose = hit.pose;
                        ModelController modelController = mainContext.Get<ModelController>();
                        modelController.SetRootPosition(targetPose.position, true);
                        modelController.SetRootRotation(targetPose.rotation, true);
                        arViewer.TargetARPlane = arPalne;
                        break;
                    }
                }
            }
        }

        private void OnTwoFingerSwipe(TwoFingerSwipeEventArgs e)
        {
            bool isHorizontal = Mathf.Abs(e.SwipeHorizontalValue) > Mathf.Abs(e.SwipeVerticalValue);

            if (isHorizontal)
            {
                IContext mainContext = MainContext.Instance;
                ARViewerConfig arViewerConfig = mainContext.Get<AppConfig>().ARViewerConfig;
                ModelController modelController = mainContext.Get<ModelController>();

                float angle = e.SwipeHorizontalValue * arViewerConfig.ModelRotationSwipeFactor;
                Quaternion rotationOffset = Quaternion.Euler(0, angle, 0);
                Transform modelGOTransform = modelController.Model.Rig.transform;
                Quaternion modelLocalRotation = modelGOTransform.localRotation;
                modelController.SetModelRotation(modelLocalRotation * rotationOffset, true);
            }
        }
    }
}