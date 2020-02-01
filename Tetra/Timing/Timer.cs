namespace Tetra.Timing
{
    public class Timer
    {
        public void SetTimeScale(float timeScale) => this.timeScale = timeScale;
        public void Update(float delta) => time += this.delta = beat.Update(delta * timeScale);

        public Beat beat { get; } = new Beat();
        public float time { get; private set; }
        public float delta { get; private set; }
        public float timeScale { get; private set; } = 1;
    }
}