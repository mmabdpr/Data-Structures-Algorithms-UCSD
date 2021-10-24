using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TestCommon;

namespace A4
{
    public class Program
    {
        public static void Main(string[] args) {}

        #region 1
        public static long ChangingMoney1(long money)
        {
            var coins = new [] {10, 5, 1};
            long counts = 0;

            for (int i = 0; i < coins.Length; i++)
            {
                while (money >= coins[i])
                {
                    counts++;
                    money -= coins[i];
                }
            }

            return counts;
        }

        public static string ProcessChangingMoney1(string inStr) => TestTools.Process(inStr, (Func<long, long>) ChangingMoney1);

        #endregion

        #region 2
        public static long MaximizingLoot2(long capacity, long[] weights, long[] values)
        {
            var items = weights.Select((w, i) => new {
                    weight = weights[i],
                    value = values[i],
                    valuesPerWeight = (float)values[i] / weights[i]
                }).OrderByDescending(o => o.valuesPerWeight).ToList();

            long value = 0;
            foreach (var item in items)
            {
                if (capacity == 0) break;
                var howMuchToTake = Math.Min(capacity, item.weight);
                if (howMuchToTake == item.weight) value += item.value;
                else value += (long)(howMuchToTake * item.valuesPerWeight);
                capacity -= howMuchToTake;
            }
            return value;
        }

        public static string ProcessMaximizingLoot2(string inStr) => TestTools.Process(inStr, (Func<long, long[], long[], long>) MaximizingLoot2);

        #endregion

        #region 3

        public static long MaximizingOnlineAdRevenue3(long slotCount, long[] adRevenue, long[] averageDailyClick)
        {
            var sortedA = adRevenue.OrderByDescending(x => x);
            var sortedB = averageDailyClick.OrderByDescending(x => x).ToArray();
            var sum = sortedA.Select((x, i) => x * sortedB[i]).Sum();
            return sum;
        }

        public static string ProcessMaximizingOnlineAdRevenue3(string inStr) => TestTools.Process(inStr, (Func<long, long[], long[], long>) MaximizingOnlineAdRevenue3);

        #endregion

        #region 4

        public static long CollectingSignatures4(long tenantCount, long[] startTimes, long[] endTimes)
        {
            var tenants = startTimes.Select((x, i) => new {
                startTime = x,
                endTime = endTimes[i]
            }).OrderBy(t => t.endTime).ToList();

            long count = 0;
            while (tenants.Count > 0)
            {
                var current = tenants[0];
                for (int i = 1; i < tenants.Count; i++)
                {
                    if (tenants[i].startTime <= current.endTime)
                    {
                        tenants[i] = null;
                    }
                }
                count++;
                tenants.RemoveAt(0);
                tenants.RemoveAll(t => t == null);
            }

            return count;
        }
        public static string ProcessCollectingSignatures4(string inStr) => TestTools.Process(inStr, (Func<long, long[], long[], long>) CollectingSignatures4);

        #endregion

        #region 5

        public static long[] MaximizeNumberOfPrizePlaces5(long n)
        {
            var prizes = new List<long>();
            long prize = 1;
            while (n > 0)
            {
                if (n - prize > prize || n == prize)
                {
                    n -= prize;
                    prizes.Add(prize);                   
                }
                prize++;
            }
            return prizes.ToArray();
        }

        public static string ProcessMaximizeNumberOfPrizePlaces5(string inStr) => TestTools.Process(inStr, (Func<long, long[]>) MaximizeNumberOfPrizePlaces5);

        #endregion

        #region 6

        public static string MaximizeSalary6(long n, long[] numbers)
        {
            Array.Sort(numbers, new Comparer());
            return string.Join("", numbers);
        }

        public static string ProcessMaximizeSalary6(string inStr) => TestTools.Process(inStr, (Func<long, long[], string>)MaximizeSalary6);

        #endregion


    }

    #region 6
    public class Comparer : IComparer
    {
        public int Compare(object x, object y)
        {
            long a = (long)x;
            long b = (long)y;
            return long.Parse(a.ToString() + b.ToString()) < long.Parse(b.ToString()+a.ToString()) ? 1 : -1;
        }
    }

    #endregion
}
