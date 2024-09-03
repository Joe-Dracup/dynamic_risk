using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shoolese.Data
{
    public static class TypeExtensions
    {
        /// <summary>
        ///  Returns true if a Type is generic, and if that type leverages the Nullable<T> monad.
        /// </summary>
        public static bool IsNullable(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        /// <summary>
        /// If Type is Nullable<T>, returns T, else, returns T.
        /// </summary>
        public static Type GetTypeOrUnderlyingType(this Type type) =>
            type.IsNullable()
            ? Nullable.GetUnderlyingType(type)
            : type;

        //TODO: Test
        /// <summary>
        /// Returns IEnumerable of a Type's member property types.
        /// </summary>
        public static IEnumerable<Type> GetAllPropertyTypes(this Type type) =>
            type.GetProperties().Select(x => x.PropertyType);

        //TODO: Test
        /// <summary>
        /// Returns IEnumerable of a Type's member property types. Flattens Nullable<T>'s to T's.
        /// </summary>
        public static IEnumerable<Type> GetAllRootPropertyTypes(this Type type) =>
            type.GetAllPropertyTypes().Select(x => x.GetTypeOrUnderlyingType());
    }
}
