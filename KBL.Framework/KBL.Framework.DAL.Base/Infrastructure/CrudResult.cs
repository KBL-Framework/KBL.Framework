using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.DAL.Base.Infrastructure
{
    public class CrudResult<T> : ICrudResult<T> where T : IEntity
    {
        #region Fields
        private T _result;
        private ResultType _resultType;
        #endregion

        #region Properties
        public long ID
        {
            get
            {
                if (_result == null)
                {
                    return 0;
                }
                return _result.ID;
            }
        }
        public ResultType IsSuccess { get { return _resultType; } }
        public T Result { get { return _result; } }
        #endregion

        #region Cstors
        public CrudResult(ResultType resultType, T entity = default(T))
        {
            _resultType = resultType;
            _result = entity;
        }
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        #endregion

    }
}
