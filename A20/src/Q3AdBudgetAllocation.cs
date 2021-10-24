using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A3
{
    public class Q3AdBudgetAllocation : Processor
    {
        public Q3AdBudgetAllocation(string testDataName)
            : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long, long[][], long[], string[]>)Solve);

        public string[] Solve(long eqCount, long varCount, long[][] A, long[] b)
        {
            Clauses = new List<List<long>>();

            for (int eq = 0; eq < eqCount; eq++) {
                var vars = FindVariables(A[eq]);
                var pCount = Math.Pow(2, vars.Count);
                for (int p = 0; p < pCount; p++) {
                    var potentialClause = new List<long>(3);
                    var eqVal = 0;
                    int perm = p;
                    for (int k = 0; k < vars.Count; k++) {
                        if (perm % 2 == 1) {
                            eqVal += vars[k].v;
                            potentialClause.Add(-vars[k].i);
                        }
                        else {
                            potentialClause.Add(vars[k].i);
                        }

                        perm = perm >> 1;
                    }

                    if (eqVal > b[eq]) {
                        Clauses.Add(potentialClause);
                    }
                }
            }
            
            var ans = Clauses.Select(c => $"{string.Join(' ', c)} 0").Prepend($"{Clauses.Count} {10000}").ToArray();
            return ans;
        }

        public List<(int i, int v)> FindVariables(long[] row)
        {
            var ans = new List<(int i, int v)>(3);

            for (int col = 0; col < row.Length; col++) {
                if (row[col] != 0) 
                    ans.Add((col + 1, (int) row[col]));
            }

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
