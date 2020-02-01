using System.Collections.Generic;
using Unity.Mathematics;

namespace Tetra.Collision
{
    internal class NarrowPhase
    {
        public void Update(BroadPhase broadPhase)
        {
            PrepareContainedBy();
            FindContainedBy(broadPhase.containedByCandidates);

            PrepareIntersections();
            FindIntersections(broadPhase.intersectionCandidates);

            PrepareDepenetrations();
            FindDepenetrations(broadPhase.depenetrationCandidates);
        }

        public void FindContainedBy(Dictionary<AbstractCollider, List<AbstractCollider>> containsCandidates)
        {
            foreach (AbstractCollider colliderA in containsCandidates.Keys)
                FindContainedBy(colliderA, containsCandidates[colliderA], containedBy[colliderA]);
        }

        public void FindContainedBy(AbstractCollider colliderA, List<AbstractCollider> candidates, HashSet<AbstractCollider> containedByA)
        {
            foreach (AbstractCollider colliderB in candidates)
                if (Collide.ContainedBy(colliderA, colliderB))
                    containedByA.Add(colliderB);
        }

        public void FindIntersections(Dictionary<AbstractCollider, List<AbstractCollider>> intersectionCandidates)
        {
            foreach (AbstractCollider colliderA in intersectionCandidates.Keys)
                FindIntersections(colliderA, intersectionCandidates[colliderA], intersections[colliderA]);
        }

        public void FindIntersections(AbstractCollider colliderA, List<AbstractCollider> candidates, HashSet<AbstractCollider> intersectionsA)
        {
            foreach (AbstractCollider colliderB in candidates)
            {
                ColliderPair pair = new ColliderPair(colliderA, colliderB);

                if (!checkedPairs.Contains(pair) && Collide.Intersects(colliderA, colliderB))
                {
                    checkedPairs.Add(pair);
                    intersectionsA.Add(colliderB);
                    intersections[colliderB].Add(colliderB);
                }
            }
        }

        public void FindDepenetrations(Dictionary<AbstractCollider, List<AbstractCollider>> depenetrationCandidates)
        {
            foreach (AbstractCollider collider in depenetrationCandidates.Keys)
                FindDepenetrations(collider, depenetrationCandidates[collider], depenetrationsByCollider[collider]);
        }

        public void FindDepenetrations(AbstractCollider collider, List<AbstractCollider> candidates, Dictionary<AbstractCollider, float2> lookup)
        {
            float2 total = float2.zero;

            foreach (AbstractCollider candidate in candidates)
            {
                float2 depenetration = Collide.Depenetrate(collider, candidate);
                lookup.Add(candidate, depenetration);
                total += depenetration;
            }

            depenetrations[collider] = total;
        }

        public void Clear()
        {
            depenetrations.Clear();
            intersections.Clear();
            containedBy.Clear();
        }

        private void PrepareDepenetrations()
        {
            cachedColliders.Clear();

            foreach (AbstractCollider collider in depenetrations.Keys)
                cachedColliders.Add(collider);

            foreach (AbstractCollider collider in cachedColliders)
            {
                depenetrations[collider] = float2.zero;
                depenetrationsByCollider[collider].Clear();
            }
        }

        private void PrepareIntersections()
        {
            checkedPairs.Clear();

            foreach(HashSet<AbstractCollider> hashSet in intersections.Values)
                hashSet.Clear();
        }

        private void PrepareContainedBy()
        {
            foreach (HashSet<AbstractCollider> hashSet in containedBy.Values)
                hashSet.Clear();
        }

        public Dictionary<AbstractCollider, float2> depenetrations { get; }
        public Dictionary<AbstractCollider, HashSet<AbstractCollider>> containedBy { get; }
        public Dictionary<AbstractCollider, HashSet<AbstractCollider>> intersections { get; }
        public Dictionary<AbstractCollider, Dictionary<AbstractCollider, float2>> depenetrationsByCollider { get; }

        private List<AbstractCollider> cachedColliders { get; }
        private HashSet<ColliderPair> checkedPairs { get; }

        public NarrowPhase(int collisions = 1000)
        {
            checkedPairs = new HashSet<ColliderPair>();
            cachedColliders = new List<AbstractCollider>(collisions);
            depenetrations = new Dictionary<AbstractCollider, float2>();
            containedBy = new Dictionary<AbstractCollider, HashSet<AbstractCollider>>(collisions);
            intersections = new Dictionary<AbstractCollider, HashSet<AbstractCollider>>(collisions);
            depenetrationsByCollider = new Dictionary<AbstractCollider, Dictionary<AbstractCollider, float2>>(collisions);
        }
    }
}