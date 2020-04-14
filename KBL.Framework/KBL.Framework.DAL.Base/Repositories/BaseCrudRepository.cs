using Dapper;
using KBL.Framework.DAL.Base.Entities;
using KBL.Framework.DAL.Base.Infrastructure;
using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using KBL.Framework.DAL.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Polly;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace KBL.Framework.DAL.Base.Repositories
{
    public abstract class BaseCrudRepository<T> : BaseRepository, ICrudRepository<T> where T : IEntity
    {
        #region Fields        
        protected string _addProcedureName = "";
        protected string _updateProcedureName = "";
        protected string _deleteProcedureName = "";
        protected IDbConnection _connection;
        protected IDbTransaction _transaction;
        protected string _tableName;
        protected Policy _retryPolicy;
        protected Policy _retryPolicyAsync;
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public BaseCrudRepository(IDbTransaction transaction, IConfiguration configuration) : base(configuration)
        {
            _transaction = transaction;
            _connection = _transaction.Connection;
            _tableName = CreateTableName(typeof(T).Name);
            _retryPolicy = Policy.Handle<SqlException>(e => e is SqlException && (_connection == null || _connection?.State != ConnectionState.Open)).Retry(10);
            _retryPolicyAsync = Policy.Handle<SqlException>(e => e is SqlException && (_connection == null || _connection?.State != ConnectionState.Open)).RetryAsync(10);
        }
        #endregion

        #region Public methods
        public ICrudResult<T> Add(T entity)
        {
            ICrudResult<T> result;
            try
            {
                DynamicParameters parameters = CreateAddParams(entity);
                _retryPolicy.Execute(() => _connection.Execute(_addProcedureName, parameters, _transaction, commandType: CommandType.StoredProcedure));
                result = CreateAddResult(entity, parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in CrudBaseRepository.Add()! Entity {typeof(T).Name}.");
                result = new CrudResult<T>(ResultType.Error);
            }
            return result;
        }

        public async Task<ICrudResult<T>> AddAsync(T entity)
        {
            ICrudResult<T> result;
            try
            {
                DynamicParameters parameters = CreateAddParams(entity);
                var x = await _retryPolicyAsync.ExecuteAsync(async () => await _connection.ExecuteAsync(_addProcedureName, parameters, _transaction, commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false)).ConfigureAwait(false);
                result = CreateAddResult(entity, parameters);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in CrudBaseRepository.Add()! Entity {typeof(T).Name}.");
                result = new CrudResult<T>(ResultType.Error);
            }
            return result;
        }

        public ICrudResult<T> Delete(T entity)
        {
            ICrudResult<T> result;
            try
            {
                if (entity is AuditableEntity && (entity as IEntity)?.DeletedDateTime != null)
                {
                    return new CrudResult<T>(ResultType.OK);
                }
                DynamicParameters parameters = CreateDeleteParams(entity);
                _retryPolicy.Execute(() => _connection.Execute(_deleteProcedureName, parameters, _transaction, null, CommandType.StoredProcedure));
                result = new CrudResult<T>(ResultType.OK);
                _logger.Info($"Entity {typeof(T).Name} ID = {entity.ID} was successfully deleted.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in CrudBaseRepository.Delete()! Entity {typeof(T).Name}.");
                result = new CrudResult<T>(ResultType.Error);
            }
            return result;
        }

        public ICrudResult<T> UnDelete(T entity)
        {
            ICrudResult<T> result;
            try
            {
                if (entity is AuditableEntity && (entity as IEntity)?.DeletedDateTime == null)
                {
                    return new CrudResult<T>(ResultType.OK);
                }
                DynamicParameters parameters = CreateUnDeleteParams(entity);
                _retryPolicy.Execute(() => _connection.Execute(_deleteProcedureName, parameters, _transaction, null, CommandType.StoredProcedure));
                result = new CrudResult<T>(ResultType.OK);
                _logger.Info($"Entity {typeof(T).Name} ID = {entity.ID} was successfully undeleted.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in CrudBaseRepository.UnDelete()! Entity {typeof(T).Name}.");
                result = new CrudResult<T>(ResultType.Error);
            }
            return result;
        }

        public async Task<ICrudResult<T>> DeleteAsync(T entity)
        {
            ICrudResult<T> result;
            try
            {
                if (entity is AuditableEntity && (entity as IEntity)?.DeletedDateTime != null)
                {
                    return new CrudResult<T>(ResultType.OK);
                }
                DynamicParameters parameters = CreateDeleteParams(entity);
                var x = await _retryPolicyAsync.ExecuteAsync(async () => await _connection.ExecuteAsync(_deleteProcedureName, parameters, _transaction, null, CommandType.StoredProcedure).ConfigureAwait(false)).ConfigureAwait(false);
                result = new CrudResult<T>(ResultType.OK);
                _logger.Info($"Entity {typeof(T).Name} ID = {entity.ID} was successfully deleted.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in CrudBaseRepository.Delete()! Entity {typeof(T).Name}.");
                result = new CrudResult<T>(ResultType.Error);
            }
            return result;
        }

        public async Task<ICrudResult<T>> UnDeleteAsync(T entity)
        {
            ICrudResult<T> result;
            try
            {
                if (entity is AuditableEntity && (entity as IEntity)?.DeletedDateTime == null)
                {
                    return new CrudResult<T>(ResultType.OK);
                }
                DynamicParameters parameters = CreateUnDeleteParams(entity);
                var x = await _retryPolicyAsync.ExecuteAsync(async () => await _connection.ExecuteAsync(_deleteProcedureName, parameters, _transaction, null, CommandType.StoredProcedure).ConfigureAwait(false)).ConfigureAwait(false);
                result = new CrudResult<T>(ResultType.OK);
                _logger.Info($"Entity {typeof(T).Name} ID = {entity.ID} was successfully undeleted.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in CrudBaseRepository.UnDelete()! Entity {typeof(T).Name}.");
                result = new CrudResult<T>(ResultType.Error);
            }
            return result;
        }

        public ICrudResult<T> Update(T entity)
        {
            ICrudResult<T> result;
            try
            {
                DynamicParameters parameters = CreateUpdateParams(entity);
                _retryPolicy.Execute(() => _connection.Execute(_updateProcedureName, parameters, _transaction, commandType: CommandType.StoredProcedure));
                result = new CrudResult<T>(ResultType.OK);
                _logger.Info($"Entity {typeof(T).Name} ID = {entity.ID} was successfully updated.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in CrudBaseRepository.Update()! Entity {typeof(T).Name}.");
                result = new CrudResult<T>(ResultType.Error);
            }
            return result;
        }

        public async Task<ICrudResult<T>> UpdateAsync(T entity)
        {
            ICrudResult<T> result;
            try
            {
                DynamicParameters parameters = CreateUpdateParams(entity);
                var x = await _retryPolicyAsync.ExecuteAsync(async () => await _connection.ExecuteAsync(_updateProcedureName, parameters, _transaction, commandType: CommandType.StoredProcedure).ConfigureAwait(false)).ConfigureAwait(false);
                result = new CrudResult<T>(ResultType.OK);
                _logger.Info($"Entity {typeof(T).Name} ID = {entity.ID} was successfully updated.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in CrudBaseRepository.Update()! Entity {typeof(T).Name}.");
                result = new CrudResult<T>(ResultType.Error);
            }
            return result;
        }
        #endregion

        #region Private methods
        private ICrudResult<T> CreateAddResult(T entity, DynamicParameters parameters)
        {
            //To get newly created ID back  
            entity.ID = parameters.Get<long>($"{_dbDialectForParameter}ID");
            _logger.Info($"Entity {typeof(T).Name} ID = {entity.ID} was successfully created.");
            return new CrudResult<T>(ResultType.OK, entity);
        }

        private DynamicParameters CreateAddParams(T entity)
        {
            if (entity is IEntity)
            {
                (entity as IEntity).CreatedDateTime = DateTimeOffset.UtcNow;
            }
            var parameters = CreateParameters(entity);
            parameters.Add($"{_dbDialectForParameter}ID", entity.ID, dbType: DbType.Int64, direction: ParameterDirection.InputOutput);
            parameters.Add($"{_dbDialectForParameter}CreatedDateTime", (entity as IEntity).CreatedDateTime);

            if (entity is AuditableEntity)
            {
                parameters.Add($"{_dbDialectForParameter}CreatedBy", (entity as AuditableEntity).CreatedBy);
            }

            return parameters;
        }

        private DynamicParameters CreateUpdateParams(T entity)
        {
            if (entity is IEntity)
            {
                (entity as IEntity).ModifiedDateTime = DateTimeOffset.UtcNow;
            }
            var parameters = CreateParameters(entity);
            parameters.Add($"{_dbDialectForParameter}ID", entity.ID, dbType: DbType.Int64, direction: ParameterDirection.InputOutput);
            parameters.Add($"{_dbDialectForParameter}ModifiedDateTime", (entity as IEntity).ModifiedDateTime);

            if (entity is AuditableEntity)
            {
                parameters.Add($"{_dbDialectForParameter}ModifiedBy", (entity as AuditableEntity).ModifiedBy);
            }

            return parameters;
        }

        private DynamicParameters CreateDeleteParams(T entity)
        {
            var parameters = new DynamicParameters();
            parameters.Add($"{_dbDialectForParameter}ID", entity.ID, dbType: DbType.Int64);
            if (entity is IEntity)
            {
                (entity as IEntity).DeletedDateTime = DateTimeOffset.UtcNow;
                parameters.Add($"{_dbDialectForParameter}DeletedDateTime", (entity as IEntity).DeletedDateTime);

            }
            if (entity is AuditableEntity)
            {
                parameters.Add($"{_dbDialectForParameter}DeletedBy", (entity as AuditableEntity).DeletedBy);
            }

            return parameters;
        }

        private DynamicParameters CreateUnDeleteParams(T entity)
        {
            var parameters = new DynamicParameters();
            parameters.Add($"{_dbDialectForParameter}ID", entity.ID, dbType: DbType.Int64);
            if (entity is IEntity)
            {
                (entity as IEntity).DeletedDateTime = null;
                parameters.Add($"{_dbDialectForParameter}DeletedDateTime", null);

            }
            if (entity is AuditableEntity)
            {
                parameters.Add($"{_dbDialectForParameter}DeletedBy", null);
            }

            return parameters;
        }

        protected DynamicParameters CreateParameters(IEntity entity)
        {
            var parameters = new DynamicParameters();
            foreach (var propertyInfo in entity.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType.ToString().Contains("Collection") == false)
                {
                    if (!propertyInfo.Name.ToLower().Equals("id") && !typeof(IEntity).GetProperties().Select(x => x.Name).Contains(propertyInfo.Name) && !typeof(AuditableEntity).GetProperties().Select(x => x.Name).Contains(propertyInfo.Name))
                    {
                        var value = propertyInfo.GetValue(entity, null)?.ToString();
                        var type = propertyInfo.GetValue(entity, null)?.GetType();
                        if (type != null)
                        {
                            if (type.IsEnum)
                            {
                                Enum test = Enum.Parse(type, value) as Enum;
                                int x = Convert.ToInt32(test);
                                parameters.Add($"{_dbDialectForParameter}{propertyInfo.Name}", x, _typeMap[typeof(int)]);
                            }
                            else if (type.IsValueType || type == typeof(String))
                            {
                                parameters.Add($"{_dbDialectForParameter}{propertyInfo.Name}", value, _typeMap[type]);
                            }
                        }
                    }
                }
            }

            return parameters;
        }
        #endregion

    }
}
