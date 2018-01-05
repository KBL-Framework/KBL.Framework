using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.DAL.Interfaces.Repositories
{
    public interface ICrudRepository<T> where T : IEntity
    {
        ICrudResult<T> Add(T entity);
        ICrudResult<T> Delete(T entity);
        ICrudResult<T> Update(T entity);
    }
}
