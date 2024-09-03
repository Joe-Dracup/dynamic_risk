using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Shoolese.Data.Utilities
{
    internal interface ISqlExecutor
    {
        Result<IEnumerable<T>> ExecuteSelectQuery<T>(string query) where T : new();
        Result<int> Insert<T>(T objectToInsert, string tableName, string key);
        Result Replace<T>(T objectToReplace, string tableName, string keyName, dynamic key);
        Result ExecuteQuery(string query);
        Result Delete<T>(T idOfObjectToDelete, string tableName);
    }

    internal class SqlExecutor : ISqlExecutor
    {
        private readonly SqlConnection _connection;
        private string _connectionString;

        public SqlExecutor(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public Result ExecuteQuery(string query)
        {
            try
            {
                var command = GetCommand(query);
                command.ExecuteNonQuery();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure("Fucked it " + ex.Message);
            }
        }

        public Result<IEnumerable<T>> ExecuteSelectQuery<T>(string query) where T : new()
        {
            try
            {
                var objects = new List<T>();
                using (var command = GetCommand(query))
                {
                    var da = new SqlDataAdapter(command);
                    var dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        var newItem = dr.ToObject<T>();
                        objects.Add(newItem);
                    }

                    return Result.Success(objects.AsEnumerable());
                }
            } catch (Exception ex)
            {
                return Result.Failure<IEnumerable<T>>(FailureReasons.CouldNotMaterialize(typeof(T), ex.Message));
            }
        }
        public Result Delete<T>(T idOfObjectToDelete, string tableName)
        {
            var deleteObjectQuery = $"DELETE FROM {tableName} WHERE ID = {idOfObjectToDelete}";
            try
            {
                using (var command = GetCommand(deleteObjectQuery))
                {
                    var da = new SqlDataAdapter(command);
                    var dt = new DataTable();
                    da.Fill(dt);

                    return Result.Success();
                }
            } 
            catch (Exception ex)
            {
                return Result.Failure<T>(FailureReasons.CouldNotDelete(typeof(T), tableName));
            }
        }

        public Result<int> Insert<T>(T objectToInsert, string tableName, string key)
        {
            try
            {
                var command = objectToInsert.GenerateInsertQuery(_connection, tableName, key);
                var id = GetIntOrCastToInt(command.ExecuteScalar());
                return Result.Success(id);
            }
            catch (Exception ex)
            {
                return Result.Failure<int>(FailureReasons.CouldNotInsert(typeof(T), ex.Message));
            }
        }

         private int GetIntOrCastToInt(object key)
        {
            try
            {
                var val = (int)key;
                return val;
            } catch
            {
                var val = (long)key;
                return (int)val;
            }
        }

        public Result Replace<T>(T objectToReplace, string tableName, string keyName, dynamic key)
        {
            var command = GenerateReplaceQueryUtil.GenerateReplaceQuery(objectToReplace, _connection, tableName, keyName, key);
            try
            {
                command.ExecuteNonQuery();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(FailureReasons.CouldNotReplace(typeof(T), ex.Message, command));
            }
        }

        private SqlCommand GetCommand(string query)
            => new SqlCommand(query, _connection);

        internal static class FailureReasons
        {
            internal static string CouldNotMaterialize(Type T, string message) => $"Failed to materialize {T.FullName}, \n Reason: {message}";
            internal static string CouldNotInsert(Type T, string message) => $"Failed to insert {T.FullName} to the database, \n Reason: {message}";
            internal static string CouldNotReplace(Type T, string message, SqlCommand command)
            {
                var sb = new StringBuilder();

                sb.AppendLine($"Failed to replace type {T.FullName} on the database");
                sb.AppendLine($"Reason: {message}.");
                sb.AppendLine($"The Query is: '{command.CommandText}'");
                sb.AppendLine($"And the parameters are...");

                foreach (SqlParameter parameter in command.Parameters)
                {
                    sb.AppendLine($"{parameter.ParameterName} - {parameter.Value}");
                }

                return sb.ToString();
            }

            internal static string CouldNotDelete(Type t, string message) => $"Failed to delete {t.Name} on the database, \n Reason: {message}";
        }
    }
}
