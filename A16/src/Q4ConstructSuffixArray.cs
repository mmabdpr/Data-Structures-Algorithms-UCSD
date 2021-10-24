using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A6
{
    public class Q4ConstructSuffixArray : Processor
    {
        public Q4ConstructSuffixArray(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) => 
            TestTools.Process(inStr, (Func<String, long[]>)Solve);

        public long[] Solve(string text)
        {
            var ans = new long[text.Length];
            var shifts = new string[text.Length];
            for (int i = 0; i < text.Length; i++)
                shifts[i] = GetCyclicShift(text, i);
            
            Array.Sort(shifts);

            for (int i = 0; i < shifts.Length; i++) {
                var untilXLength = shifts[i].IndexOf('$') + 1;
                ans[i] = text.Length - untilXLength;
            }

            return ans;
        }

        public string GetCyclicShift(string text, int n)
        {
            n = n % text.Length;
            var preAns = new StringBuilder(n);
            preAns.Append(text.Substring(text.Length - n, n));
            preAns.Append(text.Substring(0, text.Length - n));
            return preAns.ToString();
        }
    }
}
