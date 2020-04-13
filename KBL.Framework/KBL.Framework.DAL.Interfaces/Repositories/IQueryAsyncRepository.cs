using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KBL.Framework.DAL.Interfaces.Repositories
{
    public interface IQueryAsyncRepository<T> where T : IEntity
    {
        Task<IQueryResult<T>> GetAllAsync();
        Task<IQueryResult<T>> GetAllWithDeletesAsync();
        Task<IQueryResult<T>> GetByKeyAsync(object key);
        Task<IQueryResult<T>> GetByQueryAsync(string storedQueryName, IDictionary<string, object> parameters);
    }
}
