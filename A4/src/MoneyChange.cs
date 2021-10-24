using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A6
{
    public class MoneyChange: Processor
    {
        private static readonly int[] COINS = new int[] {1, 3, 4};

        public MoneyChange(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long>) Solve);



        public long Solve(long n)
        {
            int money = (int)n;
            var dp = Enumerable.Range(0, money+1).Select(x => long.MaxValue).ToList();
            dp[0] = 0;
            for (int i = 1; i < dp.Count; i++) {
                for (int j = 0; j < COINS.Length; j++) {
                    if (COINS[j] <= i) {
                        dp[i] = Math.Min(dp[i], 1 + dp[i - COINS[j]]);
                    }
                }
            }
            return dp[money];
        }
    }
}
