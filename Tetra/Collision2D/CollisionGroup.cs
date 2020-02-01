using System.Collections.Generic;
using Tetra.Caching;

namespace Tetra.Collision
{
    internal class CollisionGroup
    {
        public void AddCollider(AbstractCollider collider) => colliders.Add(collider);
        public void RemoveCollider(AbstractCollider collider) => colliders.Remove(collider);

        public void AddGroup(CollisionGroup group, bool containedBy, bool intersect, bool depenetrate)
        {
            groups.Add(group);
            this.intersect.Add(intersect);
            this.containedBy.Add(containedBy);
            this.depenetrate.Add(depenetrate);
        }

        public void RemoveGroup(CollisionGroup group)
        {
            int index = groups.IndexOf(group);
            depenetrate.RemoveAt(index);
            intersect.RemoveAt(index);
            containedBy.RemoveAt(index);
            groups.RemoveAt(index);
        }

        public void PrepareBuckets(int total, Pool<List<AbstractCollider>> pool)
        {
            for (int i = 0; buckets.Count > i; i++)
                buckets[i].Clear();

            while (buckets.Count > total)
            {
                int index = buckets.Count - 1;
                pool.Put(buckets[index]);
                buckets.RemoveAt(index);
            }

            while (total > buckets.Count)
                buckets.Add(pool.Get());
        }

        public void SortIntoBuckets(BroadPhase broadphase)
        {
            foreach(AbstractCollider collider in colliders)
                buckets[broadphase.DeriveIndex(collider)].Add(collider);
        }

        public void FindCandidates(BroadPhase broadPhase)
        {
            for (int i = 0; buckets.Count > i; i++)
            {
                UpdateIndices(i, broadPhase.columns);

                for (int j = 0; groups.Count > j; j++)
                {
                    CollisionGroup group = groups[j];
                    bool intersect = this.intersect[j];
                    bool containedBy = this.containedBy[j];
                    bool depenetrate = this.depenetrate[j];

                    foreach (AbstractCollider collider in buckets[i])
                        AdjacentAABBIntersections(collider, broadPhase, group, indices, containedBy ? broadPhase.containedByCandidates[collider] : null, intersect ? broadPhase.intersectionCandidates[collider] : null, depenetrate ? broadPhase.depenetrationCandidates[collider] : null);
                }
            }
        }

        public void Initialize(AbstractCollider collider, BroadPhase broadPhase, NarrowPhase narrowPhase, int index)
        {
            buckets[index].Add(collider);
            UpdateIndices(index, broadPhase.columns);

            for (int i = 0; groups.Count > i; i++)
                AdjacentAABBIntersections(collider, broadPhase, groups[i], indices, containedBy[i] ? broadPhase.containedByCandidates[collider] : null, intersect[i] ? broadPhase.intersectionCandidates[collider] : null, depenetrate[i] ? broadPhase.depenetrationCandidates[collider] : null);

            narrowPhase.FindContainedBy(collider, broadPhase.containedByCandidates[collider], narrowPhase.containedBy[collider]);
            narrowPhase.FindIntersections(collider, broadPhase.intersectionCandidates[collider], narrowPhase.intersections[collider]);
            narrowPhase.FindDepenetrations(collider, broadPhase.depenetrationCandidates[collider], narrowPhase.depenetrationsByCollider[collider]);
        }

        private static void UpdateIndices(int index, int rows)
        {
            indices[0] = index;
            indices[1] = index - 1; //West
            indices[2] = index - rows - 1; //Northwest
            indices[3] = indices[2] + 1; //North
            indices[4] = indices[3] + 1; //NorthEast
            indices[5] = index + 1; //East
            indices[6] = index + rows - 1; //SouthWest
            indices[7] = indices[6] + 1; //South
            indices[8] = indices[7] + 1; //SouthEast
        }

        private static void AdjacentAABBIntersections(AbstractCollider collider, BroadPhase broadPhase, CollisionGroup group, int[] indices, List<AbstractCollider> containedBy, List<AbstractCollider> intersect, List<AbstractCollider> depenetrate)
        {
            for(int i = 0; indices.Length > i; i++)
                broadPhase.IntersectAABBs(collider, group.buckets[indices[i]], containedBy, intersect, depenetrate);
        }

        public List<AbstractCollider> colliders { get; }
        public List<List<AbstractCollider>> buckets { get; }

        private List<bool> intersect { get; }
        private List<bool> containedBy { get; }
        private List<bool> depenetrate { get; }
        private List<CollisionGroup> groups { get; }
        private static int[] indices { get; } = new int[9];

        public CollisionGroup(int bucketSize = 1000, int groupCount = 10)
        {
            intersect = new List<bool>(groupCount);
            containedBy = new List<bool>(groupCount);
            depenetrate = new List<bool>(groupCount);
            colliders = new List<AbstractCollider>();
            groups = new List<CollisionGroup>(groupCount);
            buckets = new List<List<AbstractCollider>>(bucketSize);
        }
    }
}