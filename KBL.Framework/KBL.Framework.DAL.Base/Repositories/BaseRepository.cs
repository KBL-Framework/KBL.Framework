using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;

namespace KBL.Framework.DAL.Base.Repositories
{
    public abstract class BaseRepository
    {
        #region Fields
        protected static readonly string ROOT_CONFIG_PATH = "KBL.Framework.DAL.Config";
        protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected static IDictionary<Type, DbType> _typeMap = FillTypeDictonary();
        protected string _dbDialectForParameter = "";
        protected string _connectionString = "";
        protected IConfiguration _configuration;
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
            _dbDialectForParameter = "@";
            SetRepositoryMetadata();
            //FillTypeDictonary();
        }
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        protected abstract void SetRepositoryMetadata();
        protected static Dictionary<Type, DbType> FillTypeDictonary()
        {
            Dictionary<Type, DbType> typeMap;
            typeMap = new Dictionary<Type, DbType>
            {
                [typeof(byte)] = DbType.Byte,
                [typeof(sbyte)] = DbType.SByte,
                [typeof(short)] = DbType.Int16,
                [typeof(ushort)] = DbType.UInt16,
                [typeof(int)] = DbType.Int32,
                [typeof(uint)] = DbType.UInt32,
                [typeof(long)] = DbType.Int64,
                [typeof(ulong)] = DbType.UInt64,
                [typeof(float)] = DbType.Single,
                [typeof(double)] = DbType.Double,
                [typeof(decimal)] = DbType.Decimal,
                [typeof(bool)] = DbType.Boolean,
                [typeof(string)] = DbType.String,
                [typeof(char)] = DbType.StringFixedLength,
                [typeof(Guid)] = DbType.Guid,
                [typeof(DateTime)] = DbType.DateTime,
                [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
                [typeof(byte[])] = DbType.Binary,
                [typeof(byte?)] = DbType.Byte,
                [typeof(sbyte?)] = DbType.SByte,
                [typeof(short?)] = DbType.Int16,
                [typeof(ushort?)] = DbType.UInt16,
                [typeof(int?)] = DbType.Int32,
                [typeof(uint?)] = DbType.UInt32,
                [typeof(long?)] = DbType.Int64,
                [typeof(ulong?)] = DbType.UInt64,
                [typeof(float?)] = DbType.Single,
                [typeof(double?)] = DbType.Double,
                [typeof(decimal?)] = DbType.Decimal,
                [typeof(bool?)] = DbType.Boolean,
                [typeof(char?)] = DbType.StringFixedLength,
                [typeof(Guid?)] = DbType.Guid,
                [typeof(DateTime?)] = DbType.DateTime,
                [typeof(DateTimeOffset?)] = DbType.DateTimeOffset
            };
            //typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;

            return typeMap;
        }

        protected virtual string CreateTableName(string className)
        {
            string tableName = _configuration[$"{ROOT_CONFIG_PATH}:Tables:{className}"];
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = className + "s";
            }
            return tableName;
        }
        #endregion
    }
}
