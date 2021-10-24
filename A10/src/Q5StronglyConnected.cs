using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestCommon;

namespace A12
{
    public class Q5StronglyConnected: Processor
    {
        public Q5StronglyConnected(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long>)Solve);

        public long Solve(long nodeCount, long[][] edges)
        {
            RGraph = new Dictionary<long, Vertex>();
            Graph = new Dictionary<long, Vertex>();
            for (long i = 1; i <= nodeCount; i++) {
                RGraph[i] = new Vertex(i);
                Graph[i] = new Vertex(i);
            }

            foreach (var e in edges) {
                RGraph[e.Last()].Adjs.Add(RGraph[e.First()]);
                Graph[e.First()].Adjs.Add(Graph[e.Last()]);
            }

            PostVisited = new List<long>();
            DFS();
            SCCs();

            return CC;
        }

        public Dictionary<long, Vertex> RGraph;
        public Dictionary<long, Vertex> Graph;
        public long CC = 1;
        public long Clock = 1;

        public void DFS()
        {
            foreach (var v in RGraph.Values)
                v.Visited = false;

            foreach (var v in RGraph.Values) {
                if (!v.Visited) {
                    Explore(v);
                }
            }
        }
        
        public void Explore(Vertex v)
        {
            v.Visited = true;

            foreach (var w in v.Adjs)
                if (!w.Visited)
                    Explore(w);
            
            PostVisit(v);
        }

        public void Explore2(Vertex v)
        {
            v.Visited = true;

            foreach (var w in v.Adjs)
                if (!w.Visited)
                    Explore2(w);
        }

        public void SCCs()
        {
            foreach (var v in Graph.Values)
                v.Visited = false;

            CC = 0;
            for (int i = PostVisited.Count - 1; i >= 0; i--) {
                var v = Graph[PostVisited[i]];
                if (!v.Visited) {
                    Explore2(v);
                    CC++;
                }
            }
        }

        private void PreVisit(Vertex v)
        {
            v.Pre = Clock;
            Clock++;
        }

        private void PostVisit(Vertex v)
        {
            v.Post = Clock;
            Clock++;
            PostVisited.Add(v.Key);
        }

        public List<long> PostVisited;

        public class Vertex
        {
            public bool Visited { get; set; }
            public long Pre { get; set; } = -1;
            public long Post { get; set; } = -1;
            public long CCNum { get; set; }
            public long Key { get; private set; }
            public HashSet<Vertex> Adjs { get; private set; } = new HashSet<Vertex>();
            public Vertex(long key, bool visited = false, long ccNum = -1)
            {
                this.Key = key;
                this.Visited = visited;
                this.CCNum = ccNum;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"{this.Key} (");
                sb.Append(String.Join(", ", Adjs.Select(a => a.Key.ToString())));
                sb.Append(") " + (this.Visited ? "V" : "UV"));
                return sb.ToString();
            }
        }

    }
}
