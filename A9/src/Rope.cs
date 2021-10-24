using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestCommon;

namespace A11
{
    public class Rope : Processor
    {
        public Rope(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<string, long[][], string>)Solve);


        public string Solve(string text, long[][] queries)
        {
            root = new Vertex(text[0], text.Length, null, null, null);
            var node = root;
            for (int i = 1; i < text.Length; i++) {
                var newNode = new Vertex(text[i], text.Length - i, null, null, node);
                node.right = newNode;
                node = newNode;
            }
            root = splay(node);
            foreach (var q in queries)
            {
                long i = q[0],
                    j = q[1],
                    k = q[2];
                
                var s1 = split(root, i + 1);
                var s2 = split(s1.right , j - i + 2);
                var r = merge(s1.left, s2.right);
                var s3 = split(r, k + 1);
                root = merge(merge(s3.left, s2.left), s3.right);



                // int cutLen = j - i + 1;
                // string cut = text.Substring(i, cutLen);
                // text = text.Remove(i, cutLen);
                // text = text.Insert(k, cut);

            }
            return ToInOrder();
        }


        /////////////////////////////////////////////////////////////// starter from edx

        public class Vertex
        {
            public char key;
            // Sum of all the keys in the subtree - remember to update
            // it after each operation that changes the tree.
            public long sum;
            public Vertex left;
            public Vertex right;
            public Vertex parent;

            public Vertex(char key, long sum, Vertex left, Vertex right, Vertex parent)
            {
                this.key = key;
                this.sum = sum;
                this.left = left;
                this.right = right;
                this.parent = parent;
            }
        }

        void update(Vertex v)
        {
            if (v == null) return;
            v.sum = 1 + (v.left != null ? v.left.sum : 0) + (v.right != null ? v.right.sum : 0);
            if (v.left != null)
            {
                v.left.parent = v;
            }
            if (v.right != null)
            {
                v.right.parent = v;
            }
        }

        void smallRotation(Vertex v)
        {
            Vertex parent = v.parent;
            if (parent == null)
            {
                return;
            }
            Vertex grandparent = v.parent.parent;
            if (parent.left == v)
            {
                Vertex m = v.right;
                v.right = parent;
                parent.left = m;
            }
            else
            {
                Vertex m = v.left;
                v.left = parent;
                parent.right = m;
            }
            update(parent);
            update(v);
            v.parent = grandparent;
            if (grandparent != null)
            {
                if (grandparent.left == parent)
                {
                    grandparent.left = v;
                }
                else
                {
                    grandparent.right = v;
                }
            }
        }

        void bigRotation(Vertex v)
        {
            if (v.parent.left == v && v.parent.parent.left == v.parent)
            {
                // Zig-zig
                smallRotation(v.parent);
                smallRotation(v);
            }
            else if (v.parent.right == v && v.parent.parent.right == v.parent)
            {
                // Zig-zig
                smallRotation(v.parent);
                smallRotation(v);
            }
            else
            {
                // Zig-zag
                smallRotation(v);
                smallRotation(v);
            }
        }

        // Makes splay of the given vertex and returns the new root.
        Vertex splay(Vertex v)
        {
            if (v == null) return null;
            while (v.parent != null)
            {
                if (v.parent.parent == null)
                {
                    smallRotation(v);
                    break;
                }
                bigRotation(v);
            }
            return v;
        }

        public class VertexPair
        {
            public Vertex left;
            public Vertex right;
            public VertexPair()
            {
            }
            public VertexPair(Vertex left, Vertex right)
            {
                this.left = left;
                this.right = right;
            }
        }

        // Searches for the given key in the tree with the given root
        // and calls splay for the deepest visited node after that.
        // Returns pair of the result and the new root.
        // If found, result is a pointer to the node with the given key.
        // Otherwise, result is a pointer to the node with the smallest
        // bigger key (next value in the order).
        // If the key is bigger than all keys in the tree,
        // then result is null.
        // VertexPair find(Vertex root, char key)
        // {
        //     Vertex v = root;
        //     Vertex last = root;
        //     Vertex next = null;
        //     while (v != null)
        //     {
        //         if (v.key >= key && (next == null || v.key < next.key))
        //             next = v;
        //         last = v;
        //         if (v.key == key)
        //             break;
        //         if (v.key < key)
        //             v = v.right;
        //         else
        //             v = v.left;
        //     }
        //     root = splay(last);
        //     return new VertexPair(next, root);
        // }

        VertexPair find(Vertex root, long k)
        {
            Vertex res = null;

            Vertex node = root;
            Vertex last = root;

            while (node != null)
            {
                var lSum = node.left == null ? 0 : node.left.sum;
                last = node;
                if ((lSum + 1) == k)
                {
                    res = node;
                    break;
                }
                else if (k > lSum + 1)
                {
                    k = k - (lSum + 1);
                    node = node.right;
                }
                else
                    node = node.left;
            }

            root = splay(last);

            return new VertexPair(res, root);
        }

        VertexPair split(Vertex root, long key)
        {
            VertexPair result = new VertexPair();
            VertexPair findAndRoot = find(root, key);
            root = findAndRoot.right;
            result.right = findAndRoot.left;
            if (result.right == null)
            {
                result.left = root;
                return result;
            }
            result.right = splay(result.right);
            result.left = result.right.left;
            result.right.left = null;
            if (result.left != null)
                result.left.parent = null;
            update(result.left);
            update(result.right);
            return result;
        }

        Vertex merge(Vertex left, Vertex right)
        {
            if (left == null) return right;
            if (right == null) return left;
            while (right.left != null)
                right = right.left;
            right = splay(right);
            right.left = left;
            update(right);
            return right;
        }

        public string ToInOrder()
        {
            var node = root;
            if (node == null)
                return null;

            var res = new StringBuilder();
            var s = new Stack<Vertex>();
            
            while (node != null || s.Any()) {
                if (node != null) {
                    s.Push(node);
                    node = node.left;
                } else {
                    node = s.Pop();
                    res.Append(node.key);
                    node = node.right;
                }
            }

            return res.ToString();
        }

        Vertex root = null;

        // void insert(char x)
        // {
        //     Vertex left = null;
        //     Vertex right = null;
        //     Vertex new_vertex = null;
        //     VertexPair leftRight = split(root, x);
        //     left = leftRight.left;
        //     right = leftRight.right;
        //     if (right == null || right.key != x)
        //         new_vertex = new Vertex(x, x, null, null, null);
        //     root = merge(merge(left, new_vertex), right);
        // }



    }
}
