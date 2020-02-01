using Unity.Mathematics;

namespace Tetra.Splines
{
    public sealed class CubicBezierSplineSolver : SplineSolver
    {
        public override float3 GetPoint(float t)
        {
            float d = 1f - t;
            return d * d * d * points[0] + 3f * d * d * t * points[1] + 3f * d * t * t * points[2] + t * t * t * points[3];
        }

        public CubicBezierSplineSolver(float3[] nodes, int subdivisions = 5) : base(nodes, subdivisions) { }
    }
}