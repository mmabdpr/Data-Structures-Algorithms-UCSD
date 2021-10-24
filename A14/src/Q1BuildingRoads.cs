using System;
using TestCommon;

namespace A4
{
    public class Q1BuildingRoads : Processor
    {
        public Q1BuildingRoads(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], double>)Solve);


        public double Solve(long pointCount, long[][] points)
        {
            var costs = new double[pointCount];
            for (long i = 0; i < costs.Length; i++)
                costs[i] = double.MaxValue;
            costs[0] = 0;
            var proc = new bool[pointCount];
            while (true)
            {
                double minCost = double.MaxValue;
                long minCostIndex = -1;
                for (long i = 0; i < costs.Length; i++) {
                    if (!proc[i] && minCost > costs[i]) {
                        minCost = costs[i];
                        minCostIndex = i;
                    }
                }
                if (minCostIndex == -1) break;
                proc[minCostIndex] = true;
                var x1 = points[minCostIndex][0];
                var y1 = points[minCostIndex][1];
                for (long i = 0; i < pointCount; i++) {
                    if (proc[i] || i == minCostIndex) continue;
                    var x2 = points[i][0];
                    var y2 = points[i][1];
                    double d = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
                    if (costs[i] > d) {
                        costs[i] = d;
                    }
                }
            }
            
            double s = 0;
            for (int i = 0; i < costs.Length; i++)
                s += costs[i];

            return Math.Round(s, 6);
        }
    }
}
