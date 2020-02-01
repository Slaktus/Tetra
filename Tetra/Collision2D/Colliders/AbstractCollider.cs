using Unity.Mathematics;

namespace Tetra.Collision
{
    public abstract class AbstractCollider
    {
        public abstract void Draw(UnityEngine.Color color);
        public abstract void SetPosition(float2 position);
        protected abstract void SetAABB();

        public float2 position { get; protected set; }
        public AABB aabb { get; protected set; }
        public abstract float2 center { get; }
    }
}