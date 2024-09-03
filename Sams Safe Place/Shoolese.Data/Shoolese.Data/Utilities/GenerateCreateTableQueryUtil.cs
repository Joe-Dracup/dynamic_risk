using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Shoolese.Data.Utilities
{
    public static class GenerateCreateTableQueryUtil
    {
        public static Result<string> GenerateCreateTableCommand<T>(string schemaName = "dbo", string tableName = null)
        {
            var verifyTypeIsSuitable = Result.Combine(
                    ",",
                    ReflectionHelper.VerifyOnlyExpectedTypes<T>(Constants.AllowedTypes),
                    ReflectionHelper.VerifyFirstPropertyContainsTheSubstringIdAndIsAnIntOrLong<T>()
                );

            if (verifyTypeIsSuitable.IsFailure)
                return Result.Failure<string>(verifyTypeIsSuitable.Error);

            if (tableName == null)
                tableName = typeof(T).Name;

            var createQueryResult = QueryBuilder<T>(schemaName, tableName);
            if (createQueryResult.IsFailure)
                return Result.Failure<string>(createQueryResult.Error);

            return Result.Success<string>(createQueryResult.Value);
        }

        private static Result<string> QueryBuilder<T>(string schemaName, string tableName)
        {
            var rows = CreateRowsForType<T>();
            if (rows.IsFailure)
                return Result.Failure<string>(rows.Error);

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"IF NOT EXISTS ( SELECT  * FROM sys.schemas WHERE   name = N'{schemaName}') EXEC('CREATE SCHEMA {schemaName}'); ");
            stringBuilder.Append("IF(NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = ");
            stringBuilder.Append($"'{schemaName}' \n");
            stringBuilder.Append(" AND TABLE_NAME = ");
            stringBuilder.Append($"'{tableName}' ))\n");
            stringBuilder.Append($"CREATE TABLE {schemaName}.{tableName} (\n");
            foreach(var row in rows.Value)
            {
                stringBuilder.Append(row+"\n");
            }
            stringBuilder.Append($");");

            return Result.Success(stringBuilder.ToString());
        }

        private static Result<IEnumerable<string>> CreateRowsForType<T>()
        {
            var propertiesMappedToTypes = ReflectionHelper.MapCSharpTypesToSqlDbTypes<T>();
            if (propertiesMappedToTypes.IsFailure)
                return Result.Failure<IEnumerable<string>>(propertiesMappedToTypes.Error);

            var preppedLines = new List<string>();

            for (var i = 0; i < propertiesMappedToTypes.Value.Count(); i++)
            {
                var propertyTypeMapping = propertiesMappedToTypes.Value.ToList()[i];
                if (propertyTypeMapping.Key.Name.ToUpper().Contains("ID") && i == 0 ) // First one only.
                {
                    preppedLines.Add(PrepIdRow(propertyTypeMapping));
                } else if (propertiesMappedToTypes.Value.Count() - 1 == i)
                {
                    preppedLines.Add(propertyTypeMapping.PrepareCreateTableRowForType());
                } else
                {
                    preppedLines.Add(PrepRow(propertyTypeMapping));
                }
            }

            return preppedLines;
        }

        private static string PrepRow(KeyValuePair<PropertyInfo, SqlDbType> propertyTypeMapping) =>
            propertyTypeMapping.PrepareCreateTableRowForType() + ",\n";

        private static string PrepIdRow(KeyValuePair<PropertyInfo, SqlDbType> propertyTypeMapping)
            => $"[{propertyTypeMapping.Key.Name}] INT IDENTITY(1,1) PRIMARY KEY, \n";
    }
}
