using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Queries;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace KBL.Framework.DAL.Base.Queries
{
    public abstract class BaseQueryAsync<T> : IQueryAsync<T> where T : IEntity
    {
        #region Fields
        #endregion

        #region Properties
        public string Name { get; }
        public IDictionary<string, string> NeedfulParameters { get; set; }
        #endregion

        #region Cstors
        public BaseQueryAsync(string name)
        {
            Name = name;
            NeedfulParameters = new Dictionary<string, string>();
            InitializeParameters();
        }
        #endregion

        #region Public methods
        public async Task<IEnumerable<T>> ExecuteAsync(IDictionary<string, object> parameters, IDbConnection connection)
        {
            foreach (var item in NeedfulParameters.Keys)
            {
                if (!parameters.ContainsKey(item))
                {
                    throw new KeyNotFoundException($"Key {item} in parameters collection not found!");
                }
            }
            return await ProceedExecuteAsync(parameters, connection).ConfigureAwait(false);
        }
        #endregion

        #region Private methods
        protected abstract Task<IEnumerable<T>> ProceedExecuteAsync(IDictionary<string, object> parameters, IDbConnection connection);
        protected abstract void InitializeParameters();
        #endregion

    }
}
