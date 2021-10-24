using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A5
{
    public class Q4SuffixTree : Processor
    {
        public Q4SuffixTree(string testDataName) : base(testDataName)
        {
            this.VerifyResultWithoutOrder = true;
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<String, String[]>)Solve);

        public string[] Solve(string text)
        {
            BuildSuffixTree(text);

            var ans = new List<string>(10);
            for (int i = 1; i < SFTree.Count; i++) {
                var s = Text.Substring(SFTree[i].StartIndex, SFTree[i].Length);
                ans.Add(s);
            }

            return ans.ToArray();
        }

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
            if (stNode == null)
                throw new Exception("stNode is null");
            if (index >= Text.Length || index + length - 1 >= Text.Length)
                return;
            var child = stNode.GetChild(Text[index]);
            if (child == null) {
                var newNode = new STNode {
                    Id = SFTree.Count,
                    RealStartIndex = realStartIndex,
                    StartIndex = index,
                    Length = length
                };
                stNode.SetChild(Text[index], newNode);
                SFTree.Add(newNode);
            } else {
                var commonLength = GetCommonLengthOfSuffixes(index, child.StartIndex, length, child.Length);
                if (commonLength < 1)
                    throw new Exception("commonLength below 1");

                var nextChildChar = Text[index + commonLength];
                if (nextChildChar == '$')
                    throw new Exception("nextChildChar == $");

                if (commonLength < child.Length) {
                    var firstRemainingChar = Text[child.StartIndex + commonLength];
                    if (firstRemainingChar == '$') {
                        if (child.GetChild(firstRemainingChar) == null) {
                            var branchCurrentNode = new STNode {
                                Id = SFTree.Count,
                                RealStartIndex = child.RealStartIndex,
                                StartIndex = child.StartIndex + commonLength,
                                Length = child.Length - commonLength
                            };
                            child.Length = commonLength;
                            child.SetChild(firstRemainingChar, branchCurrentNode);
                            SFTree.Add(branchCurrentNode);
                        }
                    }
                    else {
                        var branchCurrentNode = new STNode {
                            Id = SFTree.Count,
                            RealStartIndex = child.RealStartIndex,
                            StartIndex = child.StartIndex + commonLength,
                            Length = child.Length - commonLength
                        };
                        child.Length = commonLength;
                        branchCurrentNode.Children = child.Children;
                        child.Children = new STNode.TChildren();
                        child.SetChild(firstRemainingChar, branchCurrentNode); // ex?
                        SFTree.Add(branchCurrentNode);
                    }
                }
                
                var nextChild = child.GetChild(nextChildChar);
                if (nextChild == null) {
                    // add new suffix
                    var newNode = new STNode {
                        Id = SFTree.Count,
                        RealStartIndex = realStartIndex,
                        StartIndex = index + commonLength,
                        Length = length - commonLength
                    };
                    child.SetChild(nextChildChar, newNode);
                    SFTree.Add(newNode);
                }
                else {
                    BuildSuffixTreeForSuffix(child, realStartIndex, index + commonLength, length - commonLength);
                }
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
            public static string Text { get; set; }
            private int _length = -1;
            public long Id { get; set; }

            public bool IsLeaf => Text[StartIndex + Length] == '$';

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
                public STNode _ { get; set; }
            }
            public TChildren Children { get; set; } = new TChildren();

            public bool HasAnyChild() => Children.A != null
                    || Children.C != null
                    || Children.G != null
                    || Children.T != null
                    || Children._ != null;

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
                sb.Append($" $({(GetChild('$') == null ? -1 : GetChild('$').Id)})");
                sb.Append($" \"{(this.StartIndex == -1 ? string.Empty : Text.Substring(StartIndex, Length))}\"");
                return sb.ToString();
            }
        }
    }
}
