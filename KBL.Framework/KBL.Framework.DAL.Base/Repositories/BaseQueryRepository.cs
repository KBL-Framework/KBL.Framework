using KBL.Framework.DAL.Base.Infrastructure;
using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using KBL.Framework.DAL.Interfaces.Queries;
using KBL.Framework.DAL.Interfaces.Repositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace KBL.Framework.DAL.Base.Repositories
{
    public abstract class BaseQueryRepository<T> : BaseRepository, IDisposable, IQueryRepository<T> where T : IEntity
    {
        #region Fields        
        protected IDbConnection _connection;
        protected string _tableName = "";
        protected IDictionary<string, IQuery<T>> _storedQueries;
        //protected string _connectionString;
        protected string _keyColumnName = "";
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public BaseQueryRepository(IConfiguration configuration) : base(configuration)
        {

            if (string.IsNullOrEmpty(_keyColumnName))
            {
                _keyColumnName = "ID";
            }
        }
        #endregion

        #region Public methods
        public void Dispose()
        {
            _connection?.Dispose();
        }

        public IQueryResult<T> GetAll()
        {
            return PrepareGetAllData(false);
        }

        public IQueryResult<T> GetAllWithDeletes()
        {
            return PrepareGetAllData(true);
        }

        public IQueryResult<T> GetByKey(object key)
        {
            try
            {
                IEnumerable<T> data = null;
                if (key == null || string.IsNullOrEmpty(key.ToString()))
                {
                    throw new ArgumentNullException("Key value is null or empty!");
                }

                data = ExecuteGetByKeyCommand(key.ToString());
                ResultType resultValue;
                if (data != null/* && data.Any()*/)
                {
                    resultValue = ResultType.OK;
                }
                else
                {
                    resultValue = ResultType.SqlError;
                }

                var result = new QueryResult<T>(resultValue, data) as IQueryResult<T>;
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error on GetByKey(object key) for table {_tableName}, key value is {key.ToString()}");
                return new QueryResult<T>(ResultType.Error, null);
            }
            finally
            {
                _connection?.Close();
            }
        }

        public IQueryResult<T> GetByQuery(string storedQueryName, IDictionary<string, object> parameters)
        {
            ResultType resultValue;
            try
            {
                if (!_storedQueries.ContainsKey(storedQueryName))
                {
                    throw new IndexOutOfRangeException("Stored query dont existst for this repo.");
                }

                IEnumerable<T> data = null;
                using (_connection = new SqlConnection(_connectionString))
                {
                    data = _storedQueries[storedQueryName].Execute(parameters, _connection);
                }
                if (data != null)
                {
                    resultValue = ResultType.OK;
                }
                else
                {
                    resultValue = ResultType.SqlError;
                }

                var result = new QueryResult<T>(resultValue, data) as IQueryResult<T>;
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error on GetByQuery() for table {_tableName}, storedQuery name is {_tableName}");
                return new QueryResult<T>(ResultType.Error, null);
            }
            finally
            {
                _connection?.Close();
            }
        }
        #endregion

        #region Private methods
        protected IQueryResult<T> PrepareGetAllData(bool includeDeletes)
        {
            try
            {
                IEnumerable<T> data = null;
                ResultType resultValue = default(ResultType);
                //if (resultValue == ResultType.OK)
                {
                    data = ExecuteGetAllCommand(includeDeletes);
                    if (data != null)
                    {
                        resultValue = ResultType.OK;
                    }
                    else
                    {
                        resultValue = ResultType.SqlError;
                    }
                }
                var result = new QueryResult<T>(resultValue, data) as IQueryResult<T>;
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error on PrepareGetAllData(IDictionary<string,string> parameters) for table {_tableName}");
                return new QueryResult<T>(ResultType.Error, null);
            }
            finally
            {
                _connection?.Close();
            }
        }

        protected virtual IEnumerable<T> ExecuteGetAllCommand(bool includeDeletes)
        {
            IEnumerable<T> data = null;
            string query = $"SELECT * FROM {_tableName}";
            if (!includeDeletes)
            {
                query += " WHERE DeletedDateTime IS NULL";
            }
            using (_connection = new SqlConnection(_connectionString))
            {
                data = _connection.Query<T>(query, commandType: System.Data.CommandType.Text).ToList();
            }

            return data;
        }

        protected virtual IEnumerable<T> ExecuteGetByKeyCommand(string keyValue)
        {
            IEnumerable<T> data = null;
            string query = $"SELECT * FROM {_tableName} WHERE {_keyColumnName} = {_dbDialectForParameter}{_keyColumnName} AND DeletedDateTime IS NULL";
            DynamicParameters parms = new DynamicParameters();
            parms.Add($"{_dbDialectForParameter}{_keyColumnName}", keyValue, _typeMap[typeof(long?)]);

            using (_connection = new SqlConnection(_connectionString))
            {
                data = _connection.Query<T>(query, parms, commandType: System.Data.CommandType.Text).ToList();
            }
            return data;
        }

        protected override void SetRepositoryMetadata()
        {
            _tableName = CreateTableName(typeof(T).Name);
            _keyColumnName = _configuration[$"{ROOT_CONFIG_PATH}:PrimaryKeyColumn"];

            SetQueries();
        }

        protected abstract void SetQueries();
    #endregion
}
}
