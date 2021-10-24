using System;
using System.Collections.Generic;
using System.Linq;

namespace A2
{
    public class Program
    {
        static void Main()
        {
        }

        public static string Process(string input)
        {
            var inData = input.Split(new[] {'\n', '\r', ' '},
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToList();

            // return NaiveMaxPairwiseProduct(inData).ToString();
            // return FastPairwiseProduct(inData).ToString();
            return EvenFasterPairwiseProduct(inData).ToString();
        }

        public static int NaiveMaxPairwiseProduct(List<int> numbers)
        {
            int max = 0;
            for (int i = 0; i < numbers.Count; i++)
            {
                for (int j = i + 1; j < numbers.Count; j++)
                {
                    max = Math.Max(max, numbers[i] * numbers[j]);
                }
            }

            return max;
        }

        public static int FastPairwiseProduct(List<int> numbers)
        {
            if (numbers.Count == 0 || numbers.Count == 1) return 0;

            int index1 = 0;
            for (int i = 1; i < numbers.Count; i++)
            {
                if (numbers[i] > numbers[index1]) index1 = i;
            }

            var index2 = index1 == 0 ? 1 : 0;
            for (int i = 1; i < numbers.Count; i++)
            {
                if (i != index1 &&
                    numbers[i] > numbers[index2] &&
                    numbers[i] <= numbers[index1]) index2 = i;
            }

            return numbers[index1] * numbers[index2];
        }

        public static int EvenFasterPairwiseProduct(List<int> numbers)
        {
            int max1 = 0, max2 = 0;
            foreach (int x in numbers)
            {
                if (x > max1)
                {
                    max2 = max1;
                    max1 = x;
                }
                else
                {
                    if (x > max2)
                    {
                        max2 = x;
                    }
                }
            }

            return max1 * max2;
        }
    }
}