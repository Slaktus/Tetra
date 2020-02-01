using MaterialPropertyBlock = UnityEngine.MaterialPropertyBlock;
using Unity.Mathematics;

namespace Tetra.EntityManagement
{
    public abstract class Entity
    {
        public abstract void Update();
        public abstract void Activate();
        public abstract void Deactivate();

        public virtual void SetTimeScale(float timeScale) => this.timeScale = timeScale;

        public void SetScale(float3 scale) => this.scale = scale;
        public void SetPosition(float3 position) => this.position = position;
        public void SetRotation(quaternion rotation) => this.rotation = rotation;

        public Entity SetSession(Session session)
        {
            this.session = session;
            return this;
        }

        public float timeScale { get; private set; } = 1;
        public float4x4 matrix => float4x4.TRS(position, rotation, scale);
        public MaterialPropertyBlock materialPropertyBlock { get; private set; } = new MaterialPropertyBlock();
        public quaternion rotation { get; protected set; } = quaternion.identity;
        public float3 scale { get; protected set; } = new float3(1, 1, 1);
        public float3 position { get; protected set; }
        public bool active => session != null;
        public abstract int id { get; }

        protected Session session { get; private set; }
    }
}