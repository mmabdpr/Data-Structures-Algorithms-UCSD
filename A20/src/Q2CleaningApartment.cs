using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A3
{
    public class Q2CleaningApartment : Processor
    {
        public Q2CleaningApartment(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<int, int, long[,], string[]>)Solve);

        public string[] Solve(int V, int E, long[,] matrix)
        {
            Clauses = new List<List<long>>();


            for (int i = 1; i <= V; i++)
                ExactlyOneOf(Enumerable.Range(1, V).Select(j => VarNum(i, j)).ToList());

            for (int j = 1; j <= V; j++)
                ExactlyOneOf(Enumerable.Range(1, V).Select(i => VarNum(i, j)).ToList());

            var adjMatrix = new bool[V+1, V+1];
            for (int edge = 0; edge < E; edge++) {
                int u = (int) matrix[edge, 0];
                int v = (int) matrix[edge, 1];

                adjMatrix[u, v] = true;
                adjMatrix[v, u] = true;
            }

            for (int i = 1; i <= V; i++) {
                for (int j = 1; j <= V; j++) {
                    if (adjMatrix[i, j]) continue;
                    for (int k = 1; k <= V - 1; k++) {
                        NotSimultaneously(new List<long> {VarNum(i, k), VarNum(j, k + 1)});
                        NotSimultaneously(new List<long> {VarNum(j, k), VarNum(i, k + 1)});
                    }
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
