using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A5
{
    public class Q5ShortestNonSharedSubstring : Processor
    {
        public Q5ShortestNonSharedSubstring(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<String, String, String>)Solve);

        private string Solve(string text1, string text2)
        {
            SFTree.Clear();
            MarkedNodes.Clear();
            var s = $"{text1}#{text2}$";
            BuildSuffixTree(s);
            var res = FindNonSharedSubstring();
            return res;
        }
        
        public string FindNonSharedSubstring()
        {
            var res = Text;
            STNode resNode = null;

            MarkCandidateNodes(SFTree[0]);
            for (int i = 0; i < MarkedNodes.Count; i++) {
                var node = MarkedNodes[i];
                if (node.IsLeaf) {
                    if (Text[node.StartIndex] != '#') {
                        var candidateRes =
                            Text.Substring(node.RealStartIndex, node.StartIndex + 1 - node.RealStartIndex); 
                        if ((resNode == null || candidateRes.Length < res.Length) 
                            || (candidateRes.Length == res.Length && node.MinRealStartIndex < resNode.MinRealStartIndex)) {
                            res = candidateRes;
                            resNode = node;
                        }
                    }
                }
                else {
                    var candidateRes = Text.Substring(node.RealStartIndex,
                        node.StartIndex + node.Length - node.RealStartIndex);
                    if ((resNode == null || candidateRes.Length < res.Length)
                        || (candidateRes.Length == res.Length && node.MinRealStartIndex < resNode.MinRealStartIndex)) {
                        res = candidateRes;
                        resNode = node;
                    }
                }
            }
            
            return res;
        }

        public void MarkCandidateNodes(STNode node)
        {
            if (node == null)
                return;

            foreach (var child in node.GetExistingChildren())
                MarkCandidateNodes(child);

            if (node.IsLeaf) {
                for (int i = node.StartIndex; i < node.StartIndex + node.Length; i++) {
                    if (Text[i] == '#') {
                        node.Marked = true;
                        MarkedNodes.Add(node);
                        break;
                    }
                }
            }
            else {
                if (node.GetExistingChildren().All(c => c.Marked)) {
                    node.Marked = true;
                    MarkedNodes.Add(node);
                }
            }
        }
        
        public List<STNode> MarkedNodes = new List<STNode>(10);
        
        public string Text;

        public void BuildSuffixTree(string text)
        {
            this.Text = text.Replace("\n", "");
            STNode.Text = this.Text;
            SFTree.Clear();
            var root = new STNode {
                Id = 0,
            };
            SFTree.Add(root);
            for (int i = Text.Length - 1; i >= 0; i--) {
                BuildSuffixTreeForSuffix(root, i, i, Text.Length - i);
            }
        }

        public void BuildSuffixTreeForSuffix(STNode stNode, int realStartIndex, int index, int length)
        {
            while (true) {
                if (stNode == null) throw new Exception("stNode is null");
                if (index >= Text.Length || index + length - 1 >= Text.Length) return;
                var child = stNode.GetChild(Text[index]);
                if (child == null) {
                    var newNode = new STNode {
                        Id = SFTree.Count,
                        RealStartIndex = realStartIndex,
                        MinRealStartIndex = realStartIndex,
                        StartIndex = index,
                        Length = length
                    };
                    stNode.SetChild(Text[index], newNode);
                    SFTree.Add(newNode);
                    if (newNode.MinRealStartIndex < stNode.MinRealStartIndex)
                        stNode.MinRealStartIndex = newNode.MinRealStartIndex;
                }
                else {
                    var commonLength = GetCommonLengthOfSuffixes(index, child.StartIndex, length, child.Length);
                    if (commonLength < 1) throw new Exception("commonLength below 1");

                    var nextChildChar = Text[index + commonLength];
                    if (nextChildChar == '$') throw new Exception("nextChildChar == $");

                    if (commonLength < child.Length) {
                        var firstRemainingChar = Text[child.StartIndex + commonLength];
                        if (firstRemainingChar == '$') {
                            if (child.GetChild(firstRemainingChar) == null) {
                                var branchCurrentNode = new STNode {
                                    Id = SFTree.Count,
                                    RealStartIndex = child.RealStartIndex,
                                    MinRealStartIndex = child.MinRealStartIndex,
                                    StartIndex = child.StartIndex + commonLength,
                                    Length = child.Length - commonLength
                                };
                                child.Length = commonLength;
                                child.SetChild(firstRemainingChar, branchCurrentNode);
                                SFTree.Add(branchCurrentNode);
                                if (branchCurrentNode.MinRealStartIndex < child.MinRealStartIndex)
                                    child.MinRealStartIndex = branchCurrentNode.MinRealStartIndex;
                            }
                        }
                        else {
                            var branchCurrentNode = new STNode {
                                Id = SFTree.Count,
                                RealStartIndex = child.RealStartIndex,
                                MinRealStartIndex = child.MinRealStartIndex,
                                StartIndex = child.StartIndex + commonLength,
                                Length = child.Length - commonLength
                            };
                            child.Length = commonLength;
                            branchCurrentNode.Children = child.Children;
                            child.Children = new STNode.TChildren();
                            child.SetChild(firstRemainingChar, branchCurrentNode); // ex?
                            SFTree.Add(branchCurrentNode);
                            if (branchCurrentNode.MinRealStartIndex < child.MinRealStartIndex)
                                child.MinRealStartIndex = branchCurrentNode.MinRealStartIndex;
                        }
                    }

                    var nextChild = child.GetChild(nextChildChar);
                    if (nextChild == null) {
                        // add new suffix
                        var newNode = new STNode {
                            Id = SFTree.Count,
                            RealStartIndex = realStartIndex,
                            MinRealStartIndex = realStartIndex,
                            StartIndex = index + commonLength,
                            Length = length - commonLength
                        };
                        child.SetChild(nextChildChar, newNode);
                        SFTree.Add(newNode);
                        if (newNode.MinRealStartIndex < child.MinRealStartIndex)
                            child.MinRealStartIndex = newNode.MinRealStartIndex;
                    }
                    else {
                        stNode = child;
                        index = index + commonLength;
                        length = length - commonLength;
                        continue;
                    }
                }

                break;
            }
        }

        private int GetCommonLengthOfSuffixes(int index1, int index2, int length1, int length2)
        {
            int l = 0;
            int bound = Math.Min(length1, length2);
            for (int i = 0; i < bound; i++) {
                if (Text[index1 + i] == Text[index2 + i])
                    l++;
                else
                    break;
            }

            return l;
        }

        public List<STNode> SFTree = new List<STNode>(10);

        public class STNode
        {
            public long MinRealStartIndex { get; set; }
            public bool Marked { get; set; }
            public static string Text { get; set; }
            private int _length = -1;
            public long Id { get; set; }

            public bool IsLeaf
            {
                get
                {
                    if (StartIndex == -1 || Length == -1)
                        return false;
                    return Text[StartIndex + Length - 1] == '$';
                }
            }

            public int RealStartIndex { get; set; } = -1; // if IsLeaf it doesn't mean anything
            public int StartIndex { get; set; } = -1;

            public int Length
            {
                get => _length;
                set
                {
                    if (value < 1)
                        throw new Exception("set length to 0");
                    _length = value;
                }
            }

            public class TChildren
            {
                public STNode A { get; set; }
                public STNode C { get; set; }
                public STNode G { get; set; }
                public STNode T { get; set; }
                public STNode X { get; set; }
                public STNode _ { get; set; }
            }

            public TChildren Children { get; set; } = new TChildren();

            public IEnumerable<STNode> GetExistingChildren()
            {
                if (Children.A != null)
                    yield return Children.A;
    
                if (Children.C != null)
                    yield return Children.C;

                if (Children.G != null)
                    yield return Children.G;
    
                if (Children.T != null)
                    yield return Children.T;
    
                if (Children.X != null)
                    yield return Children.X;
    
                if (Children._ != null)
                    yield return Children._;
            }

            public STNode GetChild(char c)
            {
                switch (c) {
                    case 'A':
                        return Children.A;
                    case 'C':
                        return Children.C;
                    case 'G':
                        return Children.G;
                    case 'T':
                        return Children.T;
                    case '#':
                        return Children.X;
                    case '$':
                        return Children._;
                    default:
                        throw new Exception($"wrong child {c}");
                }
            }

            public void SetChild(char c, STNode stNode)
            {
                switch (c) {
                    case 'A' when Children.A == null:
                        Children.A = stNode;
                        break;
                    case 'C' when Children.C == null:
                        Children.C = stNode;
                        break;
                    case 'G' when Children.G == null:
                        Children.G = stNode;
                        break;
                    case 'T' when Children.T == null:
                        Children.T = stNode;
                        break;
                    case '#' when Children.X == null:
                        Children.X = stNode;
                        break;
                    case '$' when Children._ == null:
                        Children._ = stNode;
                        break;
                    default:
                        throw new Exception($"wrong child or overriding child {c}");
                }
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append($"{(IsLeaf ? "*" : null)}");
                sb.Append($"{Id}:");
                sb.Append($" A({(GetChild('A') == null ? -1 : GetChild('A').Id)})");
                sb.Append($" C({(GetChild('C') == null ? -1 : GetChild('C').Id)})");
                sb.Append($" G({(GetChild('G') == null ? -1 : GetChild('G').Id)})");
                sb.Append($" T({(GetChild('T') == null ? -1 : GetChild('T').Id)})");
                sb.Append($" #({(GetChild('#') == null ? -1 : GetChild('#').Id)})");
                sb.Append($" $({(GetChild('$') == null ? -1 : GetChild('$').Id)})");
                sb.Append($" \"{(this.StartIndex == -1 ? string.Empty : Text.Substring(StartIndex, Length))}\"");
                return sb.ToString();
            }
        }
    }
}
