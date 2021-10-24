using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A2
{
    public class Q2BipartiteGraph : Processor
    {
        public Q2BipartiteGraph(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long>)Solve);


        public long Solve(long nodeCount, long[][] edges)
        {
            Graph = Enumerable.Range(1, (int)nodeCount)
                .ToDictionary(x => (long)x, x => new Vertex(x));
            
            foreach (var e in edges) {
                Graph[e.First()].Adjs.Add(Graph[e.Last()]);
                Graph[e.Last()].Adjs.Add(Graph[e.First()]);
            }

            var isBP = BFS(1);
            return isBP ? 1 : 0;
        }

        public bool BFS(long startNode)
        {
            var q = new Queue<Vertex>();
            long c = -1;
            Graph[startNode].Color = c;
            q.Enqueue(Graph[startNode]);
            while (q.Any())
            {
                var v = q.Dequeue();
                c = -v.Color;
                foreach (var ve in v.Adjs) {
                    if (ve.Color == 0) {
                        ve.Color = c;
                        q.Enqueue(ve);
                    } else if (ve.Color == v.Color) {
                        return false;
                    }
                }
            }
            return true;
        }

        public Dictionary<long, Vertex> Graph;

        public class Vertex
        {
            public long Id { get; }
            public HashSet<Vertex> Adjs { get; set; } = new HashSet<Vertex>();
            public long Color { get; set; } = 0;
            public Vertex(long id) {
                Id = id;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"{this.Id} (");
                sb.Append(String.Join(", ", Adjs.Select(a => a.Id.ToString())));
                sb.Append($") of {this.Color}");
                return sb.ToString();
            }
        }
    }

}
