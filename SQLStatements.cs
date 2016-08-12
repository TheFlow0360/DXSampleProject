using DevExpress.Data;
using DevExpress.Data.Filtering;

namespace DevExpressGridInconsistencyDemo
{
    using System;
    using System.Collections.Generic;

    // ReSharper disable once InconsistentNaming
    public static class SQLStatements
    {
        public static String TableIdField(String tableName)
        {
            return $"{tableName}_ID";
        }

        public static QueryStatement GetSelectStatement(String tableName, QueryParameter queryParameter)
        {
            string tableIdColumn = TableIdField(tableName);
            string where = BuildFilterString(queryParameter);
            string orderBy = BuildOrderBy(queryParameter);
            string groupBy = BuildGroupBy(queryParameter);
            string summaryItems = BuildSummaryItems(queryParameter);


            List<string> select;
            if (queryParameter.PagingActive)
            {
                if (!String.IsNullOrWhiteSpace(groupBy))
                    throw new InvalidOperationException("Tried to use group by with paging.");
                select = new List<String>
                {
                    "WITH CTEPaging",
                    "AS (",
                    $"SELECT {tableIdColumn}",
                    $"FROM {tableName}",
                    String.IsNullOrWhiteSpace(where) ? String.Empty : "WHERE " + where,
                    "ORDER BY " + (String.IsNullOrWhiteSpace(orderBy) ? tableIdColumn : orderBy),
                    "OFFSET(@SKIP) ROWS",
                    "FETCH NEXT @TAKE ROWS ONLY",
                    ")",
                    "SELECT [table].*",
                    "FROM CTEPaging cte",
                    $"INNER JOIN {tableName} [table] ON [table].{tableIdColumn} = cte.{tableIdColumn};"
                };
            }
            else
                select = new List<string>
                {
                    "SELECT " + (String.IsNullOrWhiteSpace(groupBy) && String.IsNullOrWhiteSpace(summaryItems) ? "*" : BuildSelectedColumnsForAggregates(groupBy, summaryItems)),
                    $"FROM {tableName}",
                    String.IsNullOrWhiteSpace(where) ? String.Empty : "WHERE " + where,
                    String.IsNullOrWhiteSpace(groupBy) ? String.Empty : "GROUP BY " + groupBy,
                    String.IsNullOrWhiteSpace(groupBy) && String.IsNullOrWhiteSpace(summaryItems)
                        ? "ORDER BY " + (String.IsNullOrWhiteSpace(orderBy) ? tableIdColumn : orderBy)
                        : (String.IsNullOrWhiteSpace(orderBy) ? String.Empty : "ORDER BY " + orderBy)
                };

            var queryStatement = new QueryStatement(select);
            if (queryParameter.PagingActive)
            {
                queryStatement.AddParameter("TAKE", queryParameter.Take);
                queryStatement.AddParameter("SKIP", queryParameter.Skip);
            }           

            return queryStatement;
        }

        public static QueryStatement GetSelectKeysStatement(string tableName, QueryParameter queryParameter)
        {
            string tableIdColumn = TableIdField(tableName);
            string where = BuildFilterString(queryParameter);
            string orderBy = BuildOrderBy(queryParameter);
            string groupBy = BuildGroupBy(queryParameter);
            string summaryItems = BuildSummaryItems(queryParameter);

            if (!queryParameter.PagingActive || !String.IsNullOrWhiteSpace(groupBy))
                throw new InvalidOperationException();

            List<string> select = new List<String>
            {
                $"SELECT {tableIdColumn}",
                $"FROM {tableName}",
                String.IsNullOrWhiteSpace(where) ? String.Empty : "WHERE " + where,
                "ORDER BY " + (String.IsNullOrWhiteSpace(orderBy) ? tableIdColumn : orderBy),
                "OFFSET(@SKIP) ROWS",
                "FETCH NEXT @TAKE ROWS ONLY"
            };

            var queryStatement = new QueryStatement(select);
            queryStatement.AddParameter("TAKE", queryParameter.Take);
            queryStatement.AddParameter("SKIP", queryParameter.Skip);

            return queryStatement;
        }

        private static string BuildSelectedColumnsForAggregates(string groupBy, string summaryItems)
        {
            string columns;
            if (String.IsNullOrWhiteSpace(groupBy))
                columns = "COUNT(*)";
            else
                columns = String.Join(", ", groupBy, "COUNT(*)");
            if (!String.IsNullOrWhiteSpace(summaryItems))
                columns = String.Join(", ", columns, summaryItems);
            return columns;
        }

        public static QueryStatement GetRowCountStatement(string tableName, QueryParameter queryParameter)
        {
            string @where = BuildFilterString(queryParameter);

            var select = new List<String>
                             {
                                 "SELECT COUNT(*)",
                                 $"FROM {tableName}",
                                 String.IsNullOrWhiteSpace(where) ? String.Empty : "WHERE " + where
                             };

            return new QueryStatement(select);
        }

        public static QueryStatement GetColumnNames(string tableName)
        {
            return new QueryStatement($"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'");
        }

        private static string BuildFilterString(QueryParameter queryParameter)
        {
            return CriteriaToWhereClauseHelper.GetMsSqlWhere(queryParameter.Filter); // TODO check if MSSQL Server Version is needed
        }

        private static string BuildOrderBy(QueryParameter queryParameter)
        {
            if (queryParameter.SortBy == null || queryParameter.SortBy.Length == 0)
                return string.Empty;
            List<string> orderByClauses = new List<string>();
            foreach (ServerModeOrderDescriptor sortInfo in queryParameter.SortBy)
            {
                orderByClauses.Add(sortInfo.ToString());
            }
            return String.Join(", ", orderByClauses);
        }

        private static string BuildGroupBy(QueryParameter queryParameter)
        {
            return CriteriaToWhereClauseHelper.GetMsSqlWhere(queryParameter.GroupBy);
        }

        private static string BuildSummaryItems(QueryParameter queryParameter)
        {
            if (queryParameter.Summary == null || queryParameter.Summary.Length == 0)
                return string.Empty;
            List<string> summaryItems = new List<string>();
            foreach (ServerModeSummaryDescriptor summaryDescriptor in queryParameter.Summary)
            {
                string aggregateFunc;
                switch (summaryDescriptor.SummaryType)
                {
                    case Aggregate.Exists:
                        aggregateFunc = "EXISTS";
                        break;
                    case Aggregate.Count:
                        aggregateFunc = "COUNT";
                        break;
                    case Aggregate.Max:
                        aggregateFunc = "MAX";
                        break;
                    case Aggregate.Min:
                        aggregateFunc = "MIN";
                        break;
                    case Aggregate.Avg:
                        aggregateFunc = "AVG";
                        break;
                    case Aggregate.Sum:
                        aggregateFunc = "SUM";
                        break;
                    case Aggregate.Single:
                        throw new NotSupportedException();      // TODO check what this is supposed to be
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                summaryItems.Add(aggregateFunc + "(" + (!object.ReferenceEquals(summaryDescriptor.SummaryExpression, null) ? summaryDescriptor.SummaryExpression.ToString() : "*") + ")");
            }
            return String.Join(", ", summaryItems);
        }
    }
}
