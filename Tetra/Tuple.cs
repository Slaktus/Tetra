namespace Tetra
{
    public struct Tuple<T, Y>
    {
        public T a { get; }
        public Y b { get; }

        public Tuple(T a = default, Y b = default)
        {
            this.a = a;
            this.b = b;
        }
    }
}