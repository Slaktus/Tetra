namespace Tetra.CommandSystem
{
    public abstract class Routine<T,Y> : Routine where T : ICommandable where Y : struct, IRoutineHandleable
    {
        public virtual void Update()
        {
            if (state == State.Running)
                Running();
        }

        public void Handle(Y command)
        {
            switch (command.state)
            {
                case State.Begin:
                    state = State.Running;
                    Begin(command);
                    break;

                case State.End:
                    state = State.None;
                    End(command);
                    break;
            }
        }

        protected abstract void Begin(Y command);
        protected abstract void End(Y command);
        protected abstract void Running();
        public abstract void Clear();

        protected T client { get; }

        public Routine(T client) => this.client = client;
    }

    public abstract class Routine
    {
        public State state { get; protected set; }

        public enum State
        {
            None = 0,
            Begin = 1,
            Running = 2,
            End = 3
        }
    }

    public interface IRoutineHandleable
    {
        Routine.State state { get; }
    }

    public interface ICommandable
    {
        int id { get; }

        void Command<T>(T command) where T : struct, IRoutineHandleable;
    }
}