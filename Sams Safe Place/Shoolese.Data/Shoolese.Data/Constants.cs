using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Shoolese.Data
{
    public static class Constants
    {

        public static Dictionary<Type, SqlDbType> CSharpTypeToSqlTypeDictionary = new Dictionary<Type, SqlDbType>()
        {
            { typeof(string), SqlDbType.VarChar },
            { typeof(DateTime), SqlDbType.DateTime },
            { typeof(int), SqlDbType.Int },
            { typeof(long), SqlDbType.BigInt },
            { typeof(bool), SqlDbType.Bit },
            { typeof(decimal), SqlDbType.Decimal }
        };

        public static Dictionary<Type, string> CSharpToSqlTypeAsStringsDictionary = new Dictionary<Type, string>()
        {
            { typeof(string), "VARCHAR(1000)" },
            { typeof(DateTime), "DATETIME" },
            { typeof(int), "INT" },
            { typeof(long), "BIGINT" },
            { typeof(bool), "BIT" },
            { typeof(decimal), "DECIMAL(10,10)" },
        };

        public static IEnumerable<Type> AllowedTypes => CSharpTypeToSqlTypeDictionary.Select(x => x.Key);
        public static IEnumerable<string> AllowedTypesNames => AllowedTypes.Select(x => x.Name);
    }
}
