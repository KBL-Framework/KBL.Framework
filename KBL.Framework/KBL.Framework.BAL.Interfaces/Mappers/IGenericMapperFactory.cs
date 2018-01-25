using KBL.Framework.BAL.Interfaces.Entities;
using KBL.Framework.DAL.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.BAL.Interfaces.Mappers
{
    public interface IGenericMapperFactory<DetailDto, GridDto, Entity> : IMapperFactory where GridDto : IDto where DetailDto : IDto where Entity : IEntity
    {
    }
}
