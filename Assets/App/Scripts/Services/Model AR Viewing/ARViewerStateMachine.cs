
using App.Infrastructure.CommonInterfaces;
using App.Infrastructure.StateMachine;
using App.Services.Input;
using App.Services.ModelARViewing.States;

namespace App.Services.ModelARViewing
{
    public class ARViewerStateMachine : IStateMachine<IARViewerState>, ITickable
    {
        public IARViewerState CurrentState { get; private set; }

        public void SetState(IARViewerState state)
        {
            if (CurrentState == null)
            {
                CurrentState = state;
                CurrentState.Enter();
                return;
            }

            if (CurrentState == state)
            {
                return;
            }

            (CurrentState as IExitable)?.Exit();
            CurrentState = state;
            CurrentState.Enter();
        }

        public void Tick()
        {
            if (CurrentState == null)
            {
                return;
            }

            (CurrentState as ITickable)?.Tick();
        }
    }
}