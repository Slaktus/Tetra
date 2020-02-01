using Unity.Mathematics;
using Tetra.Splines;

namespace Tetra.Tweening
{
    public sealed class IntTween : PrimitiveTween<int>
    {
        public override void UpdateValue() => value = (int) Easing.Ease(ease, from, to, math.clamp(elapsedTime - delay, 0, duration), duration);

        public IntTween(int from , int to , float duration , Easing.Type ease , bool start = true, float delay = 0) : base(duration, ease, start, delay) => SetFromTo(from, to);
        public IntTween() : base() { }
    }

    public sealed class FloatTween : PrimitiveTween<float>
    {
        public override void UpdateValue() => value = Easing.Ease(ease, from, to, math.clamp(elapsedTime - delay, 0, duration), duration);

        public FloatTween(float from, float to, float duration, Easing.Type ease, bool start = true, float delay = 0) : base(duration, ease, start, delay) => SetFromTo(from, to);
        public FloatTween() : base() { }
    }

    public sealed class Float2Tween : PrimitiveTween<float2>
    {
        public override void UpdateValue() => value = Easing.Ease(ease, from, to, math.clamp(elapsedTime - delay, 0, duration), duration);

        public Float2Tween(float2 from, float2 to, float duration, Easing.Type ease, bool start = true, float delay = 0) : base(duration, ease, start, delay) => SetFromTo(from, to);
        public Float2Tween() : base() { }
    }

    public sealed class Float3Tween : PrimitiveTween<float3>
    {
        public override void UpdateValue() => value = Easing.Ease(ease, from, to, math.clamp(elapsedTime - delay, 0, duration), duration);

        public Float3Tween(float3 from, float3 to, float duration, Easing.Type ease, bool start = true, float delay = 0) : base(duration, ease, start, delay) => SetFromTo(from, to);
        public Float3Tween() : base() { }
    }

    public sealed class Float4Tween : PrimitiveTween<float4>
    {
        public override void UpdateValue() => value = Easing.Ease(ease, from, to, math.clamp(elapsedTime - delay, 0, duration), duration);

        public Float4Tween(float4 from, float4 to, float duration, Easing.Type ease, bool start = true, float delay = 0) : base(duration, ease, start, delay) => SetFromTo(from, to);
        public Float4Tween() : base() { }
    }

    public sealed class QuaternionTween : PrimitiveTween<quaternion>
    {
        public override void UpdateValue() => value = Easing.Ease(ease, from, to, math.clamp(elapsedTime - delay, 0, duration), duration);

        public QuaternionTween(quaternion from, quaternion to, float duration, Easing.Type ease, bool start = true, float delay = 0) : base(duration, ease, start, delay) => SetFromTo(from, to);
        public QuaternionTween() : base() { }
    }

    public class SplineTween : Tween<float3>
    {
        public override void UpdateValue() => value = _spline.GetPointOnPath(Easing.Ease(ease, math.clamp(elapsedTime - delay, 0, duration), 0, duration, duration));

        public void SetSpline(Spline spline)
        {
            _spline = spline;
            _spline.BuildPath();

            if (state == State.Running)
                UpdateValue();
        }

        private Spline _spline;

        public SplineTween(Spline spline, float duration, Easing.Type ease, bool start = true, float delay = 0) : base(duration, ease, start, delay) => SetSpline(spline);
        public SplineTween() : base() { }
    }

    public abstract class PrimitiveTween<T> : Tween<T> where T : struct
    {
        public void Restart()
        {
            elapsedTime = 0;
            value = from;
            Start();
        }

        public T from { get; set; }
        public T to { get; set; }

        public void SetFromTo(T from, T to, bool forceUpdate = false)
        {
            this.from = from;
            this.to = to;

            if (state == State.Running || forceUpdate)
                UpdateValue();
        }

        public PrimitiveTween(float duration, Easing.Type ease, bool start = false, float delay = 0) : base(duration, ease, start, delay) { }
        public PrimitiveTween() : base() { }
    }

    public abstract class Tween<T> : Tween where T : struct
    {
        public T Update(float delta)
        {
            if (state != State.Running)
                return value;

            elapsedTime += delta;

            if (!delayed)
            {
                if (elapsedTime - delay >= duration)
                {
                    elapsedTime = duration + delay;
                    Stop();
                }

                if (duration >= elapsedTime - delay)
                    UpdateValue();
            }

            return value;
        }

        public T SetElapsedTime(float elapsedTime)
        {
            this.elapsedTime = math.clamp(elapsedTime, 0f, duration + delay);
            UpdateValue();
            return value;
        }

        public T value { get; protected set; }

        protected Tween(float duration, Easing.Type ease, bool start = false, float delay = 0) : base(ease, start ? State.Running : State.Paused, duration, delay) { }

        protected Tween() : base() { }
    }

    public abstract class Tween
    {
        public abstract void UpdateValue();
        public void Pause() => state = State.Paused;
        public void Stop() => state = State.Complete;
        public void Start() => state = State.Running;
        public void SetDelay(float delay) => this.delay = delay;
        public void SetEase(Easing.Type ease) => this.ease = ease;
        public void SetDuration(float duration) => this.duration = duration;

        public void Complete()
        {
            state = State.Complete;
            elapsedTime = duration + delay;
            UpdateValue();
        }

        public bool delayed => delay > elapsedTime;

        public Easing.Type ease { get; private set; }
        public State state { get; private set; }

        protected float duration { get; private set; }
        protected float elapsedTime { get; set; }
        protected float delay { get; set; }

        public enum State
        {
            Paused,
            Running,
            Complete
        }

        protected Tween(Easing.Type ease , State state, float duration, float delay = 0)
        {
            this.ease = ease;
            this.state = state;
            this.delay = delay;
            this.duration = duration;
        }

        protected Tween() { }
    }
}
