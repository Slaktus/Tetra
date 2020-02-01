using Unity.Mathematics;

namespace Tetra.Splines
{
    public abstract class SplineSolver
    {
        /// <summary>
        /// Directly evaluates the point on the curve without referring to lookup table for constant speed correction
        /// </summary>
        public abstract float3 GetPoint(float t);

        /// <summary>
        /// Calculates total length, segment start locations and segment distances
        /// </summary>
        public virtual void BuildPath()
        {
            int totalSudivisions = points.Length * subdivisions;
            float timePerSlice = 1f / totalSudivisions;
            length = 0;

            //We don't need the first point since it isthe from value
            keys = new float[totalSudivisions];
            values = new float[keys.Length];
            float3 previous = GetPoint(0);

            //Skip the first point and wrap one extra node skip the first point
            for (var i = 1; totalSudivisions + 1 > i; i++)
            {
                float time = timePerSlice * i;
                float3 current = GetPoint(time);
                length += math.distance(current, previous);
                values[i - 1] = length;
                keys[i - 1] = time;
                previous = current;
            }
        }
        
        /// <summary>
        /// Gets the point on the curve using a lookup table by walking the spline and checking the distance between points
        /// </summary>
        public virtual float3 GetPointOnPath(float t)
        {
            //Store the previous and next points from the lookup and get the start point
            float nextNodeTime = 0f;
            float nextNodeLength = 0f;
            float previousNodeTime = 0f;
            float previousNodeLength = 0f;
            float targetDistance = length * t;
            int start = targetDistance > 0 ? (int)(targetDistance / subdivisions) : 0;

            //Find the two nodes the target distance falls between
            for (int i = start; keys.Length > i; ++i)
            {
                float key = keys[i];
                float value = values[i];

                if (value >= targetDistance)
                {
                    nextNodeTime = key;
                    nextNodeLength = value;

                    if (previousNodeTime > 0)
                    {
                        int index = -1;

                        for (int j = i; keys.Length > j && 0 > index; j++)
                            if (keys[j] > previousNodeTime)
                                index = j - 1;

                        previousNodeLength = values[index];
                    }

                    break;
                }
                else
                    previousNodeTime = key;
            }

            //Translate the values from the lookup table and estimate the arc length between the known nodes from the lookup table
            float segmentTime = nextNodeTime - previousNodeTime;
            float segmentLength = nextNodeLength - previousNodeLength;
            float distanceIntoSegment = targetDistance - previousNodeLength;
            return GetPoint(previousNodeTime + (distanceIntoSegment / segmentLength) * segmentTime);
        }

        public virtual int GetTotalPointsBetweenPoints(float tA, float tB)
        {
            //Store the previous and next points from the lookup and get the start point
            float nextNodeLength = 0f;
            float targetDistance = length * tA;
            int start = targetDistance > 0 ? (int)(targetDistance / subdivisions) : 0;

            //Find the two nodes the target distance falls between
            for (int i = start; keys.Length > i; ++i)
            {
                float value = values[i];

                if (value >= targetDistance)
                {
                    nextNodeLength = value;
                    break;
                }
            }

            //Once again store the previous and next points from the lookup and get the start point
            targetDistance = length * tB;
            start = targetDistance > 0 ? (int)(targetDistance / subdivisions) : 0;

            float previousNodeTime = 0f;
            float previousNodeLength = 0f;

            //Find the two points the target distance falls between
            for (int i = start; keys.Length > i; ++i)
            {
                float key = keys[i];
                float value = values[i];

                if (value >= targetDistance)
                {
                    if (previousNodeTime > 0)
                    {
                        int index = -1;

                        for (int j = 0; keys.Length > j && 0 > index; j++)
                            if (keys[j] > key)
                                index = j - 1;

                        previousNodeLength = values[index];
                    }

                    break;
                }
                else
                    previousNodeTime = key;
            }

            //Round the value since we only need an approximation
            return (int)(nextNodeLength + 0.5f) + (int)(previousNodeLength + 0.5f);
        }

        public float3[] points { get; }
        public float length { get; protected set; }

        protected float[] keys { get; set; }
        protected float[] values { get; set; }

        //How many times should each segment be subdivided? Higher values means bigger lookup tables and longer lookups, but more accurate constant speed approximation
        private int subdivisions { get; }

        protected SplineSolver(float3[] points, int subdivisions = 5)
        {
            this.points = points;
            this.subdivisions = subdivisions;
        }
    }
}