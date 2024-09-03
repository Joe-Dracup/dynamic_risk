using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Shoolese.Data
{
    public static class ReflectionHelper
    {

        public static string GetNameOfFirstProperty<T>() => typeof(T).GetProperties().First().Name;

        #region Verify First Property is Called Id and is an Int
        public static Result VerifyFirstPropertyContainsTheSubstringIdAndIsAnIntOrLong<T>()
        {
            var firstPropertyName = GetNameOfFirstProperty<T>();
            var firstPropertyType = typeof(T).GetAllRootPropertyTypes().First();
            if (firstPropertyName.ToUpper().Contains("ID") && (firstPropertyType == typeof(int)) || firstPropertyType == typeof(long))
                return Result.Success();

            return Result.Failure(FailureReasons.FirstPropertyOfTypeIsNotAnIntegerId(typeof(T).Name, firstPropertyName));
        }
        #endregion

        #region Verify Only Expected Types Exist on a type
        public static Result VerifyOnlyExpectedTypes<T>(IEnumerable<Type> allowedTypes) =>
            VerifyOnlyExpectedTypes(typeof(T).GetAllRootPropertyTypes(), allowedTypes);

        /// <summary>
        /// Returns true if all the provided property info represents primitives (including nullable<primitive>)
        /// </summary>
        public static Result VerifyOnlyExpectedTypes(IEnumerable<Type> types, IEnumerable<Type> allowedTypes)
        {
            foreach(var type in types)
            {
                if (!allowedTypes.Contains(type))
                    return Result.Failure(FailureReasons.TypeWasNotInAllowedList(type.Name, allowedTypes.Select(x => x.Name)));
            }

            return Result.Success();
        }
        #endregion


        public static Result<IEnumerable<KeyValuePair<PropertyInfo, SqlDbType>>> MapCSharpTypesToSqlDbTypes<T>()
        {
            var output = new List<KeyValuePair<PropertyInfo, SqlDbType>>();

            foreach(var property in typeof(T).GetProperties())
            {
                var attemptToPair = property.ResolveTypeToSqlType();
                if (attemptToPair.IsFailure)
                    return Result.Failure<IEnumerable<KeyValuePair<PropertyInfo, SqlDbType>>>(attemptToPair.Error);

                output.Add(attemptToPair.Value);
            }

            return Result.Success(output.AsEnumerable());
        }
        
        public static class FailureReasons
        {
            public static string TypeWasNotInAllowedList(string typeName, IEnumerable<string> allowedTypeNames)
                => $"Type: {typeName}, was not in the allowed list, which is: {string.Join(',', allowedTypeNames)}";

            public static string FirstPropertyOfTypeIsNotAnIntegerId(string typeName, string firstPropertyName)
                => $"{typeName}.{firstPropertyName} is not called 'ID', and it is not an integer! - Need this for keys";
        }
    }
}
