using System;

namespace A9 {
    public static class EnergyValues
    {
        public static Equation ReadEquation(double[,] matrix)
        {
            var rowCount = matrix.GetLength(0);
            var colCount = matrix.GetLength(1);
            var a = new double[rowCount, colCount - 1];
            var b = new double[rowCount];
            for (var row = 0; row < rowCount; ++row) {
                for (var column = 0; column < colCount - 1; ++column)
                    a[row, column] = matrix[row, column];
                b[row] = matrix[row, colCount - 1];
            }

            return new Equation(a, b);
        }

        public static Position SelectPivotElement(double[,] a, bool[] usedRows, bool[] usedColumns)
        {
            // This algorithm selects the first free element.
            // You'll need to improve it to pass the problem. TODO
            var pivotElement = new Position(0, 0);
            while (usedRows[pivotElement.Row])
                ++pivotElement.Row;
            while (usedColumns[pivotElement.Column])
                ++pivotElement.Column;

            var max = a[pivotElement.Row, pivotElement.Column];
            var rowCount = a.GetLength(0);
            var pc = pivotElement.Column;
            for (var r = pivotElement.Row + 1; r < rowCount; r++) {
                if (Math.Abs(a[r, pc]) > Math.Abs(max)) {
                    max = a[r, pc];
                    pivotElement.Row = r;
                }
            }
            
            return pivotElement;
        }

        public static void SwapLines(double[,] a, double[] b, bool[] usedRows, Position pivotElement)
        {
            var colCount = a.GetLength(0);
            var pivotPreviousRow = pivotElement.Row;
            pivotElement.Row = pivotElement.Column;

            for (var column = 0; column < colCount; ++column) {
                var tmpa = a[pivotElement.Column, column];
                a[pivotElement.Column, column] = a[pivotPreviousRow, column];
                a[pivotPreviousRow, column] = tmpa;
            }

            var tmpb = b[pivotElement.Column];
            b[pivotElement.Column] = b[pivotPreviousRow];
            b[pivotPreviousRow] = tmpb;
            
            var tmpu = usedRows[pivotElement.Column];
            usedRows[pivotElement.Column] = usedRows[pivotPreviousRow];
            usedRows[pivotPreviousRow] = tmpu;
            
        }

        public static void ProcessPivotElement(double[,] a, double[] b, Position pivotElement)
        {
            var colCount = a.GetLength(1);
            var pc = pivotElement.Column;
            var pr = pivotElement.Row;
            var pVal = a[pr, pc];
            for (var c = 0; c < colCount; c++)
                a[pr, c] /= pVal;
            b[pr] /= pVal;

            var rowCount = a.GetLength(0);
            for (var r = 0; r < rowCount; r++) {
                if (r == pr) continue;
                var alpha = -a[r, pc];
                for (var c = 0; c < colCount; c++)
                    a[r, c] += alpha * a[pr, c];
                b[r] += alpha * b[pr];
            }
        }

        private static void MarkPivotElementUsed(Position pivotElement, bool[] usedRows, bool[] usedColumns)
        {
            usedRows[pivotElement.Row] = true;
            usedColumns[pivotElement.Column] = true;
        }

        public static double[] SolveEquation(Equation equation)
        {
            var a = equation.A;
            var b = equation.B;
            var rowCount = a.GetLength(0);
            var colCount = a.GetLength(1);
            var usedColumns = new bool[colCount];
            var usedRows = new bool[rowCount];
            
            for (var step = 0; step < rowCount; ++step) {
                var pivotElement = SelectPivotElement(a, usedRows, usedColumns);
                SwapLines(a, b, usedRows, pivotElement);
                ProcessPivotElement(a, b, pivotElement);
                MarkPivotElementUsed(pivotElement, usedRows, usedColumns);
            }

            return b;
        }
    }
}