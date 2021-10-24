using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A8
{
    public class PacketProcessing : Processor
    {
        public PacketProcessing(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[], long[], long[]>)Solve);

        public long[] Solve(long bufferSize, 
            long[] arrivalTimes, 
            long[] processingTimes)
        {
            var packets = arrivalTimes.Select(
                (at, i) => new Packet(i, (int)at, (int)processingTimes[i])
            ).ToList();
            var result = PacketProcess(packets, (int)bufferSize);
            return result;
        }

        public static long[] PacketProcess(List<Packet> packets, int bufferSize) {
            var result = new long[packets.Count];
            var finishTime = new Queue<long>();
            long lastFinishTime = 0;
            foreach (var packet in packets) {
                while (finishTime.Count != 0 && finishTime.Peek() <= packet.ArrivalTime) { 
                    finishTime.Dequeue();
                }
                if (finishTime.Count < bufferSize) {
                    lastFinishTime = Math.Max(lastFinishTime, packet.ArrivalTime);
                    result[packet.Id] = lastFinishTime;
                    lastFinishTime += packet.ProcessingTime;
                    finishTime.Enqueue(lastFinishTime);
                } else {
                    result[packet.Id] = -1;
                }
            }
            return result.ToArray();
        }

    }
    public class Packet {
        public int Id;
        public int ArrivalTime;
        public int ProcessingTime;
        public Packet(int id, int arrivalTime, int processingTime) {
            Id = id;
            ArrivalTime = arrivalTime;
            ProcessingTime = processingTime;
        }
    }
}
