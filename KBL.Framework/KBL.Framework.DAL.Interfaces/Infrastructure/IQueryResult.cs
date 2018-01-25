using KBL.Framework.DAL.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.DAL.Interfaces.Infrastructure
{
    public interface IQueryResult<T> where T : IEntity
    {
        ResultType IsSuccess { get; }
        IEnumerable<T> ResultList { get; }
        T FirstResult { get; }
    }
}
