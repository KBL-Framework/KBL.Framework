using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Queries;
using System.Collections.Generic;
using System.Data;

namespace KBL.Framework.DAL.Base.Queries
{
    public abstract class BaseQuery<T> : IQuery<T> where T : IEntity
    {
        #region Fields

        #endregion

        #region Properties
        public string Name { get; }
        public IDictionary<string, string> NeedfulParameters { get; set; }
        #endregion

        #region Cstors
        public BaseQuery(string name)
        {
            Name = name;
            NeedfulParameters = new Dictionary<string, string>();
            InitializeParameters();
        }
        #endregion

        #region Public methods
        public IEnumerable<T> Execute(IDictionary<string, object> parameters, IDbConnection connection)
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
        protected abstract IEnumerable<T> ProceedExecute(IDictionary<string, object> parameters, IDbConnection connection);
        protected abstract void InitializeParameters();
        #endregion
    }
}
