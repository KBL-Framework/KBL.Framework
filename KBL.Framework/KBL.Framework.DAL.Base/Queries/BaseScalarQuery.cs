using KBL.Framework.DAL.Interfaces.Queries;
using System.Collections.Generic;
using System.Data;

namespace KBL.Framework.DAL.Base.Queries
{
    /// <summary>
    /// Scalar query for return T result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseScalarQuery<T> : IQuery<T> where T : class
    {
        #region Fields
        #endregion

        #region Properties
        public string Name { get; }
        public IDictionary<string, string> NeedfulParameters { get; set; }
        #endregion

        #region Cstors
        protected BaseScalarQuery(string name)
        {
            Name = name;
            NeedfulParameters = new Dictionary<string, string>();
            InitializeParameters();
        }
        #endregion

        #region Public methods
        public T Execute(IDictionary<string, object> parameters, IDbConnection connection)
        {
            foreach (var item in NeedfulParameters.Keys)
            {
                if (!parameters.ContainsKey(item))
                {
                    throw new KeyNotFoundException($"Key {item} in parameters collection not found!");
                }
            }
            return ProceedExecute(parameters, connection);
        }
        #endregion

        #region Private methods
        protected abstract T ProceedExecute(IDictionary<string, object> parameters, IDbConnection connection);
        protected abstract void InitializeParameters();
        #endregion
    }
}
