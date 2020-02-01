using System.Collections.Generic;

namespace Tetra.CommandSystem
{
    public sealed class Commander
    {
        public void Enqueue(IQueueable command) => queueables.Enqueue(command);
        public void SetQueue(Queue<IQueueable> queueables) => this.queueables = queueables != null ? queueables : this.queueables;

        public void Update()
        {
            while (queueables.Count > 0)
                queueables.Dequeue().Handle();
        }

        private Queue<IQueueable> queueables { get; set; } = new Queue<IQueueable>();

        public struct QueueableCommand<T> : IQueueable where T : struct, IRoutineHandleable
        {
            public void Handle() => commandable.Command(command);

            public ICommandable commandable { get; }
            public T command { get; }

            public QueueableCommand(ICommandable commandable, T command)
            {
                this.commandable = commandable;
                this.command = command;
            }
        }
    }
}

public interface IQueueable
{
    void Handle();
}