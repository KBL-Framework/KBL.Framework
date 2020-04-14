namespace KBL.Framework.DAL.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        bool SaveChanges();
        bool Rollback();
    }
}
