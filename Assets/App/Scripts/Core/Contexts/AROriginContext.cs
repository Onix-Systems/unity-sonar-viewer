
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using App.Infrastructure.Contexts;
using App.Services.ModelARViewing;

namespace App.Contexts
{
    public class AROriginContext : Context
    {
        [field: SerializeField] public ModelController ModelController { get; private set; }
        [field: SerializeField] public ARSession ARSession { get; private set; }
        [field: SerializeField] public ARSessionOrigin ARSessionOrigin { get; private set; }
        [field: SerializeField] public ARPlaneManager ARPlaneManager { get; private set; }
        [field: SerializeField] public ARRaycastManager ARRaycastManager { get; private set; }
        [field: SerializeField] public ARAnchorManager ARAnchorManager { get; private set; }

        private ARPlanesProvider _arPlaneManager;

        protected override void SetupServices(out List<object> services)
        {
            services = new List<object>
            {
                ARSession,
                ARSessionOrigin,
                ARPlaneManager,
                ARRaycastManager,
                ARAnchorManager,
                ModelController,
                new ARViewer(),
                new ARViewerStateMachine()
            };

            _arPlaneManager = new ARPlanesProvider(ARPlaneManager);
            services.Add(_arPlaneManager);
        }
    }
}
