using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A6
{
    public class LCSOfThree: Processor
    {
        public LCSOfThree(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long[], long[], long[], long>)Solve);

        public long Solve(long[] seq1, long[] seq2, long[] seq3)
        {
            var dp = new long[seq1.Length + 1, seq2.Length + 1, seq3.Length + 1];
            for (int i = 0; i <= seq1.Length; i++) {
                for (int j = 0; j <= seq2.Length; j++) {
                    for (int k = 0; k <= seq3.Length; k++) {
                        if (i == 0 || j == 0 || k == 0) {
                            dp[i, j, k] = 0;
                        } else if (seq1[i - 1] == seq2[j - 1] && seq1[i - 1] == seq3[k - 1]) {
                            dp[i , j, k] = dp[i - 1, j - 1, k - 1] + 1;
                        } else {
                            dp[i , j, k] = Enumerable.Max(new long[] {
                                dp[i - 1, j - 1, k - 1],
                                dp[i, j - 1, k - 1],
                                dp[i - 1, j, k - 1],
                                dp[i - 1, j - 1, k],
                                dp[i, j, k - 1],
                                dp[i - 1, j, k],
                                dp[i, j - 1, k],
                            });
                        }
                    }
                }
            }
            return dp[seq1.Length, seq2.Length, seq3.Length];
        }
    }
}
