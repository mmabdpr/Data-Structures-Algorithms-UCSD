using System;
using System.Collections.Generic;
using System.Linq;
using TestCommon;

namespace A11
{
    public class IsItBST : Processor
    {
        public IsItBST(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long[][], bool>)Solve);

        public static List<BST.Node> Nodes = Enumerable.Range(0, 200_000).Select(x => new BST.Node(0)).ToList();

        public bool Solve(long[][] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                var key = (int)nodes[i][0];
                var lIndex = (int)nodes[i][1];
                var rIndex = (int)nodes[i][2];

                Nodes[i].Key = key;
                Nodes[i].Left = lIndex == -1 ? null : Nodes[lIndex];
                Nodes[i].Right = rIndex == -1 ? null : Nodes[rIndex];
            }

            var tree = new BST(Nodes[0]);

            var l = tree.ToInOrder();
            var y = l.First();
            var isSorted = l.Skip(1).All(x =>
            {
                bool b = y < x;
                y = x;
                return b;
            });

            return isSorted;
        }
    }    
}
