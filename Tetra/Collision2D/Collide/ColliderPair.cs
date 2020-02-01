namespace Tetra.Collision
{
    internal struct ColliderPair
    {
        public AbstractCollider a { get; }
        public AbstractCollider b { get; }

        public override int GetHashCode() => a.GetHashCode() + b.GetHashCode();

        public ColliderPair(AbstractCollider a, AbstractCollider b)
        {
            this.a = a;
            this.b = b;
        }
    }
}