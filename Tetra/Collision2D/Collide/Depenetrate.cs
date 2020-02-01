using Unity.Mathematics;

namespace Tetra.Collision
{
    public static partial class Collide
    {
        public static float2 Depenetrate<T,Y>(T colliderA, Y colliderB) where T : AbstractCollider where Y : AbstractCollider
        {
            switch (colliderA)
            {
                case CircleCollider circleColliderA:
                    return Depenetrate(circleColliderA, colliderB);

                case ShapeCollider shapeColliderA:
                    return Depenetrate(shapeColliderA, colliderB);

                default:
                    return float2.zero;
            }
        }

        private static float2 Depenetrate<T>(CircleCollider circleColliderA, T colliderB) where T : AbstractCollider
        {
            switch (colliderB)
            {
                case CircleCollider circleColliderB:
                    return Depenetrate(circleColliderA, circleColliderB);

                case ShapeCollider shapeColliderB:
                    return Depenetrate(circleColliderA, shapeColliderB);

                default:
                    return float2.zero;
            }
        }

        private static float2 Depenetrate<T>(ShapeCollider shapeColliderA, T colliderB) where T : AbstractCollider
        {
            switch (colliderB)
            {
                case CircleCollider circleColliderB:
                    return Depenetrate(shapeColliderA, circleColliderB);

                case RectangleCollider rectangleColliderB:
                    switch (shapeColliderA)
                    {
                        case RectangleCollider rectangleColliderA:
                            return Depenetrate(rectangleColliderA, rectangleColliderB);

                        default:
                            return Depenetrate(shapeColliderA, rectangleColliderB as ShapeCollider);
                    }

                case ShapeCollider shapeColliderB:
                    return Depenetrate(shapeColliderA, shapeColliderB);

                default:
                    return float2.zero;
            }
        }

        private static float2 Depenetrate(CircleCollider circleA, CircleCollider circleB)
        {
            float2 direction = circleA.position - circleB.position;
            float summedRadii = circleA.radius + circleB.radius;
            float distance = math.length(direction);

            return summedRadii > distance ? math.normalizesafe(direction) * (summedRadii - distance) : float2.zero;
        }

        private static float2 Depenetrate(CircleCollider circle, ShapeCollider shape)
        {
            float2 nearest = shape.position + shape.points[0];
            float lowest = math.distancesq(circle.position, nearest);

            for (int i = 1; shape.points.Count > i; i++)
            {
                float distance = math.distancesq(circle.position, shape.position + shape.points[i]);

                if (lowest > distance)
                {
                    lowest = distance;
                    nearest = shape.position + shape.points[i];
                }
            }

            float2 depenetration = float2.zero;
            float depth = float.PositiveInfinity;
            float2 axis = math.normalizesafe(nearest - circle.position);
            float overlap = LineOverlap(Project(shape, axis), Project(circle, axis));

            if (overlap == 0)
                return float2.zero;

            if (depth > overlap)
            {
                depth = overlap;
                depenetration = axis;
            }

            for (int i = 0; shape.points.Count > i; i++)
            {
                axis = math.normalizesafe(shape.edges[i].perpendicular());
                overlap = LineOverlap(Project(shape, axis), Project(circle, axis));

                if (overlap == 0)
                    return float2.zero;

                if (depth > overlap)
                {
                    depth = overlap;
                    depenetration = axis;
                }
            }

            return depenetration * depth * (math.dot(depenetration, shape.position - circle.position) >= 0 ? -1 : 1);
        }

        private static float2 Depenetrate(ShapeCollider shape, CircleCollider circle) => -Depenetrate(circle, shape);

        private static float2 Depenetrate(ShapeCollider shapeA, ShapeCollider shapeB)
        {
            float2 depenetration = float2.zero;
            float depth = float.PositiveInfinity;

            for (int i = 0; shapeA.points.Count > i; i++)
            {
                float2 axis = math.normalizesafe(shapeA.edges[i].perpendicular());
                float overlap = LineOverlap(Project(shapeA, axis), Project(shapeB, axis));

                if (overlap == 0)
                    return float2.zero;

                if (depth > overlap)
                {
                    depth = overlap;
                    depenetration = axis;
                }
            }

            for (int i = 0; shapeB.points.Count > i; i++)
            {
                float2 axis = math.normalizesafe(shapeB.edges[i].perpendicular());
                float overlap = LineOverlap(Project(shapeA, axis), Project(shapeB, axis));

                if (overlap == 0)
                    return float2.zero;

                if (depth > overlap)
                {
                    depth = overlap;
                    depenetration = axis;
                }
            }

            return depenetration * depth * (math.dot(depenetration, shapeB.position - shapeA.position) >= 0 ? -1 : 1);
        }

        private static float2 Depenetrate(RectangleCollider rectangleA, RectangleCollider rectangleB)
        {
            float2 depenetration = float2.zero;
            float depth = float.PositiveInfinity;

            for (int i = 0; rectangleA.axes.Count > i; i++)
            {
                float2 axis = math.normalizesafe(rectangleA.axes[i]);
                float overlap = LineOverlap(Project(rectangleA, axis), Project(rectangleB, axis));

                if (overlap == 0)
                    return float2.zero;

                if (depth > overlap)
                {
                    depth = overlap;
                    depenetration = axis;
                }
            }

            for (int i = 0; rectangleB.axes.Count > i; i++)
            {
                float2 axis = math.normalizesafe(rectangleB.axes[i]);
                float overlap = LineOverlap(Project(rectangleA, axis), Project(rectangleB, axis));

                if (overlap == 0)
                    return float2.zero;

                if (depth > overlap)
                {
                    depth = overlap;
                    depenetration = axis;
                }
            }

            return depenetration * depth * (math.dot(depenetration, rectangleB.position - rectangleA.position) >= 0 ? -1 : 1);
        }
    }
}