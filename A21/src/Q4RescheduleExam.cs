using System;
using System.Collections.Generic;
using System.Linq;
using TestCommon;
// ReSharper disable All

namespace A11
{
    public class Q4RescheduleExam : Processor
    {
        public Q4RescheduleExam(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, char[], long[][], char[]>)Solve);

        public static readonly char[] colors_3 = new char[] { 'R', 'G', 'B' };

        public override Action<string, string> Verifier =>
            TestTools.GraphColorVerifier;


        public virtual char[] Solve(long nodeCount, char[] colors, long[][] edges)
        {
            Clauses = new List<long[]>();

            foreach (var e in edges)
                foreach (var co in colors_3)
                    NotSimultaneously(new List<long> { VarNum((int)e[0], ColorToNum(co)), VarNum((int)e[1], ColorToNum(co)) });

            for (int i = 1; i <= nodeCount; i++) {
                Clauses.Add(new long[] { -VarNum(i, ColorToNum(colors[i - 1])), -VarNum(i, ColorToNum(colors[i - 1])) });
                
                var complementColors = colors_3.Except(new [] { colors[i - 1] }).Select(x => ColorToNum(x)).ToArray();
                ExactlyOneOf(new List<long> { VarNum(i, complementColors[0]), VarNum(i, complementColors[1]) });
            }

            var c = Clauses.Count;
            var v = 3 + 10 * (int) nodeCount;
            var cnf = Clauses.ToArray();

            var twoSatSolver = new Q1CircuitDesign("");
            var (possible, satAns) = twoSatSolver.Solve(v, c, cnf);

            if (!possible)
                return "Impossible".ToCharArray();

            var ans = satAns.Where(x => x > 0 && x % 10 < 4 && x % 10 != 0 && x > 10).Select(x => colors_3[((x % 10) - 1) % 3]).ToArray();

            return ans;
        }

        public void NotSimultaneously(List<long> literals)
        {
            Clauses.Add(literals.Select(l => -l).ToArray());
        }

        public List<long[]> Clauses;

        public void ExactlyOneOf(List<long> literals)
        {
            Clauses.Add(literals.ToArray());
            for (int i = 0; i < literals.Count; i++)
            {
                for (int j = i + 1; j < literals.Count; j++)
                {
                    Clauses.Add(new[] { -literals[i], -literals[j] });
                }
            }
        }

        public long VarNum(int i, int j)
        {
            return 10 * i + j;
        }

        public static int ColorToNum(char color)
        {
            switch (color)
            {
                case 'R':
                    return 1;
                case 'G':
                    return 2;
                case 'B':
                    return 3;
            }

            return -1;
        }

    }
}
