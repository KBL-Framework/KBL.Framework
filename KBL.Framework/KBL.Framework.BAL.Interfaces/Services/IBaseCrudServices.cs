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
        long Create(DetailDto dto, string createdBy);
        bool Update(DetailDto dto, string modifiedBy);
        bool Delete(DetailDto dto, string deletedBy);
        DetailDto Get(long id);
        IEnumerable<GridDto> GetAll();
    }
}
