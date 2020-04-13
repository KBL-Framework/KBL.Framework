namespace KBL.Framework.DAL.Base.Infrastructure
{
    public sealed class AuditingHelper
    {
        #region Fields
        #endregion

        #region Properties
        public string AuditingTableName { get; } = "logs.AuditEntities";
        public string AuditingTableSchema { get; } = "logs";
        public string AuditingProcedureName { get; } = "";
        public bool IsAuditingEnable { get; set; }
        public static AuditingHelper Instance { get; } = new AuditingHelper();
        #endregion

        #region Cstors
        private AuditingHelper()
        {
            IsAuditingEnable = false;
        }
        #endregion

        #region Public methods
        #endregion

        #region Private methods
        #endregion
    }
}
