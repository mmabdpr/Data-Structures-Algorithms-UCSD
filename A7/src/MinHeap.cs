using System;
using System.Collections.Generic;
using System.Linq;

namespace A9 {
    public class MinHeap {
        private int size;
        private int maxSize;
        private long[] heap;
        private List<Tuple<long, long>> swaps = new List<Tuple<long, long>>();
        public MinHeap(int maxSize, long[] heap = null) {
            this.heap = new long[maxSize];
            this.maxSize = maxSize;
            
            if (heap == null)
                return;
            
            this.size = Math.Min(maxSize, heap.Length);
            this.heap = heap;
        }

        public IReadOnlyList<long> Heap { get => heap; }
        public int MaxSize { get => maxSize; set => maxSize = value; }
        public int Size { get => size; set => size = value; }
        public Tuple<long, long>[] Swaps { get => swaps.ToArray(); }

        public int ParentIndex(int index) {
            return (int)Math.Floor((double)(index - 1) / 2);
        }
        
        public int LeftChildIndex(int index) {
            return 2 * index + 1;
        }

        public int RightChildIndex(int index) {
            return 2 * index + 2;
        }

        public void SiftUp(int index) {
            while (index > 0 && heap[ParentIndex(index)] > heap[index]) {
                Swap(index, ParentIndex(index));
                index = ParentIndex(index);
            }
        }

        public void SiftDown(int index) {
            var maxIndex = index;
            var l = LeftChildIndex(index);
            if (l < size && heap[l] < heap[maxIndex])
                maxIndex = l;
            var r = RightChildIndex(index);
            if (r < size && heap[r] < heap[maxIndex])
                maxIndex = r;
            if (index != maxIndex) {
                Swap(index, maxIndex);
                SiftDown(maxIndex);
            }
        }

        public void Insert(long val) {
            if (size == maxSize)
                throw new InsufficientMemoryException();
            
            size++;
            heap[size - 1] = val;
            SiftUp(size - 1);
        }

        public long ExtractMin() {
            var result = heap[0];
            heap[0] = heap[size - 1];
            size--;
            SiftDown(0);
            return result;  
        }

        public void Remove(int index) {
            heap[index] = -long.MaxValue;
            SiftUp(index);
            ExtractMin();
        }

        public void ChangePriority(int index, int priority) {
            var oldp = heap[index];
            heap[index] = priority;
            if (priority > oldp)
                SiftDown(index);
            else
                SiftUp(index);
        }

        public void BuildHeap(long[] nums) {
            size = nums.Length;
            maxSize = Math.Max(maxSize, size);
            heap = nums;
            for (int i = (int)Math.Floor((double)size / 2); i >= 0; i--) {
                SiftDown(i);
            }
        }

        private void Swap(int index1, int index2) {
            var tmp = heap[index1];
            heap[index1] = heap[index2];
            heap[index2] = tmp;
            swaps.Add(new Tuple<long, long>(index1, index2));
        }
    }
}