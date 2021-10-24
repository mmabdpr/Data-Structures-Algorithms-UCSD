using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A6
{
    public class Q1ConstructBWT : Processor
    {
        public Q1ConstructBWT(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<String, String>)Solve);

        public string Solve(string text)
        {
            var shifts = new string[text.Length];
            for (int i = 0; i < text.Length; i++)
                shifts[i] = GetCyclicShift(text, i);
            
            Array.Sort(shifts);

            var ans = new char[text.Length];
            for (int i = 0; i < text.Length; i++) {
                ans[i] = shifts[i][text.Length - 1];
            }

            return string.Join("", ans);
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
