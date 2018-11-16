using KBL.Framework.BAL.Base.Services;
using KBL.Framework.BAL.Interfaces.Mappers;
using KBL.Framework.DAL.Interfaces.Repositories;
using KBL.Framework.TestApi.DAL.Repos;
using KBL.Framework.TestApi.DAL.UoW;
using KBL.Framework.TestApi.DTOs;
using KBL.Framework.TestApi.Model;
using KBL.Framework.TestApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBL.Framework.TestApi.Services
{
    public class UserServices : BaseCrudServices<UserDto, UserDto, User, IUserQueryRepository, ICrudRepository<User>, ITestUoW, IMapperFactory>, IUserServices
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public UserServices(IUserQueryRepository queryRepository, Framework.DAL.Interfaces.UnitOfWork.IUnitOfWork unitOfWork, IGenericMapperFactory<UserDto,UserDto,User> mapperFactory)
            : base(queryRepository, unitOfWork, mapperFactory)
        {
        }
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        protected override void SetCrudRepository()
        {
            _crudRepo = _uow.UserRepo;
        }
        #endregion

    }
}
