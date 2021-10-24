using System;
using System.Collections.Generic;
using TestCommon;

namespace A9
{
    public class MergingTables : Processor
    {
        public MergingTables(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long[], long[], long[], long[]>)Solve);

        public long[] Solve(long[] tableSizes, long[] sourceTables, long[] targetTables)
        {
            tables = new List<Table>(tableSizes.Length);
            maxNumOfRows = -1;
            for (int i = 0; i < tableSizes.Length; i++)
            {
                tables.Add(new Table(tableSizes[i]));
                maxNumOfRows = Math.Max(tables[i].NumberOfRows, maxNumOfRows);
            }

            var requests = new List<MergeRequest>(sourceTables.Length);
            for (int i = 0; i < sourceTables.Length; i++)
                requests.Add(new MergeRequest(
                        tables[(int)sourceTables[i] - 1],
                        tables[(int)targetTables[i] - 1]));

            var result = Solve(requests);

            return result.ToArray();
        }

        private static long maxNumOfRows = -1;
        private static List<Table> tables;
        public static List<long> Solve(List<MergeRequest> requests)
        {
            var result = new List<long>(requests.Count);
            foreach (var request in requests)
            {
                Merge(request.Source, request.Destination);
                result.Add(maxNumOfRows);
            }

            return result;
        }

        public static void Merge(Table source, Table destination)
        {
            var realDestination = destination.GetParent();
            var realSource = source.GetParent();

            if (realDestination == realSource)
                return;

            // merge two components here
            // use Rank heuristic
            // update maximumNumberOfRows

            if (realDestination.Rank > realSource.Rank)
            {
                realSource.Parent = realDestination;
                
                realDestination.NumberOfRows += realSource.NumberOfRows;
                realSource.NumberOfRows = 0;
                maxNumOfRows = Math.Max(realDestination.NumberOfRows,
                    maxNumOfRows);
            }
            else
            {
                realDestination.Parent = realSource;

                realSource.NumberOfRows += realDestination.NumberOfRows;
                maxNumOfRows = Math.Max(realSource.NumberOfRows,
                    maxNumOfRows);
                realDestination.NumberOfRows = 0;

                if (realSource.Rank == realDestination.Rank)
                    realSource.Rank++;
            }
        }

    }

    public class MergeRequest
    {
        public Table Source;
        public Table Destination;
        public MergeRequest(Table source, Table destination)
        {
            Source = source;
            Destination = destination;
        }
    }

    public class Table
    {
        public Table Parent;
        public long Rank;
        public long NumberOfRows;

        public Table(long numberOfRows)
        {
            this.NumberOfRows = numberOfRows;
            Rank = 0L;
            Parent = this;
        }
        public Table GetParent()
        {
            if (Parent != this)
                Parent = Parent.GetParent();

            return Parent;
        }
    }
}