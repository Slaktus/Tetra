using System.Collections.Generic;
using Unity.Mathematics;

namespace Tetra.Collision
{
    public class RectangleCollider : ShapeCollider
    {
        public override void SetPosition(float2 position) => aabb = aabb.SetPosition(this.position = position);

        public void SetDimensions(float width, float height)
        {
            radians = 0;
            this.width = width;
            this.height = height;

            SetPoints();
            SetEdges();
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

            SetPoints();

            for (int i = 0; points.Count > i; i++)
                points[i] = Rotate(points[i], math.cos(radians), math.sin(radians));

            SetEdges();
            return this;
        }

        public void SetPoints(List<float2> points)
        {
            this.points.Clear();

            for (int i = 0; points.Count > i; i++)
                this.points.Add(points[i]);

            SetAABB();
        }

        protected override void SetAABB() => aabb = new AABB(this);

        private void SetPoints()
        {
            points.Clear();
            points.Add(new float2(-width * 0.5f, -height * 0.5f));
            points.Add(new float2(-width * 0.5f, height * 0.5f));
            points.Add(new float2(width * 0.5f, height * 0.5f));
            points.Add(new float2(width * 0.5f, -height * 0.5f));
            SetAABB();
        }

        private void SetEdges()
        {
            edges.Clear();

            for (int i = 0; points.Count - 1 > i; i++)
                edges.Add(points[i + 1] - points[i]);

            edges.Add(points[points.Count - 1] - points[0]);

            SetAxes();
        }

        private void SetAxes()
        {
            axes.Clear();
            axes.Add(edges[0] / math.lengthsq(edges[0]));
            axes.Add(edges[edges.Count - 1] / math.lengthsq(edges[edges.Count - 1]));
        }

        public float width { get; private set; }
        public float height { get; private set; }
        public override float2 center => float2.zero;
        public float xMin => position.x - (width * 0.5f);
        public float xMax => position.x + (width * 0.5f);
        public float yMin => position.y - (height * 0.5f);
        public float yMax => position.y + (height * 0.5f);
        public List<float2> axes { get; } = new List<float2>(2);

        public RectangleCollider() : base() => SetDimensions(0, 0);
        public RectangleCollider(float width, float height) : base() => SetDimensions(width, height);
        public RectangleCollider(RectangleCollider rectangleCollider) : this(rectangleCollider.width, rectangleCollider.height) { }
    }
}