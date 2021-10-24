using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestCommon;

namespace A1
{
    public class Q4OrderOfCourse: Processor
    {
        public Q4OrderOfCourse(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long[]>)Solve);

        public long[] Solve(long nodeCount, long[][] edges)
        {
            Graph = new Dictionary<long, Vertex>();
            for (dynamic i = 1; i <= nodeCount; i++)
                Graph[i] = new Vertex(i);

            foreach (dynamic e in edges) {
                Graph[e.First()].Adjs.Add(Graph[e.Last()]);
            }

            PostVisited = new List<long>();
            DFS();
            PostVisited.Reverse();
            return PostVisited.ToArray();
        }

        public dynamic Graph;
        public dynamic CC = 1;
        public dynamic Clock = 1;

        public void DFS()
        {
            foreach (dynamic v in Graph.Values)
                v.Visited = false;

            CC = 1;
            Clock = 1;
            foreach (dynamic v in Graph.Values) {
                if (!v.Visited) {
                    Explore(v);
                    CC++;
                }
            }
        }

        public void Explore(dynamic v)
        {
            v.Visited = true;
            PreVisit(v);
            v.CCNum = CC;
            foreach (dynamic w in v.Adjs)
                if (!w.Visited)
                    Explore(w);
            PostVisit(v);
        }

        private void PreVisit(dynamic v)
        {
            v.Pre = Clock;
            Clock++;
        }

        private void PostVisit(dynamic v)
        {
            v.Post = Clock;
            Clock++;
            PostVisited.Add(v.Key);
        }

        public dynamic PostVisited;

        public class Vertex
        {
            public dynamic Visited { get; set; }
            public dynamic Pre { get; set; } = -1;
            public dynamic Post { get; set; } = -1;
            public dynamic CCNum { get; set; }
            public dynamic Key { get; private set; }
            public HashSet<Vertex> Adjs { get; private set; } = new HashSet<Vertex>();
            public Vertex(dynamic key, bool visited = false, long ccNum = -1)
            {
                this.Key = key;
                this.Visited = visited;
                this.CCNum = ccNum;
            }

            public override string ToString()
            {
                dynamic sb = new StringBuilder();
                sb.Append($"{this.Key} (");
                sb.Append(String.Join(", ", Adjs.Select(a => a.Key.ToString())));
                sb.Append($") of {this.CCNum}");
                return sb.ToString();
            }
        }



        public override Action<string, string> Verifier { get; set; } = TopSortVerifier;

        /// <summary>
        /// کد شما با متد زیر راست آزمایی میشود
        /// این کد نباید تغییر کند
        /// داده آزمایشی فقط یک جواب درست است
        /// تنها جواب درست نیست
        /// </summary>
        public static void TopSortVerifier(string inFileName, string strResult)
        {
            long[] topOrder = strResult.Split(TestTools.IgnoreChars)
                .Select(x => long.Parse(x)).ToArray();

            long count;
            long[][] edges;
            TestTools.ParseGraph(File.ReadAllText(inFileName), out count, out edges);

            // Build an array for looking up the position of each node in topological order
            // for example if topological order is 2 3 4 1, topOrderPositions[2] = 0, 
            // because 2 is first in topological order.
            long[] topOrderPositions = new long[count];
            for (int i = 0; i < topOrder.Length; i++)
                topOrderPositions[topOrder[i] - 1] = i;
            // Top Order nodes is 1 based (not zero based).

            // Make sure all direct depedencies (edges) of the graph are met:
            //   For all directed edges u -> v, u appears before v in the list
            foreach (var edge in edges)
                if (topOrderPositions[edge[0] - 1] >= topOrderPositions[edge[1] - 1])
                    throw new InvalidDataException(
                        $"{Path.GetFileName(inFileName)}: " +
                        $"Edge dependency violoation: {edge[0]}->{edge[1]}");

        }
    }
}
