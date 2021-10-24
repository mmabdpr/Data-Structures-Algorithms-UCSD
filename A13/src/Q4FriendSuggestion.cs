using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medallion.Collections;
using TestCommon;

namespace A3
{
    public class Q4FriendSuggestion:Processor
    {
        public Q4FriendSuggestion(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long, long[][], long,long[][], long[]>)Solve);

        public long[] Solve(long nodeCount, long edgeCount, 
                              long[][] edges, long queriesCount, 
                              long[][] queries)
        {
            sw.Start();

            Graph = Enumerable.Range(0, (int)nodeCount)
                .Select(x => new Vertex((int)x))
                .ToArray();
            
            foreach (var e in edges) {
                Graph[(int)e[0] - 1].Adjs.Add((Graph[(int)e[1] - 1], (int)e[2]));
            }

            RGraph = Enumerable.Range(0, (int)nodeCount)
                .Select(x => new Vertex((int)x))
                .ToArray();
            
            foreach (var e in edges) {
                RGraph[(int)e[1] - 1].Adjs.Add((RGraph[(int)e[0] - 1], (int)e[2]));
            }

            var ans = new List<long>(10);
            foreach (var q in queries) {
                var startNode = q[0];
                var endNode = q[1];

                foreach (var v in Graph) {
                    v.Dist = long.MaxValue;
                }

                foreach (var v in RGraph) {
                    v.Dist = long.MaxValue;
                }

                preProcessTime += sw.ElapsedMilliseconds;
                sw.Restart();
                var d = BDDijkstra((int)startNode - 1, (int)endNode - 1);
                bdTime += sw.ElapsedMilliseconds;
                
                ans.Add(d);
                sw.Restart();
            }
            System.Console.WriteLine($"\n**** pt: {preProcessTime}, bt: {bdTime}");
            return ans.ToArray();
        }

        public long BDDijkstra(int startNode, int endNode)
        {
            var startV = Graph[startNode];
            startV.Dist = 0;
            var reached = new HashSet<Vertex>();
            reached.Add(startV);
            var pQ = new MinHeap<Vertex>(Graph.Length, new Vertex(-1) { Dist = -long.MaxValue });
            pQ.BuildHeap(Graph.ToArray());
            var proc = new bool[Graph.Length];
            
            var startVR = RGraph[endNode];
            startVR.Dist = 0;
            var reachedR = new HashSet<Vertex>();
            reachedR.Add(startVR);
            var pQR = new MinHeap<Vertex>(RGraph.Length, new Vertex(-1) { Dist = -long.MaxValue });
            pQR.BuildHeap(RGraph.ToArray());
            var procR = new bool[RGraph.Length];
            
            while (reached.Any() || reachedR.Any()) {
                if (reached.Any()) {
                    var u = pQ.ExtractMin();
                    reached.Remove(u);
                    proc[u.Id] = true;
                    foreach (var (vertex, weight) in u.Adjs) {
                        if (!proc[vertex.Id] && u.Dist + weight < vertex.Dist) {
                            vertex.Dist = u.Dist + weight;
                            pQ.ChangePriority(vertex.HeapIndex, vertex);
                            reached.Add(vertex);
                        }
                    }
                    if (procR[u.Id]) {
                        return ShortestPath(proc, procR);
                    }
                }

                if (reachedR.Any()) {
                    var u = pQR.ExtractMin();
                    reachedR.Remove(u);
                    procR[u.Id] = true;
                    foreach (var (vertex, weight) in u.Adjs) {
                        if (!procR[vertex.Id] && u.Dist + weight < vertex.Dist) {
                            vertex.Dist = u.Dist + weight;
                            pQR.ChangePriority(vertex.HeapIndex, vertex);
                            reachedR.Add(vertex);
                        }
                    }
                    if (proc[u.Id]) {
                        return ShortestPath(proc, procR);
                    }
                }
            }
            return -1;
        }

        public static long preProcessTime = 0;
        public static long bdTime = 0;
        public static Stopwatch sw = new Stopwatch();

        public long ShortestPath(bool[] proc, bool[] procR)
        {
            
            var d = long.MaxValue;
            for (int i = 0; i < proc.Length; i++) {
                if ((proc[i] ||
                    procR[i]) &&
                    Graph[i].Dist != long.MaxValue && 
                    RGraph[i].Dist != long.MaxValue) {
                        d = Math.Min(d, Graph[i].Dist + RGraph[i].Dist);
                    }
            }

            return d == long.MaxValue ? -1 : d;
        }

        public Vertex[] Graph;
        public Vertex[] RGraph;

        public class Vertex : IHeapValue<Vertex>    
        {
            public int Id { get; }
            public List<(Vertex Vertex, int Weight)> Adjs { get; set; } = new List<(Vertex Vertex, int Weight)>();
            public long Dist { get; set; } = long.MaxValue;
            public int HeapIndex { get; set; } = -1;

            public Vertex(int id) {
                Id = id;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"{this.Id} (");
                sb.Append(String.Join(", ", Adjs.Select(a => a.Vertex.Id.ToString())));
                sb.Append($") of {this.Dist} p?");
                return sb.ToString();
            }

            public int CompareTo(Vertex other)
            {
                return this.Dist.CompareTo(other.Dist);
            }
        }
    }
}
