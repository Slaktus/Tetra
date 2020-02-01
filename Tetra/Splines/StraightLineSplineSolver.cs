using Unity.Mathematics;

namespace Tetra.Splines
{
    public sealed class StraightLineSplineSolver : SplineSolver
    {
        public override void BuildPath()
        {
            //No need to build lookup for less than 3 points and more than 1 segment
            if (3 > points.Length)
                return;

            //We don't need the first and last points since they are the from and to values
            values = new float[points.Length - 1];

            for (int i = 0; values.Length > i; i++)
            {
                float distance = math.distance(points[i], points[i + 1]);
                values[i] = distance;
                length += distance;
            }

            //We don't need the first and last points since they are the from and to values
            keys = new float[points.Length - 2];

            //Now that we have the total length we can calculate the start locations
            float accruedLength = 0f;

            for (int i = 0; values.Length - 1 > i; i++)
                keys[i] = (accruedLength += values[i]) / length;
        }

        public override float3 GetPoint(float t) => GetPointOnPath(t);

        public override float3 GetPointOnPath(float t)
        {
            //Use lerp rather than lookup if there is less than three points
            if (3 > points.Length)
                return math.lerp(points[0], points[1], t);

            _currentSegment = 0;

            for (int k = 0; keys.Length > k; k++)
            {
                if (t > keys[k])
                {
                    _currentSegment = k + 1;
                    continue;
                }

                break;
            }

            //Need to find the distance travelled up to this point and subtract it from the total distance to see exactly how far along the current segment the point is
            float totalDistanceTravelled = t * length;
            int i = _currentSegment - 1; // we want all the previous segment lengths

            while (i >= 0)
                totalDistanceTravelled -= values[i--];

            return math.lerp(points[_currentSegment], points[_currentSegment + 1], totalDistanceTravelled / values[_currentSegment]);
        }

        private int _currentSegment;

        public StraightLineSplineSolver(float3[] nodes, int subdivisions = 5) : base(nodes, subdivisions) { }
    }
}