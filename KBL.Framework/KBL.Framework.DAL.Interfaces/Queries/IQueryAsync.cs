using KBL.Framework.DAL.Interfaces.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace KBL.Framework.DAL.Interfaces.Queries
{
    public interface IQueryAsync<T> : IQueryCommon where T : IEntity
    {
        Task<IEnumerable<T>> ExecuteAsync(IDictionary<string, object> parameters, IDbConnection connection);
    }
}
