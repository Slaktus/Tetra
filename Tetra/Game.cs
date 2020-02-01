using System.Collections.Generic;
using Tetra.EntityManagement;
using Tetra.UIManagement;
using Tetra.Caching;
using System;

namespace Tetra
{
    public abstract class Game
    {
        public abstract void Restart();
        public abstract void Replay();
        public abstract void Start();
        public abstract void Stop();

        public void Add(Session session) => sessions.Add(session);
        public void Remove(Session session) => sessions.Remove(session);
        public int IndexOf(Session session) => sessions.IndexOf(session);
        public void SetCurrent(Session session) => currentSessionIndex = IndexOf(session);

        protected Pool<T> GetPool<T>() where T : new() => pools[typeof(T)] as Pool<T>;

        protected Pool<T> AddPool<T>() where T : new()
        {
            Pool<T> pool = new Pool<T>(() => new T());
            pools.Add(typeof(T), pool);
            return pool;
        }

        public UI ui { get; }
        public Session session => sessions.Count > 0 ? sessions[currentSessionIndex] : null;

        protected int currentSessionIndex { get; private set; }

        private List<Session> sessions { get; } = new List<Session>();
        private Dictionary<Type, Pool> pools { get; } = new Dictionary<Type, Pool>();

        public Game() => ui = new UI(this);
    }
}