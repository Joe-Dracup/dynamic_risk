using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Shoolese.Data
{
    public static class CSharpToTSQLQueryHelper
    {
        //TODO: Get a bunch of tests for this
        /// <summary>
        ///  Returns a string of the form "[ColumnName] <SqlDatatype> [Null / Not Null]" for use in Queries.
        /// </summary>
        public static string PrepareCreateTableRowForType(this KeyValuePair<PropertyInfo, SqlDbType> propertyTypeMapping) =>
            $"[{propertyTypeMapping.Key.Name}] " +  // ColumnName
            $"{ResolveUnderlyingTypeToSqlDbType(propertyTypeMapping.Key.PropertyType)}" + // Type
            $" {propertyTypeMapping.Key.PropertyType.TypeIsNullOrNotNull()}"; // NULL or NOT NULL

        private static string TypeIsNullOrNotNull(this Type type) =>
            type.IsNullable()
            ? ""
            : "NOT NULL";

        private static string ResolveUnderlyingTypeToSqlDbType(this Type type)
            => Constants.CSharpToSqlTypeAsStringsDictionary[type.GetTypeOrUnderlyingType()];

        public static Result<KeyValuePair<PropertyInfo,SqlDbType>> ResolveTypeToSqlType(this PropertyInfo propertyInfo)
        {
            var canGetFromDict = Constants.CSharpTypeToSqlTypeDictionary.TryGetValue(propertyInfo.PropertyType.GetTypeOrUnderlyingType(), out SqlDbType outputType);
            if (!canGetFromDict)
                return Result.Failure<KeyValuePair<PropertyInfo, SqlDbType>>(FailureReasons.TypeCouldNotBeResolvedToSqlDbType(propertyInfo.Name, Constants.AllowedTypesNames));

            return Result.Success(new KeyValuePair<PropertyInfo, SqlDbType>(propertyInfo, outputType));
        }
        
        public static class FailureReasons
        {
            public static string TypeCouldNotBeResolvedToSqlDbType(string nameOfType, IEnumerable<string> supportedTypes)
                => $"{nameOfType} could not be resolved to a SQL DB Type, Currently supported types are {string.Join(',', supportedTypes)}";
        }
    }
}
