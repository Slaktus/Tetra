using Unity.Mathematics;

namespace Tetra.Collision
{
    public static partial class Collide
    {
        public static bool ContainedBy(float2 point, AABB aabb) => point.x >= aabb.position.x - aabb.extents.x && aabb.position.x + aabb.extents.x >= point.x && point.y >= aabb.position.y - aabb.extents.y && aabb.position.y + aabb.extents.y >= point.y;

        public static bool ContainedBy<T>(float2 point, T collider) where T : AbstractCollider
        {
            switch (collider)
            {
                case CircleCollider circleCollider:
                    return ContainedBy(point, circleCollider);

                case RectangleCollider rectangleCollider:
                    return ContainedBy(point, rectangleCollider);

                case ShapeCollider shapeCollider:
                    return ContainedBy(point, shapeCollider);

                default:
                    return false;
            }
        }

        public static bool ContainedBy<T,Y>(T colliderA, Y colliderB ) where T : AbstractCollider where Y : AbstractCollider
        {
            switch (colliderA)
            {
                case CircleCollider circleColliderA:
                    return ContainedBy(circleColliderA, colliderB);

                case ShapeCollider shapeColliderA:
                    return ContainedBy(shapeColliderA, colliderB);

                default:
                    return false;
            }
        }

        private static bool ContainedBy<T>(CircleCollider circleColliderA, T colliderB) where T : AbstractCollider
        {
            switch (colliderB)
            {
                case CircleCollider circleColliderB:
                    return ContainedBy(circleColliderA, circleColliderB);

                case ShapeCollider shapeColliderB:
                    return ContainedBy(shapeColliderB, circleColliderA);

                default:
                    return false;
            }
        }

        private static bool ContainedBy<T>(ShapeCollider shapeColliderA, T colliderB) where T : AbstractCollider
        {
            switch (colliderB)
            {
                case CircleCollider circleColliderB:
                    return ContainedBy(shapeColliderA, circleColliderB);

                case RectangleCollider rectangleColliderB:
                    return ContainedBy(shapeColliderA, rectangleColliderB);

                case ShapeCollider shapeColliderB:
                    return ContainedBy(shapeColliderA, shapeColliderB);

                default:
                    return false;
            }
        }

        private static bool ContainedBy(ShapeCollider shapeColliderA, ShapeCollider shapeColliderB)
        {
            for (int i = 0; shapeColliderA.points.Count > i; i++)
                if (!ContainedBy(shapeColliderA.position + shapeColliderA.points[i], shapeColliderB))
                    return false;

            return true;
        }

        private static bool ContainedBy(ShapeCollider shapeCollider, RectangleCollider rectangleCollider)
        {
            for (int i = 0; shapeCollider.points.Count > i; i++)
                if (!ContainedBy(shapeCollider.position + shapeCollider.points[i], rectangleCollider))
                    return false;

            return true;
        }

        private static bool ContainedBy(ShapeCollider shapeCollider, CircleCollider circleCollider)
        {
            for (int i = 0; shapeCollider.points.Count > i; i++)
                if (!ContainedBy(shapeCollider.position + shapeCollider.points[i], circleCollider))
                    return false;

            return true;
        }

        private static bool ContainedBy(CircleCollider circleColliderA, CircleCollider circleColliderB) => (circleColliderB.radius - circleColliderA.radius) * (circleColliderB.radius - circleColliderA.radius) > math.distancesq(circleColliderA.position, circleColliderB.position);

        private static bool ContainedBy(float2 point, CircleCollider circleCollider) => circleCollider.radius * circleCollider.radius > math.distancesq(point, circleCollider.position);

        private static bool ContainedBy(float2 point, RectangleCollider rectangleCollider) => rectangleCollider.radians != 0
            ? ContainedBy(point, rectangleCollider as ShapeCollider)
            : rectangleCollider.xMax > point.x && point.x > rectangleCollider.xMin && rectangleCollider.yMax > point.y && point.y > rectangleCollider.yMin;

        private static bool ContainedBy(float2 point, ShapeCollider shapeCollider)
        {
            bool result = false;
            int j = shapeCollider.points.Count - 1;
            float2 position = shapeCollider.position;

            for (int i = 0; shapeCollider.points.Count > i; i++)
            {
                float2 current = position + shapeCollider.points[i];
                float2 previous = position + shapeCollider.points[j];

                if ((point.y > current.y && previous.y >= point.y || point.y > previous.y && current.y >= point.y) && point.x > current.x + (point.y - current.y) / (previous.y - current.y) * (previous.x - current.x))
                    result = !result;

                j = i;
            }

            return result;
        }
    }
}