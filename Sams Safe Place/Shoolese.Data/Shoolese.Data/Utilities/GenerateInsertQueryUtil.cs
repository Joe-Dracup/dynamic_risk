using System;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace Shoolese.Data.Utilities
{
    public static class GenerateInsertQueryUtil
    {
        public static SqlCommand GenerateInsertQuery<T>(this T itemToQuerify, SqlConnection connection, string tableName, string key)
        {
            var properties = GetAllPropertiesExcept(typeof(T), key);
            var query = $"INSERT INTO {tableName}({GetColumnsList(properties)}) OUTPUT INSERTED.{key} VALUES ({GetValuesList(properties)})";
            var command = new SqlCommand(query, connection) { CommandType = CommandType.Text };

            foreach(var property in properties)
                command.Parameters.Add($"@{property}", GetDbTypeOfProperty(typeof(T), property)).Value = GetValue(itemToQuerify, property) ?? DBNull.Value;

            return command;
        }

        private static IEnumerable<string> GetAllPropertiesExcept(Type type, string key) => type.GetProperties().Select(x => x.Name).Where(x => x != key);

        private static string GetColumnsList(IEnumerable<string> columnNames) => $"[{string.Join("],[", columnNames)}]";

        private static string GetValuesList(IEnumerable<string> columnNames) => $"@{string.Join(",@", columnNames)}";

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
                    throw new Exception("An unhandled C# to SqlDbType");
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
