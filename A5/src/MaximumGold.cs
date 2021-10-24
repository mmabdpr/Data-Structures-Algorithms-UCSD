using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A7
{
    public class MaximumGold : Processor
    {
        public MaximumGold(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[], long>)Solve);

        public long Solve(long W, long[] goldBars)
        {
            var n = goldBars.Length;
            var dp = Enumerable.Range(0, (int)W + 1).Select(_ => 
                Enumerable.Range(0, n + 1).Select(i => -1L).ToArray()).ToArray();
            for (int i = 0; i < W + 1; i++) {
                for (int j = 0; j < n + 1; j++) {
                    if (i == 0 || j == 0) {
                        dp[i][j] = 0;
                        continue;
                    }
                    if (goldBars[j - 1] > i) {
                        dp[i][j] = dp[i][j - 1];
                    } else {
                        dp[i][j] = Enumerable.Max(
                            new long[] {
                                dp[i][j - 1],
                                dp[i - goldBars[j - 1]][j - 1] + goldBars[j - 1]
                            }
                        );
                    }
                }   
            }
            return dp[(int)W][n];
        }
    }
}
