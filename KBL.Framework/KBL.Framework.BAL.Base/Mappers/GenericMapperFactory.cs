using AutoMapper;
using KBL.Framework.BAL.Interfaces.Entities;
using KBL.Framework.BAL.Interfaces.Mappers;
using KBL.Framework.DAL.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.BAL.Base.Services
{
    public class GenericMapperFactory<DetailDto, GridDto, Entity> : IGenericMapperFactory<DetailDto, GridDto, Entity> where GridDto : IDto where DetailDto : IDto where Entity : IEntity
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Cstors
        #endregion

        #region Public methods
        public IMapper CreateMapperFromGridDto()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GridDto, Entity>();
            });

            var mapper = config.CreateMapper();

            return mapper;
        }

        public IMapper CreateMapperToGridDto()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Entity, GridDto>();
            });

            var mapper = config.CreateMapper();

            return mapper;
        }

        public IMapper CreateMapperFromDetailDto()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DetailDto, Entity>();
            });

            var mapper = config.CreateMapper();

            return mapper;
        }

        public IMapper CreateMapperToDetailDto()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Entity, DetailDto>();
            });

            var mapper = config.CreateMapper();

            return mapper;
        }
        #endregion

        #region Private methods
        #endregion
    }
}
