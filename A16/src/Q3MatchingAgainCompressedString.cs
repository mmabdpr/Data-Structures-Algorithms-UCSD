using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A6
{
    public class Q3MatchingAgainCompressedString : Processor
    {
        public Q3MatchingAgainCompressedString(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<String, long, String[], long[]>) Solve);

        public long[] Solve(string bwt, long n, string[] patterns)
        {
            var firstColumn = bwt.ToCharArray();
            Array.Sort(firstColumn);

            var startIndexes = Enumerable.Repeat(-1, 256).ToArray();
            for (int i = 0; i < firstColumn.Length; i++) {
                int c = firstColumn[i];
                if (startIndexes[c] == -1)
                    startIndexes[c] = i;
            }

            ConstructCountArrays(bwt);

            var ans = new long[patterns.Length];

            for (int i = 0; i < patterns.Length; i++) {
                var pattern = patterns[i];
                var patternLength = pattern.Length;
                var top = 0;
                var bottom = bwt.Length - 1;
                while (top <= bottom) {
                    if (patternLength != 0) {
                        var symbol = pattern[--patternLength];
                        var rangeContainsSymbol = false;
                        for (int j = top; j <= bottom; j++) {
                            if (bwt[j] == symbol) {
                                rangeContainsSymbol = true;
                                break;
                            }
                        }
                        if (rangeContainsSymbol) {
                            switch (symbol) {
                                case 'A':
                                    top = startIndexes[symbol] + ACounts[top];
                                    bottom = startIndexes[symbol] + ACounts[bottom + 1] - 1;
                                    break;
                                case 'C':
                                    top = startIndexes[symbol] + CCounts[top];
                                    bottom = startIndexes[symbol] + CCounts[bottom + 1] - 1;
                                    break;
                                case 'G':
                                    top = startIndexes[symbol] + GCounts[top];
                                    bottom = startIndexes[symbol] + GCounts[bottom + 1] - 1;
                                    break;
                                case 'T':
                                    top = startIndexes[symbol] + TCounts[top];
                                    bottom = startIndexes[symbol] + TCounts[bottom + 1] - 1;
                                    break;
                                case '$':
                                    top = startIndexes[symbol] + XCounts[top];
                                    bottom = startIndexes[symbol] + XCounts[bottom + 1] - 1;
                                    break;
                            }
                        }
                        else {
                            ans[i] = 0;
                            break;
                        }
                    }
                    else {
                        ans[i] = bottom - top + 1;
                        break;
                    }
                }
            }

            return ans;
        }


        private class NuCount
        {
            public int A;
            public int C;
            public int G;
            public int T;
            public int X;
        }

        public void ConstructCountArrays(string bwt)
        {
            int length = bwt.Length;

            ACounts = new int[length + 1];
            CCounts = new int[length + 1];
            GCounts = new int[length + 1];
            TCounts = new int[length + 1];
            XCounts = new int[length + 1];

            var totalCounts = new NuCount();

            for (int i = 0; i < length; i++) {
                ACounts[i] = totalCounts.A;
                CCounts[i] = totalCounts.C;
                GCounts[i] = totalCounts.G;
                TCounts[i] = totalCounts.T;
                XCounts[i] = totalCounts.X;

                switch (bwt[i]) {
                    case 'A':
                        totalCounts.A++;
                        break;
                    case 'C':
                        totalCounts.C++;
                        break;
                    case 'G':
                        totalCounts.G++;
                        break;
                    case 'T':
                        totalCounts.T++;
                        break;
                    case '$':
                        totalCounts.X++;
                        break;
                }
            }

            ACounts[length] = totalCounts.A;
            CCounts[length] = totalCounts.C;
            GCounts[length] = totalCounts.G;
            TCounts[length] = totalCounts.T;
            XCounts[length] = totalCounts.X;
        }

        public static int[] ACounts;
        public static int[] CCounts;
        public static int[] GCounts;
        public static int[] TCounts;
        public static int[] XCounts;
    }
}