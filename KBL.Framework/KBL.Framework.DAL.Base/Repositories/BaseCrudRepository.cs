using KBL.Framework.DAL.Base.Infrastructure;
using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using KBL.Framework.DAL.Interfaces.Repositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using KBL.Framework.DAL.Base.Entities;
using Newtonsoft.Json;

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
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public BaseCrudRepository(IDbTransaction transaction, IConfiguration configuration) : base(configuration)
        {
            _transaction = transaction;
            _connection = _transaction.Connection;
            _tableName = CreateTableName(typeof(T).Name);
        }
        #endregion

        #region Public methods
        public ICrudResult<T> Add(T entity)
        {
            ICrudResult<T> result;
            try
            {
                if (entity is IEntity)
                {
                    (entity as IEntity).CreatedDateTime = DateTime.UtcNow;
                }                
                var parameters = CreateParameters(entity);
                parameters.Add($"{_dbDialectForParameter}ID", entity.ID, dbType: DbType.Int64, direction: ParameterDirection.InputOutput);                
                parameters.Add($"{_dbDialectForParameter}CreatedDateTime", (entity as IEntity).CreatedDateTime);

                if (entity is AuditableEntity)
                {
                    parameters.Add($"{_dbDialectForParameter}CreatedBy", (entity as AuditableEntity).CreatedBy);
                }                

                _connection.Execute(_addProcedureName, parameters, _transaction, commandType: CommandType.StoredProcedure);
                
                //To get newly created ID back  
                entity.ID = parameters.Get<long>($"{_dbDialectForParameter}ID");
                _logger.Info($"Entity {nameof(T)} ID = {entity.ID} was successfully created.");
                result = new CrudResult<T>(ResultType.OK, entity);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in CrudBaseRepository.Add()! Entity {nameof(T)}.");
                result = new CrudResult<T>(ResultType.Error);
            }
            return result;
        }

        public ICrudResult<T> Delete(T entity)
        {
            ICrudResult<T> result;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add($"{_dbDialectForParameter}ID", entity.ID, dbType: DbType.Int64);
                if (entity is IEntity)
                {
                    (entity as IEntity).DeletedDateTime = DateTime.UtcNow;
                    parameters.Add($"{_dbDialectForParameter}DeletedDateTime", (entity as IEntity).DeletedDateTime);

                }
                if (entity is AuditableEntity)
                {
                    parameters.Add($"{_dbDialectForParameter}DeletedBy", (entity as AuditableEntity).DeletedBy);
                }
                _connection.Execute(_deleteProcedureName, parameters, _transaction, commandType: CommandType.StoredProcedure);
                result = new CrudResult<T>(ResultType.OK);
                _logger.Info($"Entity {nameof(T)} ID = {entity.ID} was successfully deleted.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in CrudBaseRepository.Delete()! Entity {nameof(T)}.");
                result = new CrudResult<T>(ResultType.Error);
            }

            return result;
        }

        public ICrudResult<T> Update(T entity)
        {
            ICrudResult<T> result;
            try
            {                
                if (entity is IEntity)
                {
                    (entity as IEntity).ModifiedDateTime = DateTime.UtcNow;
                }
                var parameters = CreateParameters(entity);
                parameters.Add($"{_dbDialectForParameter}ID", entity.ID, dbType: DbType.Int64, direction: ParameterDirection.InputOutput);
                parameters.Add($"{_dbDialectForParameter}ModifiedDateTime", (entity as IEntity).ModifiedDateTime);
                
                if (entity is AuditableEntity)
                {
                    parameters.Add($"{_dbDialectForParameter}ModifiedBy", (entity as AuditableEntity).ModifiedBy);
                }
                _connection.Execute(_updateProcedureName, parameters, _transaction, commandType: CommandType.StoredProcedure);
                result = new CrudResult<T>(ResultType.OK);
                _logger.Info($"Entity {nameof(T)} ID = {entity.ID} was successfully updated.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in CrudBaseRepository.Update()! Entity {nameof(T)}.");
                result = new CrudResult<T>(ResultType.Error);
            }
            return result;
        }
        #endregion

        #region Private methods
        protected DynamicParameters CreateParameters(IEntity entity)
        {
            var parameters = new DynamicParameters();
            foreach (var propertyInfo in entity.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType.ToString().Contains("Collection") == false)
                {
                    if (!propertyInfo.Name.ToLower().Equals("id") && !typeof(IEntity).GetProperties().Select(x=>x.Name).Contains(propertyInfo.Name) && !typeof(AuditableEntity).GetProperties().Select(x => x.Name).Contains(propertyInfo.Name))
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
