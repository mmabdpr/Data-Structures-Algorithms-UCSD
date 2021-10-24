using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A6
{
    public class LCSOfTwo: Processor
    {
        public LCSOfTwo(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long[], long[], long>)Solve);

        public long Solve(long[] seq1, long[] seq2)
        {
            var dp = new long[seq1.Length + 1, seq2.Length + 1];
            for (int i = 0; i <= seq1.Length; i++) {
                for (int j = 0; j <= seq2.Length; j++) {
                    if (i == 0 || j == 0) {
                        dp[i, j] = 0;
                    } else if (seq1[i - 1] == seq2[j - 1]) {
                        dp[i , j] = dp[i - 1, j - 1] + 1;
                    } else {
                        dp[i , j] = Enumerable.Max(new long[] {
                            dp[i - 1, j - 1],
                            dp[i, j - 1],
                            dp[i - 1, j]
                        });
                    }
                }
            }
            return dp[seq1.Length, seq2.Length];
        }
    }
}
