using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A8
{
    public class Q2Airlines : Processor
    {
        public Q2Airlines(string testDataName) : base(testDataName)
        {
        }
        
        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long, long[][], long[]>) Solve);

        public virtual long[] Solve(long flightCount, long crewCount, long[][] info)
        {
            // graph = initial residual graph
            var vertexCount = (int) flightCount + (int)crewCount + 2;
            var graph = new FlowGraph(vertexCount);

            for (var i = 0; i < flightCount; ++i) {
                for (var j = 0; j < crewCount; ++j) {
                    if (info[i][j] == 1) {
                        graph.AddEdge(i + 1, (int)flightCount + j + 1, 1);
                    }
                }
            }

            for (var f = 0; f < flightCount; f++) {
                graph.AddEdge(0, f + 1, 1);
            }

            for (var c = 0; c < crewCount; c++) {
                graph.AddEdge((int)flightCount + c + 1, vertexCount - 1, 1);
            }

            FordFulkerson(graph);

            var ans = Enumerable.Repeat(-1L, (int) flightCount).ToArray();
            foreach (var edge in graph.Edges) {
                if (edge.From == 0) continue;
                if (edge.To == vertexCount - 1) continue;
                if (edge.Flow == 1) {
                    ans[edge.From - 1] = edge.To - flightCount;
                }
            }

            return ans;
        }

        private static int FordFulkerson(FlowGraph graph)
        {
            int flow = 0;

            var reachable = FindPathInResidualGraph(graph);

            while (reachable) {
                var augmentedFlow = FindMinFlowOfPath(graph);
                flow += augmentedFlow;
                UpdateResidualGraph(graph, augmentedFlow);
                reachable = FindPathInResidualGraph(graph);
            }

            return flow;
        }

        private static int[] _previous;

        private static bool FindPathInResidualGraph(FlowGraph graph)
        {
            _previous = Enumerable.Repeat(-1, graph.Size()).ToArray();

            var queue = new Queue<int>();
            queue.Enqueue(0);

            while (queue.Count > 0) {
                var node = queue.Dequeue();
                if (node == graph.Size() - 1)
                    return true;
                foreach (var edgeId in graph.GetIds(node)) {
                    var edge = graph.GetEdge(edgeId);
                    if (edge.Flow == edge.Capacity || _previous[edge.To] != -1) continue;
                    if (edge.To == edge.From) continue;
                    _previous[edge.To] = edgeId;
                    queue.Enqueue(edge.To);
                }
            }

            return false;
        }

        // assuming that the path is valid
        private static int FindMinFlowOfPath(FlowGraph graph)
        {
            var minFlow = int.MaxValue;
            var node = graph.Size() - 1;
            while (node != 0) {
                var edge = graph.GetEdge(_previous[node]);
                if (minFlow > edge.Capacity - edge.Flow)
                    minFlow = edge.Capacity - edge.Flow;

                node = edge.From;
            }

            if (minFlow == int.MaxValue)
                throw new Exception();

            return minFlow;
        }

        private static void UpdateResidualGraph(FlowGraph graph, int minFlow)
        {
            var node = graph.Size() - 1;
            while (node != 0) {
                var edgeId = _previous[node];
                var edge = graph.GetEdge(edgeId);
                graph.AddFlow(edgeId, minFlow);
                node = edge.From;
            }
        }

        private class Edge
        {
            public readonly int From;
            public readonly int To;
            public readonly int Capacity;
            public int Flow;

            public Edge(int from, int to, int capacity)
            {
                From = from;
                To = to;
                Capacity = capacity;
                Flow = 0;
            }

            public override string ToString()
            {
                return $"{From}->{To}: {Flow}/{Capacity}";
            }
        }

        /* This class implements a bit unusual scheme to store the graph edges, in order
         * to retrieve the backward edge for a given edge quickly. */
        private class FlowGraph
        {
            /* List of all - forward and backward - edges */
            public readonly List<Edge> Edges;

            /* These adjacency lists store only indices of edges from the edges list */
            private readonly List<int>[] _graph;

            public FlowGraph(int n)
            {
                _graph = new List<int>[n];
                for (var i = 0; i < n; ++i)
                    _graph[i] = new List<int>();
                Edges = new List<Edge>();
            }

            public void AddEdge(int from, int to, int capacity)
            {
                /* Note that we first append a forward edge and then a backward edge,
                 * so all forward edges are stored at even indices (starting from 0),
                 * whereas backward edges are stored at odd indices. */
                var forwardEdge = new Edge(from, to, capacity);
                var backwardEdge = new Edge(to, from, 0);
                _graph[from].Add(Edges.Count());
                Edges.Add(forwardEdge);
                _graph[to].Add(Edges.Count());
                Edges.Add(backwardEdge);
            }

            public int Size()
            {
                return _graph.Length;
            }

            public List<int> GetIds(int from)
            {
                return _graph[from];
            }

            public Edge GetEdge(int id)
            {
                return Edges[id];
            }

            public void AddFlow(int id, int flow)
            {
                /* To get a backward edge for a true forward edge (i.e id is even), we should get id + 1
                 * due to the described above scheme. On the other hand, when we have to get a "backward"
                 * edge for a backward edge (i.e. get a forward edge for backward - id is odd), id - 1
                 * should be taken.
                 *
                 * It turns out that id ^ 1 works for both cases. Think this through! */
                Edges[id].Flow += flow;
                Edges[id ^ 1].Flow -= flow;
            }
        }
    }
}