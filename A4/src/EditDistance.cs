using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A6
{
    public class EditDistance: Processor
    {
        public EditDistance(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<string, string, long>)Solve);

        public long Solve(string str1, string str2)
        {
            var dp = new long[str1.Length + 1, str2.Length + 1];
            for (int i = 0; i <= str1.Length; i++) {
                for (int j = 0; j <= str2.Length; j++) {
                    if (i == 0) {
                        dp[i, j] = j;
                    } else if (j == 0) {
                        dp[i, j] = i;
                    } else if (str1[i - 1] == str2[j - 1]) {
                        dp[i, j] = dp[i - 1, j - 1];
                    } else {
                        dp[i, j] = Enumerable.Min(new long[] {
                            dp[i - 1, j - 1],
                            dp[i - 1, j],
                            dp[i, j - 1]
                        }) + 1;
                    }
                }
            }
            return dp[str1.Length, str2.Length];
        }

    }
}
