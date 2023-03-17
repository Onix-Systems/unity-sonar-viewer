
using System;
using UnityEngine.XR.ARFoundation;
using App.Infrastructure.CommonInterfaces;
using App.Services.ModelARViewing.States;
using App.Infrastructure.Contexts;

using IDisposable = App.Infrastructure.CommonInterfaces.IDisposable;

namespace App.Services.ModelARViewing
{
    public class ARViewer: IInitializable, IDisposable
    {
        public ModelObject Model { get; private set; }
        public ARPlane TargetARPlane { get; set; }
        public IARViewerState State => MainContext.Instance.Get<ARViewerStateMachine>().CurrentState;
        public bool ARPlanesDetected => MainContext.Instance.Get<ARPlanesProvider>().ARPlanesCount > 0;

        private ARViewerStateFactory _stateFactory;

        public event Action OnTargetARPlaneRemoved;

        public ARViewer()
        {
            _stateFactory = new ARViewerStateFactory();
        }

        public void Initialize()
        {
            ResetViewer();
            IContext mainContext = MainContext.Instance;
            ARPlanesProvider arPlanesProvider = mainContext.Get<ARPlanesProvider>();
            arPlanesProvider.OnPlaneRemoved += OnPlaneRemoved;
        }

        public void Dispose()
        {
            IContext mainContext = MainContext.Instance;
            
            if (mainContext.TryGet(out ARPlanesProvider arPlanesProvider))
            {
                arPlanesProvider.OnPlaneRemoved -= OnPlaneRemoved;
            }
        }

        public void ResetViewer()
        {
            IContext mainContext = MainContext.Instance;
            ARViewerStateMachine fsm = mainContext.Get<ARViewerStateMachine>();
            IARViewerState state = _stateFactory.Create<NoModelState>();
            fsm.SetState(state);
            Model = null;

            mainContext.Get<ARSession>().Reset();
        }

        public void SetEnvironmentScanning()
        {
            IContext mainContext = MainContext.Instance;
            ARViewerStateMachine fsm = mainContext.Get<ARViewerStateMachine>();
            IARViewerState state = _stateFactory.Create<EnvironmentScanning>();
            
            fsm.SetState(state);
        }

        public void SetModelPositioning(ModelObject model)
        {
            IContext mainContext = MainContext.Instance;
            ARViewerStateMachine fsm = mainContext.Get<ARViewerStateMachine>();

            Model = model;
            IARViewerState state = _stateFactory.Create<ModelPositioningWithScreenCenter>();
            fsm.SetState(state);
        }

        public void SetModelTransfromEdit(ModelObject model)
        {
            IContext mainContext = MainContext.Instance;
            ARViewerStateMachine fsm = mainContext.Get<ARViewerStateMachine>();

            Model = model;
            IARViewerState state = _stateFactory.Create<ModelTransformEditing>();
            fsm.SetState(state);
        }

        private void OnPlaneRemoved(ARPlane arPlane)
        {
            if (arPlane == TargetARPlane)
            {
                OnTargetARPlaneRemoved?.Invoke();
                TargetARPlane = null;
            }
        }
    }
}