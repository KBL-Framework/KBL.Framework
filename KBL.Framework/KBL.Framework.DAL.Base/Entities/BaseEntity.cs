using KBL.Framework.DAL.Interfaces.Entities;
using System;

namespace KBL.Framework.DAL.Base.Entities
{
    public abstract class BaseEntity : IEntity
    {
        #region Properties
        public long ID { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }
        public DateTimeOffset? ModifiedDateTime { get; set; }
        public DateTimeOffset? DeletedDateTime { get; set; }
        #endregion
    }

}
