using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.DAL.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        bool SaveChanges();
        bool Rollback();
    }
}
