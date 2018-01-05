using System;
using System.Collections.Generic;
using System.Text;

namespace KBL.Framework.DAL.Interfaces.Entities
{
    public interface IEntity
    {
        long ID { get; set; }
        DateTime CreatedDateTime { get; set; }
        //long CreatedBy { get; set; }
        DateTime? ModifiedDateTime { get; set; }
        //long? ModifiedBy { get; set; }
        DateTime? DeletedDateTime { get; set; }
        //long? DeletedBy { get; set; }
    }
}
