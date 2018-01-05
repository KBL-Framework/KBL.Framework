using KBL.Framework.DAL.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.DAL.Interfaces.Infrastructure
{
    public interface ICrudResult<T> where T : IEntity
    {
        ResultType IsSuccess { get; }
        long ID { get; }
        T Result { get; }
    }
}
