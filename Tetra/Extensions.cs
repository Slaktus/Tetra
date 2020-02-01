using Unity.Mathematics;

namespace Tetra
{
    public static class Extensions
    {
        public static float2 perpendicular(this float2 float2) => new float2(-float2.y, float2.x);
    }
}