using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A9
{
    public class Q2OptimalDiet : Processor
    {
        public Q2OptimalDiet(string testDataName) : base(testDataName)
        {
            
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<int,int, double[,], String>)Solve);

        public string Solve(int cst, int v, double[,] matrix1)
        {
            CreateTableau(cst, v, matrix1);

            var (s, opt) = Solve();

            switch (s) {
                case SimplexState.unbounded:
                    return "Infinity";
                case SimplexState.infeasible:
                    return "No Solution";
                default: {
                    var ans = $"Bounded Solution\n{string.Join(' ', opt.Select(Q1InferEnergyValues.Round))}";
                    return ans;
                }
            }
        }

        private void CreateTableau(int constraints, int variables, double[,] matrix)
        {
            a = new double[constraints, variables + constraints];
            b = new double[constraints];
            c = new double[variables + constraints + 1];
            for (int row = 0; row < constraints; row++) {
                for (int col = 0; col < variables; col++) {
                    a[row, col] = matrix[row, col];
                }

                a[row, row + variables] = 1;
                
                b[row] = matrix[row, variables];
            }

            for (int col = 0; col < variables; col++) 
                c[col] = -matrix[constraints, col];

            n = variables;
            m = constraints;
            state = SimplexState.optimal;
            basicVars = Enumerable.Range(n, m).ToArray();
        }

        private int n, m;
        private double[,] a;
        private double[] b, c;
        private SimplexState state;
        private int[] basicVars;

        public enum SimplexState
        {
            unbounded = 0,
            infeasible = 1,
            optimal = 2
        }

        public (SimplexState s, double[] opt) Solve()
        {
            state = Math.Abs(c.Last()) < TOLERANCE ? SimplexState.optimal : SimplexState.infeasible;
            // initial check for infeasible? break
            if (state == SimplexState.infeasible)
                return (SimplexState.infeasible, null);

            // phase 2 = GJ
            int s = 0;
            Position p = FindPivot();
            while (state != SimplexState.unbounded && !(p.Row == -1 || p.Column == -1)) {
                ProcessPivot(p);
                p = FindPivot();
                s++;
            }
            
            // check for unbounded
            if (state == SimplexState.unbounded)
                return (SimplexState.unbounded, null);

            var ans = OptSol();
            // check if answer is in feasible region?
            if (!CheckAns(ans)) {
                return (SimplexState.infeasible, null);
            }

            return (SimplexState.optimal, ans);
        }

        public bool CheckAns(double[] ans)
        {
            for (int row = 0; row < m; row++) {
                double x = 0;
                for (int col = 0; col < n; col++) {
                    x += a[row, col] * ans[col];
                }

                if (x > b[row])
                    return false;
            }

            return true;
        }

        public double[] OptSol()
        {
            var ans = new double[n];
            for (int r = 0; r < m; r++) {
                if (basicVars[r] >= n) continue;
                ans[basicVars[r]] = b[r];
            }

            return ans;
        }

        public Position FindPivot()
        {
            // find entering
            var enteringC = c[0];
            var enteringCol = 0;
            for (var col = 0; col < n + m; col++) {
                if (c[col] < enteringC) {
                    enteringC = c[col];
                    enteringCol = col;
                }
            }

            if (enteringC >= 0) {
                state = SimplexState.optimal;
                return new Position(-1, -1);
            }

            // find departing
            var departingRow = 0;
            var minRatio = double.PositiveInfinity;
            for (var row = 0; row < m; row++) {
                if (Math.Abs(a[row, enteringCol]) < TOLERANCE) continue;
                var ratio = b[row] / a[row, enteringCol];
                if (ratio < 0) continue;
                if (ratio < minRatio) {
                    minRatio = ratio;
                    departingRow = row;
                }
            }

            if (double.IsInfinity(minRatio)) {
                state = SimplexState.unbounded;
                return null;
            }

            return new Position(enteringCol, departingRow);
        }

        public void ProcessPivot(Position pivotElement) // Elimination
        {
            var pc = pivotElement.Column;
            var pr = pivotElement.Row;
            var pVal = a[pr, pc];
            if (Math.Abs(pVal) < TOLERANCE)
                throw new Exception("pivot value zero");
            
            for (var col = 0; col < n + m; col++)
                a[pr, col] /= pVal;
            b[pr] /= pVal;

            for (var r = 0; r < m; r++) {
                if (r == pr) continue;
                var alpha = -a[r, pc];
                for (var col = 0; col < n + m; col++)
                    a[r, col] += alpha * a[pr, col];
                b[r] += alpha * b[pr];
            }

            var calpha = -c[pc];
            for (var col = 0; col < n + m; col++) {
                c[col] += calpha * a[pr, col];
            }
            c[n + m] += calpha * b[pr];
            
            // keep track of entering & departing
            basicVars[pr] = pc;
        }

        
        
        
        
        
        
        
        
        private const double TOLERANCE = 0.00000000000000000000000000000000000000000000000000000001;
    }
}