using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A7
{
    public class Q2CunstructSuffixArray : Processor
    {
        public Q2CunstructSuffixArray(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<String, long[]>)Solve);

        private long[] Solve(string text)
        {
            return BuildSuffixArray(text);
        }

        public static long[] BuildSuffixArray(string s)
        {
            var order = SortCharacters(s);
            var cls = ComputeCharClasses(s, order);
            var l = 1;
            while (l < s.Length) {
                order = SortDoubled(s, l, order, cls);
                cls = UpdateClasses(order, cls, l);
                l = 2 * l;
            }
            return order;
        }

        public static long[] SortCharacters(string s)
        {
            var order = new long[s.Length];
            var count = new long[256];
            for (int i = 0; i < s.Length; i++)
                count[s[i]]++;
            for (int i = 1; i < 256; i++)
                count[i] += count[i - 1];
            for (int i = s.Length - 1; i >= 0; i--) {
                char c = s[i];
                count[c]--;
                order[count[c]] = i;
            }
            return order;
        }

        public static long[] ComputeCharClasses(string s, long[] order)
        {
            var cls = new long[s.Length];
            cls[order[0]] = 0;
            for (int i = 1; i < s.Length; i++) {
                if (s[(int)order[i]] != s[(int)order[i - 1]])
                    cls[order[i]] = cls[order[i - 1]] + 1;
                else
                    cls[order[i]] = cls[order[i - 1]];
            }
            return cls;
        }

        public static long[] SortDoubled(string s, long l, long[] order, long[] cls) {
            var count = new long[s.Length];
            var newOrder = new long[s.Length];
            for (int i = 0; i < s.Length; i++)
                count[cls[i]]++;
            for (int i = 1; i < s.Length; i++)
                count[i] += count[i - 1];
            for (int i = s.Length - 1; i >= 0; i--) {
                var start = (order[i] - l + s.Length) % s.Length;
                var cl = cls[start];
                count[cl] = count[cl] - 1;
                newOrder[count[cl]] = start;
            }
            return newOrder;
        }

        public static long[] UpdateClasses(long[] newOrder, long[] cls, long l) {
            var n = newOrder.Length;
            var newClass = new long[n];
            newClass[newOrder[0]] = 0;
            for (int i = 1; i < n; i++) {
                var cur = newOrder[i];
                var prev = newOrder[i - 1];
                var mid = (cur + l) % n;
                var midPrev = (prev + l) % n;
                if (cls[cur] != cls[prev] || cls[mid] != cls[midPrev])
                    newClass[cur] = newClass[prev] + 1;
                else
                    newClass[cur] = newClass[prev];
            }
            return newClass;
        }
    }
}
