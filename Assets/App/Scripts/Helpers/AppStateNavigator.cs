
using App.Infrastructure.Contexts;
using App.Infrastructure.StateMachine;
using App.Core.Factories;
using App.Services.AppFSM;

namespace App.Helpers
{
    public static class AppStateNavigator 
    {
        public static void GoTo<TState>() where TState : class, IAppState, new()
        {
            IStateMachine<IAppState> stateMachine = MainContext.Instance.Get<AppStateMachine>();
            IStateFactory stateFactory = MainContext.Instance.Get<AppStateFactory>();

            TState state = stateFactory.Create<TState>();
            stateMachine.SetState(state);
        }
    }
}