using Microsoft.SolverFoundation.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestCommon;

namespace A3
{
    public class Q1FrequencyAssignment : Processor
    {
        public Q1FrequencyAssignment(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<int, int, long[,], string[]>)Solve);

        public const int ColorCount = 3;
        
        public string[] Solve(int V, int E, long[,] matrix)
        {
            Clauses = new List<List<long>>(V);
            
            for (int i = 1; i <= V; i++)
                ExactlyOneOf(Enumerable.Range(1, ColorCount).Select(j => VarNum(i, j)).ToList());

            for (int edge = 0; edge < E; edge++) {
                int u = (int) matrix[edge, 0];
                int v = (int) matrix[edge, 1];

                for (int j = 1; j <= ColorCount; j++) {
                    NotSimultaneously(new List<long> { VarNum(u, j), VarNum(v, j) });
                }
            }

            var ans = Clauses.Select(c => $"{string.Join(' ', c)} 0").Prepend($"{Clauses.Count} {10000}").ToArray();
            return ans;
        }

        public void NotSimultaneously(List<long> literals)
        {
            Clauses.Add(literals.Select(l => -l).ToList());
        }

        public List<List<long>> Clauses;
        
        public void ExactlyOneOf(List<long> literals)
        {
            Clauses.Add(literals);
            for (int i = 0; i < literals.Count; i++) {
                for (int j = i + 1; j < literals.Count; j++) {
                    Clauses.Add(new List<long> { -literals[i], -literals[j] });
                }
            }
        }

        public long VarNum(int i, int j)
        {
            return 10 * i + j;
        }

        public override Action<string, string> Verifier { get; set; } =
            TestTools.SatVerifier;
    }
}
