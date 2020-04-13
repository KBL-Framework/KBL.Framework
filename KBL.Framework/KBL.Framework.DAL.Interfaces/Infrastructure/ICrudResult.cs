using KBL.Framework.DAL.Interfaces.Entities;

namespace KBL.Framework.DAL.Interfaces.Infrastructure
{
    public interface ICrudResult<T> where T : IEntity
    {
        ResultType IsSuccess { get; }
        long ID { get; }
        T Result { get; }
    }
}
