using KBL.Framework.DAL.Base.Entities;
using KBL.Framework.DAL.Base.Queries.Implementations;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using KBL.Framework.DAL.Interfaces.Queries;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace KBL.Framework.DAL.Base.Repositories.Implementations
{
    public class AuditEntitiesQueryRepository : BaseQueryRepository<AuditEntity>
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public AuditEntitiesQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }
        #endregion

        #region Public methods
        public IQueryResult<AuditEntity> GetHistory(long entityId, string classFullName)
        {
            Dictionary<string, object> parms = new Dictionary<string, object>();
            parms.Add("entityId", entityId);
            parms.Add("classFullName", classFullName);
            return GetByQuery(nameof(GetEntityHistory), parms);
        }
        #endregion

        #region Private methods
        protected override void SetQueries()
        {
            _storedQueries = new Dictionary<string, IQuery<AuditEntity>>()
            {
                { nameof(GetEntityHistory), new GetEntityHistory()}
            };
        }
        #endregion
    }
}
