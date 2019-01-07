using KBL.Framework.DAL.Interfaces.Repositories;
using KBL.Framework.DAL.Interfaces.UnitOfWork;
using KBL.Framework.TestApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBL.Framework.TestApi.DAL.UoW
{
    public interface ITestUoW : IUnitOfWork
    {
        ICrudRepository<User> UserRepo { get; }
    }
}
