using Dapper;
using KBL.Framework.DAL.Base.Entities;
using KBL.Framework.DAL.Base.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBL.Framework.DAL.Base.Queries.Implementations
{
    public class GetEntityHistory : BaseQuery<AuditEntity>
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public GetEntityHistory() : base(nameof(GetEntityHistory))
        {
        }
        #endregion

        #region Public methods

        #endregion

        #region Private methods
        protected override void InitializeParameters()
        {
            NeedfulParameters = new Dictionary<string, string>();
            NeedfulParameters.Add("entityId", "@entityId");
            NeedfulParameters.Add("classFullName", "@classFullName");
        }

        protected override IEnumerable<AuditEntity> ProceedExecute(IDictionary<string, object> parameters, IDbConnection connection)
        {
            string query = $@"SELECT *
                              FROM {AuditingHelper.Instance.AuditingTableName} s
                            WHERE s.entityId LIKE @entityId
                              AND s.classFullName = @classFullName
                            ORDER BY s.CreatedDateTime";

            var data = connection.Query<AuditEntity>(query, new { @entityId = (long)parameters["entityId"], @classFullName = parameters["classFullName"] as string }, commandType: CommandType.Text);
            return data.ToList();
        }
        #endregion

    }
}
