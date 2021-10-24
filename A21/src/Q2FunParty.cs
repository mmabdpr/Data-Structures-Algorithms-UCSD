using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestCommon;

namespace A11
{
    public class Q2FunParty : Processor
    {
        public Q2FunParty(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long,long[],long[][], long>)Solve);

        public virtual long Solve(long n, long[] funFactors, long[][] hierarchy)
        {
            Graph = new Dictionary<long, Vertex>();

            for (long i = 0; i < n; i++)
                Graph[i + 1] = new Vertex(i + 1, funFactors[i]);

            for (int i = 0; i < hierarchy.Length; i++) {
                Graph[hierarchy[i][0]].Adjs.Add(Graph[hierarchy[i][1]]);
                Graph[hierarchy[i][1]].Adjs.Add(Graph[hierarchy[i][0]]);
            }

            var ans = FindMaxFF();

            return ans;
        }

        private long FindMaxFF()
        {
            if (Graph.Count == 0)
                return 0;

            DFS(Graph[1], null);

            var maxFF = Graph[1].SubtreeFF;

            return maxFF;
        }

        private void DFS(Vertex v, Vertex p)
        {
            foreach (var child in v.Adjs) 
                if (child != p) 
                    DFS(child, v);
            
            var subffGrandChildren = 0L;
            var subffChildren = 0L;

            foreach (var child in v.Adjs) {
                if (child == p) continue;
                subffChildren += child.SubtreeFF;
                foreach (var gc in child.Adjs) {
                    if (gc == v) continue;
                    subffGrandChildren += gc.SubtreeFF;
                }
            }

            v.SubtreeFF = Math.Max(v.FF + subffGrandChildren, subffChildren);
        }

        public Dictionary<long, Vertex> Graph;

        public class Vertex
        {
            public long SubtreeFF { get; set; } = long.MaxValue;
            public long FF { get; set;}
            public long Key { get; private set; }
            public HashSet<Vertex> Adjs { get; private set; } = new HashSet<Vertex>();
            public Vertex(long key, long ff)
            {
                this.Key = key;
                this.FF = ff;
            }

            public IEnumerable<Vertex> Grandchildren {
                get {
                    foreach (var e in Adjs) {
                        foreach (var v in e.Adjs) {
                            yield return v;
                        }
                    }
                }
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"{this.Key} (");
                sb.Append(String.Join(", ", Adjs.Select(a => a.Key.ToString())));
                sb.Append(") " + (this.FF));
                return sb.ToString();
            }
        }
    }
}
