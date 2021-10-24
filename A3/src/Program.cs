using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCommon;

namespace A5
{
    public class Program
    {
        public static Random random = new Random();
        static void Main(string[] args) { }

        #region 1
        public static long[] BinarySearch1(long[] a, long[] b)
            => b.Select(toFind => BinarySearch(a, toFind, 0, a.Length - 1)).ToArray();

        public static long BinarySearch(long[] numbers, long toFind, int firstIndex, int lastIndex)
        {
            if (firstIndex > lastIndex) {
                return -1;
            }
            int midIndex = (firstIndex - lastIndex) / 2 + lastIndex;
            long midNumber = numbers[midIndex];
            if (midNumber == toFind) {
                return midIndex;
            }
            else if (midNumber < toFind) {
                return BinarySearch(numbers, toFind, midIndex + 1, lastIndex);
            }
            else {
                return BinarySearch(numbers, toFind, firstIndex, midIndex - 1);
            }
        }
        public static string ProcessBinarySearch1(string inStr) =>
            TestTools.Process(inStr, (Func<long[], long[], long[]>)BinarySearch1);
        #endregion
        #region 2
        public static long MajorityElement2(long n, long[] a)
        {
            long candidate = a[0];
            int i = 1;
            int count = 1;
            while (i < n) {
                if (candidate == a[i]) {
                    count++;
                } else {
                    count--;
                    if (count == 0) {
                        candidate = a[i];
                        count = 1;
                    }
                }
                i++;
            }
            count = 0;
            foreach (long x in a)
            {
                if (candidate == x) {
                    count++;
                }
            }
            if (count > a.Length/2) {
                return 1;
            } else {
                return 0;
            }
        }

        public static string ProcessMajorityElement2(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[], long>)MajorityElement2);
        #endregion
        #region 3
        public static long[] ImprovingQuickSort3(long n, long[] a)
        {
            ThreeWayQuickSort(a, 0, a.Length - 1);
            return a;
        }

        public static void ThreeWayQuickSort(long[] a, int low, int high)
        {
            if (low >= high) {
                return;
            }
            var pi = ThreeWayPartition(a, low, high);
            ThreeWayQuickSort(a, low, pi.pi1 - 1);
            ThreeWayQuickSort(a, pi.pi2 + 1, high);
        }

        public static (int pi1, int pi2) ThreeWayPartition(long[] a, int low, int high)
        {
            var randomIndex = random.Next(low, high+1);
            long pivot = a[randomIndex];
            ArraySwap(a, randomIndex, high);
            int lowPivot = high;
            int highPivot = high;
            int i = low;
            while (i < lowPivot) {
                if (a[i] == pivot) {
                    ArraySwap(a, lowPivot - 1, i);
                    lowPivot--;
                } else if (a[i] < pivot) {
                     i++;
                } else {
                    ArraySwap(a, highPivot, i);
                    ArraySwap(a, i, lowPivot - 1);
                    lowPivot--;
                    highPivot--;
                }
            }
            return (lowPivot, highPivot);
        }
        private static void ArraySwap(long[] a, int i, int j)
        {
            long tmp = a[i];
            a[i] = a[j];
            a[j] = tmp;
        }

        public static string ProcessImprovingQuickSort3(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[], long[]>)ImprovingQuickSort3);
        #endregion
        #region 4
        public static long NumberofInversions4(long n, long[] a)
        {
            return CustomMergeSort(a, 0, a.Length - 1);
        }

        public static int CustomMergeSort(long[] a, int low, int high)
        {
            if (low >= high) {
                return 0;
            }
            int mid = (high - low) / 2 + low;
            int numberofInversions = 0;
            numberofInversions += CustomMergeSort(a, low, mid);
            numberofInversions += CustomMergeSort(a, mid + 1, high);
            numberofInversions += CustomMerge(a, low, mid, high);
            return numberofInversions;
        }
        public static int CustomMerge(long[] a, int low, int mid, int high)
        {
            int n1 = mid - low + 1;
            int n2 = high - mid;
            long[] tmpL = a.Skip(low).Take(n1).ToArray();
            long[] tmpR = a.Skip(low+n1).Take(n2).ToArray();
            int i = 0;
            int j = 0;
            int walk = low;
            int numberofInversions = 0;
            while (i < n1 && j < n2) {
                if (tmpL[i] <= tmpR[j]) {
                    a[walk] = tmpL[i];
                    i++;
                } else {
                    a[walk] = tmpR[j];
                    j++;
                    numberofInversions += mid - (low + i) + 1;
                }
                walk++;
            }
            while (i < n1) {
                a[walk] = tmpL[i];
                walk++;
                i++;
            }
            while (j < n2) {
                a[walk] = tmpR[j];
                walk++;
                j++;
            }
            return numberofInversions;
        }

        public static string ProcessNumberofInversions4(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[], long>)NumberofInversions4);
        #endregion
        #region 5
        public static long[] OrganizingLottery5(long[] points, long[] startSegments,
            long[] endSegment)
        {
            var items = points.Select(p => new {value = p, type=2})
                .Concat(startSegments.Select(s => new {value = s, type=1}))
                .Concat(endSegment.Select(e => new {value = e, type=3}))
                .OrderBy(item => item.value).ThenBy(item => item.type).ToList();
            int scnt = 0;
            var cnt = new Dictionary<long, long>();
            foreach (var item in items) {
                switch (item.type) {
                    case 1:
                        scnt++;
                        break;
                    case 2:
                        cnt[item.value] = scnt;
                        break;
                    case 3:
                        scnt--;
                        break;
                }
            }
            return points.Select((p, i) => cnt[p]).ToArray();
        }

        public static string ProcessOrganizingLottery5(string inStr) =>
            TestTools.Process(inStr,(Func<long[], long[], long[], long[]>)OrganizingLottery5);
        #endregion
        #region 6
        public static double ClosestPoints6(long n, long[] xPoints, long[] yPoints)
        {
            double x;
            x = LargeMinDistance(xPoints.Select((p, i) => new Point
            {
                X = xPoints[i],
                Y = yPoints[i]
            }).ToList());
            return x;
        }

        public static double LargeMinDistance(List<Point> points)
        {
            if (points.Count <= 3) {
                return SmallMinDistance(points);
            }
            var sortedXP = points.OrderBy(p => p.X).ToList();
            int mid = points.Count / 2;
            var sortedXPL = sortedXP.Take(mid).ToList();
            var sortedXPR = sortedXP.Skip(mid).ToList();
            var minL = LargeMinDistance(sortedXPL);
            var minR = LargeMinDistance(sortedXPR);
            var minLR = Math.Min(minL, minR);
            var line = (sortedXPL.Last().X + sortedXPR.First().X)/2;
            var hypridMin = HypridMinDistance(sortedXPL, sortedXPR, line, minLR);
            return Math.Round(Math.Min(minLR, hypridMin), 4);
        }

        private static double HypridMinDistance(List<Point> sortedXPL,
            List<Point> sortedXPR,
            long line,
            double width)
        {
            var nearLinePoints = sortedXPR.Where(p => Math.Abs(p.X - line) <= width).ToList()
                .Concat(sortedXPL.Where(p => Math.Abs(p.X - line) <= width)).OrderBy(p => p.Y).ToList();
            double minDistance = width;
            for (int i = 0; i < nearLinePoints.Count; i++) {
                for (int j = i + 1; j < Math.Min(i + 8, nearLinePoints.Count); j++) {
                    if (Math.Abs(nearLinePoints[i].Y - nearLinePoints[j].Y) <= width) {
                        minDistance = Math.Min(minDistance, Distance(nearLinePoints[i], nearLinePoints[j]));
                    }
                }
            }
            return minDistance;
        }

        public static double SmallMinDistance(List<Point> points)
        {
            double minDistance = double.MaxValue;
            for (int i = 0; i < points.Count; i++) {
                for (int j = i + 1; j < points.Count; j++) {
                    minDistance = Math.Min(minDistance, Distance(points[i], points[j]));
                }
            }
            return Math.Round(minDistance, 4);
        }

        public static double Distance(Point p1, Point p2) => Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

        public static string ProcessClosestPoints6(string inStr) =>
           TestTools.Process(inStr, (Func<long, long[], long[], double>)
               ClosestPoints6);
        #endregion
    }

    public struct Point
    {
        public long X { get; set; }
        public long Y { get; set; } 
    }
}
