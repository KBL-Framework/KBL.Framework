using KBL.Framework.DAL.Interfaces.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace KBL.Framework.DAL.Base.Repositories
{
    public class GenericCrudRepository<T> : BaseCrudRepository<T> where T : IEntity
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public GenericCrudRepository(IDbTransaction transaction, IConfiguration configuration) : base(transaction, configuration)
        {
        }
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        protected override void SetRepositoryMetadata()
        {
            string type = typeof(T).Name;
            string schema = _configuration[$"{ROOT_CONFIG_PATH}:ProcedureSchema"];
            string procedureNamePattern = _configuration[$"{ROOT_CONFIG_PATH}:ProcedurePattern"];
            string tableName = CreateTableName(type);
            _addProcedureName = $"[{schema}].[{procedureNamePattern.Replace("<Table>", tableName).Replace("<Action>", "Create").Replace("<Entity>", type)}]";
            _updateProcedureName = $"[{schema}].[{procedureNamePattern.Replace("<Table>", tableName).Replace("<Action>", "Update").Replace("<Entity>", type)}]";
            _deleteProcedureName = $"[{schema}].[{procedureNamePattern.Replace("<Table>", tableName).Replace("<Action>", "Delete").Replace("<Entity>", type)}]";
        }        
        #endregion
    }
}
