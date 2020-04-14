using System;

namespace KBL.Framework.BAL.Base.Entities
{
    public class BaseTimestampDto : BaseDto
    {
        #region Properties
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public DateTime? DeletedDateTime { get; set; }
        #endregion
    }
}
