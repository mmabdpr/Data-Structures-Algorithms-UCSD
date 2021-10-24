using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;
namespace A3
{
    public class Q2DetectingAnomalies:Processor
    {
        public Q2DetectingAnomalies(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long>)Solve);


        public long Solve(long nodeCount, long[][] edges)
        {
            // BellmanFord
            var d = Enumerable.Range(0, (int)nodeCount).Select(x => (long)int.MaxValue).ToArray();
            d[0] = 0;

            for (int i = 0; i < nodeCount - 1; i++) {
                foreach (var e in edges) {
                    if (d[e[0] - 1] + e[2] < d[e[1] - 1]) {
                        d[e[1] - 1] = d[e[0] - 1] + e[2];
                    }
                }
            }

            foreach (var e in edges) {
                if (d[e[0] - 1] + e[2] < d[e[1] - 1]) {
                    return 1;
                }
            }

            return 0;
        }
    }
}
