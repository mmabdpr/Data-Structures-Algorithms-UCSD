using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A4
{
    public class Q2Clustering : Processor
    {
        public Q2Clustering(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long, double>)Solve);


        public double Solve(long pointCount, long[][] points, long clusterCount)
        {
            var costs = Enumerable.Range(0, (int)pointCount).Select(x => double.MaxValue).ToList();
            costs[0] = 0;
            var proc = new bool[pointCount];
            while (true)
            {
                double minCost = double.MaxValue;
                long minCostIndex = -1;
                for (int i = 0; i < costs.Count; i++) {
                    if (!proc[i] && minCost > costs[i]) {
                        minCost = costs[i];
                        minCostIndex = i;
                    }
                }
                if (minCostIndex == -1) break;
                proc[minCostIndex] = true;
                var x1 = points[minCostIndex][0];
                var y1 = points[minCostIndex][1];
                for (int i = 0; i < pointCount; i++) {
                    if (proc[i] || i == minCostIndex) continue;
                    var x2 = points[i][0];
                    var y2 = points[i][1];
                    double d = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
                    if (costs[i] > d) {
                        costs[i] = d;
                    }
                }
            }
            
            costs.Sort();
            double maxTC;
            long count = 0;
            do {
                maxTC = costs[costs.Count - 1];
                costs.RemoveAt(costs.Count - 1);
            } while (++count < clusterCount - 1);

            return Math.Round(maxTC, 6);
        }
    }
}
