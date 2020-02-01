using System.Collections.Generic;
using Unity.Mathematics;

namespace Tetra.Collision
{
    public struct AABB
    {
        public void Draw(UnityEngine.Color color)
        {
            float3 a = new float3(position.x - extents.x, position.y + extents.y, 0);
            float3 b = new float3(position.x + extents.x, position.y + extents.y, 0);
            float3 c = new float3(position.x + extents.x, position.y - extents.y, 0);
            float3 d = new float3(position.x - extents.x, position.y - extents.y, 0);

            UnityEngine.Debug.DrawLine(a, b, color);
            UnityEngine.Debug.DrawLine(b, c, color);
            UnityEngine.Debug.DrawLine(c, d, color);
            UnityEngine.Debug.DrawLine(d, a, color);
        }

        public AABB SetPosition(float2 position)
        {
            this.position = position + new float2(minX + extents.x, minY + extents.y);
            return this;
        }

        public float2 position { get; private set; }
        public float2 extents { get; private set; }
        public float minX { get; private set; }
        public float maxX { get; private set; }
        public float minY { get; private set; }
        public float maxY { get; private set; }

        public AABB(CircleCollider collider) : this(collider.position, collider.radius) { }

        public AABB(ShapeCollider collider) : this(collider.position, collider.points) { }

        public AABB(float2 position, List<float2> points)
        {
            minX = points[0].x;
            maxX = points[0].x;
            minY = points[0].y;
            maxY = points[0].y;

            for (int i = 1; points.Count > i; i++)
            {
                minX = math.min(minX, points[i].x);
                maxX = math.max(maxX, points[i].x);
                minY = math.min(minY, points[i].y);
                maxY = math.max(maxY, points[i].y);
            }

            extents = new float2(math.abs(maxX - minX), math.abs(maxY - minY)) * 0.5f;
            this.position = position + new float2(minX + extents.x, minY + extents.y);
        }

        public AABB(float2 position, float radius)
        {
            extents = new float2(radius);
            minX = minY = -radius;
            maxX = maxY = radius;

            this.position = position + new float2(minX + extents.x, minY + extents.y);
        }

        public AABB(float2 position, float2 extents)
        {
            minX = -extents.x;
            maxX = extents.x;
            minY = -extents.y;
            maxY = extents.y;

            this.position = position + new float2(minX + extents.x, minY + extents.y);
            this.extents = extents;
        }
    }
}