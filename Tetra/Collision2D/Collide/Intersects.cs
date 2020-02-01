using Unity.Mathematics;

namespace Tetra.Collision
{
    public static partial class Collide
    {
        public static bool Intersects(AABB a, AABB b) => (a.extents.x + b.extents.x) >= math.abs(a.position.x - b.position.x) && (a.extents.y + b.extents.y) >= math.abs(a.position.y - b.position.y);

        public static bool Intersects<T,Y>(T colliderA, Y colliderB) where T : AbstractCollider where Y : AbstractCollider
        {
            switch (colliderA)
            {
                case CircleCollider circleColliderA:
                    return Intersects(circleColliderA, colliderB);

                case ShapeCollider shapeColliderA:
                    return Intersects(shapeColliderA, colliderB);

                default:
                    return false;
            }
        }

        private static bool Intersects<T>(CircleCollider circleColliderA, T colliderB) where T : AbstractCollider
        {
            switch (colliderB)
            {
                case CircleCollider circleColliderB:
                    return Intersects(circleColliderA, circleColliderB);

                case ShapeCollider shapeColliderB:
                    return Intersects(circleColliderA, shapeColliderB);

                default:
                    return false;
            }
        }

        private static bool Intersects<T>(ShapeCollider shapeColliderA, T colliderB) where T : AbstractCollider
        {
            switch (colliderB)
            {
                case CircleCollider circleColliderB:
                    return Intersects(shapeColliderA, circleColliderB);

                case RectangleCollider rectangleColliderB:
                    switch (shapeColliderA)
                    {
                        case RectangleCollider rectangleColliderA:
                            return Intersects(rectangleColliderA, rectangleColliderB);

                        case PolygonCollider polygonColliderA:
                            return Intersects(polygonColliderA, rectangleColliderB);

                        default:
                            return false;
                    }

                case PolygonCollider polygonColliderB:
                    switch (shapeColliderA)
                    {
                        case RectangleCollider rectangleColliderA:
                            return Intersects(rectangleColliderA, polygonColliderB);

                        case PolygonCollider polygonColliderA:
                            return Intersects(polygonColliderA, polygonColliderB);

                        default:
                            return false;
                    }

                default:
                    return false;
            }
        }

        private static bool Intersects(CircleCollider circleA, CircleCollider circleB) => (circleA.radius + circleB.radius) * (circleA.radius + circleB.radius) > math.distancesq(circleA.position, circleB.position);

        private static bool Intersects(ShapeCollider shape, CircleCollider circle) => Intersects(circle, shape);

        private static bool Intersects(CircleCollider circle, ShapeCollider shape)
        {
            float2 nearest = shape.position + shape.points[0];
            float minDistance = math.distancesq(circle.position, nearest);

            for (int i = 1; shape.points.Count > i; i++)
            {
                float distance = math.distancesq(circle.position, shape.position + shape.points[i]);

                if (minDistance > distance)
                {
                    minDistance = distance;
                    nearest = shape.position + shape.points[i];
                }
            }

            float2 axis = math.normalizesafe(nearest - circle.position);

            if (LineOverlap(Project(shape, axis), Project(circle, axis)) == 0)
                return false;
            
            for (int i = 0; shape.points.Count > i; i++)
            {
                axis = math.normalizesafe(shape.edges[i].perpendicular());

                if (LineOverlap(Project(shape, axis), Project(circle, axis)) == 0)
                    return false;
            }
            
            return true;
        }

        private static bool Intersects(PolygonCollider polygon, RectangleCollider rectangle) => Intersects(rectangle, polygon);

        private static bool Intersects(RectangleCollider rectangle, PolygonCollider polygon)
        {
            for (int i = 0; rectangle.axes.Count > i; i++)
            {
                float2 axis = rectangle.axes[i];

                if (LineOverlap(Project(rectangle, axis), Project(polygon, axis)) == 0)
                    return false;
            }

            for (int i = 0; polygon.points.Count > i; i++)
            {
                float2 axis = polygon.edges[i].perpendicular();

                if (LineOverlap(Project(rectangle, axis), Project(polygon, axis)) == 0)
                    return false;
            }

            return true;
        }

        private static bool Intersects(PolygonCollider polygonA, PolygonCollider polygonB)
        {
            for (int i = 0; polygonA.points.Count > i; i++)
            {
                float2 axis = polygonA.edges[i].perpendicular();

                if (LineOverlap(Project(polygonA, axis), Project(polygonB, axis)) == 0)
                    return false;
            }

            for (int i = 0; polygonB.points.Count > i; i++)
            {
                float2 axis = polygonB.edges[i].perpendicular();

                if (LineOverlap(Project(polygonA, axis), Project(polygonB, axis)) == 0)
                    return false;
            }

            return true;
        }

        private static bool Intersects(RectangleCollider rectangleA, RectangleCollider rectangleB)
        {
            for (int i = 0; rectangleA.axes.Count > i; i++)
            {
                float2 axis = rectangleA.axes[i];

                if (LineOverlap(Project(rectangleA, axis), Project(rectangleB, axis)) == 0)
                    return false;
            }

            for (int i = 0; rectangleB.axes.Count > i; i++)
            {
                float2 axis = rectangleB.axes[i];

                if (LineOverlap(Project(rectangleA, axis), Project(rectangleB, axis)) == 0)
                    return false;
            }

            return true;
        }
    }
}