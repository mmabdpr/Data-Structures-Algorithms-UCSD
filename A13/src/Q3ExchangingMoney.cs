using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;
namespace A3
{
    public class Q3ExchangingMoney:Processor
    {
        public Q3ExchangingMoney(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long, string[]>)Solve);

        public string[] Solve(long nodeCount, long[][] edges,long startNode)
        {
            Graph = Enumerable.Range(1, (int)nodeCount)
                .ToDictionary(x => (long)x, x => new Vertex(x));
            
            foreach (var e in edges) {
                Graph[e[0]].Adjs.Add((Graph[e[1]], (int)e[2]));
            }

            BellmanFord(startNode);

            var ans = new List<string>();
            for (int i = 1; i <= nodeCount; i++) {
                if (!Graph[i].Reachable) {
                    ans.Add("*");
                } else if (Graph[i].OnNegCycle) {
                    ans.Add("-");
                } else {
                    ans.Add(Graph[i].Dist.ToString());
                }
            }

            return ans.ToArray();
        }

        public void BellmanFord(long startNode)
        {
            Graph[startNode].Dist = 0;
            Graph[startNode].Reachable = true;
            for (int i = 0; i < Graph.Count - 1; i++) {
                foreach (var v in Graph.Values) {
                    foreach (var e in v.Adjs) {
                        if ((v.Dist != int.MaxValue || e.Vertex.Dist != int.MaxValue) 
                                && v.Dist + e.Weight < e.Vertex.Dist) {
                            e.Vertex.Dist = v.Dist + e.Weight;
                            e.Vertex.Reachable = true;
                        }
                    }
                }
            }

            var onNegCyVs = new HashSet<Vertex>();
            foreach (var v in Graph.Values) {
                foreach (var e in v.Adjs) {
                    if (e.Vertex.Reachable &&
                        (v.Dist != int.MaxValue || e.Vertex.Dist != int.MaxValue) 
                                && v.Dist + e.Weight < e.Vertex.Dist) {
                        e.Vertex.OnNegCycle = true;
                        onNegCyVs.Add(e.Vertex);
                    }
                }
            }

            var q = new Queue<Vertex>(onNegCyVs);
            while (q.Any()) {
                var ve = q.Dequeue();
                foreach (var nve in ve.Adjs) {
                    if (!nve.Vertex.BFSProcessed) {
                        q.Enqueue(nve.Vertex);
                        nve.Vertex.OnNegCycle = true;
                    }
                }
                ve.BFSProcessed = true;
            }
        }

        public Dictionary<long, Vertex> Graph;

        public class Vertex    
        {
            public long Id { get; }
            public HashSet<(Vertex Vertex, int Weight)> Adjs { get; set; } = new HashSet<(Vertex Vertex, int Weight)>();
            public long Dist { get; set; } = (long)int.MaxValue;
            public bool OnNegCycle { get; set; } = false;
            public bool Reachable = false;
            public bool BFSProcessed = false;
            public Vertex(long id) {
                Id = id;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"{this.Id} (");
                sb.Append(String.Join(", ", Adjs.Select(a => a.Vertex.Id.ToString())));
                sb.Append($") d: {this.Dist}");
                sb.Append($" nc: {this.OnNegCycle}");
                sb.Append($" re: {this.Reachable}");
                return sb.ToString();
            }
        }
    }
}
