using Unity.Mathematics;

namespace Tetra.Splines
{
    public sealed class BezierSplineSolver : SplineSolver
    {
        //http://www.gamedev.net/topic/551455-length-of-a-generalized-quadratic-bezier-curve-in-3d/
        public float QuadBezierLength(float3 startPoint, float3 controlPoint, float3 endPoint)
        {
            _lengthPoints[0] = controlPoint - startPoint;
            _lengthPoints[1] = startPoint - 2f * controlPoint + endPoint;

            float length;
            bool3 comparison = _lengthPoints[1] != float3.zero;

            if (comparison.x || comparison.y || comparison.z)
            {
                //Coefficients of f(t) = c*t^2 + b*t + a.
                float c = 4.0f * math.dot(_lengthPoints[1], _lengthPoints[1]); // c > 0 to be in this block of code
                float b = 8.0f * math.dot(_lengthPoints[0], _lengthPoints[1]);
                float a = 4.0f * math.dot(_lengthPoints[0], _lengthPoints[0]); // a > 0 by assumption
                float q = 4.0f * a * c - b * b; // = 16*|Cross(A0,A1)| >= 0

                float mult0 = 0.25f / c;
                float sumCBA = c + b + a;
                float twoCpB = 2.0f * c + b;
                float mult1 = q / (8.0f * math.pow(c, 1.5f));
                length = mult0 * (twoCpB * math.sqrt(sumCBA) - b * math.sqrt(a)) + mult1 * (math.log(2.0f * math.sqrt(c * sumCBA) + twoCpB) - math.log(2.0f * math.sqrt(c * a) + b));
            }
            else
                length = 2.0f * math.length(_lengthPoints[0]);

            return length;
        }

        public override float3 GetPoint(float t)
        {
            //Wrap t if it is outside 0-1 range
            t = Helpers.Wrap(t);

            int currentCurve;

            if (UnityEngine.Mathf.Approximately(t, 1f))
            {
                t = 1f;
                currentCurve = _curveCount - 1;
            }
            else
            {
                //Evaluate the curve, then assign the remainder to t
                t = t * _curveCount;
                currentCurve = (int)t;
                t -= currentCurve;
            }

            int nodeIndex = currentCurve * 3;

            float3 pointA = points[nodeIndex];
            float3 pointB = points[nodeIndex + 1];
            float3 pointC = points[nodeIndex + 2];
            float3 pointD = points[nodeIndex + 3];

            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            float3 point = uuu * pointA; //First term
            point += 3f * uu * t * pointB; //Second term
            point += 3f * u * tt * pointC; //Third term
            point += ttt * pointD; //Fourth term

            return point;
        }

        private int _curveCount { get; }
        private static float3[] _lengthPoints { get; } = new float3[2];

        public BezierSplineSolver(float3[] nodes, int subdivisions = 5) : base(nodes, subdivisions) => _curveCount = (nodes.Length - 1) / 3;
    }
}