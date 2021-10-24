using System;
using System.Collections.Generic;
using System.Linq;
using TestCommon;
using System.Diagnostics;

namespace A11
{
    public class BinaryTreeTraversals : Processor
    {
        public BinaryTreeTraversals(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long[][], long[][]>)Solve);

        public static List<BST.Node> Nodes = Enumerable.Range(0, 200_000).Select(x => new BST.Node(0)).ToList();

        public long[][] Solve(long[][] nodes)
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

            var ans = new List<long[]>();
            ans.Add(tree.ToInOrder().ToArray());
            ans.Add(tree.ToPreOrder().ToArray());
            ans.Add(tree.ToPostOrder().ToArray());
            
            return ans.ToArray();
        }

        // public long[][] Solve(long[][] nodes)
        // {
        //     var pre = new List<long>();
        //     var post = new List<long>();
        //     var ino = new List<long>();

        //     PreOrder(nodes, 0, pre);
        //     PostOrder(nodes, 0, post);
        //     InOrder(nodes, 0, ino);

        //     var ans = new List<long[]>();
        //     ans.Add(ino.ToArray());
        //     ans.Add(pre.ToArray());
        //     ans.Add(post.ToArray());
            
        //     return ans.ToArray();
        // }

        // public void PreOrder(long[][] nodes, int i, List<long> ans)
        // {
        //     if (i == -1)
        //         return;
        //     ans.Add(nodes[i][0]);
        //     PreOrder(nodes, (int)nodes[i][1], ans);
        //     PreOrder(nodes, (int)nodes[i][2], ans);
        // }
        
        // public void PostOrder(long[][] nodes, int i, List<long> ans)
        // {
        //     if (i == -1)
        //         return;
        //     PostOrder(nodes, (int)nodes[i][1], ans);
        //     PostOrder(nodes, (int)nodes[i][2], ans);
        //     ans.Add(nodes[i][0]);
        // }
        
        // public void InOrder(long[][] nodes, int i, List<long> ans)
        // {
        //     if (i == -1)
        //         return;
        //     InOrder(nodes, (int)nodes[i][1], ans);
        //     ans.Add(nodes[i][0]);
        //     InOrder(nodes, (int)nodes[i][2], ans);
        // }
    }
}
