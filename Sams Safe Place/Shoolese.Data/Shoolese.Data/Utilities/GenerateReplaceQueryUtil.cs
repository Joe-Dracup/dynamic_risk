using System;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace Shoolese.Data.Utilities
{
    public static class GenerateReplaceQueryUtil
    {
        public static SqlCommand GenerateReplaceQuery<T>(T itemToQuerify, SqlConnection connection, string tableName, string keyName, dynamic key)
        {
            var properties = GetAllExcept(typeof(T), keyName);
            var query = $"UPDATE {tableName} SET {GetColumnVariablePairs(GetAllExcept(typeof(T), keyName))} WHERE {keyName} = {key}";
            var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };

            foreach (var property in properties)
            {
                command.Parameters.Add($"@{property}", GetDbTypeOfProperty(typeof(T), property)).Value = GetValue(itemToQuerify, property) ?? DBNull.Value;
                Console.WriteLine($"ColumnName: {property}, Value: {GetValue(itemToQuerify, property)}");
            }
                
            command.Parameters.Add($@"{keyName}", GetDbTypeOfProperty(typeof(T), keyName)).Value = key ?? DBNull.Value;
            Console.WriteLine($"ColumnName: {keyName}, Value: {key}");

            return command;
        }

        private static string GetColumnVariablePairs(IEnumerable<string> columns) => string.Join(", ", columns.Select(x => SubIn(x)));

        private static string SubIn(string columnName) => $"[{columnName}] = @{columnName}";

        private static IEnumerable<string> GetAllExcept(Type type, string key) => type.GetProperties().Select(x => x.Name).Where(x => x != key);

        private static SqlDbType GetDbTypeOfProperty(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName);
            string underlyingName;

            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                underlyingName = Nullable.GetUnderlyingType(property.PropertyType).Name;
            } 
            else 
            {
                underlyingName = property.PropertyType.Name;
            }

            switch (underlyingName)
            {
                case "String":
                    return SqlDbType.VarChar;
                case "DateTime":
                    return SqlDbType.DateTime;
                case "Int32":
                    return SqlDbType.Int;
                case "Int64":
                    return SqlDbType.BigInt;
                case "Boolean":
                    return SqlDbType.Bit;
                case "Decimal":
                    return SqlDbType.Decimal;
                default:
                    return SqlDbType.Variant;
            }
        }

        private static dynamic GetValue<T>(T itemToQuerify, string propertyName)
        {
            var propertyInfo = itemToQuerify.GetType().GetProperty(propertyName);
            var getMethod = propertyInfo.GetMethod;
            return getMethod.Invoke(itemToQuerify, null);
        }
    }
}
