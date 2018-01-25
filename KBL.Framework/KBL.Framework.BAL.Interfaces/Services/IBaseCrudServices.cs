using KBL.Framework.BAL.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.BAL.Interfaces.Services
{
    public interface IBaseCrudServices<DetailDto, GridDto> where DetailDto : IDto where GridDto : IDto
    { 
        long Create(DetailDto dto);
        bool Update(DetailDto dto);
        bool Delete(DetailDto dto);
        DetailDto Get(long id);
        IEnumerable<GridDto> GetAll();
    }
}
