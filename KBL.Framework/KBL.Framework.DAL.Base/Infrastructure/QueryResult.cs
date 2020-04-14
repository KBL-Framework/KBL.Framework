using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace KBL.Framework.DAL.Base.Infrastructure
{
    public class QueryResult<T> : IQueryResult<T> where T : IEntity
    {
        #region Fields
        private ResultType _resultType;
        private IEnumerable<T> _data;
        private T _first;
        #endregion

        #region Properties
        public ResultType IsSuccess { get { return _resultType; } }
        public IEnumerable<T> ResultList { get { return _data; } }
        public T FirstResult { get { return _first; } }
        #endregion

        #region Cstors
        public QueryResult(ResultType resultType, IEnumerable<T> data)
        {
            _resultType = resultType;
            _data = data;
            if (_data != null)
            {
                _first = _data.FirstOrDefault();
            }
        }
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        #endregion
    }
}
