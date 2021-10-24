using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A7
{
    public class Q3PatternMatchingSuffixArray : Processor
    {
        public Q3PatternMatchingSuffixArray(string testDataName) : base(testDataName)
        {
            this.VerifyResultWithoutOrder = true;
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<string, long, string[], long[]>)Solve, "\n");

        private long[] Solve(string text, long n, string[] patterns)
        {
            text = $"{text}$";
            var order = Q2CunstructSuffixArray.BuildSuffixArray(text);

            var ans = new List<long>((int)n);
            for (int i = 0; i < n; i++) {
                var indexes = FindPattern(text, patterns[i], order);
                ans.AddRange(indexes);
            }

            return ans.Count == 0 ? new long[] {-1} : ans.Distinct().ToArray();
        }

        public static List<long> FindPattern(string text, string pattern, long[] order)
        {
            var ans = new List<long>();

            var l = 0;
            var r = text.Length;
            int mid;

            while (l < r) {
                mid = (l + r) / 2;
                if (string.CompareOrdinal(pattern, GetSuffixWithLengthOfPattern(text, order[mid], pattern.Length)) > 0)
                    l = mid + 1;
                else
                    r = mid;
            }
            var s = l;
            r = text.Length;

            while (l < r) {
                mid = (l + r) / 2;
                if (string.CompareOrdinal(pattern, GetSuffixWithLengthOfPattern(text, order[mid], pattern.Length)) < 0)
                    r = mid;
                else
                    l = mid + 1;
            }

            if (s < order.Length && string.CompareOrdinal(pattern, GetSuffixWithLengthOfPattern(text, order[s], pattern.Length)) == 0) {
                for (int i = s; i < r; i++)
                    ans.Add(order[i]);
            }

            return ans;
        }

        public static string GetSuffixWithLengthOfPattern(string s, long i, long l)
        {
            var ll = Math.Min(l, s.Length - i);
            return s.Substring((int)i, (int)ll);
        }
    }
}
