using System.Collections.Generic;
using Unity.Mathematics;
using Tetra.Caching;
using System;

namespace Tetra.Collision
{
    public class CollisionSystem
    {
        public void Update() => narrowPhase.Update(broadPhase.Update(groups, listPool));

        public void RemoveCollisionGroup<T>() => groups.Remove(typeof(T));
        public float2 Depenetrate(AbstractCollider collider) => narrowPhase.depenetrations[collider];
        public void AddCollisionGroup<T>() => groups.Add(typeof(T), new CollisionGroup(bucketSize, groupCount));
        public void AddGroupToCheck<T, Y>(bool containedBy, bool intersect, bool depenetrate) => groups[typeof(T)].AddGroup(groups[typeof(Y)], containedBy, intersect, depenetrate);
        public bool ContainedBy(AbstractCollider colliderA, AbstractCollider colliderB) => narrowPhase.containedBy[colliderA].Contains(colliderB);
        public bool Intersects(AbstractCollider colliderA, AbstractCollider colliderB) => narrowPhase.intersections[colliderA].Contains(colliderB);

        public float2 Depenetrate(AbstractCollider colliderA, AbstractCollider colliderB)
        {
            Dictionary<AbstractCollider, float2> lookup = narrowPhase.depenetrationsByCollider[colliderA];
            return lookup.ContainsKey(colliderB) ? lookup[colliderB] : float2.zero;
        }

        public void AddCollider<T>(AbstractCollider collider)
        {
            CollisionGroup group = groups[typeof(T)];

            group.AddCollider(collider);
            AddColliderToLookups(collider);
            int index = broadPhase.DeriveIndex(collider);

            if (broadPhase.columns > index || index >= group.buckets.Count - broadPhase.columns)
                Update();
            else
                group.Initialize(collider, broadPhase, narrowPhase, index);
        }

        public void RemoveCollider<T>(AbstractCollider collider)
        {
            groups[typeof(T)].RemoveCollider(collider);

            PutCollectionInPool(narrowPhase.containedBy[collider]);
            PutCollectionInPool(narrowPhase.intersections[collider]);
            PutCollectionInPool(narrowPhase.depenetrationsByCollider[collider]);

            PutCollectionInPool(broadPhase.containedByCandidates[collider]);
            PutCollectionInPool(broadPhase.intersectionCandidates[collider]);
            PutCollectionInPool(broadPhase.depenetrationCandidates[collider]);

            RemoveColliderFromLookups(collider);
        }

        private void AddColliderToLookups(AbstractCollider collider)
        {
            narrowPhase.depenetrations.Add(collider, float2.zero);
            narrowPhase.containedBy.Add(collider, hashSetPool.Get());
            narrowPhase.intersections.Add(collider, hashSetPool.Get());
            narrowPhase.depenetrationsByCollider.Add(collider, dictionaryPool.Get());

            broadPhase.containedByCandidates.Add(collider, listPool.Get());
            broadPhase.intersectionCandidates.Add(collider, listPool.Get());
            broadPhase.depenetrationCandidates.Add(collider, listPool.Get());
        }

        private void RemoveColliderFromLookups(AbstractCollider collider)
        {
            narrowPhase.containedBy.Remove(collider);
            narrowPhase.intersections.Remove(collider);
            narrowPhase.depenetrations.Remove(collider);
            narrowPhase.depenetrationsByCollider.Remove(collider);

            broadPhase.containedByCandidates.Remove(collider);
            broadPhase.intersectionCandidates.Remove(collider);
            broadPhase.depenetrationCandidates.Remove(collider);
        }

        private void PutCollectionInPool(List<AbstractCollider> list)
        {
            list.Clear();
            listPool.Put(list);
        }

        private void PutCollectionInPool(HashSet<AbstractCollider> hashSet)
        {
            hashSet.Clear();
            hashSetPool.Put(hashSet);
        }

        private void PutCollectionInPool(Dictionary<AbstractCollider,float2> dictionary)
        {
            dictionary.Clear();
            dictionaryPool.Put(dictionary);
        }

        public void Clear()
        {
            groups.Clear();
            broadPhase.Clear();
            narrowPhase.Clear();
        }

        private int groupCount { get; }
        private int bucketSize { get; }
        private Pool<List<AbstractCollider>> listPool { get; }
        private Dictionary<Type, CollisionGroup> groups { get; }
        private Pool<HashSet<AbstractCollider>> hashSetPool { get; }
        private Pool<Dictionary<AbstractCollider,float2>> dictionaryPool { get; }

        internal BroadPhase broadPhase { get; }
        internal NarrowPhase narrowPhase { get; }

        public CollisionSystem(int gridSize = 10, int groupCount = 10, int poolSize = 1000, int bucketSize = 1000)
        {
            this.bucketSize = bucketSize;
            this.groupCount = groupCount;

            narrowPhase = new NarrowPhase();
            broadPhase = new BroadPhase(gridSize);
            groups = new Dictionary<Type, CollisionGroup>(groupCount);
            hashSetPool = new Pool<HashSet<AbstractCollider>>(() => new HashSet<AbstractCollider>(), poolSize);
            listPool = new Pool<List<AbstractCollider>>(() => new List<AbstractCollider>(bucketSize), poolSize);
            dictionaryPool = new Pool<Dictionary<AbstractCollider, float2>>(() => new Dictionary<AbstractCollider, float2>(bucketSize), poolSize);
        }
    }
}