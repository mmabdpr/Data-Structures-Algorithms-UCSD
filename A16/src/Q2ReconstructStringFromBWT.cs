using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A6
{
    public class Q2ReconstructStringFromBWT : Processor
    {
        public Q2ReconstructStringFromBWT(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<String, String>)Solve);

        public string Solve(string bwt)
        {
            var firstColumn = bwt.ToCharArray();
            Array.Sort(firstColumn);

            var startIndexes = Enumerable.Repeat(-1, 256).ToArray();
            var counts = Enumerable.Repeat(0, 256).ToArray();
            var shortcutCounts = Enumerable.Repeat(-1, bwt.Length).ToArray();

            for (int i = 0; i < bwt.Length; i++) {
                int c = bwt[i];
                shortcutCounts[i] = counts[c]++;
                
                c = firstColumn[i];
                if (startIndexes[c] == -1)
                    startIndexes[c] = i;
            }

            var originalString = new char[bwt.Length];
            int shortcutIndex = 0;
            for (int i = 0; i < bwt.Length; i++) {
                char c = firstColumn[shortcutIndex];
                originalString[bwt.Length - i - 1] = c;
                shortcutIndex = startIndexes[bwt[shortcutIndex]] + shortcutCounts[shortcutIndex];
            }

            return string.Join("", originalString);
        }
    }
}
