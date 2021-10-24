using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A5
{
    public class Q1ConstructTrie : Processor
    {
        public Q1ConstructTrie(string testDataName) : base(testDataName)
        {
            this.VerifyResultWithoutOrder = true;
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<long, String[], String[]>) Solve);

        public string[] Solve(long n, string[] patterns)
        {
            Trie.Clear();
            Trie.Add(new TrieNode() { Id = 0 });
            for (int i = 0; i < patterns.Length; i++)
                BuildTrieForPattern(Trie[0], patterns[i]);

            var ans = new List<string>(10);

            for (int i = 0; i < Trie.Count; i++) {
                foreach (var child in Trie[i].Children) {
                    ans.Add($"{i}->{child.Id}:{child.Nucleotide}");
                }
            }

            return ans.ToArray();
        }

        public void BuildTrieForPattern(TrieNode tNode, string pattern, int index = 0)
        {
            if (index == pattern.Length) {
                tNode.IsEndOfPattern = true;
                return;
            }
                
            var c = pattern[index];
            var nextNode = tNode.GetChild(c);
            if (nextNode == null) {
                nextNode = new TrieNode() { Id = Trie.Count };
                tNode.SetChild(c, nextNode);
                Trie.Add(nextNode);
            }

            BuildTrieForPattern(nextNode, pattern, index + 1);
        }

        public List<TrieNode> Trie = new List<TrieNode>(10);

        public class TrieNode
        {
            public long Id { get; set; }
            public bool IsEndOfPattern { get; set; }
            private class ChildrenType
            {
                public TrieNode A { get; set; }
                public TrieNode C { get; set; }
                public TrieNode G { get; set; }
                public TrieNode T { get; set; }
            }
            private ChildrenType _children { get; } = new ChildrenType();
            public IEnumerable<(char Nucleotide, long Id)> Children { get {
                if (_children.A != null)
                    yield return ('A', _children.A.Id);

                if (_children.C != null)
                    yield return ('C', _children.C.Id);
                
                if (_children.G != null)
                    yield return ('G', _children.G.Id);
                
                if (_children.T != null)
                    yield return ('T', _children.T.Id);
            }}

            public TrieNode GetChild(char c)
            {
                if (c == 'A') 
                    return _children.A;
                if (c == 'C') 
                    return _children.C;
                if (c == 'G') 
                    return _children.G;
                if (c == 'T') 
                    return _children.T;
                return null;
            }

            public void SetChild(char c, TrieNode tNode)
            {
                if (c == 'A') 
                    _children.A = tNode;
                if (c == 'C') 
                    _children.C = tNode;
                if (c == 'G') 
                    _children.G = tNode;
                if (c == 'T') 
                    _children.T = tNode;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"{(IsEndOfPattern ? "*" : null)}");
                sb.Append($"{Id}:");
                sb.Append($" A({(GetChild('A') == null ? -1 : GetChild('A').Id)})");
                sb.Append($" C({(GetChild('C') == null ? -1 : GetChild('C').Id)})");
                sb.Append($" G({(GetChild('G') == null ? -1 : GetChild('G').Id)})");
                sb.Append($" T({(GetChild('T') == null ? -1 : GetChild('T').Id)})");
                return sb.ToString();
            }
        }
    }
}
