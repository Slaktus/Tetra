using Unity.Mathematics;

namespace Tetra.Splines
{
    public sealed class CatmullRomSplineSolver : SplineSolver
    {
        public override float3 GetPoint(float t)
        {
            int numberOfSections = points.Length - 3;
            int currentNode = (int)math.min(math.floor(t * numberOfSections), numberOfSections - 1);

            float3 pointA = points[currentNode];
            float3 pointB = points[currentNode + 1];
            float3 pointC = points[currentNode + 2];
            float3 pointD = points[currentNode + 3];
            float u = (t * numberOfSections) - currentNode;

            return 0.5f *
                (
                    (-pointA + 3f * pointB - 3f * pointC + pointD) * (u * u * u)
                    + (2f * pointA - 5f * pointB + 4f * pointC - pointD) * (u * u)
                    + (-pointA + pointC) * u
                    + 2f * pointB
                );
        }

        public CatmullRomSplineSolver(float3[] nodes, int subdivisions = 5) : base(nodes, subdivisions) { }
    }
}