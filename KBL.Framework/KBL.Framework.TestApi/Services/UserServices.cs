using KBL.Framework.BAL.Base.Entities;
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
        private readonly EntityHistoryServices _historyServices;
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public UserServices(IUserQueryRepository queryRepository, Framework.DAL.Interfaces.UnitOfWork.IUnitOfWork unitOfWork, IGenericMapperFactory<UserDto, UserDto, User> mapperFactory, EntityHistoryServices historyServices)
            : base(queryRepository, unitOfWork, mapperFactory)
        {
            _historyServices = historyServices;
        }
        #endregion

        #region Public methods
        public IEnumerable<EntityHistoryDto<User>> GetHistory(long id)
        {
            return _historyServices.GetHistory<User>(id);
        }

        public void Delete(long id)
        {
            var item = _queryRepo.GetAllWithDeletes().ResultList.Where(x => x.ID == id).FirstOrDefault();
            if (item != null)
            {
                _uow.UserRepo.Delete(item);
                _uow.SaveChanges();
            }
        }

        #endregion

        #region Private methods
        protected override void SetCrudRepository()
        {
            _crudRepo = _uow.UserRepo;
        }
        #endregion

    }
}
