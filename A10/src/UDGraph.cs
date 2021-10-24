using System.Collections.Generic;

namespace A12
{
    public class UDGraph
    {
        public Dictionary<long, Vertex> Graph = new Dictionary<long, Vertex>();
        public long CC = 0;

        public void DFS()
        {
            foreach (var v in Graph.Values)
                v.Visited = false;

            CC = 1;
            foreach (var v in Graph.Values) {
                if (!v.Visited) {
                    Explore(v);
                    CC++;
                }
            }
        }

        public void Explore(Vertex v)
        {
            v.Visited = true;
            v.CCNum = CC;
            foreach (var vertex in Graph.Values)
                foreach (var w in vertex.Adjs)
                    if (!w.Visited)
                        Explore(w);
        }

        public class Vertex
        {
            public bool Visited { get; set; }
            public long CCNum { get; set; }
            public long Key { get; private set; }
            public HashSet<Vertex> Adjs { get; private set; }
            public Vertex(long key, bool visited = false, long ccNum = -1)
            {
                this.Key = key;
                this.Visited = visited;
                this.CCNum = ccNum;
                Adjs = new HashSet<Vertex>();
            }
        }
    }
}