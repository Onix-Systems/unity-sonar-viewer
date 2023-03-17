
using System;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using App.Infrastructure.Contexts;
using App.Infrastructure.CommonInterfaces;

using IDisposable = App.Infrastructure.CommonInterfaces.IDisposable;

namespace App.Services.ModelARViewing
{
    public class ARPlanesProvider: IInitializable, IDisposable
    {
        public int ARPlanesCount => _arPlanes.Count;

        private ARPlaneManager _arPlaneManager;
        private Dictionary<TrackableId, ARPlane> _arPlanes;

        public event Action<ARPlane> OnPlaneAdded;
        public event Action<ARPlane> OnPlaneRemoved;

        public ARPlanesProvider(ARPlaneManager arPlaneManager)
        {
            _arPlaneManager = arPlaneManager;
            _arPlanes = new Dictionary<TrackableId, ARPlane>();
        }

        public void Initialize()
        {
            IContext mainContext = MainContext.Instance;
            ARViewerConfig arConfig = mainContext.Get<AppConfig>().ARViewerConfig;

            if (arConfig.ShowARPlanes)
            {
                _arPlaneManager.planePrefab = arConfig.ARPlanePrefab;
            }
            else
            {
                _arPlaneManager.planePrefab = null;
            }

            _arPlaneManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
            _arPlanes.Clear();
            List<ARPlane> arPlanes = new List<ARPlane>();

            foreach (ARPlane arPlane in _arPlaneManager.trackables)
            {
                arPlanes.Add(arPlane);
            }

            UpdatePlanes(arPlanes);
            _arPlaneManager.planesChanged += OnARPlanesChangedCallback;
        }

        public void Dispose()
        {
            _arPlaneManager.planesChanged -= OnARPlanesChangedCallback;
            OnPlaneAdded = null;
            OnPlaneRemoved = null;
        }

        public void Enable()
        {
            if (!_arPlaneManager.enabled)
            {
                _arPlaneManager.enabled = true;
            }
        }

        public void Disable()
        {

            if (_arPlaneManager.enabled)
            {
                _arPlaneManager.enabled = false;
            }
        }

        public bool IsEnabled() => _arPlaneManager.enabled;

        public void ResetPlanes()
        {
            bool wasEnabled = false;

            if (IsEnabled())
            {
                wasEnabled = true;
                Disable();
            }

            if (wasEnabled)
            {
                Enable();
            }
        }

        private void UpdatePlanes(List<ARPlane> added, List<ARPlane> updated = null, List<ARPlane> removed = null)
        {
            if (removed != null && removed.Count > 0)
            {
                foreach (ARPlane plane in removed)
                {
                    TrackableId id = plane.trackableId;
                    RemoveARPlane(id);
                    DisablePlane(plane);
                }
            }

            if (added != null && added.Count > 0)
            {
                foreach (ARPlane plane in added)
                {
                    if (!IsPlaneValid(plane))
                    {
                        TrackableId id = plane.trackableId;
                        RemoveARPlane(id);
                        DisablePlane(plane);
                        continue;
                    }

                    EnablePlane(plane);
                    AddARPlane(plane.trackableId, plane);
                }
            }

            if (updated != null && updated.Count > 0)
            {
                foreach (ARPlane plane in updated)
                {
                    if (!IsPlaneValid(plane))
                    {
                        TrackableId id = plane.trackableId;
                        RemoveARPlane(id);
                        DisablePlane(plane);
                        continue;
                    }

                    EnablePlane(plane);
                    AddARPlane(plane.trackableId, plane);
                }
            }
        }


        private void AddARPlane(TrackableId trackableId, ARPlane arPlane)
        {
            if (_arPlanes.TryAdd(trackableId, arPlane))
            {
                OnPlaneAdded?.Invoke(arPlane);
            }
        }

        private void RemoveARPlane(TrackableId trackableId)
        {
            if (_arPlanes.TryGetValue(trackableId, out ARPlane arPlane))
            {
                _arPlanes.Remove(trackableId);
                OnPlaneRemoved?.Invoke(arPlane);
            }
        }

        private bool IsPlaneValid(ARPlane plane)
        {
            if (plane.alignment.IsVertical())
            {
                return false;
            }

            if (plane.trackingState != TrackingState.Tracking)
            {
                return false;
            }

            float planeSqr = (plane.extents.x * 2f) * (plane.extents.y * 2f);
            IContext mainContext = MainContext.Instance;
            ARViewerConfig config = mainContext.Get<AppConfig>().ARViewerConfig;

            if (planeSqr < config.ARPlaneMinSquareSize)
            {
                return false;
            }

            if (plane.classification == PlaneClassification.Window || 
                plane.classification == PlaneClassification.Ceiling || 
                plane.classification == PlaneClassification.Door || 
                plane.classification == PlaneClassification.Wall)
            {
                return false;
            }

            return true;
        }
                    
        private void EnablePlane(ARPlane plane)
        {
            if (!plane.gameObject.activeSelf)
            {
                plane.gameObject.SetActive(true);
            }
        }

        private void DisablePlane(ARPlane plane)
        {
            if (plane.gameObject.activeSelf)
            {
                plane.gameObject.SetActive(false);
            }
        }

        private void OnARPlanesChangedCallback(ARPlanesChangedEventArgs args)
        {
            UpdatePlanes(args.added, args.updated, args.removed);
        }
    }
}
