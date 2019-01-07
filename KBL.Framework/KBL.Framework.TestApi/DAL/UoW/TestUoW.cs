using KBL.Framework.DAL.Base.Repositories;
using KBL.Framework.DAL.Base.UnitOfWork;
using KBL.Framework.DAL.Interfaces.Repositories;
using KBL.Framework.TestApi.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBL.Framework.TestApi.DAL.UoW
{
    public class TestUoW : BaseUoW, IDisposable, ITestUoW
    {
        private ICrudRepository<User> _userRepo;

        public ICrudRepository<User> UserRepo =>_userRepo ?? (_userRepo = new GenericCrudRepository<User>(GetTransaction(), _configuration));

        public TestUoW(IConfiguration configuration) : base(configuration)
        {

        }
        
    }
}
