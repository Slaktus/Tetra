using System.Collections.Generic;
using Unity.Mathematics;
using Tetra.Caching;

namespace Tetra.EntityManagement
{
    internal class Roster<T> : Roster where T : Entity
    {
        public override void SetTimeScale(float timeScale)
        {
            this.timeScale = timeScale;

            for (int i = 0; active.Count > i; i++)
                active[i].SetTimeScale(timeScale);
        }

        public T Get(int id)
        {
            for (int i = 0; active.Count > i; i++)
                if (active[i].id == id)
                    return active[i];

            return null;
        }

        public override void Update()
        {
            for (int i = 0; active.Count > i; i++)
                if (active[i].active)
                    active[i].Update();
        }

        public override void Cull()
        {
            for (int i = 0; cull.Count > i; i++)
            {
                active.Remove(cull[i]);
                pool.Put(cull[i]);
            }

            cull.Clear();
        }

        public T Add(float3 position = default)
        {
            T entity = pool.Get();
            active.Add(entity);
            entity.SetSession(session);
            entity.SetPosition(position);
            entity.SetTimeScale(timeScale);
            entity.Activate();
            return entity;
        }

        public void Remove(T entity)
        {
            entity.Deactivate();
            entity.SetSession(null);
            cull.Add(entity);
        }

        public override void Clear()
        {
            Cull();

            for (int i = 0; active.Count > i; i++)
                Remove(active[i]);

            Cull();
        }

        public List<T> active { get; } = new List<T>();

        private List<T> cull { get; } = new List<T>();
        private float timeScale { get; set; } = 1;
        private Session session { get; }
        private Pool<T> pool { get; }

        public Roster(Pool<T> pool, Session session)
        {
            this.pool = pool;
            this.session = session;
        }
    }

    public abstract class Roster
    {
        public abstract void SetTimeScale(float timeScale);
        public abstract void Update();
        public abstract void Clear();
        public abstract void Cull();
    }
}