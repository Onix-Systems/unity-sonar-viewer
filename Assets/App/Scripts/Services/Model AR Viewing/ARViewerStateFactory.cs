
using App.Infrastructure.StateMachine;

namespace App.Services.ModelARViewing
{
    public class ARViewerStateFactory : IStateFactory
    {
        public TState Create<TState>() where TState : class, IState, new()
        {
            TState arViewrState = new TState();
            return arViewrState;
        }
    }
}