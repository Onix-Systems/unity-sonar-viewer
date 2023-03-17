
using App.Infrastructure.Contexts;
using App.Infrastructure.StateMachine;

namespace App.Services.ModelARViewing.States
{
    public class NoModelState : IARViewerState, IState
    {
        public void Enter()
        {
            IContext mainContext = MainContext.Instance;
            ARPlanesProvider arPlaneProvider = mainContext.Get<ARPlanesProvider>();
            ModelController modelController = mainContext.Get<ModelController>();
            
            arPlaneProvider.Disable();
            arPlaneProvider.ResetPlanes();
            modelController.SetModelVisible(false);
            modelController.RemoveModel();
        }
    }
}