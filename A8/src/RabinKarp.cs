using System;
using System.Collections.Generic;
using System.Linq;
using TestCommon;

namespace A10
{
    public class RabinKarp : Processor
    {
        public RabinKarp(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<string, string, long[]>)Solve);

        public long[] Solve(string pattern, string text)
        {
            var p = 1000000007;
            var rand = new Random();
            var x = rand.Next(1, p - 1);
            List<long> occurrences = new List<long>();
            long pHash = HashingWithChain.PolyHash(pattern, p, x);
            long[] h = PreComputeHashes(text, pattern.Length, p, x);
            for (int i = 0; i <= text.Length - pattern.Length; i++)
            {
                if (pHash != h[i])
                    continue;
                
                if (string.Join("", text.Skip(i).Take(pattern.Length)) == pattern)
                    occurrences.Add(i);
            }
            return occurrences.ToArray();
        }

        public static long[] PreComputeHashes(
            string T, 
            int P, 
            int p, 
            int x)
        {
            var h = new long[T.Length - P + 1];
            var s = string.Join("", T.Skip(T.Length - P));
            h[T.Length - P]  = HashingWithChain.PolyHash(s, p, x);
            long y = 1;
            for (int i = 1; i <= P; i++)
            {   
                var tmp = (y * x);
                y = ((tmp % p) + p) % p;
            }
            for (int i = T.Length - P - 1; i >= 0; i--)
            {
                long tmp = (x * h[i + 1] + T[i] - y * T[i + P]);
                h[i] = ((tmp % p) + p) % p;
            }
            return h;
        }
    }
}
