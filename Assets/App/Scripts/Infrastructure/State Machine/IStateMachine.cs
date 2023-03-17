

namespace App.Infrastructure.StateMachine
{
    public interface IStateMachine<TState> where TState: IState
    {
        TState CurrentState { get; }

        void SetState(TState state);
    }
}
