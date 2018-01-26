using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.DAL.Base.Entities
{
    public abstract class AuditableEntity : BaseEntity
    {
        #region Properties
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public long? DeletedBy { get; set; }
        #endregion
    }
}
