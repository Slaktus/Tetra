using System.Collections.Generic;
using Unity.Mathematics;

namespace Tetra.Collision
{
    public abstract class ShapeCollider : AbstractCollider
    {
        public abstract ShapeCollider SetRotation(float radians);

        protected static float2 Rotate(float2 point, float cosTheta, float sinTheta) => new float2(point.x * cosTheta - point.y * sinTheta, point.x * sinTheta + point.y * cosTheta);

        public override void Draw(UnityEngine.Color color)
        {
            float3 first = new float3(position + points[0], 0);
            UnityEngine.Vector3 prev = first;

            for (int i = 1; points.Count > i; i++)
            {
                float3 current = new float3(position + points[i], 0);
                UnityEngine.Debug.DrawLine(prev, current, color);
                prev = current;
            }

            float3 last = new float3(position + points[points.Count - 1], 0);
            UnityEngine.Debug.DrawLine(first, last, color);
        }

        public List<float2> edges { get; }
        public List<float2> points { get; }
        public float radians { get; protected set; }
        public float height => aabb.extents.x * 2;
        public float width => aabb.extents.x * 2;

        protected ShapeCollider(List<float2> points)
        {
            this.points = new List<float2>(points.Count);
            edges = new List<float2>(points.Count);
        }

        protected ShapeCollider()
        {
            edges = new List<float2>(4);
            points = new List<float2>(4);
        }
    }
}