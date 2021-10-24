using System;
using System.Collections.Generic;
using System.Linq;
using TestCommon;

namespace A10
{
    public class HashingWithChain : Processor
    {
        public HashingWithChain(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, string[], string[]>)Solve);


        public string[] Solve(long bucketCount, string[] commands)
        {
            List<string> result = new List<string>();
            hashSet = new StrHashSet((int)bucketCount);
            foreach (var cmd in commands)
            {
                var toks = cmd.Split();
                var cmdType = toks[0];
                var arg = toks[1];

                switch (cmdType)
                {
                    case "add":
                        Add(arg);
                        break;
                    case "del":
                        Delete(arg);
                        break;
                    case "find":
                        result.Add(Find(arg));
                        break;
                    case "check":
                        result.Add(Check(int.Parse(arg)));
                        break;
                }
            }
            return result.ToArray();
        }

        public StrHashSet hashSet;
        public const long BigPrimeNumber = 1000000007;
        public const long ChosenX = 263;

        public static int PolyHash(
            string str,
            long p = BigPrimeNumber, long x = ChosenX)
        {
            long hash = 0;
            for (int i = str.Length - 1; i >= 0; --i)
                hash = (hash * x + str[i]) % p;
            return (int)hash;
        }

        public void Add(string str)
        {
            hashSet.Add(str);
        }

        public string Find(string str)
        {
            return hashSet.Find(str) ? "yes" : "no";
        }

        public void Delete(string str)
        {
            hashSet.Remove(str);
        }

        public string Check(int i)
        {
            return hashSet.Check(i);
        }
    }
}
