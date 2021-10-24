using System;
using System.Collections.Generic;
using System.Linq;

namespace A11
{
    public class BST
    {
        public class Node
        {
            public long Key { get; set; }
            protected Node _LeftChild;

            public Node Left
            {
                get => _LeftChild;
                set
                {
                    _LeftChild = value;
                    if (value != null)
                        _LeftChild.Parent = this;
                }
            }
            protected Node _RightChild;
            public Node Right
            {
                get => _RightChild;
                set
                {
                    _RightChild = value;
                    if (value != null)
                        _RightChild.Parent = this;
                }
            }
            public const string NullChar = "-";

            public Node Parent { get; set; }
            public bool IsLeftChild => Parent != null && ReferenceEquals(Parent.Left, this);
            public bool IsRightChild => Parent != null && ReferenceEquals(Parent.Right, this);

            public override string ToString()
            {
                try
                {
                    return ($"{Key}({(Left != null ? Left.ToString() : NullChar)}," +
                           $"{(Right != null ? Right.ToString() : NullChar)})")
                        .Replace("(-,-)", "")
                        // .Replace("(-,-)", "")
                        .Trim();
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }

            public Node(long key,
                Node leftChild = null,
                Node rightChild = null,
                Node parent = null)
            {
                Key = key;
                Left = leftChild;
                Right = rightChild;
                Parent = parent;
            }
        }

        public void Clear() => Root = null;

        public Node Root { get; protected set; }

        /// <summary>
        /// If DebugMode is on, the entire tree will be checked
        /// for parent-child consistence and making sure there are 
        /// no loops. It adds a huge performance cost, so only turn
        /// it on when you are trying to find a bug.
        /// You can add more calls to EnsureBSTConsistency where needed.
        /// </summary>
        public static bool DebugMode { get; set; } = false;

        public virtual Node Find(long key) => BSTFind(key);

        public Node BSTFind(long key) 
        {
            if (Root == null)
                return null;

            var n = Root;
            Node parent = null;
            
            while (n != null)
            {
                if (n.Key == key)
                    return n;
                if (key > n.Key) {
                    parent = n;
                    n = n.Right;
                } else {
                    parent = n;
                    n = n.Left;
                }
            }

            return parent;
        }

        public static BST ParseBST(IEnumerable<long> preOrderList)
        {
            var root = ParseBST(ref preOrderList);
            return new BST(root);
        }

        public static Node ParseBST(ref IEnumerable<long> preOrderList)
        {
            if (!preOrderList.Any())
                return null;

            long nextNode = preOrderList.First();
            preOrderList = preOrderList.Skip(1);

            if (nextNode == -1)
                return null;

            Node n = new Node(nextNode);

            n.Left = ParseBST(ref preOrderList);
            n.Right = ParseBST(ref preOrderList);

            return n;
        }

        public BST(Node root = null)
        {
            this.Root = root;
        }

        public override string ToString()
            => Root?.ToString();

        public virtual void Insert(long key) => BSTInsert(key);

        public void BSTInsert(long key) 
        {
            var node = new Node(key);

            if (Root == null) {
                Root = node;
                return;
            }

            var place = BSTFind(key);
            
            if (place.Key == key)
                return;

            if (place.Key > key)
                place.Left = node;
            else
                place.Right = node;
        }

        public virtual void Delete(Node node) 
        {
            if (Root == null || node == null)
                return;

            // var node = Find(n.Key);
            // if (node.Key != n.Key || !ReferenceEquals(node, n))
            //     return;

            if (node.Right == null) {
                UpdateParentWithNewNode(node.Parent, node, node.Left);
                return;
            }

            var next = Next(node);
            UpdateParentWithNewNode(next.Parent, next, next.Right);
            
            next.Right = node.Right;
            next.Left = node.Left;

            UpdateParentWithNewNode(node.Parent, node, next);
        }

        public virtual void Delete(long key) 
        {
            if (Root == null)
                return;

            var node = BSTFind(key);
            if (node.Key != key)
                return;

            if (node.Right == null) {
                UpdateParentWithNewNode(node.Parent, node, node.Left);
                return;
            }

            var next = Next(node);
            UpdateParentWithNewNode(next.Parent, next, next.Right);
            
            next.Right = node.Right;
            next.Left = node.Left;

            UpdateParentWithNewNode(node.Parent, node, next);
        }

        public Node LeftDescendant(Node node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        public Node RightAncestor(Node node)
        {
            while (node.IsRightChild) {
                node = node.Parent;
            }
            node = node.Parent;
            return node;
        }

        public Node Next(Node n) 
        {
            if (Root == null || n == null)
                return null;

//            var node = Find(n.Key);
//            if (node.Key != n.Key || !ReferenceEquals(node, n))
//                return null;

            // var next = Find(node.Key + 1);
            // return next.Key == node.Key ? null : next;
            var node = n;
            if (node.Right != null)
                return LeftDescendant(node.Right);
            else
                return RightAncestor(node);
        }

        public Node Next(long key)
        {
            if (Root == null)
                return null;

            var node = BSTFind(key);
            if (node.Key != key)
                return null;

            if (node.Right != null)
                return LeftDescendant(node.Right);
            else
                return RightAncestor(node);
        }

        public IEnumerable<Node> RangeSearch(long x, long y)
        {
            var nodes = new LinkedList<Node>();
            var node = BSTFind(x);
            if (node.Key < x)
                node = Next(node);
            while (node != null && node.Key <= y)
            {
                nodes.AddLast(node);
                node = Next(node);
            }
            return nodes;
        }

        public static bool EnsureBSTConsistency(BST.Node r)
        {
            if (r == null)
                return true;

            Queue<Node> nodes = new Queue<Node>();
            nodes.Enqueue(r);

            while (nodes.Count > 0)
            {

                var n = nodes.Dequeue();

                // Make sure left child points back to parent
                if (n.Left != null && !ReferenceEquals(n.Left.Parent, n))
                    return false;

                // Make sure right child points back to parent
                if (n.Right != null && !ReferenceEquals(n.Right.Parent, n))
                    return false;

                // Make sure no node is its own parent
                if (n.Parent != null && ReferenceEquals(n, n.Parent))
                    return false;

                if (n.Left != null)
                    nodes.Enqueue(n.Left);

                if (n.Right != null)
                    nodes.Enqueue(n.Right);

            }
            return true;
        }

        protected void UpdateParentWithNewNode(Node parent, Node n, Node newNode)
        {
            if (parent == null)
            {
                Root = newNode;
                if (Root != null) 
                    Root.Parent = null;

                return;
            }

            if (n.IsLeftChild)
                parent.Left = newNode;
            else
                parent.Right = newNode;
        }

        protected Node RotateRight(Node x)
        {
            var p = x.Parent;
            var y = x.Left;
            var b = y.Right;

            UpdateParentWithNewNode(p, x, y);

            x.Left = b;
            y.Right = x;

            return x;
        }

        protected Node RotateLeft(Node x)
        {
            var p = x.Parent;
            var y = x.Right;
            var b = y.Left;

            UpdateParentWithNewNode(p, x, y);

            x.Right = b;
            y.Left = x;

            return x;
        }
    
        public List<long> ToPreOrder()
        {
            var node = Root;
            if (node == null)
                return null;
            
            var res = new List<long>();
            var s = new Stack<Node>();
            
            s.Push(node);

            while (s.Any())
            {
                node = s.Pop();
                res.Add(node.Key);
                if (node.Right != null)
                    s.Push(node.Right);
                if (node.Left != null)
                    s.Push(node.Left);
            }

            return res;
        }

        public List<long> ToInOrder()
        {
            var node = Root;
            if (node == null)
                return null;

            var res = new List<long>();
            var s = new Stack<Node>();
            
            while (node != null || s.Any()) {
                if (node != null) {
                    s.Push(node);
                    node = node.Left;
                } else {
                    node = s.Pop();
                    res.Add(node.Key);
                    node = node.Right;
                }
            }

            return res;
        }

        // public IEnumerable<long> ToPostOrder()
        // {
        //     var node = Root;
        //     if (node == null)
        //         return null;

        //     var res = new LinkedList<long>();
        //     var stack = new Stack<Node>();
        //     Node prev = null;

        //     stack.Push(node);

        //     while (stack.Any()) {
        //         node = stack.Peek();
        //         if (prev == null || prev.Right == node || prev.Left == node) {
        //             if (node.Left != null) {
        //                 stack.Push(node.Left);
        //             } else if (node.Right != null)
        //                 stack.Push(node.Right);
        //         } else if (node.Left == prev) {
        //             if (node.Right != null)
        //                 stack.Push(node.Right);
        //         } else {
        //             res.AddLast(node.Key);
        //             stack.Pop();
        //         }
        //         prev = node;
        //     }

        //     return res;
        // }

        public List<long> ToPostOrder()
        {
            var node = Root;
            if (node == null)
                return null;

            var res = new List<long>();
            var s = new Stack<Node>();

            s.Push(node);

            while (s.Any()) {
                node = s.Pop();
                res.Add(node.Key);
                if (node.Left != null)
                    s.Push(node.Left);
                if (node.Right != null)
                    s.Push(node.Right);
            }

            res.Reverse();

            return res;
        }

        public List<long> ToLevelOrder()
        {
            var node = Root;
            if (node == null)
                return null;

            var res = new List<long>();
            var q = new Queue<Node>();

            q.Enqueue(node);

            while (q.Any()) {
                node = q.Dequeue();
                res.Add(node.Key);
                if (node.Left != null)
                    q.Enqueue(node.Left);
                if (node.Right != null)
                    q.Enqueue(node.Right);
            }

            return res;
        }
    }
}
