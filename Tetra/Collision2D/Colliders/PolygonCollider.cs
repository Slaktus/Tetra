using System.Collections.Generic;
using Unity.Mathematics;

namespace Tetra.Collision
{
    public class PolygonCollider : ShapeCollider
    {
        public override void SetPosition(float2 position) => aabb = aabb.SetPosition(this.position = position);

        public void SetPoints(List<float2> points)
        {
            radians = 0;
            this.points.Clear();
            cachedPoints.Clear();

            for (int i = 0; points.Count > i; i++)
            {
                this.points.Add(points[i]);
                cachedPoints.Add(points[i]);
            }

            SetAABB();
            SetEdges();
            SetCentroid();
        }

        public override ShapeCollider SetRotation(float radians)
        {
            if (radians == 0 || this.radians == radians)
                return this;

            this.radians = radians;

            if (0 > radians)
                radians += math.PI * 2 * math.ceil((-radians) / (math.PI * 2));
            else if (radians >= math.PI * 2)
                radians -= math.PI * 2 * math.floor(radians / (math.PI * 2));

            for (int i = 0; points.Count > i; i++)
                points[i] = Rotate(cachedPoints[i], math.cos(radians), math.sin(radians));

            SetAABB();
            SetEdges();
            return this;
        }

        protected override void SetAABB() => aabb = new AABB(this);

        private void SetEdges()
        {
            edges.Clear();

            for (int i = 0; points.Count - 1 > i; i++)
                edges.Add(points[i + 1] - points[i]);

            edges.Add(points[points.Count - 1] - points[0]);
        }

        private void SetCentroid()
        {
            float2 centroid = float2.zero;

            for (int i = 0; points.Count > i; i++)
                centroid += points[i];

            this.centroid = centroid / points.Count;
        }

        public override float2 center => centroid;

        private float2 centroid { get; set; }
        private List<float2> cachedPoints { get; }

        public PolygonCollider(List<float2> points) : base(points)
        {
            cachedPoints = new List<float2>(points.Count);
            SetPoints(points);
        }

        public PolygonCollider(PolygonCollider polygonCollider) : this(polygonCollider.cachedPoints) { }
    }
}