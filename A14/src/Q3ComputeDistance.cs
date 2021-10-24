using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A4
{
    public class Q3ComputeDistance : Processor
    {
        public Q3ComputeDistance(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long,long, long[][], long[][], long, long[][], long[]>)Solve);

        public long[] Solve(long nodeCount, 
                            long edgeCount,
                            long[][] points,
                            long[][] edges,
                            long queriesCount,
                            long[][] queries)
        {
            long i = -1;
            Graph = points
                .Select(x => new Vertex(++i, points[i][0], points[i][1]))
                .ToArray();
            
            foreach (var e in edges) {
                Graph[e[0] - 1].Adjs.Add((Graph[e[1] - 1], e[2]));
            }

            i = -1;
            RGraph = points
                .Select(x => new Vertex(++i, points[i][0], points[i][1]))
                .ToArray();
            
            foreach (var e in edges) {
                RGraph[e[1] - 1].Adjs.Add((RGraph[e[0] - 1], e[2]));
            }

            var ans = new List<long>(10);
            foreach (var q in queries) {
                var startNode = q[0];
                var endNode = q[1];

                foreach (var v in Graph) {
                    v.Dist = MaxValue;
                    v.Potential = v.EuclidianDist(Graph[endNode - 1]);
                }

                foreach (var v in RGraph) {
                    v.Dist = MaxValue;
                    v.Potential = v.EuclidianDist(RGraph[startNode - 1]);
                }

                var d = AStar(startNode - 1, endNode - 1);
                
                ans.Add(d);
            }
            // using (var outputFile = new StreamWriter("/home/mm/q3.log", true))
            // {
            //     outputFile.WriteLine($"Total:{nodeCount}, AvgVisitCount:{ProcCount / queriesCount}");
            // }
            return ans.ToArray();
        }

        public long AStar(long startNode, long endNode)
        {
            var startV = Graph[startNode];
            startV.Dist = 0;
            var reached = new HashSet<Vertex>();
            reached.Add(startV);
            var pQ = new MinHeap<Vertex, long>(Graph.Length, -MaxValue);
            pQ.BuildHeap(Graph.ToArray());
            var proc = new bool[Graph.Length];
            
            while (reached.Any()) {
                var u = pQ.ExtractMin();
                ProcCount++;
                if (u.Id == endNode) break;
                reached.Remove(u);
                proc[u.Id] = true;
                foreach (var (vertex, weight) in u.Adjs) {
                    if (proc[vertex.Id]) continue;
                    if (u.Dist + weight - u.Potential + vertex.Potential < vertex.Dist) {
                        pQ.ChangePriority(vertex.HeapIndex, u.Dist + weight - u.Potential + vertex.Potential);
                        reached.Add(vertex);
                    }
                }
            }
            if (Graph[endNode].Dist == MaxValue) return -1; 
            return Graph[endNode].Dist + Graph[startNode].Potential - Graph[endNode].Potential;
        }

        public static readonly long MaxValue = long.MaxValue;
        public long ProcCount { get; set; }

        public Vertex[] Graph;
        public Vertex[] RGraph;

        public class Vertex : IHeapValue<long>    
        {
            public long Id { get; }
            public long X { get; set; }
            public long Y { get; set; }
            public long Potential { get; set; }
            public List<(Vertex Vertex, long Weight)> Adjs { get; set; } = new List<(Vertex Vertex, long Weight)>();
            public long Dist { get; set; } = MaxValue;
            public long HeapIndex { get; set; } = -1;
            public long Priority { get => Dist; set => Dist = value; }
            public Vertex(long id, long x, long y) {
                Id = id;
                X = x;
                Y = y;
            }

            public long EuclidianDist(Vertex other)
            {
                return (long)Math.Sqrt(Math.Pow(this.X - other.X, 2) + Math.Pow(this.Y - other.Y, 2));
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"{this.Id} (");
                sb.Append(String.Join(", ", Adjs.Select(a => a.Vertex.Id.ToString())));
                sb.Append($") of {this.Dist} p? {this.Potential}");
                return sb.ToString();
            }

            public int CompareTo(Vertex other)
            {
                return this.Dist.CompareTo(other.Dist);
            }
        }

    }

}
