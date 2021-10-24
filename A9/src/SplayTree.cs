using System;
using System.Diagnostics;

namespace A11
{
    public class SplayTree: BST
    {
        public SplayTree(Node r=null)
            :base(r)
        { }

        public void Splay(long key)
        {
            var node = BSTFind(key);
            if (node.Key == key)
                Splay(node);
        }

        public override Node Find(long key)
        {
            var node = BSTFind(key);
            Splay(node);
            return node;
        }

        public override void Insert(long key)
        {
            BSTInsert(key);
            Find(key);
        }

        public override void Delete(long key)
        {
            var node = BSTFind(key);
            if (node.Key == key)
                Delete(node);
        }

        public override void Delete(Node n)
        {
            Splay(Next(n));
            Splay(n);
            var l = n.Left;
            var r = n.Right;

            if (r == null) {
                Root = l;
            } else {
                r.Left = l;
                Root = r;
            }

            if (Root != null) 
                Root.Parent = null;
        }

        public void Splay(Node n)
        {
            while (n != null && n.Parent != null)
            {
                if (n.IsLeftChild)
                {
                    if (n.Parent.Parent == null)
                        RotateRight(n.Parent);
                    else if (n.Parent.IsLeftChild)
                        ApplyZigZigRight(n);
                    else if (n.Parent.IsRightChild)
                        ApplyZigZagRight(n);
                }
                else
                {
                    if (n.Parent.Parent == null)
                        RotateLeft(n.Parent);
                    else if (n.Parent.IsRightChild)
                        ApplyZigZigLeft(n);
                    else if (n.Parent.IsLeftChild)
                        ApplyZigZagLeft(n);
                }
            }

            if (DebugMode && !EnsureBSTConsistency(this.Root))
                Debugger.Break();
        }

        private void ApplyZigZagLeft(Node n)
        {
            var green = n.Left;
            var blue = n.Right;
            var p = n.Parent;
            var red = p.Left;
            var q = p.Parent;
            var black = q.Right;

            var topParent = q.Parent;

            UpdateParentWithNewNode(topParent, q, n);

            p.Right = green;
            q.Left = blue;
            n.Left = p;
            n.Right = q;
        }

        private void ApplyZigZagRight(Node n) 
        {
            var green = n.Right;
            var blue = n.Left;
            var p = n.Parent;
            var red = p.Right;
            var q = p.Parent;
            var black = q.Left;

            var topParent = q.Parent;

            UpdateParentWithNewNode(topParent, q, n);

            p.Left = green;
            q.Right = blue;
            n.Right = p;
            n.Left = q;
        }

        private void ApplyZigZigLeft(Node n)
        { 
            var p = n.Parent;
            var q = p.Parent;
            var red = n.Right;
            var green = n.Left;
            var blue = p.Left;
            var black = q.Left;

            var topParent = q.Parent;

            RotateLeft(p);
            UpdateParentWithNewNode(topParent, q, n);
            
            q.Right = blue;
            p.Left = q;
            
        }

        private void ApplyZigZigRight(Node n) 
        { 
            var p = n.Parent;
            var q = p.Parent;
            var red = n.Left;
            var green = n.Right;
            var blue = p.Right;
            var black = q.Right;

            var topParent = q.Parent;

            RotateRight(p);
            UpdateParentWithNewNode(topParent, q, n);

            q.Left = blue;
            p.Right = q;
        }
    }
}
