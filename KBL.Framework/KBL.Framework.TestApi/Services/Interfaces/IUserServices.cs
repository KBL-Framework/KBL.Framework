using KBL.Framework.BAL.Base.Entities;
using KBL.Framework.BAL.Interfaces.Services;
using KBL.Framework.TestApi.DTOs;
using KBL.Framework.TestApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBL.Framework.TestApi.Services.Interfaces
{
    public interface IUserServices : IBaseCrudServices<UserDto, UserDto>
    {
        Task<long> CreateAsync(UserDto dto, string createdBy);
        IEnumerable<EntityHistoryDto<User>> GetHistory(long id);
        //void Delete(long id);
        Task UpdateAsync(UserDto dto, string modifiedBy);
        Task DeleteAsync(long id);
    }
}
