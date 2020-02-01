using Unity.Mathematics;

namespace Tetra.Collision
{
    public static partial class Collide
    {
        private static float LineOverlap(float2 a, float2 b) => math.max(0, math.min(a.y, b.y) - math.max(a.x, b.x));

        private static float2 Project(ShapeCollider shape, float2 axis)
        {
            float min = math.dot(shape.position + shape.points[0], axis);
            float max = min;

            for (int i = 1; shape.points.Count > i; i++)
            {
                float dot = math.dot(shape.position + shape.points[i], axis);
                min = math.min(min, dot);
                max = math.max(max, dot);
            }

            return new float2(min, max);
        }

        private static float2 Project(CircleCollider circle, float2 axis)
        {
            float dot = math.dot(circle.position, axis);
            return new float2(dot - circle.radius, dot + circle.radius);
        }

        private static int Project(float2 from, float2 to, float2 point)
        {
            to -= from;
            point -= from;
            float ccw = (point.x * to.y) - (point.y * to.x);

            if (ccw == 0)
            {
                ccw = (point.x * to.x) + (point.y * to.y);

                if (ccw > 0)
                {
                    point -= to;
                    ccw = (point.x * to.x) + (point.y * to.y);

                    if (0 > ccw)
                        ccw = 0;
                }
            }

            return 0 > ccw
                ? -1
                : ccw > 0
                    ? 1
                    : 0;
        }
    }
}