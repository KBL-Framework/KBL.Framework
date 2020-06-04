using KBL.Framework.DAL.Interfaces.Repositories;
using KBL.Framework.DAL.Interfaces.UnitOfWork;
using Microsoft.Extensions.Configuration;
using NLog;
using Polly;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace KBL.Framework.DAL.Base.UnitOfWork
{
    public abstract class BaseUoW : IUnitOfWork, IDisposable
    {
        #region Fields
        protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected IDbConnection _connection;
        protected IDbTransaction _transaction;
        protected IConfiguration _configuration;
        protected Policy _retryPolicy; //= Policy.Handle<SqlException>(e => e is SqlException && (_connection == null || _connection?.State != ConnectionState.Open)).Retry(10);        
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public BaseUoW(IConfiguration configuration)
        {
            _configuration = configuration;
            _retryPolicy = Policy.Handle<SqlException>(e => e is SqlException && (_connection == null || _connection?.State != ConnectionState.Open)).Retry(10);
        }
        #endregion

        #region Public methods
        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        public bool SaveChanges()
        {
            try
            {
                _logger.Debug($"Saving transaction");
                if (_transaction == null || _transaction.Connection == null)
                {
                    _logger.Debug($"Transaction is null. Creating new one.");
                    _transaction = GetTransaction();
                }

                _transaction.Commit();
                _logger.Debug($"Successfully saved transaction");
                return true;
            }
            catch (Exception ex)
            {
                _transaction.Rollback();
                _logger.Error(ex, $"Error for saving transaction. Transaction was rollbacked.");
                return false;
            }
            finally
            {
                _transaction.Dispose();
                //_transaction = _connection.BeginTransaction();
                ResetRepositories();
            }
        }

        public bool Rollback()
        {
            try
            {
                _logger.Debug($"Rollback transaction");
                _transaction.Rollback();
                _logger.Debug($"Successfully rollbacked transaction");
                return true;
            }
            catch (Exception ex)
            {
                _transaction?.Rollback();
                _logger.Error(ex, $"Error for saving transaction. Transaction was rollbacked.");
                return false;
            }
            finally
            {
                _transaction?.Dispose();
                //_transaction = _connection.BeginTransaction();
                ResetRepositories();
            }
        }
        #endregion

        #region Private methods
        protected void CreateConnection()
        {
            try
            {
                _connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                _connection.Open();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error on CreateConnection in BaseUoW.");
                throw;
            }
        }
        protected virtual void ResetRepositories()
        {
            FieldInfo[] fields = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(ICrudRepository<>))
                {
                    field.SetValue(this, null);
                }
            }
        }

        protected IDbTransaction GetTransaction()
        {
            if (_connection == null || _connection?.State != ConnectionState.Open)
            {
                _retryPolicy.Execute(() => CreateConnection());
            }
            if (_transaction == null || _transaction.Connection == null)
            {
                if (string.IsNullOrEmpty(_configuration["KBL.Framework.DAL.Config:Transaction:IsolationLevel"]))
                {
                    _transaction = _connection.BeginTransaction();
                }
                else
                {
                    Enum.TryParse(_configuration["KBL.Framework.DAL.Config:Transaction:IsolationLevel"], out System.Data.IsolationLevel isolationLevel);
                    _transaction = _connection.BeginTransaction(isolationLevel);
                }
            }
            return _transaction;
        }
        #endregion

    }
}
