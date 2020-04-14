using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using System.Collections.Generic;

namespace KBL.Framework.DAL.Interfaces.Repositories
{
    public interface IQueryRepository<T> where T : IEntity
    {
        IQueryResult<T> GetAll();
        IQueryResult<T> GetAllWithDeletes();
        IQueryResult<T> GetByKey(object key);
        IQueryResult<T> GetByQuery(string storedQueryName, IDictionary<string, object> parameters);
    }
}
