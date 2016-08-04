using System;
using System.Data.SqlClient;

namespace DevExpressGridInconsistencyDemo
{
    using System.Collections.Generic;

    public class EntityManager: IEntityManager
    {
        private string _connectionString;
        private string _tableName;

        public string IdColumn => $"{_tableName}_ID";

        public EntityManager(string connectionString, string tableName)
        {
            _connectionString = connectionString;
            _tableName = tableName;
        }

        public List<DataInformation> GetColumnInformation()
        {
            QueryStatement statement = SQLStatements.GetColumnNames(_tableName);
            List<DataInformation> columns = new List<DataInformation>();
            this.ReadFromDatabase(
                statement,
                reader =>
                {
                    while (reader.Read())
                    {
                        var columnName = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
                        var type = reader.GetString(reader.GetOrdinal("DATA_TYPE"));
                        columns.Add(new DataInformation(ConvertFromSqlTypeToType(type), columnName));
                    }
                });
            return columns;
        }

        public List<DataFieldList> GetEntities(QueryParameter queryParameter)
        {
            QueryStatement statement = SQLStatements.GetSelectStatement(_tableName, queryParameter);
            List<DataFieldList> entityList = new List<DataFieldList>();
            this.ReadFromDatabase(
                statement,
                reader =>
                {
                    while (reader.Read())
                    {
                        DataFieldList dataFields = ReadDataFieldList(reader);
                        entityList.Add(dataFields);
                    }
                });
            return entityList;
        }

        public int GetEntityCount(QueryParameter queryParameter)
        {
            QueryStatement statement = SQLStatements.GetRowCountStatement(_tableName, queryParameter);
            DataFieldList dataFieldList = new DataFieldList();
            this.ReadFromDatabase(
                statement,
                reader =>
                {
                    if (reader.Read())
                    {
                        dataFieldList = ReadDataFieldList(reader);
                    }
                });
            return dataFieldList[0].Value as int? ?? 0;
        }

        private void ReadFromDatabase(QueryStatement queryStatement, Action<SqlDataReader> onDbRead)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(queryStatement.Statement, connection))
                {
                    foreach (var param in queryStatement.Parameters)
                    {
                        var sqlParameter = new SqlParameter(param.Key, param.Value ?? DBNull.Value);
                        command.Parameters.Add(sqlParameter);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        onDbRead(reader);
                    }
                }
            }
        }

        private static DataFieldList ReadDataFieldList(SqlDataReader reader)
        {
            DataFieldList dataFieldList = new DataFieldList();
            for (int index = 0; index < reader.FieldCount; index++)
            {
                object value = reader.GetValue(index);
                if (value is DBNull)
                {
                    value = null;
                }
                dataFieldList.Add(new DataField(reader.GetFieldType(index), reader.GetName(index), value));
            }
            return dataFieldList;
        }

        private static Type ConvertFromSqlTypeToType(String sqlType)
        {
            if (String.IsNullOrWhiteSpace(sqlType))
            {
                throw new ArgumentNullException(nameof(sqlType));
            }

            switch (sqlType.ToLower())
            {
                case "nvarchar":
                case "varchar":
                case "xml":
                    return typeof(String);
                case "bigint":
                    return typeof(Int64);
                case "int":
                    return typeof(Int32);
                case "smallint":
                    return typeof(Int16);
                case "tinyint":
                    return typeof(Byte);
                case "real":
                    return typeof(Single);
                case "float":
                    return typeof(Double);
                case "decimal":
                    return typeof(Decimal);
                case "date":
                case "time":
                case "datetime":
                    return typeof(DateTime);
                case "uniqueidentifier":
                    return typeof(Guid);
                case "bit":
                    return typeof(Boolean);
                case "blob":
                case "varbinary":
                case "timestamp":
                    return typeof(Byte[]);
            }
            throw new ArgumentException(sqlType, nameof(sqlType));
        }
    }
}
