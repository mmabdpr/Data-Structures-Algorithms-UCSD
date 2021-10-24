using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A7
{
    public class Q1FindAllOccur : Processor
    {
        public Q1FindAllOccur(string testDataName) : base(testDataName)
        {
			this.VerifyResultWithoutOrder = true;
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<String, String, long[]>)Solve, "\n");

        public long[] Solve(string text, string pattern)
        {
            var newString = $"{pattern}${text}";
            var s = ComputePrefixFunction(newString);
            var ans = new List<long>();
            for (int i = pattern.Length + 1; i <= newString.Length - 1; i++)
                if (s[i] == pattern.Length)
                    ans.Add(i - 2 * pattern.Length);

            return ans.Count == 0 ? new long[] {-1} : ans.ToArray();
        }

        public long[] ComputePrefixFunction(string p)
        {
            var s = new long[p.Length];
            s[0] = 0;
            var border = 0;
            
            for (int i = 1; i < p.Length; i++) {
                while (border > 0 && p[i] != p[border])
                    border = (int) s[border - 1];
                if (p[i] == p[border])
                    border++;
                else
                    border = 0;
                s[i] = border;
            }            

            return s;
        }
    }
}
