using KBL.Framework.DAL.Interfaces.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.DAL.Base.Repositories
{
    public class GenericQueryRepository<T> : BaseQueryRepository<T> where T : IEntity
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public GenericQueryRepository(IConfiguration configuration) : base(configuration)
        {
        }
        #endregion

        #region Public methods
        #endregion

        #region Private methods       
        protected override void SetQueries()
        {            
        }
        #endregion
    }
}
