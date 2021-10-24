using System;
using System.Collections.Generic;
using System.Linq;

namespace A3
{
    public class Program
    {
        #region pre

        static void Main() {}

        public static string Process(string inStr, Func<long, long> longProcessor)
        {
            long n = long.Parse(inStr);
            return longProcessor(n).ToString();
        }
        
        public static string Process(string inStr, Func<long, long, long> longProcessor)
        {
            var toks = inStr.Split(new[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            long a = long.Parse(toks[0]);
            long b = long.Parse(toks[1]);
            return longProcessor(a, b).ToString();
        }


        #endregion
        
        #region 1

        public static long Fibonacci(long n)
        {
            if (n <= 1) return n;

            int a = 0;
            int b = 1;
            int c = 1;

            int i = 2;

            while (i <= n)
            {
                c = a + b;
                a = b;
                b = c;
                i++;
            }

            return c;
        }

        public static string ProcessFibonacci(string inStr) => Process(inStr, Fibonacci);

        #endregion

        #region 2

        public static long Fibonacci_LastDigit(long n)
        {
            if (n <= 1) return n;

            int a = 0;
            int b = 1;
            int c = 1;

            int i = 2;

            while (i <= n)
            {
                c = (a + b) % 10;
                a = b;
                b = c;
                i++;
            }

            return c;
            
            // Fibonacci_Mod(n, 10)
        }

        public static string ProcessFibonacci_LastDigit(string inStr) => Process(inStr, Fibonacci_LastDigit);

        #endregion

        #region 3

        // ReSharper disable once InconsistentNaming
        public static long GCD(long a, long b)
        {
            if (a < b)
            {
                a = b - a;
                b = b - a;
                a = b + a;
            }

            while (b != 0)
            {
                long r = a % b;
                a = b;
                b = r;
            }

            return a;
        }

        // ReSharper disable once InconsistentNaming
        public static string ProcessGCD(string inStr) => Process(inStr, GCD);

        #endregion

        #region 4

        // ReSharper disable once InconsistentNaming
        public static long LCM(long a, long b)
        {
            return (a * b) / GCD(a, b);
        }
        
        // ReSharper disable once InconsistentNaming
        public static string ProcessLCM(string inStr) => Process(inStr, LCM);

        #endregion

        #region 5

        public static long Fibonacci_Mod(long n, long m)
        {
            if (n <= 1) return n;

            long a = 0;
            long b = 1;
            long c;

            var fibMod = new List<long>();
            fibMod.Add(0); fibMod.Add(1);

            do
            {
                c = (a + b) % m;
                fibMod.Add(c);
                a = b;
                b = c;

                if (fibMod.Count > 3 && Enumerable.SequenceEqual(fibMod.Skip(fibMod.Count - 3).ToArray(), new long[] {0, 1, 1}))
                {
                    fibMod.RemoveAt(fibMod.Count - 1);
                    fibMod.RemoveAt(fibMod.Count - 2);
                    fibMod.RemoveAt(fibMod.Count - 3);
                    break;
                }

            } while (true);

            int l = fibMod.Count;

            var ans = fibMod[(int)(n % l)];
            
            return ans;
        }

        public static string ProcessFibonacci_Mod(string inStr) => Process(inStr, Fibonacci_Mod);

        #endregion

        #region 6

        public static long Fibonacci_Sum(long n) => (Fibonacci_Mod(n+2, 10) + 9) % 10;

        public static string ProcessFibonacci_Sum(string inStr) => Process(inStr, Fibonacci_Sum);

        #endregion

        #region 7

        public static long Fibonacci_Partial_Sum(long n, long m)
        {
            if (n < m)
            {
                n = m - n;
                m = m - n;
                n = m + n;
            }
            return (Fibonacci_Sum(n) + 10 - Fibonacci_Sum(m - 1)) % 10;
        }

        public static string ProcessFibonacci_Partial_Sum(string inStr) => Process(inStr, Fibonacci_Partial_Sum);

        #endregion

        #region 8

        public static long Fibonacci_Sum_Squares(long n)
        {
            var fld = Fibonacci_Mod(n, 10);
            var fldm1 = Fibonacci_Mod(n - 1, 10);
            return (fld * (fld + fldm1)) % 10;
        }

        public static string ProcessFibonacci_Sum_Squares(string inStr) => Process(inStr, Fibonacci_Sum_Squares);

        #endregion
    }
}
