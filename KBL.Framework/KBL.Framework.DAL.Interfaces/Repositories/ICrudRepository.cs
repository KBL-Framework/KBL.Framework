using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KBL.Framework.DAL.Interfaces.Repositories
{
    public interface ICrudRepository<T> where T : IEntity
    {
        ICrudResult<T> Add(T entity);
        ICrudResult<T> Delete(T entity);
        ICrudResult<T> Update(T entity);
        Task<ICrudResult<T>> AddAsync(T entity);
        Task<ICrudResult<T>> DeleteAsync(T entity);
        Task<ICrudResult<T>> UpdateAsync(T entity);
        //Task<ICrudResult<T>> AddAsync(T entity, CancellationToken cancellationToken);
        //Task<ICrudResult<T>> DeleteAsync(T entity, CancellationToken cancellationToken);
        //Task<ICrudResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken);
    }
}
