namespace KBL.Framework.BAL.Base.Entities
{
    public class AuditableDto : BaseTimestampDto
    {
        #region Properties       
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string DeletedBy { get; set; }
        #endregion
    }
}
