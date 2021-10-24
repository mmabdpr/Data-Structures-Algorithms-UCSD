using TestCommon;
using System;

namespace A9
{
    public class ConvertIntoHeap : Processor
    {
        public ConvertIntoHeap(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long[], Tuple<long, long>[]>)Solve);

        public Tuple<long, long>[] Solve(
            long[] array)
        {
            var minHeap = new MinHeap(array.Length);
            minHeap.BuildHeap(array);
            var ans = minHeap.Swaps;
            return ans;
        }
    }

}