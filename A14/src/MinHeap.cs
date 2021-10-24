using System;
using System.Collections.Generic;
using System.Linq;

namespace A4
{
    public class MinHeap<V, T> where V : IHeapValue<T> where T : IComparable<T>
    {
        private long size;
        private long maxSize;
        private T minValue;
        private V[] heap;
        public MinHeap(long maxSize, T minValue, V[] heap = null)
        {
            this.heap = new V[maxSize];
            this.maxSize = maxSize;
            this.minValue = minValue;

            if (heap == null)
                return;

            this.size = Math.Min(maxSize, heap.Length);
            this.heap = heap;
        }

        public IReadOnlyList<V> Heap { get => heap; }
        public long MaxSize { get => maxSize; }
        public long Size { get => size; }

        public void Insert(V val)
        {
            if (size == maxSize)
                throw new InsufficientMemoryException();

            size++;
            heap[size - 1] = val;
            SiftUp(size - 1);
        }

        public V ExtractMin()
        {
            var result = heap[0];
            heap[0] = heap[size - 1];
            heap[0].HeapIndex = 0;
            size--;
            SiftDown(0);
            return result;
        }

        public void Remove(long index)
        {
            heap[index].Priority = minValue;
            SiftUp(index);
            ExtractMin();
        }

        public void ChangePriority(long index, T newPriority)
        {
            var oldp = heap[index].Priority;
            heap[index].Priority = newPriority;
            if (newPriority.CompareTo(oldp) == 1)
                SiftDown(index);
            else
                SiftUp(index);
        }

        public void BuildHeap(V[] nums)
        {
            size = nums.Length;
            maxSize = Math.Max(maxSize, size);
            heap = nums;
            for (long i = 0; i < heap.Length; i++)
            {
                heap[i].HeapIndex = i;
            }
            for (long i = (size / 2); i >= 0; i--)
            {
                SiftDown(i);
            }
        }
        private long ParentIndex(long index)
        {
            return (index - 1) / 2;
        }

        private long LeftChildIndex(long index)
        {
            return 2 * index + 1;
        }

        private long RightChildIndex(long index)
        {
            return 2 * index + 2;
        }

        private void SiftUp(long index)
        {
            while (index > 0 && heap[ParentIndex(index)].Priority.CompareTo(heap[index].Priority) == 1)
            {
                Swap(index, ParentIndex(index));
                index = ParentIndex(index);
            }
        }

        private void SiftDown(long index)
        {
            var maxIndex = index;
            var l = LeftChildIndex(index);
            if (l < size && heap[l].Priority.CompareTo(heap[maxIndex].Priority) == -1)
                maxIndex = l;
            var r = RightChildIndex(index);
            if (r < size && heap[r].Priority.CompareTo(heap[maxIndex].Priority) == -1)
                maxIndex = r;
            if (index != maxIndex)
            {
                Swap(index, maxIndex);
                SiftDown(maxIndex);
            }
        }

        private void Swap(long index1, long index2)
        {
            heap[index1].HeapIndex = index2;
            heap[index2].HeapIndex = index1;
            (heap[index1], heap[index2]) = (heap[index2], heap[index1]);
        }
    }

    public interface IHeapValue<T> where T : IComparable<T>
    {
        long HeapIndex { get; set; }
        T Priority { get; set; }
    }
}