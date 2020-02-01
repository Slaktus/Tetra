using System.Collections.Generic;
using Unity.Mathematics;
using System;

namespace Tetra.EntityManagement
{
    internal class Context
    {
        public void Remove<T>() => rosters.Remove(typeof(T));
        public void Clear<T>() => rosters[typeof(T)].Clear();
        public T Get<T>(int id) where T : Entity => (rosters[typeof(T)] as Roster<T>).Get(id);
        public void Add<T>(Roster<T> roster) where T : Entity => rosters.Add(typeof(T), roster);
        public List<T> Active<T>() where T : Entity => (rosters[typeof(T)] as Roster<T>).active as List<T>;
        public void Remove<T>(T entity) where T : Entity => (rosters[typeof(T)] as Roster<T>).Remove(entity);
        public T Add<T>(float3 position = default) where T : Entity => (rosters[typeof(T)] as Roster<T>).Add(position);
        public void SetTimeScale<T>(float timeScale) where T : Entity => rosters[typeof(T)].SetTimeScale(timeScale);

        public void SetTimeScale(float timeScale)
        {
            foreach (Roster roster in rosters.Values)
                roster.SetTimeScale(timeScale);
        }

        public void Update()
        {
            foreach(Roster roster in rosters.Values)
                roster.Update();
        }

        public void Cull()
        {
            foreach (Roster roster in rosters.Values)
                roster.Cull();
        }

        public void Clear(bool clearRosters = false)
        {
            foreach(Roster roster in rosters.Values)
                roster.Clear();

            if (clearRosters)
                rosters.Clear();
        }

        private Dictionary<Type, Roster> rosters { get; } = new Dictionary<Type, Roster>();
    }
}