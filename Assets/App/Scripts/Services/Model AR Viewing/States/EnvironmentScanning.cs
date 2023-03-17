
using App.Infrastructure.Contexts;

namespace App.Services.ModelARViewing.States
{
    public class EnvironmentScanning : IARViewerState
    {
        public void Enter()
        {
            IContext mainContext = MainContext.Instance;
            ARPlanesProvider arPlanesProvider = mainContext.Get<ARPlanesProvider>();
            ModelController modelController = mainContext.Get<ModelController>();
            
            modelController.DetachModelFromARPlane();
            modelController.SetModelVisible(false);

            arPlanesProvider.Enable();
        }
    }
}