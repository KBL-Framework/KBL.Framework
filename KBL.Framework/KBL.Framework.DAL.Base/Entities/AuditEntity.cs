namespace KBL.Framework.DAL.Base.Entities
{
    public class AuditEntity : BaseEntity
    {
        #region Properties
        public long EntityId { get; set; }
        public string TableName { get; set; }
        public string ClassFullName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        #endregion
    }
}
