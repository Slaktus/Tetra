using System.Collections.Generic;
using Unity.Mathematics;
using Tetra.Caching;

namespace Tetra.UIManagement
{
    internal class Roster<T> : Roster where T : Element
    {
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
            T element = pool.Get();
            active.Add(element);
            element.SetPosition(position);
            return element;
        }

        public void Remove(T entity) => cull.Add(entity);

        public override void Clear()
        {
            Cull();

            for (int i = 0; active.Count > i; i++)
                Remove(active[i]);

            Cull();
        }

        public List<T> active { get; } = new List<T>();

        private List<T> cull { get; } = new List<T>();
        private Pool<T> pool { get; }

        public Roster(Pool<T> pool) => this.pool = pool;
    }

    public abstract class Roster
    {
        public abstract void Update();
        public abstract void Clear();
        public abstract void Cull();
    }
}