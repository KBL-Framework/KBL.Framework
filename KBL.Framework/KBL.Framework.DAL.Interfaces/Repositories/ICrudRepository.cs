using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using System.Threading.Tasks;

namespace KBL.Framework.DAL.Interfaces.Repositories
{
    public interface ICrudRepository<T> where T : IEntity
    {
        ICrudResult<T> Add(T entity);
        ICrudResult<T> Delete(T entity);
        ICrudResult<T> UnDelete(T entity);
        ICrudResult<T> Update(T entity);
        Task<ICrudResult<T>> AddAsync(T entity);
        Task<ICrudResult<T>> DeleteAsync(T entity);
        Task<ICrudResult<T>> UnDeleteAsync(T entity);
        Task<ICrudResult<T>> UpdateAsync(T entity);
    }
}
