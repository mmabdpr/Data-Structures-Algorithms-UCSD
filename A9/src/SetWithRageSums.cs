using System;
using System.Collections.Generic;
using TestCommon;

namespace A11
{
    public class SetWithRageSums : Processor
    {
        public SetWithRageSums(string testDataName) : base(testDataName)
        {
            CommandDict =
                        new Dictionary<char, Func<string, string>>()
                        {
                            ['+'] = Add,
                            ['-'] = Del,
                            ['?'] = Find,
                            ['s'] = Sum
                        };
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<string[], string[]>)Solve);

        public readonly Dictionary<char, Func<string, string>> CommandDict;

        protected const long M = 1_000_000_001;

        protected long X = 0;

        public string[] Solve(string[] lines)
        {
            X = 0;
            List<string> result = new List<string>();
            root = null;
            foreach (var line in lines)
            {
                char cmd = line[0];
                string args = line.Substring(1).Trim();
                var output = CommandDict[cmd](args);
                if (null != output)
                    result.Add(output);
            }
            return result.ToArray();
        }

        private long Convert(long i)
            => i = (i + X) % M;

        private string Add(string arg)
        {
            long i = Convert(long.Parse(arg));
            insert((int)i);

            return null;
        }

        private string Del(string arg)
        {
            long i = Convert(long.Parse(arg));
            
            erase((int)i);

            return null;
        }

        private string Find(string arg)
        {
            long i = Convert(int.Parse(arg));

            return !find((int)i) ?
                "Not found" : "Found";
        }

        private string Sum(string arg)
        {
            var toks = arg.Split();
            long l = Convert(long.Parse(toks[0]));
            long r = Convert(long.Parse(toks[1]));

            long s = 0;

            s = sum((int)l, (int)r);

            X = s;

            return s.ToString();
        }

        /////////////////////////////////////////////////////////////// starter from edx

        public class Vertex
        {
            public int key;
            // Sum of all the keys in the subtree - remember to update
            // it after each operation that changes the tree.
            public long sum;
            public Vertex left;
            public Vertex right;
            public Vertex parent;

            public Vertex(int key, long sum, Vertex left, Vertex right, Vertex parent)
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
            v.sum = v.key + (v.left != null ? v.left.sum : 0) + (v.right != null ? v.right.sum : 0);
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
        VertexPair find(Vertex root, int key)
        {
            Vertex v = root;
            Vertex last = root;
            Vertex next = null;
            while (v != null)
            {
                if (v.key >= key && (next == null || v.key < next.key))
                    next = v;
                last = v;
                if (v.key == key)
                    break;
                if (v.key < key)
                    v = v.right;
                else
                    v = v.left;
            }
            root = splay(last);
            return new VertexPair(next, root);
        }

        VertexPair split(Vertex root, int key)
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

        // Code that uses splay tree to solve the problem

        Vertex root = null;

        void insert(int x)
        {
            Vertex left = null;
            Vertex right = null;
            Vertex new_vertex = null;
            VertexPair leftRight = split(root, x);
            left = leftRight.left;
            right = leftRight.right;
            if (right == null || right.key != x)
                new_vertex = new Vertex(x, x, null, null, null);
            root = merge(merge(left, new_vertex), right);
        }

        void erase(int x)
        {
            // Implement erase yourself
            if (root == null)
                return;

            if (root != null && root.right == null && root.left == null && root.key == x) {
                root = null;
                return;
            }

            var res = split(root, x);
            if (res.right != null && res.right.key == x) {
                var tmp = split(res.right, x + 1);
                root = merge(res.left, tmp.right);
                root.parent = null;
            } else {
                root = merge(res.left, res.right);
                root.parent = null;
            }
        }

        bool find(int x)
        {
            // Implement find yourself
            if (root == null)
                return false;

            if (root.key == x)
                return true;

            var res = find(root, x);
            root = res.right;
            if (res.left != null && res.left.key == x)
                return true;
            
            return false;
        }

        public long sum(int from, int to)
        {
            VertexPair leftMiddle = split(root, from);
            Vertex left = leftMiddle.left;
            Vertex middle = leftMiddle.right;
            VertexPair middleRight = split(middle, to + 1);
            middle = middleRight.left;
            Vertex right = middleRight.right;
            long ans = 0;
            // Complete the implementation of sum

            if (middle != null)
                ans = middle.sum;

            root = merge(merge(left, middle), right);

            return ans;
        }

    }
}
