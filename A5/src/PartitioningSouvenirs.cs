using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A7
{
    public class PartitioningSouvenirs : Processor
    {
        public PartitioningSouvenirs(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[], long>)Solve);

        public long Solve(long souvenirsCount, long[] souvenirs)
        {
            if (souvenirsCount == 0) {
                return 0;
            }
            var aSum = (int)Enumerable.Sum(souvenirs);
            if (aSum % 3 == 0) {
                var dp = Enumerable.Range(0, (int)souvenirsCount + 1).Select(_ => 
                    Enumerable.Range(0, aSum / 3 + 1).Select(x => Enumerable.Range(0, aSum / 3 + 1).
                        Select(i => false).ToArray()).ToArray()).ToArray();
                return DpIs3Sum(souvenirs, (int)souvenirsCount, aSum / 3, aSum / 3, dp) ? 1 : 0;
            } else {
                return 0;
            }
        }

        public bool DpIs3Sum(long[] A, int n, int subset_1, int subset_2, bool[][][] dp) {
            for (int i = 0; i < n + 1; i++) {
                for (int j = 0; j < subset_1 + 1; j++) {
                    for (int k = 0; k < subset_2 + 1; k++) {
                        if (j == 0 && k == 0) {
                            dp[i][j][k] = true;
                            continue;
                        }
                        if (i == 0 && (j != 0 || k != 0)) {
                            dp[i][j][k] = false;
                            continue;
                        }
                        if (A[i - 1] <= j && A[i - 1] <= k) {
                            dp[i][j][k] = dp[i - 1][j - A[i - 1]][k]
                                || dp[i - 1][j][k - A[i - 1]]
                                || dp[i - 1][j][k];
                            continue;
                        }
                        if (A[i - 1] <= j) {
                            dp[i][j][k] = dp[i - 1][j - A[i - 1]][k]
                                || dp[i - 1][j][k];
                            continue;
                        }
                        if (A[i - 1] <= k) {
                            dp[i][j][k] = dp[i - 1][j][k - A[i - 1]]
                                || dp[i - 1][j][k];
                            continue;
                        }
                        dp[i][j][k] = false;
                    }
                }
            }
            return dp[n][subset_1][subset_2];
        }

    }
}
