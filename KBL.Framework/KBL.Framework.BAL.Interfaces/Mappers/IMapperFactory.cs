using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.BAL.Interfaces.Mappers
{
    public interface IMapperFactory
    {
        IMapper CreateMapperFromGridDto();
        IMapper CreateMapperToGridDto();
        IMapper CreateMapperFromDetailDto();
        IMapper CreateMapperToDetailDto();
    }
}
