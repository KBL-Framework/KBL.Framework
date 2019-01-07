using KBL.Framework.DAL.Base.Repositories;
using KBL.Framework.DAL.Interfaces.Queries;
using KBL.Framework.TestApi.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBL.Framework.TestApi.DAL.Repos
{
    public class UserQueryRepository : BaseQueryRepository<User>, IUserQueryRepository
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public UserQueryRepository(IConfiguration configuration) : base(configuration)
        {

        }
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        protected override void SetQueries()
        {
            _storedQueries = new Dictionary<string, IQuery<User>>();
        }
        #endregion

    }
}
