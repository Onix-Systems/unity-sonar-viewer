
using App.Infrastructure.StateMachine;

namespace App.Core.Factories
{
    public class AppStateFactory : IStateFactory
    {
        public TState Create<TState>() where TState : class, IState, new()
        {
            return new TState();
        }
    }
}
