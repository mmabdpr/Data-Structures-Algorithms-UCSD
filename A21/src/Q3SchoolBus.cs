using System;
using System.Collections.Generic;
using System.Linq;
using TestCommon;
// ReSharper disable All

namespace A11
{
    public class Q3SchoolBus : Processor
    {
        public Q3SchoolBus(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr)=>
            TestTools.Process(inStr, (Func<long, long[][], Tuple<long, long[]>>)Solve);

        public override Action<string, string> Verifier { get; set; } =
            TestTools.TSPVerifier;

        public virtual Tuple<long, long[]> Solve(long nodeCount, long[][] edges)
        {
            var matrix = new long[nodeCount + 1, nodeCount + 1];

            for (int i = 0; i <= nodeCount; i++)
                for (int j = 0; j <= nodeCount; j++)
                    matrix[i, j] = int.MaxValue;

            foreach (var edge in edges)
            {
                matrix[edge[0], edge[1]] = edge[2];
                matrix[edge[1], edge[0]] = edge[2];
            }

            var (subsets, subsetsWithSize) = GenerateAllSubsets((int) nodeCount);
            var cost = new long[1 << (int) nodeCount, nodeCount + 1];
            var path = new List<long>[1 << (int) nodeCount, nodeCount + 1];

            for (int i = 0; i < cost.GetLength(0); i++)
                for (int j = 0; j < cost.GetLength(1); j++)
                    cost[i, j] = int.MaxValue;
            cost[1, 1] = 0;
            path[1, 1] = new List<long> {1};

            for (int s = 2; s <= nodeCount; s++)
            {
                foreach (var mask in subsetsWithSize[s])
                {
                    if ((mask & 1) != 1) // must contain 1
                        continue;

                    // cost[mask, 1] = int.MaxValue;
                    var subset = subsets[mask];
                    foreach (var i in subset)
                    {
                        if (i == 1) continue;
                        foreach (var j in subset)
                        {
                            if (j == i) continue;
                            var maskWithoutI = mask & ~(1 << (i - 1));
                            if (cost[mask, i] <= cost[maskWithoutI, j] + matrix[j, i]) continue;
                            cost[mask, i] = cost[maskWithoutI, j] + matrix[j, i];
                            path[mask, i] = new List<long>(path[maskWithoutI, j].Append(i));
                        }
                    }
                }
            }

            long ans = int.MaxValue;
            var ansI = 1;
            var maskAllIn = (1 << (int) nodeCount) - 1;
            for (int i = 1; i <= nodeCount; i++)
            {
                if (ans <= cost[maskAllIn, i] + matrix[i, 1]) continue;
                ans = cost[maskAllIn, i] + matrix[i, 1];
                ansI = i;
            }
            
            if (ans >= int.MaxValue)
                return new Tuple<long, long[]>(-1, null);

            var ansPath = path[maskAllIn, ansI].ToArray();

            return new Tuple<long, long[]>(ans, ansPath);
        }

        private (Dictionary<int, List<int>>, List<int>[]) GenerateAllSubsets(int n)
        {
            var len = 1 << n;
            var subsets = new Dictionary<int, List<int>>(len);
            var subsetsWithSize = Enumerable.Range(0, n + 1).Select(x => new List<int>()).ToArray();

            for (int mask = 0; mask < len; mask++)
            {
                var subset = new List<int>();
                for (int i = 0; i < n; i++)
                    if (((mask >> i) & 1) == 1)
                        subset.Add(i + 1);
                
                subsets.Add(mask, subset);
                subsetsWithSize[subset.Count].Add(mask);
            }

            return (subsets, subsetsWithSize);
        }
    }
}
