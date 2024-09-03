using CSharpFunctionalExtensions;
using System;
using System.Data.SqlClient;

namespace Shoolese.Data
{
    //TODO: TEST
    internal static class SQLConnectionVerifier
    {
        public static Result VerifyConnectionString(string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return Result.Success();
                }
            } 
            catch (Exception ex)
            {
                return Result.Failure(FailureReasons.ConnectionStringIsntValid(connectionString, ex.Message));
            }
        }

        public static class FailureReasons
        {
            public static string ConnectionStringIsntValid(string connectionString, string exceptionMessage) => 
                $"Could not establish connection with connection string: {connectionString}, please try a valid connection string \n Exception: {exceptionMessage}";
        }
    }
}
