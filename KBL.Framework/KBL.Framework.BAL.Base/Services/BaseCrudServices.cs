using KBL.ExceptionManager.Model.Exceptions;
using KBL.Framework.BAL.Interfaces.Entities;
using KBL.Framework.BAL.Interfaces.Mappers;
using KBL.Framework.BAL.Interfaces.Services;
using KBL.Framework.DAL.Base.Entities;
using KBL.Framework.DAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Infrastructure;
using KBL.Framework.DAL.Interfaces.Repositories;
using KBL.Framework.DAL.Interfaces.UnitOfWork;
using NLog;
using System;
using System.Collections.Generic;

namespace KBL.Framework.BAL.Base.Services
{
    public abstract class BaseCrudServices<DetailDto, GridDto, Entity, QueryRepository, CrudRepository, UoW, MapperFactory>
        : IBaseCrudServices<DetailDto, GridDto>
        where DetailDto : IDto
        where GridDto : IDto
        where Entity : IEntity
        where QueryRepository : IQueryRepository<Entity>
        where CrudRepository : ICrudRepository<Entity>
        where UoW : IUnitOfWork
        where MapperFactory : IMapperFactory
    {
        #region Fields
        protected static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected UoW _uow;
        protected QueryRepository _queryRepo;
        protected CrudRepository _crudRepo;
        protected IMapperFactory _mapperFactory;
        protected Type _type;
        #endregion

        #region Properties
        #endregion

        #region Cstors
        public BaseCrudServices(IQueryRepository<Entity> queryRepository, IUnitOfWork unitOfWork, IMapperFactory mapperFactory)
        {
            _type = typeof(Entity);
            _mapperFactory = mapperFactory;
            _queryRepo = (QueryRepository)queryRepository;
            _uow = (UoW)unitOfWork;
            SetCrudRepository();
        }
        #endregion

        #region Public 
        public virtual long Create(DetailDto dto)
        {
            _logger.Debug($"Called Create{_type.Name}().");
            return CreateEntity(dto);
        }

        public virtual long Create(DetailDto dto, string createdBy)
        {
            _logger.Debug($"Called Create{_type.Name}() by {createdBy}.");
            return CreateEntity(dto, createdBy);
        }

        public virtual bool Delete(DetailDto dto)
        {
            _logger.Debug($"Called Delete{_type.Name}() with ID = {dto.ID}.");
            return DeleteEntity(dto);
        }

        public virtual bool Delete(DetailDto dto, string deletedBy)
        {
            _logger.Debug($"Called Delete{_type.Name}() with ID = {dto.ID} by {deletedBy}.");
            return DeleteEntity(dto, deletedBy);
        }

        public virtual bool UnDelete(DetailDto dto)
        {
            _logger.Debug($"Called UnDelete{_type.Name}() with ID = {dto.ID}.");
            return UnDeleteEntity(dto);
        }

        public virtual DetailDto Get(long id)
        {
            _logger.Debug($"Called Get{_type.Name}() with ID = {id}.");
            var result = _queryRepo.GetByKey(id);
            if (result.IsSuccess == ResultType.OK)
            {
                var mapper = _mapperFactory.CreateMapperToDetailDto();
                var dto = mapper.Map<DetailDto>(result.FirstResult);
                _logger.Info($"{_type.Name} ID = {result.FirstResult.ID} was returned.");
                return dto;
            }
            throw new GetEntityException<Entity>(result.IsSuccess.ToString());
        }

        public virtual IEnumerable<GridDto> GetAll()
        {
            _logger.Debug($"Called GetAll() of {_type.Name}.");
            var result = _queryRepo.GetAll();
            if (result.IsSuccess == ResultType.OK)
            {
                var mapper = _mapperFactory.CreateMapperToGridDto();
                var dtos = mapper.Map<IEnumerable<GridDto>>(result.ResultList);
                _logger.Info($"All {_type.Name} returned.");
                return dtos;
            }
            throw new GetEntityException<Entity>(result.IsSuccess.ToString());
        }

        public virtual bool Update(DetailDto dto)
        {
            _logger.Debug($"Called Update{_type.Name}() with ID = {dto.ID}.");
            return UpdateEntity(dto);
        }

        public virtual bool Update(DetailDto dto, string modifiedBy)
        {
            _logger.Debug($"Called Update{_type.Name}() with ID = {dto.ID} by {modifiedBy}.");
            return UpdateEntity(dto, modifiedBy);
        }
        #endregion

        #region Private methods
        protected abstract void SetCrudRepository();

        protected bool UpdateEntity(DetailDto dto, string modifiedBy = null)
        {
            var mapper = _mapperFactory.CreateMapperFromDetailDto();
            var entity = mapper.Map<Entity>(dto);
            if (entity is AuditableEntity)
            {
                (entity as AuditableEntity).ModifiedBy = modifiedBy;
            }
            var result = _crudRepo.Update(entity);
            if (result.IsSuccess == ResultType.OK)
            {
                bool isSuccess = _uow.SaveChanges();
                _logger.Info($"{_type.Name} ID = {entity.ID} updated with result {isSuccess}.");
                return isSuccess;
            }
            throw new UpdateEntityException<Entity>(result.IsSuccess.ToString());
        }

        protected long CreateEntity(DetailDto dto, string createdBy = null)
        {
            var mapper = _mapperFactory.CreateMapperFromDetailDto();
            var entity = mapper.Map<Entity>(dto);
            if (entity is AuditableEntity)
            {
                (entity as AuditableEntity).CreatedBy = createdBy;
            }
            var result = _crudRepo.Add(entity);
            if (result.IsSuccess == ResultType.OK)
            {
                _logger.Info($"{_type.Name} with ID = {result.ID} was created.");
                _uow.SaveChanges();
                dto.ID = result.ID;
                return result.ID;
            }
            throw new CreateEntityException<Entity>(result.IsSuccess.ToString());
        }

        protected bool DeleteEntity(DetailDto dto, string deletedBy = null)
        {
            var mapper = _mapperFactory.CreateMapperFromDetailDto();
            var entity = mapper.Map<Entity>(dto);
            if (entity is AuditableEntity)
            {
                (entity as AuditableEntity).DeletedBy = deletedBy;
            }
            var result = _crudRepo.Delete(entity);
            if (result.IsSuccess == ResultType.OK)
            {
                _logger.Info($"{_type.Name} with ID = {result.ID} was deleted.");
                bool isSuccess = _uow.SaveChanges();
                return isSuccess;
            }
            throw new DeleteEntityException<Entity>(result.IsSuccess.ToString());
        }

        protected bool UnDeleteEntity(DetailDto dto)
        {
            var mapper = _mapperFactory.CreateMapperFromDetailDto();
            var entity = mapper.Map<Entity>(dto);
            if (entity is AuditableEntity)
            {
                (entity as AuditableEntity).DeletedBy = null;
            }
            var result = _crudRepo.UnDelete(entity);
            if (result.IsSuccess == ResultType.OK)
            {
                _logger.Info($"{_type.Name} with ID = {result.ID} was undeleted.");
                bool isSuccess = _uow.SaveChanges();
                return isSuccess;
            }
            throw new DeleteEntityException<Entity>(result.IsSuccess.ToString());
        }
        #endregion

    }
}
