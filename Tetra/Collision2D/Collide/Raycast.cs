using Unity.Mathematics;

namespace Tetra.Collision
{
    public static partial class Collide
    {
        public static bool Raycast<T>(float2 from, float2 to, T collider) where T : AbstractCollider
        {
            switch (collider)
            {
                case CircleCollider circleCollider:
                    return Raycast(from, to, circleCollider);

                case ShapeCollider shapeCollider:
                    return Raycast(from, to, shapeCollider);

                default:
                    return false;
            }
        }

        public static bool Raycast(float2 from, float2 to, float2 pointA, float2 pointB) => 0 >= (Project(from, to, pointA) * Project(from, to, pointB)) && 0 >= (Project(pointA, pointB, from) * Project(pointA, pointB, to));

        private static bool Raycast(float2 from, float2 to, CircleCollider circle)
        {
            float2 delta = to - from;

            float a = delta.x * delta.x + delta.y * delta.y;
            float b = 2 * (delta.x * (from.x - circle.position.x) + delta.y * (from.y - circle.position.y));
            float c = (from.x - circle.position.x) * (from.x - circle.position.x) + (from.y - circle.position.y) * (from.y - circle.position.y) - (circle.radius * circle.radius);

            float discriminant = b * b - 4 * a * c;

            if (0 > discriminant)
                return false;

            float positive = (-b + math.sqrt(discriminant)) / (2 * a);

            if (positive >= 0 && 1 >= positive)
                return true;

            float negative = (-b - math.sqrt(discriminant)) / (2 * a);

            return (negative >= 0 && 1 >= negative);
        }

        private static bool Raycast(float2 from, float2 to, ShapeCollider shape)
        {
            if (ContainedBy(from, shape) || ContainedBy(to, shape))
                return true;

            int previous = shape.points.Count - 1;

            for (int i = 0; shape.points.Count > i; i++)
            {
                if (Raycast(from, to, shape.points[previous], shape.points[i]))
                    return true;

                previous = i;
            }

            return false;
        }
    }
}