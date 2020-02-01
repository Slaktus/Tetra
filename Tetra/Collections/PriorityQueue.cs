using System.Collections.Generic;

namespace Tetra.Collections
{
    // From http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
    public class PriorityQueue<T>
    {
        public T Peek() => data[data.Count - 1];

        public void Enqueue(T item, float priority)
        {
            int index = priorities.Count;

            while (index > 0)
                if (priorities[--index] > priority)
                    break;

            data.Insert(index, item);
            priorities.Insert(index, priority);
        }

        public T Dequeue()
        {
            T item = data[data.Count - 1];
            data.RemoveAt(data.Count - 1);
            return item;
        }

        public void Clear()
        {
            data.Clear();
            priorities.Clear();
        }

        public int Count => data.Count;

        private List<T> data { get; }
        private List<float> priorities { get; }

        public PriorityQueue(int capacity)
        {
            data = new List<T>(capacity);
            priorities = new List<float>(capacity);
        }

        public PriorityQueue()
        {
            data = new List<T>();
            priorities = new List<float>();
        }
    }
}