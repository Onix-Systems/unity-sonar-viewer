
namespace App.Infrastructure.StateMachine
{
    public interface IStateFactory
    {
        TState Create<TState>() where TState : class, IState, new();
    }
}
