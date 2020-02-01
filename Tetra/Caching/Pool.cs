using System.Collections.Generic;
using System;

namespace Tetra.Caching
{
    public class Pool<T> : Pool
    {
        public T Get() => (available.Count > 0 ? available : Fill()).Pop();
        public void Put(T instance) => available.Push(instance);

        private Stack<T> Fill()
        {
            while (fillCount > available.Count)
                available.Push(factory());

            return available;
        }

        private Stack<T> available { get; } = new Stack<T>();
        private Func<T> factory { get; }

        public Pool(Func<T> factory, int fillCount = 10) : base(fillCount)
        {
            this.factory = factory;
            Fill();
        }
    }

    public abstract class Pool 
    {
        protected int fillCount { get; }

        protected Pool(int fillCount) => this.fillCount = fillCount;
    }
}