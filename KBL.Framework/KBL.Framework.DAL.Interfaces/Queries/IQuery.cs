using KBL.Framework.DAL.Interfaces.Entities;
using System.Collections.Generic;
using System.Data;

namespace KBL.Framework.DAL.Interfaces.Queries
{
    public interface IQuery<T> : IQueryCommon where T : IEntity
    {
        IEnumerable<T> Execute(IDictionary<string, object> parameters, IDbConnection connection);
    }
}
