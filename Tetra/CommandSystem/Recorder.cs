using System.Collections.Generic;
using Tetra.EntityManagement;
using UnityEngine;

namespace Tetra.CommandSystem
{
    public abstract class Recorder<T> : Recorder where T : struct, IRecordable
    {
        public abstract IQueueable Convert(T command);

        public override Queue<IQueueable> GetQueue(float time)
        {
            if (times.Count > 0 && time > times[times.Count - 1])
            {
                Queue<T> commands = queueables[queueables.Count - 1];
                Queue<IQueueable> queue = new Queue<IQueueable>();
                queueables.RemoveAt(queueables.Count - 1);
                times.RemoveAt(times.Count - 1);

                while (commands.Count > 0)
                    queue.Enqueue(Convert(commands.Dequeue()));

                return queue;
            }
            else
                return default;
        }

        public override void AddQueue(float time, float delta)
        {
            if (!playback)
            {
                queueables.Add(new Queue<T>());
                deltas.Add(delta);
                times.Add(time);
            }
        }

        protected List<Queue<T>> queueables { get; } = new List<Queue<T>>();

        public Recorder(int id) : base(id) { }

        public Recorder(List<Queue<T>> queueables, List<float> times, List<float> deltas, int id) : base(times, deltas, id)
        {
            this.queueables = queueables;
            queueables.Reverse();
        }
    }

    public abstract class Recorder
    {
        public void SetPlayback(bool playback) => this.playback = playback;
        public void SetSession(Session session) => this.session = session;
        public abstract IQueueable Record(IQueueable command);
        public abstract void AddQueue(float time, float delta);
        public abstract Queue<IQueueable> GetQueue(float time);

        public float GetDelta()
        {
            if (deltas.Count == 0)
                return 0;

            float delta = deltas[deltas.Count - 1];
            deltas.RemoveAt(deltas.Count - 1);
            return delta;
        }

        public int id { get; }
        public abstract Record record { get; }
        public bool playback { get; private set; }

        protected Session session { get; private set; }
        protected List<float> times { get; } = new List<float>();
        protected List<float> deltas { get; } = new List<float>();

        protected Recorder(int id) => this.id = id;

        protected Recorder(List<float> times, List<float> deltas, int id)
        {
            this.id = id;
            this.times = times;
            this.deltas = deltas;
            deltas.Reverse();
            times.Reverse();
        }
    }

    public abstract class Record 
    {
        [SerializeField] public List<float> deltas { get; }
        [SerializeField] public List<float> times { get; }

        public Record(List<float> times, List<float> deltas)
        {
            this.deltas = deltas;
            this.times = times;
        }
    }

    public interface IRecordable { }
}