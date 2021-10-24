using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A2
{
    public class Q1ShortestPath : Processor
    {
        public Q1ShortestPath(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long,long[][], long, long, long>)Solve);
        

        public long Solve(long nodeCount, long[][] edges, long startNode,  long endNode)
        {
            Graph = Enumerable.Range(1, (int)nodeCount)
                .ToDictionary(x => (long)x, x => new Vertex(x));
            
            foreach (var e in edges) {
                Graph[e.First()].Adjs.Add(Graph[e.Last()]);
                Graph[e.Last()].Adjs.Add(Graph[e.First()]);
            }

            BFS(startNode);
            var d = Graph[endNode].Dist;

            return d;
        }

        public void BFS(long startNode)
        {
            var q = new Queue<Vertex>();
            long d = 0;
            Graph[startNode].Dist = d++;
            q.Enqueue(Graph[startNode]);
            while (q.Any())
            {
                var v = q.Dequeue();
                d = v.Dist + 1;
                foreach (var ve in v.Adjs) {
                    if (ve.Dist == -1) {
                        ve.Dist = d;
                        q.Enqueue(ve);
                    }
                }
            }
        }

        public Dictionary<long, Vertex> Graph;

        public class Vertex
        {
            public long Id { get; }
            public HashSet<Vertex> Adjs { get; set; } = new HashSet<Vertex>();
            public long Dist { get; set; } = -1;
            public Vertex(long id) {
                Id = id;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"{this.Id} (");
                sb.Append(String.Join(", ", Adjs.Select(a => a.Id.ToString())));
                sb.Append($") of {this.Dist}");
                return sb.ToString();
            }
        }
    }
}
