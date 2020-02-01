using Unity.Mathematics;

namespace Tetra.Collision
{
    public class CircleCollider : AbstractCollider
    {
        public override void SetPosition(float2 position) => aabb = aabb.SetPosition(this.position = position);
        
        public void SetRadius(float radius)
        {
            this.radius = radius;
            SetAABB();
        }

        public override void Draw(UnityEngine.Color color)
        {
            int positions = 32;
            float angle = 360f / positions;
            float radians = math.radians(angle);
            float3 direction = math.up() * radius;
            float3 axis = new float3(0, 0, 1);

            float2 first = position + direction.xy;
            float2 previous = position + direction.xy;

            for (int i = 0; positions > i; i++)
            {
                float2 current = position + math.mul(quaternion.AxisAngle(axis, radians * i), direction).xy;
                UnityEngine.Debug.DrawLine(new float3(previous.xy, 0), new float3(current.xy, 0), color);
                previous = current;
            }

            UnityEngine.Debug.DrawLine(new UnityEngine.Vector3(previous.x, previous.y, 0), new UnityEngine.Vector3(first.x, first.y, 0), color);
        }

        protected override void SetAABB() => aabb = new AABB(this);

        public float radius { get; private set; }
        public override float2 center => float2.zero;

        public CircleCollider(float radius = 0) => SetRadius(radius);
        public CircleCollider(CircleCollider circleCollider) : this(circleCollider.radius) { }
    }
}