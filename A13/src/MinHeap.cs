using System;
using System.Collections.Generic;
using System.Linq;

namespace A3
{
    public class MinHeap<T> where T : IHeapValue<T>
    {
        private int size;
        private int maxSize;
        private T minValue;
        private T[] heap;
        public MinHeap(int maxSize, T minValue, T[] heap = null)
        {
            this.heap = new T[maxSize];
            this.maxSize = maxSize;
            this.minValue = minValue;

            if (heap == null)
                return;

            this.size = Math.Min(maxSize, heap.Length);
            this.heap = heap;
        }

        public IReadOnlyList<T> Heap { get => heap; }
        public int MaxSize { get => maxSize; set => maxSize = value; }
        public int Size { get => size; set => size = value; }

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
            while (index > 0 && heap[ParentIndex(index)].CompareTo(heap[index]) == 1)
            {
                Swap(index, ParentIndex(index));
                index = ParentIndex(index);
            }
        }

        public void SiftDown(int index)
        {
            var maxIndex = index;
            var l = LeftChildIndex(index);
            if (l < size && heap[l].CompareTo(heap[maxIndex]) == -1)
                maxIndex = l;
            var r = RightChildIndex(index);
            if (r < size && heap[r].CompareTo(heap[maxIndex]) == -1)
                maxIndex = r;
            if (index != maxIndex)
            {
                Swap(index, maxIndex);
                SiftDown(maxIndex);
            }
        }

        public void Insert(T val)
        {
            if (size == maxSize)
                throw new InsufficientMemoryException();

            size++;
            heap[size - 1] = val;
            SiftUp(size - 1);
        }

        public T ExtractMin()
        {
            var result = heap[0];
            heap[0] = heap[size - 1];
            size--;
            SiftDown(0);
            return result;
        }

        public void Remove(int index)
        {
            heap[index] = minValue;
            SiftUp(index);
            ExtractMin();
        }

        public void ChangePriority(int index, T priority)
        {
            var oldp = heap[index];
            heap[index] = priority;
            if (priority.CompareTo(oldp) == 1)
                SiftDown(index);
            else
                SiftUp(index);
        }

        public void BuildHeap(T[] nums)
        {
            size = nums.Length;
            maxSize = Math.Max(maxSize, size);
            heap = nums;
            for (int i = 0; i < heap.Length; i++)
            {
                heap[i].HeapIndex = i;
            }
            for (int i = (int)Math.Floor((double)size / 2); i >= 0; i--)
            {
                SiftDown(i);
            }
        }

        private void Swap(int index1, int index2)
        {
            heap[index1].HeapIndex = index2;
            heap[index2].HeapIndex = index1;
            var tmp = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = tmp;
        }
    }

    public interface IHeapValue<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}