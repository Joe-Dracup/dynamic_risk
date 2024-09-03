using CSharpFunctionalExtensions;
using Shoolese.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shoolese.Data
{
    public interface IFlexiTable<T>
    {
        public Result<int> CreateNew(T item);
        public Result<IEnumerable<T>> GetAll();
        public Result UpdateExisting(T update, dynamic id);
        public Result Delete(dynamic id);
        Result<T> GetById(int id);
    }

    public class FlexiTable<T>  : IFlexiTable<T> where T : new ()
    {
        private string _tableName;
        private ISqlExecutor _sqlExecutor;
        private string _keyName;

        public FlexiTable(string tableName, string connectionString, string nameOfKey = "Id")
        {
            _sqlExecutor = new SqlExecutor(connectionString);
            _tableName = tableName;
            _keyName = nameOfKey;

            CreateIfDoesntExist();
        }

        private void CreateIfDoesntExist()
        {
            var tableNameSplit = _tableName.Split('.');
            var query = GenerateCreateTableQueryUtil.GenerateCreateTableCommand<T>(tableNameSplit[0], tableNameSplit[1]);
            if (query.IsSuccess)
                _sqlExecutor.ExecuteQuery(query.Value);

        }

        private static IEnumerable<string> GetAllProperties(Type type) => type.GetProperties().Select(x => $"[{x.Name}]");

        public Result<int> CreateNew(T item) =>_sqlExecutor.Insert(item, _tableName, _keyName);

        public Result<IEnumerable<T>> GetAll() => _sqlExecutor.ExecuteSelectQuery<T>($"Select {string.Join(',', GetAllProperties(typeof(T)))} FROM {_tableName}");

        public Result<T> GetById(int id) 
        {
            try
            {
                return Result.Success(
                    _sqlExecutor.ExecuteSelectQuery<T>($"Select {string.Join(',', GetAllProperties(typeof(T)))} FROM {_tableName} where {_keyName} = {id}")
                    .Value.First()
                );
            } 
            catch (Exception ex)
            {
                return Result.Failure<T>($"Nope, Reason: {ex.Message}");
            }
        }

        public Result UpdateExisting(T item, dynamic id) => _sqlExecutor.Replace(item, _tableName, _keyName, id);

        public Result Delete(dynamic id) => _sqlExecutor.Delete(id, _tableName);
    }
}
