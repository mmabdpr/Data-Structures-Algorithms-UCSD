using System;
using System.Collections.Generic;
using System.Linq;

namespace A9.Generics
{
    public class MinHeap<T> where T : IHeapValue<T>
    {
        private int size;
        private int maxSize;
        private T[] heap;
        public MinHeap(int maxSize, T[] heap = null)
        {
            this.heap = new T[maxSize];
            this.maxSize = maxSize;

            if (heap == null)
                return;

            this.size = Math.Min(maxSize, heap.Length);
            this.heap = heap;
        }

        public IReadOnlyList<T> Heap { get => heap; }
        public int MaxSize { get => maxSize; set => maxSize = value; }
        public int Size { get => size; set => size = value; }
        public bool IsFull { get => size == maxSize; }
        public bool IsEmpty { get => size == 0; }

        public int ParentIndex(int index)
        {
            return (int)Math.Floor((double)(index - 1) / 2);
        }

        public int LeftChildIndex(int index)
        {
            return 2 * index + 1;
        }

        public int RightChildIndex(int index)
        {
            return 2 * index + 2;
        }

        public void SiftUp(int index)
        {
            while (index > 0 && heap[ParentIndex(index)].ComparePriority(heap[index]) > 0)
            {
                Swap(index, ParentIndex(index));
                index = ParentIndex(index);
            }
        }

        public void SiftDown(int index)
        {
            var maxIndex = index;
            var l = LeftChildIndex(index);
            if (l < size && heap[l].ComparePriority(heap[maxIndex]) < 0)
                maxIndex = l;
            var r = RightChildIndex(index);
            if (r < size && heap[r].ComparePriority(heap[maxIndex]) < 0)
                maxIndex = r;
            if (index != maxIndex)
            {
                Swap(index, maxIndex);
                SiftDown(maxIndex);
            }
        }

        public void Insert(T val)
        {
            if (IsFull)
                throw new InsufficientMemoryException();

            size++;
            heap[size - 1] = val;
            SiftUp(size - 1);
        }

        public T PeekMin() 
        {
            if (IsEmpty) 
                throw new InvalidOperationException();

            return heap[0];
        }

        public T ExtractMin()
        {
            var result = heap[0];
            heap[0] = heap[size - 1];
            size--;
            SiftDown(0);
            return result;
        }

        public void BuildHeap(T[] items)
        {
            size = items.Length;
            maxSize = Math.Max(maxSize, size);
            heap = items;
            for (int i = (int)Math.Floor((double)size / 2); i >= 0; i--)
            {
                SiftDown(i);
            }
        }

        private void Swap(int index1, int index2)
        {
            var tmp = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = tmp;
        }
    }
}