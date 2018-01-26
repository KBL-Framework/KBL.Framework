using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.DAL.Interfaces.Entities
{
    public interface IEntity
    {
        long ID { get; set; }
        DateTime CreatedDateTime { get; set; }     
        DateTime? ModifiedDateTime { get; set; }     
        DateTime? DeletedDateTime { get; set; }     
    }
}
