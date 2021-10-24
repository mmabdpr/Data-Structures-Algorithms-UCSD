using System;
using System.Linq;
using System.Runtime.InteropServices;
using TestCommon;

namespace A9
{
    public class Q1InferEnergyValues : Processor
    {
        public Q1InferEnergyValues(string testDataName) : base(testDataName)
        {
//            this.ExcludeTestCaseRangeInclusive(1,4);
//            this.ExcludeTestCaseRangeInclusive(6,28);
        }

        public override string Process(string inStr) =>
            // ReSharper disable once RedundantCast
            TestTools.Process(inStr, (Func<long, double[,], double[]>) Solve);

        public double[] Solve(long matrixSize, double[,] matrix)
        {
            var equation = EnergyValues.ReadEquation(matrix);
            var b = EnergyValues.SolveEquation(equation);
            b = b.Select(Round).ToArray();
            return b;
        }

        public static double Round(double x)
        {
            if (Math.Abs(x - (int) x) < 0.25)
                return (int) x;
            if (Math.Abs(x - (int) x) > 0.75)
                return x < 0 ? (int) x - 1 : (int) x + 1;
            return x < 0 ? (int) x - 0.5 : (int) x + 0.5;
        }
    }
}