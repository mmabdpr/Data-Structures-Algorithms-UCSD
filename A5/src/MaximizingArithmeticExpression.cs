using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A7
{
    public class MaximizingArithmeticExpression : Processor
    {
        public MaximizingArithmeticExpression(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<string, long>)Solve);

        public long Solve(string expression)
        {
            (var nums, var ops) = ParseMathExpression(expression);
            var dpMin = Enumerable.Range(0, nums.Count()).Select(i => 
                Enumerable.Range(0, nums.Count()).Select(j => {
                    if (i != j) {
                        return long.MaxValue;
                    } else {
                        return nums[i];
                    }
                }).ToArray()).ToArray();
            var dpMax = Enumerable.Range(0, nums.Count()).Select(i => 
                Enumerable.Range(0, nums.Count()).Select(j => {
                    if (i != j) {
                        return -long.MaxValue;
                    } else {
                        return nums[i];
                    }
                }).ToArray()).ToArray();
            for (int l = 1; l < nums.Count(); l++) {
                for (int i = 0; i < nums.Count() - l; i++) {
                    var j = i + l;
                    for (int k = i; k < j; k++) {
                        var minTmp = 0L;
                        var maxTmp = 0L;
                        var tmpX = 0L;
                        var tmpY = 0L;
                        if (ops[k] == '+') {
                            minTmp = dpMin[i][k] + dpMin[k + 1][j];
                            maxTmp = dpMax[i][k] + dpMax[k + 1][j];
                            tmpX = dpMin[i][k] + dpMax[k + 1][j];
                            tmpY = dpMax[i][k] + dpMin[k + 1][j];
                        } else if (ops[k] == '*') {
                            minTmp = dpMin[i][k] * dpMin[k + 1][j];
                            maxTmp = dpMax[i][k] * dpMax[k + 1][j];
                            tmpX = dpMin[i][k] * dpMax[k + 1][j];
                            tmpY = dpMax[i][k] * dpMin[k + 1][j];
                        } else if (ops[k] == '-') {
                            minTmp = dpMin[i][k] - dpMin[k + 1][j];
                            maxTmp = dpMax[i][k] - dpMax[k + 1][j];
                            tmpX = dpMin[i][k] - dpMax[k + 1][j];
                            tmpY = dpMax[i][k] - dpMin[k + 1][j];
                        }
                        dpMin[i][j] = Enumerable.Min(new long[] { maxTmp, minTmp, tmpX, tmpY, dpMin[i][j]});
                        dpMax[i][j] = Enumerable.Max(new long[] { maxTmp, minTmp, tmpX, tmpY, dpMax[i][j]});
            
                    }
                }
            }
            return dpMax.First().Last();
        }

        
        private (List<long> nums, List<char> ops) ParseMathExpression(string expression) {
            var nums = new List<long>();
            var ops = new List<char>();
            foreach (var c in expression) {
                if (char.IsNumber(c)) {
                    nums.Add(long.Parse(c.ToString()));
                } else {
                    ops.Add(c);
                }
            }
            return (nums, ops);
        }
    }
}
