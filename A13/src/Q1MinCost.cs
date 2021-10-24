using System;
// using SCG = System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;
// using C5;
using System.Collections.Generic;
using System.Diagnostics;

namespace A3
{
    public class Q1MinCost:Processor
    {
        public Q1MinCost(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long,long,long>)Solve);


        public long Solve(long nodeCount,long[][] edges, long startNode, long endNode)
        {
            Graph = Enumerable.Range(1, (int)nodeCount)
                .ToDictionary(x => (long)x, x => new Vertex(x));
            
            foreach (var e in edges) {
                Graph[e[0]].Adjs.Add((Graph[e[1]], (int)e[2]));
            }

            var sw = new Stopwatch();
            sw.Start();
            Dijkstra(startNode);
            sw.Stop();
            System.Console.WriteLine("****" + sw.ElapsedMilliseconds);

            return Graph[endNode].Dist == long.MaxValue ? -1 : Graph[endNode].Dist;
        }

        public void Dijkstra(long startNode)
        {
            var startV = Graph[startNode];
            startV.Dist = 0;
            var pQA = new List<Vertex>();
            // var pQ = new C5.IntervalHeap<Vertex>(Comparer<Vertex>.Create((u, v) => u.Dist > v.Dist ? 1 : (u.Dist < v.Dist ? -1 : 0)));
            // var pQQ = new SortedSet<Vertex>(new VertexComparer());
            // pQ.Add(startV);
            // pQQ.Add(startV);
            pQA.Add(startV);
            while (pQA.Any()) {
                // var uu = pQQ.Min();
                // pQQ.Remove(uu);
                // var u = pQ.DeleteMin();
                var u = pQA.Min();
                pQA.Remove(u);
                // if (uu != u) {
                //     // throw new Exception($"\n***** {uu}\n{u}\n");
                //     int asdfs = 12;
                // }
                u.Processed = true;
                foreach (var e in u.Adjs) {
                    if (!e.Vertex.Processed && u.Dist + e.Weight < e.Vertex.Dist) {
                        // if (pQQ.Contains(e.Vertex)) {
                        //     pQQ.Remove(e.Vertex);
                        // }
                        if (pQA.Contains(e.Vertex)) {
                            pQA.Remove(e.Vertex);
                        }
                        e.Vertex.Dist = u.Dist + e.Weight;
                        e.Vertex.Pre = u;
                        // pQ.Add(e.Vertex);
                        // pQQ.Add(e.Vertex);
                        pQA.Add(e.Vertex);
                    }
                }
            }
        }

        public Dictionary<long, Vertex> Graph;

        public class Vertex : IComparable<Vertex>    
        {
            public long Id { get; }
            public HashSet<(Vertex Vertex, int Weight)> Adjs { get; set; } = new HashSet<(Vertex Vertex, int Weight)>();
            public long Dist { get; set; } = long.MaxValue;
            public bool Processed { get; set; } = false;
            public Vertex Pre { get; set; } = null;
            public Vertex(long id) {
                Id = id;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"{this.Id} (");
                sb.Append(String.Join(", ", Adjs.Select(a => a.Vertex.Id.ToString())));
                sb.Append($") of {this.Dist} p? {this.Processed}");
                return sb.ToString();
            }

            public int CompareTo(Vertex other)
            {
                return this.Dist.CompareTo(other.Dist);
            }
        }

        public class VertexComparer : IComparer<Vertex>
        {
            public int Compare(Vertex x, Vertex y)
            {
                return x.Dist.CompareTo(y.Dist);
            }
        }
    }
}
