using System;

namespace KBL.Framework.DAL.Interfaces.Entities
{
    public interface IEntity
    {
        long ID { get; set; }
        DateTimeOffset CreatedDateTime { get; set; }
        DateTimeOffset? ModifiedDateTime { get; set; }
        DateTimeOffset? DeletedDateTime { get; set; }
    }
}
