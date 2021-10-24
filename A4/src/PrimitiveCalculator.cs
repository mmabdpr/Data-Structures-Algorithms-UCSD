using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A6
{
    public class PrimitiveCalculator: Processor
    {
        public PrimitiveCalculator(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[]>)Solve);

        public long[] Solve(long n)
        {
            var dp = Enumerable.Range(0, (int)n+1).Select(x => long.MaxValue).ToArray();
            dp[0] = 0;
            dp[1] = 0;
            int i;
            for (i = 2; i < n+1; i++) {
                dp[i] = Math.Min(dp[i], 1 + dp[i - 1]);
                if (i % 2 == 0) {
                    dp[i] = Math.Min(dp[i], 1 + dp[i / 2]);
                }
                if (i % 3 == 0) {
                    dp[i] = Math.Min(dp[i], 1 + dp[i / 3]);
                }
            }
            var interNums = new List<long>();
            i = (int)n;
            while (i != 1) {
                int nextI = i - 1;
                if (i % 2 == 0) {
                    if (dp[nextI] >= dp[i / 2]) {
                        nextI = i / 2;
                    }
                }
                if (i % 3 == 0) {
                    if (dp[nextI] >= dp[i / 3]) {
                        nextI = i / 3;
                    }
                }
                interNums.Add(nextI);
                i = nextI;
            }
            interNums.Reverse();
            interNums.Add((int)n);
            return interNums.ToArray();
        }
    }
}
