using System.Collections.Generic;
using Unity.Mathematics;
using Tetra.Caching;
using System;

namespace Tetra.Collision
{
    internal class BroadPhase
    {
        public void SetGridSize(float gridSize) => this.gridSize = gridSize;
        public int DeriveIndex(AbstractCollider collider) => DeriveIndex(DeriveRow(collider), DeriveColumn(collider));
        public void DrawLineToBucket(AbstractCollider collider) => UnityEngine.Debug.DrawLine(new float3(collider.position, 0), new float3(minX + (gridSize * (DeriveRow(collider) + 0.5f)), minY + (gridSize * (DeriveColumn(collider) + 0.5f)), 0), UnityEngine.Color.yellow);

        public BroadPhase Update(Dictionary<Type, CollisionGroup> groups, Pool<List<AbstractCollider>> pool)
        {
            PrepareBuckets(groups, pool);
            SortIntoBuckets(groups);

            PrepareCandidates();
            FindCandidates(groups);
            
            return this;
        }

        public void IntersectAABBs(AbstractCollider colliderA, List<AbstractCollider> candidates, List<AbstractCollider> containedBy, List<AbstractCollider> intersect, List<AbstractCollider> depenetrate)
        {
            foreach (AbstractCollider colliderB in candidates)
                if (colliderA != colliderB && Collide.Intersects(colliderA.aabb, colliderB.aabb))
                {
                    intersect?.Add(colliderB);
                    containedBy?.Add(colliderB);
                    depenetrate?.Add(colliderB);
                }
        }

        public void SortIntoBuckets(Dictionary<Type, CollisionGroup> groups)
        {
            foreach (CollisionGroup group in groups.Values)
                group.SortIntoBuckets(this);
        }

        private void FindCandidates(Dictionary<Type, CollisionGroup> groups)
        {
            foreach (CollisionGroup group in groups.Values)
                group.FindCandidates(this);
        }

        public void Clear()
        {
            containedByCandidates.Clear();
            intersectionCandidates.Clear();
            depenetrationCandidates.Clear();
        }

        public void DrawBuckets()
        {
            for (int y = 0; rows > y; y++)
                for (int x = 0; columns > x; x++)
                {
                    float3 a = new float3(minX + (gridSize * x), minY + (gridSize * y), 0);
                    float3 b = new float3(minX + (gridSize * (x + 1)), minY + (gridSize * y), 0);
                    float3 c = new float3(minX + (gridSize * (x + 1)), minY + (gridSize * (y + 1)), 0);
                    float3 d = new float3(minX + (gridSize * x), minY + (gridSize * (y + 1)), 0);

                    UnityEngine.Debug.DrawLine(a, b, UnityEngine.Color.red);
                    UnityEngine.Debug.DrawLine(b, c, UnityEngine.Color.red);
                    UnityEngine.Debug.DrawLine(c, d, UnityEngine.Color.red);
                    UnityEngine.Debug.DrawLine(d, a, UnityEngine.Color.red);
                }
        }

        private int DeriveIndex(int row, int column) => (column * columns) + row;
        private int DeriveRow(AbstractCollider collider) => (int)math.floor(math.unlerp(minX, maxX, collider.position.x) / gridSize * xRange);
        private int DeriveColumn(AbstractCollider collider) => (int)math.floor(math.unlerp(minY, maxY, collider.position.y) / gridSize * yRange);

        private void PrepareCandidates()
        {
            foreach (List<AbstractCollider> bucket in containedByCandidates.Values)
                bucket.Clear();

            foreach (List<AbstractCollider> bucket in intersectionCandidates.Values)
                bucket.Clear();

            foreach (List<AbstractCollider> bucket in depenetrationCandidates.Values)
                bucket.Clear();
        }

        //TODO: Add dynamic grid size! If we add a width/height parameter to colliders, we can find the biggest dimension and use that for gridSize
        private void PrepareBuckets(Dictionary<Type, CollisionGroup> groups, Pool<List<AbstractCollider>> pool)
        {
            minX = minY = float.PositiveInfinity;
            maxX = maxY = float.NegativeInfinity;

            foreach (CollisionGroup group in groups.Values)
                foreach(AbstractCollider collider in group.colliders)
                {
                    minX = math.min(minX, collider.position.x);
                    maxX = math.max(maxX, collider.position.x);
                    minY = math.min(minY, collider.position.y);
                    maxY = math.max(maxY, collider.position.y);
                }

            minX -= gridSize * 1.5f;
            minY -= gridSize * 1.5f;
            maxX += gridSize;
            maxY += gridSize;

            xRange = maxX - minX;
            yRange = maxY - minY;

            columns = math.max((int)math.ceil(xRange / gridSize), 1);
            rows = math.max((int)math.ceil(yRange / gridSize), 1);

            maxX = minX + (columns * gridSize);
            maxY = minY + (rows * gridSize);

            xRange = maxX - minX;
            yRange = maxY - minY;

            int total = columns * rows;

            foreach (CollisionGroup group in groups.Values)
                group.PrepareBuckets(total, pool);
        }

        public int columns { get; private set; }
        public int rows { get; private set; }
        public float gridSize { get; private set; }
        public Dictionary<AbstractCollider, List<AbstractCollider>> containedByCandidates { get; }
        public Dictionary<AbstractCollider, List<AbstractCollider>> intersectionCandidates { get; }
        public Dictionary<AbstractCollider, List<AbstractCollider>> depenetrationCandidates { get; }

        private float minX { get; set; }
        private float maxX { get; set; }
        private float minY { get; set; }
        private float maxY { get; set; }
        private float xRange { get; set; }
        private float yRange { get; set; }

        public BroadPhase(int gridSize = 10, int candidates = 1000)
        {
            this.gridSize = gridSize;
            containedByCandidates = new Dictionary<AbstractCollider, List<AbstractCollider>>(candidates);
            intersectionCandidates = new Dictionary<AbstractCollider, List<AbstractCollider>>(candidates);
            depenetrationCandidates = new Dictionary<AbstractCollider, List<AbstractCollider>>(candidates);
        }
    }
}