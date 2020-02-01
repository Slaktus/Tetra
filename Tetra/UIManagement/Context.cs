using System.Collections.Generic;
using Unity.Mathematics;
using System;

namespace Tetra.UIManagement
{
    internal class Context
    {
        public void Remove<T>() => rosters.Remove(typeof(T));
        public T Get<T>(int id) where T : Element => (rosters[typeof(T)] as Roster<T>).Get(id);
        public void Add<T>(Roster<T> element) where T : Element => rosters.Add(typeof(T), element);
        public List<T> Active<T>() where T : Element => (rosters[typeof(T)] as Roster<T>).active as List<T>;
        public void Remove<T>(T element) where T : Element => (rosters[typeof(T)] as Roster<T>).Remove(element);
        public T Add<T>(float3 position = default) where T : Element => (rosters[typeof(T)] as Roster<T>).Add(position);

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

        public void Clear()
        {
            foreach(Roster roster in rosters.Values)
                roster.Clear();

            rosters.Clear();
        }

        private Dictionary<Type, Roster> rosters { get; } = new Dictionary<Type, Roster>();
    }
}