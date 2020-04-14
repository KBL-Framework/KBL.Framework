using AutoMapper;

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
