using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A8
{
    public class TreeHeight : Processor
    {
        public TreeHeight(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[], long>)Solve);

        public long Solve(long nodeCount, long[] tree)
        {
            var root = Helper.ParseTree(tree);
            var maxHeight = root.CalculateHeight();
            return maxHeight;
        }

        public static class Helper {
            public static TreeNode ParseTree(long[] tree) {
                var treeNodes = Enumerable.Range(0, tree.Length)
                    .Select(i => new TreeNode(i)).ToArray();
                TreeNode root = null;
                for (int i = 0; i < tree.Length; i++) {
                    if (tree[i] == -1) {
                        root = treeNodes[i];
                    } else {
                        treeNodes[tree[i]].AddChild(treeNodes[i]);
                    }
                }
                return root;
            }
        }


        public class TreeNode {
            public long Value;
            private LinkedList<TreeNode> children = new LinkedList<TreeNode>();

            public IReadOnlyCollection<TreeNode> Children { get => children; }

            public TreeNode(long value) {
                Value = value;
            }

            public TreeNode(long value, IReadOnlyCollection<TreeNode> children) {
                Value = value;
                AddChildren(children);
            }

            public void AddChild(TreeNode child) {
                children.AddLast(child);
            }

            public void AddChildren(IReadOnlyCollection<TreeNode> children) {
                foreach (var child in children) {
                    AddChild(child);
                }
            }

            public int CalculateHeight()
            {
                int maxHeight = 1;
                var q = new Queue<(TreeNode node, int depth)>();
                q.Enqueue((this, 1));
                (TreeNode node, int depth) node;
                while (q.Count != 0) {
                    node = q.Dequeue();
                    maxHeight = Math.Max(node.depth, maxHeight);
                    foreach (var child in node.node.Children) {
                        q.Enqueue((child, node.depth + 1));
                    }
                }
                return maxHeight;
            }

            public int CalculateHeightRec() {
                int maxHeight = 1;
                foreach (var child in Children) {
                    maxHeight = Math.Max(maxHeight,
                        child.CalculateHeightRec() + 1);
                }
                return maxHeight;
            }
        }
    }
}
