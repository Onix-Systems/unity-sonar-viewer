
using App.Infrastructure.CommonInterfaces;
using App.Infrastructure.StateMachine;
using App.Services.AppFSM;
using UnityEngine;

namespace App.Services.AppFSM
{
    public class AppStateMachine: IStateMachine<IAppState>, ITickable
    {
        public IAppState CurrentState { get; private set; }

        public AppStateMachine()
        {
            Application.wantsToQuit += () =>
            {
                (CurrentState as IExitable)?.Exit();
                return true;
            };
        }

        public void SetState(IAppState state)
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
