using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestCommon;

namespace A11
{
    public class Q1CircuitDesign : Processor
    {
        public Q1CircuitDesign(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long, long[][], Tuple<bool, long[]>>)Solve);

        public override Action<string, string> Verifier =>
            TestTools.SatAssignmentVerifier;

        public virtual Tuple<bool, long[]> Solve(long v, long c, long[][] cnf)
        {
            Graph = new Dictionary<long, Vertex>();
            RGraph = new Dictionary<long, Vertex>();
            for (long i = 0; i < c; i++) {
                var first = cnf[i][0];
                var second = cnf[i][1];
                
                if (!Graph.ContainsKey(first)) {
                    Graph[first] = new Vertex(first);
                    Graph[-first] = new Vertex(-first);
                    RGraph[first] = new Vertex(first);
                    RGraph[-first] = new Vertex(-first);
                }

                if (!Graph.ContainsKey(second)) {
                    Graph[second] = new Vertex(second);
                    Graph[-second] = new Vertex(-second);
                    RGraph[second] = new Vertex(second);
                    RGraph[-second] = new Vertex(-second);
                }

                Graph[-first].Adjs.Add(Graph[second]);
                Graph[-second].Adjs.Add(Graph[first]);
                RGraph[second].Adjs.Add(RGraph[-first]);
                RGraph[first].Adjs.Add(RGraph[-second]);
            }
            
            PostVisited = new List<long>();
            Components = new Dictionary<long, SCComponent>();
            DFSR();
            SCCs();

            var sat = true;
            foreach (var vx in Graph.Values) {
                if (vx.CCNum != Graph[-vx.Key].CCNum) continue;
                sat = false;
                break;
            }

            var ans = new long[v];
            
            if (!sat) return new Tuple<bool, long[]>(false, ans);
            
            BuildSCCGraph();
            BuildSatAnswer();
            ans = Enumerable.Range(1, (int) v).Select(i =>
            {
                if (Graph.ContainsKey(i))
                {
                    var vx = Graph[i];
                    return (long) (vx.Value == true ? i : -i);
                }
                else
                {
                    return i;
                }
            }).ToArray();

            return new Tuple<bool, long[]>(true, ans);
        }

        private void BuildSatAnswer()
        {
            DFSSCCs();
            // PostVisitedSCCs.Reverse(); not needed because reverse of tsort is reverse of reverse of postorder
            foreach (var scc in PostVisitedSCCs) {
                foreach (var v in scc.Vertices) {
                    if (v.Value != null) continue;
                    v.Value = true;
                    Graph[-v.Key].Value = false;
                }
            }
        }

        private void DFSSCCs()
        {
            PostVisitedSCCs = new List<SCComponent>();
            Clock = 0;

            foreach (var v in Components.Values)
                v.Visited = false;

            foreach (var v in Components.Values) {
                if (!v.Visited) {
                    ExploreForSCCs(v);
                }
            }
        }

        private void ExploreForSCCs(SCComponent v)
        {
            v.Visited = true;

            foreach (var w in v.Adjs)
                if (!w.Visited)
                    ExploreForSCCs(w);
            
            PostVisitForSCCs(v);
        }

        private void PostVisitForSCCs(SCComponent v)
        {
            v.Post = Clock;
            Clock++;
            PostVisitedSCCs.Add(v);
        }

        public void BuildSCCGraph()
        {
            foreach (var scc in Components.Values) {
                foreach (var v in scc.Vertices) {
                    foreach (var e in v.Adjs) {
                        if (e.CCNum != v.CCNum) {
                            scc.Adjs.Add(Components[e.CCNum]);
                        }
                    }
                }
            }
        }

        public Dictionary<long, Vertex> RGraph;
        public Dictionary<long, Vertex> Graph;
        public long CC;
        public long Clock = 1;
        public List<long> PostVisited;
        public List<SCComponent> PostVisitedSCCs;
        public Dictionary<long, SCComponent> Components;

        public void DFSR()
        {
            Clock = 0;

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

        public void ExploreComponent(Vertex v, long component)
        {
            v.Visited = true;
            v.CCNum = component;
            Components[component].Vertices.Add(v);

            foreach (var w in v.Adjs)
                if (!w.Visited)
                    ExploreComponent(w, component);
        }

        public void SCCs()
        {
            foreach (var v in Graph.Values)
                v.Visited = false;

            CC = 0;
            for (int i = PostVisited.Count - 1; i >= 0; i--) {
                var v = Graph[PostVisited[i]];
                if (v.Visited) continue;
                Components[CC] = new SCComponent(CC);
                ExploreComponent(v, CC);
                CC++;
            }
        }

        private void PostVisit(Vertex v)
        {
            v.Post = Clock;
            Clock++;
            PostVisited.Add(v.Key);
        }

        public class Vertex
        {
            public bool? Value { get; set;}
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

        public class SCComponent
        {
            public List<Vertex> Vertices = new List<Vertex>();
            public bool Visited { get; set; }
            public long Pre { get; set; } = -1;
            public long Post { get; set; } = -1;
            public long CCNum { get; set; }
            public long Key { get; private set; }
            public HashSet<SCComponent> Adjs { get; private set; } = new HashSet<SCComponent>();
            public SCComponent(long key, bool visited = false, long ccNum = -1)
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
