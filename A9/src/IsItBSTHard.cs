using System;
using System.Collections.Generic;
using System.Linq;
using TestCommon;

namespace A11
{
    public class IsItBSTHard : Processor
    {
        public IsItBSTHard(string testDataName) : base(testDataName) { }

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

            var isSorted = true;

            var node = Nodes[0];

            var res = new List<BST.Node>();
            var s = new Stack<BST.Node>();
            
            while (node != null || s.Any()) {
                if (node != null) {
                    s.Push(node);
                    node = node.Left;
                } else {
                    node = s.Pop();
                    if (res.Any()) {
                        var last = res.Last();
                        if (last.Key > node.Key) {
                            isSorted = false;
                            break;
                        }
                        if (last.Key == node.Key && node.Left != null) {
                            var bst = new BST(node.Left);
                            if (bst.Find(last.Key).Key == last.Key) {
                                isSorted = false;
                                break;
                            }
                        }
                            
                    }
                    res.Add(node);
                    node = node.Right;
                }
            }

            return isSorted;
        }
    }
}
