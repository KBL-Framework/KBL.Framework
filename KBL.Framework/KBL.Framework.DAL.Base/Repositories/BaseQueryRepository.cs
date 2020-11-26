using Dapper;
using KBL.Framework.DAL.Base.Infrastructure;
using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using KBL.Framework.DAL.Interfaces.Queries;
using KBL.Framework.DAL.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace KBL.Framework.DAL.Base.Repositories
{
    public abstract class BaseQueryRepository<T> : BaseRepository, IDisposable, IQueryRepository<T>, IQueryAsyncRepository<T> where T : IEntity
    {
        #region Fields        
        protected IDbConnection _connection;
        protected string _tableName = "";
        protected IDictionary<string, IQuery<T>> _storedQueries;
        protected IDictionary<string, IQueryAsync<T>> _asyncStoredQueries;
        //protected string _connectionString;
        protected string _keyColumnName = "";
        protected Policy _retryPolicy;
        protected AsyncRetryPolicy _retryPolicyAsync;
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
            _retryPolicy = Policy.Handle<SqlException>(e => e is SqlException && (_connection == null || _connection?.State != ConnectionState.Open)).Retry(10);
            _retryPolicyAsync = Policy.Handle<SqlException>(e => e is SqlException && (_connection == null || _connection?.State != ConnectionState.Open)).RetryAsync(10);
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
                if (key == null || string.IsNullOrEmpty(key.ToString()))
                {
                    throw new ArgumentNullException("Key value is null or empty!");
                }
                var data = _retryPolicy.Execute(() => ExecuteGetByKeyCommand(key.ToString()));
                return HandleResult(data);
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
            try
            {
                if (!_storedQueries.ContainsKey(storedQueryName))
                {
                    throw new IndexOutOfRangeException("Stored query dont existst for this repo.");
                }

                IEnumerable<T> data = _retryPolicy.Execute(() => ExecuteQuery(storedQueryName, parameters));
                return HandleResult(data);
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

        public async Task<IQueryResult<T>> GetAllAsync()
        {
            return await PrepareGetAllDataAsync(false).ConfigureAwait(false);
        }

        public async Task<IQueryResult<T>> GetAllWithDeletesAsync()
        {
            return await PrepareGetAllDataAsync(true).ConfigureAwait(false);
        }

        public async Task<IQueryResult<T>> GetByKeyAsync(object key)
        {
            try
            {
                if (key == null || string.IsNullOrEmpty(key.ToString()))
                {
                    throw new ArgumentNullException("Key value is null or empty!");
                }
                var data = await _retryPolicyAsync.ExecuteAsync(async () => await ExecuteGetByKeyCommandAsync(key.ToString()).ConfigureAwait(false)).ConfigureAwait(false);
                return HandleResult(data);
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

        public async Task<IQueryResult<T>> GetByQueryAsync(string storedQueryName, IDictionary<string, object> parameters)
        {
            try
            {
                if (!_storedQueries.ContainsKey(storedQueryName))
                {
                    throw new IndexOutOfRangeException("Stored query dont existst for this repo.");
                }
                IEnumerable<T> data = await _retryPolicyAsync.ExecuteAsync(async () => await ExecuteQueryAsync(storedQueryName, parameters).ConfigureAwait(false)).ConfigureAwait(false);
                return HandleResult(data);
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
        protected abstract void SetQueries();

        protected IQueryResult<T> PrepareGetAllData(bool includeDeletes)
        {
            try
            {
                IEnumerable<T> data = _retryPolicy.Execute(() => ExecuteGetAllCommand(includeDeletes));//ExecuteGetAllCommand(includeDeletes);
                return HandleResult(data);
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

        protected async Task<IQueryResult<T>> PrepareGetAllDataAsync(bool includeDeletes)
        {
            try
            {
                IEnumerable<T> data = await _retryPolicyAsync.ExecuteAsync(async () => await ExecuteGetAllCommandAsync(includeDeletes).ConfigureAwait(false)).ConfigureAwait(false);
                return HandleResult(data);
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

        private static IQueryResult<T> HandleResult(IEnumerable<T> data)
        {
            ResultType resultValue = default(ResultType);
            if (data != null)
            {
                resultValue = ResultType.OK;
            }
            else
            {
                resultValue = ResultType.SqlError;
            }
            return new QueryResult<T>(resultValue, data) as IQueryResult<T>;
        }

        protected virtual IEnumerable<T> ExecuteGetAllCommand(bool includeDeletes)
        {
            IEnumerable<T> data = null;
            using (_connection = new SqlConnection(_connectionString))
            {
                data = _connection.Query<T>(PrepareGetAllCommand(includeDeletes), commandType: CommandType.Text).ToList();
            }
            return data;
        }

        protected virtual async Task<IEnumerable<T>> ExecuteGetAllCommandAsync(bool includeDeletes)
        {
            IEnumerable<T> data = null;
            using (_connection = new SqlConnection(_connectionString))
            {
                data = await _connection.QueryAsync<T>(PrepareGetAllCommand(includeDeletes), commandType: CommandType.Text).ConfigureAwait(false);
            }
            return data.ToList();
        }

        protected string PrepareGetAllCommand(bool includeDeletes)
        {
            string query = $"SELECT * FROM {_tableName}";
            if (!includeDeletes)
            {
                query += " WHERE DeletedDateTime IS NULL";
            }
            return query;
        }

        protected virtual IEnumerable<T> ExecuteGetByKeyCommand(string keyValue)
        {
            IEnumerable<T> data = null;
            var (parms, query) = PrepareGetByKeyCommand(keyValue);
            using (_connection = new SqlConnection(_connectionString))
            {
                data = _connection.Query<T>(query, parms, commandType: System.Data.CommandType.Text).ToList();
            }
            return data.ToList();
        }

        protected (DynamicParameters parms, string query) PrepareGetByKeyCommand(string keyValue)
        {
            string query = $"SELECT * FROM {_tableName} WHERE {_keyColumnName} = {_dbDialectForParameter}{_keyColumnName} AND DeletedDateTime IS NULL";
            DynamicParameters parms = new DynamicParameters();
            parms.Add($"{_dbDialectForParameter}{_keyColumnName}", keyValue, _typeMap[typeof(long?)]);
            return (parms, query);
        }

        protected virtual async Task<IEnumerable<T>> ExecuteGetByKeyCommandAsync(string keyValue)
        {
            IEnumerable<T> data = null;
            var (parms, query) = PrepareGetByKeyCommand(keyValue);
            using (_connection = new SqlConnection(_connectionString))
            {
                data = await _connection.QueryAsync<T>(query, parms, commandType: System.Data.CommandType.Text).ConfigureAwait(false);
            }
            return data.ToList();
        }

        protected override void SetRepositoryMetadata()
        {
            _tableName = CreateTableName(typeof(T).Name);
            _keyColumnName = _configuration[$"{ROOT_CONFIG_PATH}:PrimaryKeyColumn"];
            SetQueries();
        }

        protected IEnumerable<T> ExecuteQuery(string storedQueryName, IDictionary<string, object> parameters)
        {
            IEnumerable<T> data;
            using (_connection = new SqlConnection(_connectionString))
            {
                data = _storedQueries[storedQueryName].Execute(parameters, _connection);
            }

            return data;
        }
        protected async Task<IEnumerable<T>> ExecuteQueryAsync(string storedQueryName, IDictionary<string, object> parameters)
        {
            IEnumerable<T> data;
            using (_connection = new SqlConnection(_connectionString))
            {
                data = await _asyncStoredQueries[storedQueryName].ExecuteAsync(parameters, _connection).ConfigureAwait(false);
            }

            return data;
        }
        #endregion
    }
}
