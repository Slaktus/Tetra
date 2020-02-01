using Unity.Mathematics;

namespace Tetra.Splines
{
    public sealed class QuadraticBezierSplineSolver : SplineSolver
    {
        //http://www.gamedev.net/topic/551455-length-of-a-generalized-quadratic-bezier-curve-in-3d/
        public float QuadBezierLength(float3 startPoint, float3 controlPoint, float3 endPoint)
        {
            _lengthPoints[0] = controlPoint - startPoint;
            _lengthPoints[1] = startPoint - (2f * controlPoint) + endPoint;

            float length;
            bool3 comparison = _lengthPoints[1] != float3.zero;
            UnityEngine.Debug.Log("Confirm that this is correct!");

            if (comparison.x || comparison.y || comparison.z)
            {
                //Coefficients of f(t) = c*t^2 + b*t + a.
                float c = 4.0f * math.dot(_lengthPoints[1], _lengthPoints[1]);
                float b = 8.0f * math.dot(_lengthPoints[0], _lengthPoints[1]);
                float a = 4.0f * math.dot(_lengthPoints[0], _lengthPoints[0]);
                float q = 4.0f * a * c - b * b;  // = 16*|Cross(A0,A1)| >= 0

                float twoCpB = 2.0f * c + b;
                float sumCBA = c + b + a;
                float mult0 = 0.25f / c;
                float mult1 = q / (8.0f * math.pow(c, 1.5f));
                length = mult0 * (twoCpB * math.sqrt(sumCBA) - b * math.sqrt(a)) + mult1 * (math.log(2.0f * math.sqrt(c * sumCBA) + twoCpB) - math.log(2.0f * math.sqrt(c * a) + b));
            }
            else
                length = 2.0f * math.length(_lengthPoints[0]);

            return length;
        }

        public override float3 GetPoint(float t)
        {
            float d = 1f - t;
            return d * d * points[0] + 2f * d * t * points[1] + t * t * points[2];
        }

        private static float3[] _lengthPoints { get; } = new float3[2];

        public QuadraticBezierSplineSolver(float3[] nodes, int subdivisions = 5) : base(nodes, subdivisions) { }
    }
}