using Unity.Mathematics;

namespace Tetra.Tweening
{
    public static class Easing
    {
        public static float UnclampedLerp(float from, float to, float t) => from + (to - from) * t;

        public static float2 UnclampedLerp(float2 from, float2 to, float t) => new float2(UnclampedLerp(from.x, to.x, t), UnclampedLerp(from.y, to.y, t));

        public static float3 UnclampedLerp(float3 from, float3 to, float t) => new float3(UnclampedLerp(from.xy, to.xy, t), UnclampedLerp(from.z, to.z, t));

        public static float4 UnclampedLerp(float4 from, float4 to, float t) => new float4(UnclampedLerp(from.xyz, to.xyz, t), UnclampedLerp(from.w, to.w, t));

        public static float Ease(Type ease, float from, float to, float t, float duration) => UnclampedLerp(from, to, Ease(ease, t, duration));

        public static float2 Ease(Type ease, float2 from, float2 to, float t, float duration) => UnclampedLerp(from, to, Ease(ease, t, duration));

        public static float3 Ease(Type ease, float3 from, float3 to, float t, float duration) => UnclampedLerp(from, to, Ease(ease, t, duration));

        public static float4 Ease(Type ease, float4 from, float4 to, float t, float duration) => UnclampedLerp(from, to, Ease(ease, t, duration));

        public static quaternion Ease(Type ease, UnityEngine.Quaternion from, UnityEngine.Quaternion to, float t, float duration) => math.nlerp(from, to, Ease(ease, t, duration));

        internal static class Linear
        {
            public static float EaseNone(float t, float d) => t / d;
        }

        internal static class Quadratic
        {
            public static float EaseIn(float t, float d) => (t /= d) * t;

            public static float EaseOut(float t, float d) => -1 * (t /= d) * (t - 2);

            public static float EaseInOut(float t, float d) => ((t /= d / 2) < 1) ? 0.5f * t * t : -0.5f * ((--t) * (t - 2) - 1);
        }

        internal static class Back
        {
            public static float EaseIn(float t, float d) => (t /= d) * t * ((1.70158f + 1) * t - 1.70158f);

            public static float EaseOut(float t, float d) => ((t = t / d - 1) * t * ((1.70158f + 1) * t + 1.70158f) + 1);

            public static float EaseInOut(float t, float d)
            {
                float s = 1.70158f;

                return ((t /= d / 2) < 1)
                    ? 0.5f * (t * t * (((s *= 1.525f) + 1) * t - s))
                    : 0.5f * ((t -= 2) * t * (((s *= 1.525f) + 1) * t + s) + 2);
            }
        }

        internal static class Bounce
        {
            public static float EaseOut(float t, float d) => ((t /= d) < (1 / 2.75))
                    ? (7.5625f * t * t)
                    : (t < (2 / 2.75))
                        ? (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f)
                        : (t < (2.5 / 2.75))
                            ? (7.5625f * (t -= 2.25f / 2.75f) * t + .9375f)
                            : (7.5625f * (t -= 2.625f / 2.75f) * t + .984375f);

            public static float EaseIn(float t, float d) => 1 - EaseOut(d - t, d);

            public static float EaseInOut(float t, float d) => (t < d / 2) ? EaseIn(t * 2, d) * 0.5f : EaseOut(t * 2 - d, d) * .5f + 1 * 0.5f;
        }

        internal static class Circular
        {
            public static float EaseIn(float t, float d) => -(math.sqrt(1 - (t /= d) * t) - 1);

            public static float EaseOut(float t, float d) => math.sqrt(1 - (t = t / d - 1) * t);

            public static float EaseInOut(float t, float d) => ((t /= d / 2) < 1) ? -0.5f * (math.sqrt(1 - t * t) - 1) : 0.5f * (math.sqrt(1 - (t -= 2) * t) + 1);
        }

        internal static class Cubic
        {
            public static float EaseIn(float t, float d) => (t /= d) * t * t;

            public static float EaseOut(float t, float d) => ((t = t / d - 1) * t * t + 1);

            public static float EaseInOut(float t, float d) => ((t /= d / 2) < 1) ? 0.5f * t * t * t : 0.5f * ((t -= 2) * t * t + 2);
        }

        internal class Elastic
        {
            public static float EaseIn(float t, float d)
            {
                if (t == 0)
                    return 0;

                if ((t /= d) == 1)
                    return 1;

                float p = d * 0.3f;
                float s = p / 4;
                return -(1 * math.pow(2, 10 * (t -= 1)) * math.sin((t * d - s) * (2 * math.PI) / p));
            }

            public static float EaseOut(float t, float d)
            {
                if (t == 0)
                    return 0;

                if ((t /= d) == 1)
                    return 1;

                float p = d * 0.3f;
                float s = p / 4;
                return (1 * math.pow(2, -10 * t) * math.sin((t * d - s) * (2 * math.PI) / p) + 1);
            }

            public static float EaseInOut(float t, float d)
            {
                if (t == 0)
                    return 0;

                if ((t /= d / 2) == 2)
                    return 1;

                float p = d * (.3f * 1.5f);
                float s = p / 4;

                return 1 > t ? -0.5f * (math.pow(2, 10 * (t -= 1)) * math.sin((t * d - s) * (2 * math.PI) / p)) : (math.pow(2f, -10f * (t -= 1f)) * math.sin((t * d - s) * (2 * math.PI) / p) * 0.5f + 1f);
            }

            public static float Punch(float t, float d)
            {
                if (t == 0)
                    return 0;

                if ((t /= d) == 1)
                    return 0;

                const float p = 0.3f;

                return (math.pow(2, -10 * t) * math.sin(t * (2 * math.PI) / p));
            }
        }

        internal static class Exponential
        {
            public static float EaseIn(float t, float d) => (t == 0) ? 0 : math.pow(2, 10 * (t / d - 1));

            public static float EaseOut(float t, float d) => t == d ? 1 : (-math.pow(2, -10 * t / d) + 1);

            public static float EaseInOut(float t, float d)
            {
                if (t == 0)
                    return 0;

                if (t == d)
                    return 1;

                if ((t /= d / 2) < 1)
                    return 0.5f * math.pow(2, 10 * (t - 1));

                return 0.5f * (-math.pow(2, -10 * --t) + 2);
            }
        }

        internal static class Quartic
        {
            public static float EaseIn(float t, float d) => (t /= d) * t * t * t;

            public static float EaseOut(float t, float d) => -1 * ((t = t / d - 1) * t * t * t - 1);

            public static float EaseInOut(float t, float d)
            {
                t /= d / 2;

                if (t < 1)
                    return 0.5f * t * t * t * t;

                t -= 2;
                return -0.5f * (t * t * t * t - 2);
            }
        }

        internal static class Quintic
        {
            public static float EaseIn(float t, float d) => (t /= d) * t * t * t * t;

            public static float EaseOut(float t, float d) => (t = t / d - 1) * t * t * t * t + 1;

            public static float EaseInOut(float t, float d)
            {
                if ((t /= d / 2) < 1)
                    return 0.5f * t * t * t * t * t;

                return 0.5f * ((t -= 2) * t * t * t * t + 2);
            }
        }

        internal static class Sinusoidal
        {
            public static float EaseIn(float t, float d) => -1 * math.cos(t / d * (math.PI / 2)) + 1f;

            public static float EaseOut(float t, float d) => math.sin(t / d * (math.PI / 2));

            public static float EaseInOut(float t, float d) => -0.5f * (math.cos(math.PI * t / d) - 1);
        }

        internal static float Ease(Type ease, float t, float duration)
        {
            switch (ease)
            {
                case Type.Linear:
                    return Linear.EaseNone(t, duration);

                case Type.BackIn:
                    return Back.EaseIn(t, duration);
                case Type.BackOut:
                    return Back.EaseOut(t, duration);
                case Type.BackInOut:
                    return Back.EaseInOut(t, duration);

                case Type.BounceIn:
                    return Bounce.EaseIn(t, duration);
                case Type.BounceOut:
                    return Bounce.EaseOut(t, duration);
                case Type.BounceInOut:
                    return Bounce.EaseInOut(t, duration);

                case Type.CircIn:
                    return Circular.EaseIn(t, duration);
                case Type.CircOut:
                    return Circular.EaseOut(t, duration);
                case Type.CircInOut:
                    return Circular.EaseInOut(t, duration);

                case Type.CubicIn:
                    return Cubic.EaseIn(t, duration);
                case Type.CubicOut:
                    return Cubic.EaseOut(t, duration);
                case Type.CubicInOut:
                    return Cubic.EaseInOut(t, duration);

                case Type.ElasticIn:
                    return Elastic.EaseIn(t, duration);
                case Type.ElasticOut:
                    return Elastic.EaseOut(t, duration);
                case Type.ElasticInOut:
                    return Elastic.EaseInOut(t, duration);
                case Type.Punch:
                    return Elastic.Punch(t, duration);

                case Type.ExpoIn:
                    return Exponential.EaseIn(t, duration);
                case Type.ExpoOut:
                    return Exponential.EaseOut(t, duration);
                case Type.ExpoInOut:
                    return Exponential.EaseInOut(t, duration);

                case Type.QuadIn:
                    return Quadratic.EaseIn(t, duration);
                case Type.QuadOut:
                    return Quadratic.EaseOut(t, duration);
                case Type.QuadInOut:
                    return Quadratic.EaseInOut(t, duration);

                case Type.QuartIn:
                    return Quartic.EaseIn(t, duration);
                case Type.QuartOut:
                    return Quartic.EaseOut(t, duration);
                case Type.QuartInOut:
                    return Quartic.EaseInOut(t, duration);

                case Type.QuintIn:
                    return Quintic.EaseIn(t, duration);
                case Type.QuintOut:
                    return Quintic.EaseOut(t, duration);
                case Type.QuintInOut:
                    return Quintic.EaseInOut(t, duration);

                case Type.SineIn:
                    return Sinusoidal.EaseIn(t, duration);
                case Type.SineOut:
                    return Sinusoidal.EaseOut(t, duration);
                case Type.SineInOut:
                    return Sinusoidal.EaseInOut(t, duration);

                default:
                    return Linear.EaseNone(t, duration);
            }
        }

        public enum Type
        {
            Linear,

            SineIn,
            SineOut,
            SineInOut,

            QuadIn,
            QuadOut,
            QuadInOut,

            CubicIn,
            CubicOut,
            CubicInOut,

            QuartIn,
            QuartOut,
            QuartInOut,

            QuintIn,
            QuintOut,
            QuintInOut,

            ExpoIn,
            ExpoOut,
            ExpoInOut,

            CircIn,
            CircOut,
            CircInOut,

            ElasticIn,
            ElasticOut,
            ElasticInOut,
            Punch,

            BackIn,
            BackOut,
            BackInOut,

            BounceIn,
            BounceOut,
            BounceInOut
        }
    }
}