
namespace Tetra
{
    public static class Helpers
    {
        public static float Wrap(float t) => t > 1f ? t - (int)t
                                                    : 0f > t
                                                        ? t - (int)t + 1
                                                        : t;

        public static float Normalize(float value, float min, float max) => (value - min) / (max - min);
    }
}