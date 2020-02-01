using Unity.Mathematics;
using Tetra.Collision;

namespace Tetra.UIManagement
{
    public abstract class ClickableElement : Element
    {
        public override void Update()
        {
            if (containsMouse)
            {
                if (!hovering)
                {
                    Enter();
                    hovering = true;
                }
                else
                    Stay();

            }
            else if (hovering)
            {
                Exit();
                hovering = false;
            }
        }

        public void SetRectangleCollider()
        {
            collider = new RectangleCollider(scale.x, scale.y);
        }

        public abstract void Enter();
        public abstract void Stay();
        public abstract void Exit();

        public virtual bool Contains(float3 position) => Contains(position.xy);
        public virtual bool Contains(float2 position) => Collide.ContainedBy(position, collider);

        public virtual bool containsMouse => Contains(screen.mousePosition);
        public virtual AbstractCollider collider { get; private set; }

        private bool hovering { get; set; }
    }
}