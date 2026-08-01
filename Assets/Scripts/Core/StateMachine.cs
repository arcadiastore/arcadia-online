namespace ArcadiaOnline.Core
{
    /// <summary>
    /// State machine generik untuk AI monster dan game state.
    /// Lihat docs/02_TDD/ScriptArchitecture.md - State Machine.
    /// </summary>
    public interface IState
    {
        void Enter();
        void Update();
        void Exit();
    }

    public class StateMachine
    {
        public IState CurrentState { get; private set; }

        public void ChangeState(IState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState?.Enter();
        }

        public void Update() => CurrentState?.Update();
    }
}
