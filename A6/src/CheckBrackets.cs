using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A8
{
    public class CheckBrackets : Processor
    {
        public CheckBrackets(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<string, long>)Solve);

        public long Solve(string str)
        {
            var bracketStack = new Stack<Bracket>();
            for (int i = 0; i < str.Length; i++) {
                char c = str[i];
                if (Bracket.IsBracket(c)) {
                    var currentBracket = new Bracket {
                        index = i + 1,
                        type = c
                    };
                    if (Bracket.IsOpening(c)) {
                        bracketStack.Push(currentBracket);
                    } else {
                        if (bracketStack.Count == 0) {
                            return i + 1;
                        }
                        var lastBracket = bracketStack.Pop();
                        if (!Bracket.IsMatched(lastBracket, currentBracket)) {
                            return i + 1;
                        }
                    }
                }
            }
            return bracketStack.Count == 0 ? -1 : bracketStack.Pop().index;
        }

    }

    public class Bracket {
        public int index;
        public char type;
        public static bool IsBracket(char c) {
            return IsOpening(c) || IsClosing(c);
        }
        public static bool IsOpening(char c) {
            if (c == '[' ||
                c == '{' ||
                c == '(') {
                    return true;
                }
            return false;
        }
        public static bool IsClosing(char c) {
            if (c == ']' ||
                c == '}' ||
                c == ')') {
                    return true;
                }
            return false;
        }
        public static bool IsMatched(Bracket opening, Bracket closing) {
            if (opening.type == '{') {
                return closing.type == '}';
            } else if (opening.type == '[') {
                return closing.type == ']';
            } else if (opening.type == '(') {
                return closing.type == ')';
            }
            throw new ArgumentException("no bracket");
        }
    }

}
