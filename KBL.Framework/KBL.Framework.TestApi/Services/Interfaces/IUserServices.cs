using KBL.Framework.BAL.Interfaces.Services;
using KBL.Framework.TestApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBL.Framework.TestApi.Services.Interfaces
{
    public interface IUserServices : IBaseCrudServices<UserDto, UserDto>
    {
        
    }
}
