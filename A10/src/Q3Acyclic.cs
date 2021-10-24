using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestCommon;

namespace A12
{
    public class Q3Acyclic : Processor
    {
        public Q3Acyclic(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long>)Solve);

        public long Solve(long nodeCount, long[][] edges)
        {
            Graph = new Dictionary<long, Vertex>();
            foreach (var e in edges) {
                if (!Graph.ContainsKey(e.First()))
                    Graph[e.First()] = new Vertex(e.First());
                
                if (!Graph.ContainsKey(e.Last()))
                    Graph[e.Last()] = new Vertex(e.Last());

                Graph[e.First()].Adjs.Add(Graph[e.Last()]);
            }

            DFS();

            return HasCycle ? 1 : 0;
        }

        public bool HasCycle = false;
        public Dictionary<long, Vertex> Graph;
        public long CC = 1;


        public void DFS()
        {
            foreach (var v in Graph.Values)
                v.Visited = Status.UNVISITED;

            CC = 1;
            HasCycle = false;
            foreach (var v in Graph.Values) {
                if (v.Visited == Status.UNVISITED) {
                    Explore(v);
                    CC++;
                }
            }
        }

        public void Explore(Vertex v)
        {
            v.Visited = Status.BEING_VISITED;
            v.CCNum = CC;
            foreach (var w in v.Adjs)
                if (w.Visited == Status.UNVISITED)
                    Explore(w);
                else if (w.Visited == Status.BEING_VISITED)
                    HasCycle = true;
            v.Visited = Status.VISITED;
        }

        public enum Status
        {
            UNVISITED,
            BEING_VISITED,
            VISITED,
        }

        public class Vertex
        {
            public Status Visited { get; set; }
            public long CCNum { get; set; }
            public long Key { get; private set; }
            public HashSet<Vertex> Adjs { get; private set; }
            public Vertex(long key, Status visited = Status.UNVISITED, long ccNum = -1)
            {
                this.Key = key;
                this.Visited = visited;
                this.CCNum = ccNum;
                Adjs = new HashSet<Vertex>();
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"{this.Key} (");
                sb.Append(String.Join(", ", Adjs.Select(a => a.Key.ToString())));
                sb.Append($") of {this.CCNum}");
                return sb.ToString();
            }
        }
    }
}