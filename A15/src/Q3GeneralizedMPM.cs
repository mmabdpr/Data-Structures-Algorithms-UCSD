using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A5
{
    public class Q3GeneralizedMPM : Processor
    {
        public Q3GeneralizedMPM(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<String, long, String[], long[]>)Solve);

        public long[] Solve(string text, long n, string[] patterns)
        {
            Trie.Clear();
            Trie.Add(new TrieNode() { Id = 0 });
            for (int i = 0; i < patterns.Length; i++)
                BuildTrieForPattern(Trie[0], patterns[i]);

            var ans = FindPatterns(text);
            if (ans.Length == 0)
                ans = new long[] { -1 };

            return ans;
        }

        public long[] FindPatterns(string text)
        {
            var ans = new List<long>(10);
            for (int i = 0; i < text.Length; i++) {
                if (AnyPattern(Trie[0], text, i)) {
                    ans.Add(i);
                }
            }

            return ans.ToArray();
        }

        public bool AnyPattern(TrieNode tNode, string text, int index = 0)
        {
            if (tNode == null)
                return false;
                
            if (tNode.IsEndOfPattern)
                return true;

            if (index >= text.Length)
                return false;

            var c = text[index];
            var nextNode = tNode.GetChild(c);

            return AnyPattern(nextNode, text, index + 1);
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
            private ChildrenType Children { get; } = new ChildrenType();

            public TrieNode GetChild(char c)
            {
                if (c == 'A') 
                    return Children.A;
                if (c == 'C') 
                    return Children.C;
                if (c == 'G') 
                    return Children.G;
                if (c == 'T') 
                    return Children.T;
                return null;
            }

            public void SetChild(char c, TrieNode tNode)
            {
                if (c == 'A') 
                    Children.A = tNode;
                if (c == 'C') 
                    Children.C = tNode;
                if (c == 'G') 
                    Children.G = tNode;
                if (c == 'T') 
                    Children.T = tNode;
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
