using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

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
