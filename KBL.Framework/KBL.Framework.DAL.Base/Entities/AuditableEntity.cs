namespace KBL.Framework.DAL.Base.Entities
{
    public abstract class AuditableEntity : BaseEntity
    {
        #region Properties
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string DeletedBy { get; set; }
        #endregion
    }
}
