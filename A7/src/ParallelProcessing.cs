using System;
using System.Collections.Generic;
using System.Linq;

using A9.Generics;
using TestCommon;

namespace A9
{
    public class ParallelProcessing : Processor
    {
        public ParallelProcessing(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[], Tuple<long, long>[]>)Solve);

        public Tuple<long, long>[] Solve(long threadCount, long[] jobDuration)
        {
            var result = ParallelProcessing.Solve((int)threadCount, jobDuration);
            return result.ToArray();
        }

        public static List<Tuple<long, long>> Solve(int threadCount, long[] jobDuration)
        {
            var result = new List<Tuple<long, long>>((int)threadCount);
            var threads = new MaxHeap<Job>((int)threadCount);

            var count = Math.Min(threadCount, jobDuration.Length);
            int i = 0;
            while (i < count)
            {
                threads.Insert(new Job(0, jobDuration[i], i));
                result.Add(Tuple.Create((long)i, 0L));
                i++;
            }

            long tmpStartTime = 0;
            long freeThreadNumber = 0;

            while (i < jobDuration.Length)
            {
                if (threads.IsFull)
                {
                    var doneJob = threads.ExtractMax();
                    tmpStartTime = doneJob.FinishTime;
                    freeThreadNumber = doneJob.ThreadNumber;
                }

                threads.Insert(new Job(tmpStartTime, jobDuration[i], freeThreadNumber));
                result.Add(Tuple.Create(freeThreadNumber, tmpStartTime));
                
                i++;
            }

            return result;
        }
    }

    public class Job : IHeapValue<Job>
    {
        public long StartTime;
        public long Duration;
        public long FinishTime { get => StartTime + Duration; }
        public long ThreadNumber;

        public Job(long startTime, long duration, long threadNumber)
        {
            StartTime = startTime;
            Duration = duration;
            ThreadNumber = threadNumber;
        }

        public int ComparePriority(Job other)
        {
            if (FinishTime != other.FinishTime)
                return Math.Sign(other.FinishTime - FinishTime);

            return Math.Sign(other.ThreadNumber - ThreadNumber);
        }

    }
}
